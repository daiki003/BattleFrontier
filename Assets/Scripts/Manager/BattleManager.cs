using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class BattleManager
{
	public BattleMainPanel mainPanel;

	public SelfPlayerController player;
	public EnemyPlayerController enemy;
	public List<CardController> normalDeckCardList = new List<CardController>(); // 通常デッキのカードリスト
	public List<CardController> specialDeckCardList = new List<CardController>(); // 特殊デッキのカードリスト
	

	// カードプレイ中か、プレイ可能かを判定
	public bool stopPlayFlag;

	// チュートリアル中かどうかを判定
	public bool isTutorial;

	// 操作可能な時か判定
	public bool canOperationCard;
	// 操作可能な時か判定
	public bool isBattlePhase;
	// 自身のターンか判定
	public bool isSelfTurn;
	// 操作中のカード
	public CardController movingCard;
	public const int FLAG_AREA_NUMBER = 9;

	// ターンエンド可能かどうか
	public bool canTurnEnd
	{
		get
		{
			return !isTutorial || tutorialService.canTurnEnd;
		}
	}

	// カードプレイ可能かどうか
	public bool canPlayCard
	{
		get
		{
			return !isTutorial || tutorialService.canPlayCard;
		}
	}

	// 選択中かどうか
	public bool isSelect
	{
		get
		{
			return selectPanel.selectType != SelectType.NONE;
		}
	}

	// キャンセルできない選択中かどうか
	public bool isCantCancelSelect
	{
		get
		{
			return selectPanel.selectType != SelectType.SELECT_BONUS;
		}
	}

	// 説明表示中かどうか
	public bool isDescribe { get { return describeAreaController.isDescribe; } }
	private List<Color> cardColorList = new List<Color>()
	{
		Color.red,
		Color.blue,
		Color.yellow,
		Color.green,
		Color.cyan,
		Color.magenta
	};

	// 各クラスを保持
	[NonSerialized] public AbilityProcessor mainAbilityProcessor;
	[NonSerialized] public TutorialService tutorialService;
	[NonSerialized] public List<FlagArea> flagAreaList = new List<FlagArea>();

	// 生成したバトルプレファブを保持
	[NonSerialized] public List<PanelPrefab> battlePrefabs = new List<PanelPrefab>();
	public ResultPanel resultPanel { get { return battlePrefabs.FirstOrDefault(p => p is ResultPanel) as ResultPanel; } }
	public TurnPanel turnPanel { get { return battlePrefabs.FirstOrDefault(p => p is TurnPanel) as TurnPanel; } } 
	public InformationService informationPanel { get { return battlePrefabs.FirstOrDefault(p => p is InformationService) as InformationService; } }
	public TutorialPanel tutorialPanel { get { return battlePrefabs.FirstOrDefault(p => p is TutorialPanel) as TutorialPanel; } }
	public TrainingPhase trainingPhase { get { return battlePrefabs.FirstOrDefault(p => p is TrainingPhase) as TrainingPhase; } }
	public SelectService selectPanel { get { return battlePrefabs.FirstOrDefault(p => p is SelectService) as SelectService; } }
	public DescribeAreaController describeAreaController { get { return battlePrefabs.FirstOrDefault(p => p is DescribeAreaController) as DescribeAreaController; } }
	public SelectDialog selectDialog{ get { return battlePrefabs.FirstOrDefault(p => p is SelectDialog) as SelectDialog; } }
	public Loading loadingPanel { get { return battlePrefabs.FirstOrDefault(p => p is Loading) as Loading; } }

	// static化
	public static BattleManager instance;
	public BattleManager()
	{
		instance = this;
		mainPanel = PrefabManager.instance.createBattleMainPanel(GameManager.instance.mainCanvas);
		mainPanel.transform.SetSiblingIndex(GameManager.panelPrefabIndex);
		player = new SelfPlayerController();
		enemy = new EnemyPlayerController();

		while (battlePrefabs.Count > 0)
		{
			PanelPrefab battlePrefab = battlePrefabs.dequeue();
			battlePrefab.destroy();
		}
		battlePrefabs = PrefabManager.instance.createBattlePrefabs(mainPanel.battleTransform);

		mainAbilityProcessor = new AbilityProcessor(this);
		selectPanel.start(this);
	}

	public void update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			GameManager.instance.debugMgr.openDebugMenu();
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			GameManager.instance.debugMgr.setPpMax();
		}

		if (GameManager.instance.coroutineProcessor.allVfxList.Count > 0 && !GameManager.instance.coroutineProcessor.processing)
		{
			GameManager.instance.coroutineProcessor.startProcess();
		}

		// チュートリアルの場合、次のフェーズに移れるかを常に判定する
		if (isTutorial)
		{
			tutorialService.update();
		}

		if (player != null)
		{
			player.update();
		}
		if (selectPanel != null)
		{
			selectPanel.update();
		}
		if (describeAreaController != null)
		{
			describeAreaController.update();
		}
		if (mainPanel != null)
		{
			mainPanel.update();
		}
	}

	// バトルの初期準備をする ------------------------------------------------------------------------------------------------------------------------
	// バトル開始時の処理、新たなバトルでも、やり直しのバトルでも
	public void StartBattle(bool isFirstTurn)
	{
		foreach (PanelPrefab battlePrefab in battlePrefabs)
		{
			battlePrefab.setup();
		}

		// コルーチンをリセット
		GameManager.instance.coroutineProcessor.clearCoroutineBlock();

		// 盤面、手札、デッキ、敵データのリセット
		mainPanel.waitArea.resetCard();
		mainPanel.playArea.resetCard();
		mainPanel.center.resetCard();
		mainPanel.invisibleArea.resetCard();
		mainPanel.banishArea.resetCard();

		normalDeckCardList = new List<CardController>();
		int index = 0;
		for (int i = 1; i <= 10; i++)
		{
			foreach (Color color in cardColorList)
			{
				CardController deckCard = PrefabManager.instance.CreateNormalCard(i, color, index, BattleManager.instance.mainPanel.invisibleArea);
				normalDeckCardList.Add(deckCard);
				index++;
			}
		}

		specialDeckCardList = new List<CardController>();
		for (int i = 1; i <= 10; i++)
		{
			CardController deckCard = PrefabManager.instance.CreateNormalCard(-1, Color.white, index, BattleManager.instance.mainPanel.invisibleArea);
			specialDeckCardList.Add(deckCard);
			index++;
		}

		player.battleStart();
		enemy.battleStart();

		mainPanel.SetUpField();

		// 再開用データはここで作る
		recordRecoveryData();

		// 自分のターンが始まるまで操作不能
		canOperationCard = false;
		// ターン開始時に入れ替わるので、isFirstTurnの逆を設定
		isSelfTurn = !isFirstTurn;

		// 各種変数の初期化
		stopPlayFlag = false;
		mainPanel.turnEndButton.onClick.RemoveAllListeners();

		//ターンの決定
		ChangeTurnVfx changeTurnVfx = new ChangeTurnVfx();
		changeTurnVfx.addToAllBlockList();
	}

	// バトルを初めからやり直す
	public void Restart()
	{
		SoundManager.instance.startBattleBGM();
		StartBattle(isFirstTurn: true);
	}

	public void startRecovery(RecoveryData recoveryData)
	{
		StartBattle(isFirstTurn: true);

		// コルーチンをリセット
		GameManager.instance.coroutineProcessor.clearCoroutineBlock();

		player.battleRecovery(recoveryData);

		// パネルの設定
		foreach (PanelPrefab battlePrefab in battlePrefabs)
		{
			battlePrefab.setup();
		}
		trainingPhase.setTrainingPhase(true);

		// 再開後は必ず自分のターン
		canOperationCard = true;
		isSelfTurn = true;

		// 各種変数の初期化
		stopPlayFlag = false;
		mainPanel.turnEndButton.onClick.RemoveAllListeners();
	}

	//ターンの管理-----------------------------------------------------------------------------------------------------------------------

	// ターン開始時の処理
	public void turnStart()
	{
		// ターンの入れ替え
		isSelfTurn = !isSelfTurn;

		if (isSelfTurn)
		{
			player.TurnStart();
		}
		else
		{
			enemy.TurnStart();
		}

		// ターン開始時処理が終わった時点で再開データを更新
		recordRecoveryData();
	}

	public void onClickTurnEnd()
	{
		if (isSelect)
		{
			BattleManager.instance.onClickExtra();
			return;
		}
		if (!canTurnEnd) return;
		
		// ターン終了ボタンを押した時点で再開データを更新
		recordRecoveryData();
		// 相手にターン終了を通知
		var parameter = new NetworkParameter();
		GameManager.instance.networkManager.SendAction(NetworkOperationType.TURN_END, parameter);
		changeTurn();
	}

	public void changeTurn()
	{
		canOperationCard = false;
		SEManager.instance.playSe("TurnEnd");
		ChangeTurnVfx changeTurnVfx = new ChangeTurnVfx();
		changeTurnVfx.addToAllBlockList();
	}

	public bool CheckResult()
	{
		bool isPlayerWin = CheckWin(isSelf: true);
		bool isEnemyWin = CheckWin(isSelf: false);
		if (!isPlayerWin && !isEnemyWin)
		{
			return false;
		}

		// ゲーム終了が確定した時点で操作不能にする
		canOperationCard = false;
		GameSetVfx gameSetVfx = new GameSetVfx(isPlayerWin);
		gameSetVfx.addToAllBlockList();
		return true;
	}

	public void EnemyRetire()
	{
		// ゲーム終了が確定した時点で操作不能にする
		canOperationCard = false;
		GameSetVfx gameSetVfx = new GameSetVfx(isSelfWin: true);
		gameSetVfx.addToAllBlockList();
	}

	private bool CheckWin(bool isSelf)
	{
		int seriesPoint = 0;
		int totalPoint = 0;
		for (int i = 0; i < flagAreaList.Count; i++)
		{
			bool isPlayerArea = isSelf ? flagAreaList[i].isPlayerArea : flagAreaList[i].isEnemyArea;
			if (isPlayerArea)
			{
				seriesPoint++;
				totalPoint++;
			}
			else
			{
				seriesPoint = 0;
			}

			if (seriesPoint >= 3 || totalPoint >= 5)
			{
				return true;
			}
		}
		return false;
	}

	// カード表示関連 -------------------------------------------------------------------------------------------------------------------------------
	// プレイ領域を一時的に触れるようにする
	public void setOnPlay(bool active)
	{
		// CanvasGroup onplayCanvas = mainPanel.onplay.GetComponent<CanvasGroup>();
		// onplayCanvas.blocksRaycasts = active;

		// CanvasGroup onsoldCanvas = mainPanel.onsold.GetComponent<CanvasGroup>();
		// onsoldCanvas.blocksRaycasts = active;
	}

	// 説明関連 ----------------------------------------------------------------------------------------------------

	// 説明を表示する
	public void showDescribe(System.Object describeTarget, bool isSub = false, int magnification = 1)
	{
		describeAreaController.display(describeTarget, isSub, magnification);
	}

	// 全ての説明を閉じる
	public void deleteDescribe()
	{
		if (describeAreaController != null) describeAreaController.close();
	}

	// パネル関連 --------------------------------------------------------------------------------------------------------------
	public void showResultPanel(bool isSelfWin)
	{
		resultPanel.finishBattle(isSelfWin);
		SoundManager.instance.startWinBGM();
	}

	public void onClickExtra()
	{
		foreach (PanelPrefab panel in battlePrefabs)
		{
			panel.onClickExtra();
		}
	}

	// 再開用データを記録
	public void recordRecoveryData(string fileName = "")
	{
		GameManager.instance.recoveryManager.createRecoveryData(player, fileName);
	}
}

