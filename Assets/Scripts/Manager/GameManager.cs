using System.Globalization;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public Transform mainCanvas;
	[NonSerialized] public GameScene currentScene;
	public bool isBattle { get { return currentScene == GameScene.BATTLE; } }
	public bool isCardList { get { return currentScene == GameScene.CARD_LIST; } }
	public bool isTitle { get { return currentScene == GameScene.TITLE; } }
	public bool isBattlePhase
	{ 
		get
		{
			if (battleMgr == null)
			{
				return false;
			}
			return battleMgr.isBattlePhase;
		}	
	}
	public bool canOperationCard
	{ 
		get
		{
			if (battleMgr == null)
			{
				return false;
			}
			return battleMgr.canOperationCard;
		}	
	}

	[SerializeField] GameObject titlePanel;

	public SaveManager saveManager;
	public RecoveryManager recoveryManager;
	public MasterManager masterManager;
	public bool useLocalResorce;

	private List<CardCategory> BasicCategory = new List<CardCategory>()
	{
		CardCategory.Common,
		CardCategory.Ranger,
		CardCategory.Assassin,
		CardCategory.Blacksmith,
		CardCategory.Wizard,
		CardCategory.Dragoon,
		CardCategory.Necromancer,
		CardCategory.Hunter,
		CardCategory.Viking
	};

	// 各種Managerクラス
	public BattleManager battleMgr;
	public TitleManager titleMgr;
	public CardListManager cardListMgr;
	public DebugManager debugMgr;
	public CoroutineProcessor coroutineProcessor;
	public NetworkManager networkManager;

	public const int panelPrefabIndex = 3;

	public static GameManager instance;
	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		masterManager = new MasterManager();
		recoveryManager = new RecoveryManager();
		saveManager = new SaveManager();
	}

	void Start()
	{
		Application.targetFrameRate = 60;
		currentScene = GameScene.TITLE;
		StartCoroutine(gameStart());
		networkManager.Connect();
	}

	void Update()
	{
		if (battleMgr != null)
		{
			battleMgr.Update();
		}
	}

	// ゲームスタート時の処理
	private IEnumerator gameStart()
	{
		while (!masterManager.finishGetMaster)
		{
			yield return null;
		}

		SoundManager.instance.startTitleBGM();

		titlePanel.SetActive(true);
		titleMgr.start();
		debugMgr.setUp();

		GoToTitle();
	}

	//モード遷移関係--------------------------------------------------------------------------------------------------------------
	// バトルを開始
	public void StartTestBattle()
	{
		StartCoroutine(StartBattle(isFirstTurn: true, isOnline: false, isTestBattle: true));
	}
	
	public IEnumerator StartBattle(bool isFirstTurn, bool isOnline, bool isTestBattle)
	{
		// 効果音とともに暗転
		titleMgr.loadingPanel.close();
		titlePanel.SetActive(false);
		SoundManager.instance.battleStart();

		if (battleMgr == null)
		{
			battleMgr = new BattleManager(isOnline, isTestBattle);
		}

		currentScene = GameScene.BATTLE;
		battleMgr.mainPanel.gameObject.SetActive(false);
		yield return new WaitForSeconds(2.0f);
		SoundManager.instance.startBattleBGM();
		battleMgr.mainPanel.gameObject.SetActive(true);
		battleMgr.StartBattle(isFirstTurn);
	}

	public void startRecoveryCoroutine(RecoveryData recoveryData)
	{
		// 効果音とともに暗転
		titlePanel.SetActive(false);
		SoundManager.instance.battleStart();
		StartCoroutine(startRecovery(recoveryData));
	}

	// デッキ作成を開始
	public IEnumerator startRecovery(RecoveryData recoveryData)
	{
		battleMgr = new BattleManager();
		battleMgr.mainPanel.gameObject.SetActive(false);
		yield return new WaitForSeconds(2.0f);
		while (battleMgr.player == null || battleMgr.enemy == null)
		{
			yield return null;
		}
		SoundManager.instance.startBattleBGM();
		currentScene = GameScene.BATTLE;
		battleMgr.mainPanel.gameObject.SetActive(true);
		battleMgr.startRecovery(recoveryData);
	}

	public void startBattleCoroutine(List<SaveData.CardComponent> playerDeck, List<SaveData.CardComponent> enemyDeck)
	{
		
	}

	// カードリスト開始
	public void startCardList()
	{
		cardListMgr = new CardListManager();
		titlePanel.SetActive(false);
		cardListMgr.mainPanel.gameObject.SetActive(true);
		currentScene = GameScene.CARD_LIST;
	}

	// タイトルに戻る
	public void GoToTitle()
	{
		titlePanel.SetActive(true);
		titleMgr.goToTitle();
		setGameSpeed(1.0f);

		Resources.UnloadUnusedAssets();
		coroutineProcessor.stopAllCoroutine();
		if (!isCardList)
		{
			SoundManager.instance.startTitleBGM();
		}
		currentScene = GameScene.TITLE;
		if (battleMgr != null)
		{
			battleMgr.mainPanel.destroy();
			battleMgr = null;
		}
		if (cardListMgr != null)
		{
			cardListMgr.mainPanel.destroy();
			cardListMgr = null;
		}
		networkManager.Connect();
		networkManager.LeaveRoom();
	}

	// パネル管理 --------------------------------------------------------------------------------------------------------------
	public void showDescribe(System.Object describeTarget, bool isSub = false, int magnification = 1)
	{
		if (isBattle)
		{
			battleMgr.showDescribe(describeTarget, isSub, magnification);
		}
		else if (isTitle)
		{
			titleMgr.showDescribe(describeTarget, isSub, magnification);
		}
		else if (isCardList)
		{
			cardListMgr.showDescribe(describeTarget, isSub, magnification);
		}
	}

	public void deleteDescribe()
	{
		if (isBattle)
		{
			battleMgr.deleteDescribe();
		}
		else if (isTitle)
		{
			titleMgr.deleteDescribe();
		}
		else if (isCardList)
		{
			cardListMgr.deleteDescribe();
		}
	}

	// その他 ------------------------------------------------------------------------------------------------------
	public void setGameSpeed(float speed)
	{
		Time.timeScale = speed;
	}
}
