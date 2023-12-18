using System.Globalization;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
	[SerializeField] Transform titleTransform, inputAreaTransform;
	[SerializeField] GameObject topMenu, tutorialMenu, deleteDataMenu, nameInput;
	[SerializeField] InputField inpuitDeckName;

	[SerializeField] Text displayName;

	// デッキ選択ボタン
	[SerializeField] List<Button> deckButton;

	GameManager gameMgr;
	public List<PanelPrefab> titlePrefabs;
	public SelectDeck selectDeckPanel { get { return titlePrefabs.FirstOrDefault(p => p is SelectDeck) as SelectDeck; } }
	public DescribeAreaController describeAreaController { get { return titlePrefabs.FirstOrDefault(p => p is DescribeAreaController) as DescribeAreaController; } }
	public CreateDeckMenu createDeckMenu { get { return titlePrefabs.FirstOrDefault(p => p is CreateDeckMenu) as CreateDeckMenu; } }
	public ClassSelectMenu classSelectMenu { get { return titlePrefabs.FirstOrDefault(p => p is ClassSelectMenu) as ClassSelectMenu; } }
	public BattleMenu battleMenu { get { return titlePrefabs.FirstOrDefault(p => p is BattleMenu) as BattleMenu; } }
	public Loading loadingPanel { get { return titlePrefabs.FirstOrDefault(p => p is Loading) as Loading; } }
	public SelectDialog selectDialog{ get { return titlePrefabs.FirstOrDefault(p => p is SelectDialog) as SelectDialog; } }

	public static TitleManager instance;
	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void start()
	{
		gameMgr = GameManager.instance;
		while (titlePrefabs.Count > 0)
		{
			PanelPrefab titlePrefab = titlePrefabs.dequeue();
			Destroy(titlePrefab.gameObject);
		}

		titlePrefabs = PrefabManager.instance.createTitlePrefabs(titleTransform);
	}

	// トップ画面の各種ボタンを押した時の処理 --------------------------------------------------------------------------------------------

	public void startBattleButton()
	{
		GameManager.instance.StartTestBattle();
	}

	public void startTutorialButton(int tutorialType)
	{
		TutorialService tutorialService = getTutorialService(tutorialType);
		// StartCoroutine(gameMgr.startCreateDeck(deckNumber, category, selectingLevel, tutorialService: tutorialService));
	}

	// 「デッキ作成」ボタン
	public void openClassSelect(bool toBattle)
	{
		classSelectMenu.open();
		openClassSelectMenu();
	}

	public void openTutorial()
	{
		SEManager.instance.playSe("Button");
		openTutorialMenu();
	}

	// 「デッキ一覧」ボタン
	public void openCheckDeck()
	{
		selectDeckPanel.openForCheck();
	}
	// 「バトル」ボタン
	public void openDeckSelect()
	{
		selectDeckPanel.openForBattle();
	}
	// 「ランダムマッチ」ボタン
	public void StartRandomMatch()
	{
		GameManager.instance.networkManager.RandomMatch();
	}
	// 「ルームマッチ」ボタン
	public void OpenRoomMatch()
	{
		PrefabManager.instance.CreateInputArea(inputAreaTransform, "ルーム名を入力してください", GameManager.instance.networkManager.CreateRoom);
	}
	public void DisplayLoading()
	{
		loadingPanel.Init(roomName: null, waitingText: "対戦相手を探しています");
	}
	public void DisplayLoading(string roomName)
	{
		loadingPanel.Init(roomName, waitingText: "対戦相手を待っています");
	}
	// 「再開」ボタン
	public void startRecovery()
	{
		GameManager.instance.recoveryManager.checkRecovery(fileName: "");
	}
	
	public void openCreateDeckMenu(CardCategory category)
	{
		createDeckMenu.open(category);
	}
	public void openBattleMenu(List<SaveData.CardComponent> playerDeck)
	{
		battleMenu.open(playerDeck);
	}

	// 「名前入力」ボタン
	public void setNameInput()
	{
		nameInput.SetActive(true);
	}
	// 名前入力の決定
	public void updateName()
	{
		PlayFabController.updatePlayerName(inpuitDeckName.text);
	}

	// カードリスト開始
	public void startCardList()
	{
		SEManager.instance.playSe("Button");
		gameMgr.startCardList();
	}

	// トップ画面に表示する名前の更新
	public void updateDisplayName(string name)
	{
		displayName.text = name;
	}

	// データ削除（デバッグ）関連 -----------------------------------------------------------------------------------------------
	public void deleteAllData()
	{
		GameManager.instance.saveManager.deleteAllData();
	}

	// パネル管理
	public void goToTitle()
	{
		foreach (PanelPrefab titlePrefab in titlePrefabs)
		{
			titlePrefab.setup();
		}
		topMenu.SetActive(true);
		tutorialMenu.SetActive(false);
		deleteDataMenu.SetActive(false);
	}

	//CPU対戦ボタンを押した時の処理
	public void openBattleMenu()
	{
		tutorialMenu.SetActive(false);
		deleteDataMenu.SetActive(false);
	}

	public void openClassSelectMenu()
	{
		tutorialMenu.SetActive(false);
		deleteDataMenu.SetActive(false);
	}

	public void openTutorialMenu()
	{
		tutorialMenu.SetActive(true);
		deleteDataMenu.SetActive(false);
	}

	public void openDeleteDataMenu()
	{
		tutorialMenu.SetActive(false);
		deleteDataMenu.SetActive(true);
	}

	public void onClickExtra()
	{
		foreach (PanelPrefab panel in titlePrefabs)
		{
			panel.onClickExtra();
		}
	}

	// その他 ------------------------------------------------------------------------------------------------------
	private TutorialService getTutorialService(int tutorialType)
	{
		switch (tutorialType)
		{
			case 0: return new AttackTutorial();
			default: return null;
		}
	}

	// 説明を表示する
	public void showDescribe(System.Object describeTarget, bool isSub = false, int magnification = 1)
	{
		describeAreaController.display(describeTarget, isSub, magnification);
	}
	public void deleteDescribe()
	{
		if (describeAreaController != null) describeAreaController.close();
	}
}
