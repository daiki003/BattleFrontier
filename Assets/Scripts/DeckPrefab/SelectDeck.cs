using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDeck : PanelPrefab
{
	public GameObject selectDeckPanel, inputDeckNamePanel;
	public Transform cardTransform, deckNameTransform;
	public Text describeText, deckNameText;
	public InputField inpuitDeckName;
	public List<Button> deckButtonList;
	public List<Text> deckNameList;
	public Button decideButton;
	private int selectingDeckNumber;
	private List<CardController> saveCardList;
	List<SaveData.DeckData> deckDataList;

	// テストマッチで相手側のデッキを選択する時用
	private int enemyDeckNumber;
	private bool enemySelect;

	public override void setup()
	{
		updateDeckData();
		decideButton.onClick.RemoveAllListeners();
		setActive(false);
	}
	public override void close()
	{
		cardTransform.resetCard();
		GameManager.instance.deleteDescribe();
		selectDeckPanel.SetActive(false);
	}
	public override void onClickExtra() { }

	public void updateDeckData()
	{
		deckDataList = new List<SaveData.DeckData>();
		for (int i = 0; i < deckButtonList.Count; i++)
		{
			int index = i;
			SaveData.DeckData deckData = GameManager.instance.saveManager.getSaveData().getDeckData(i);
			deckButtonList[index].onClick.RemoveAllListeners();
			deckButtonList[index].onClick.AddListener(() => selectDeck(index));
			if (deckData != null)
			{
				deckDataList.Add(deckData);
				deckNameList[index].text = deckData.deckName;
			}
			else
			{
				deckNameList[index].text = "デッキ" + index;
			}
		}
	}

	// デッキのボタンを押した時
	public void selectDeck(int number)
	{
		if (enemySelect)
		{
			this.enemyDeckNumber = number;
		}
		else
		{
			this.selectingDeckNumber = number;
		}
		SaveData.DeckData deckData = deckDataList.Count > number ? deckDataList[number] : null;
		reflectionDeck(deckData);
	}

	// 選択中のデッキを見た目に反映
	public void reflectionDeck(SaveData.DeckData deckData)
	{
		
	}

	public void setActive(bool active)
	{
		selectDeckPanel.SetActive(active);
		if (active)
		{
			updateDeckData();
			selectDeck(0);
		}
		else
		{
			cardTransform.resetCard();
			GameManager.instance.deleteDescribe();
		}
	}

	public void openForSave(List<CardController> lastCardList)
	{
		describeText.text = "デッキの保存場所を選択してください";
		saveCardList = lastCardList;
		decideButton.gameObject.SetActive(true);
		decideButton.onClick.RemoveAllListeners();
		decideButton.onClick.AddListener(saveDeck);
		setActive(true);
	}
	public void openForCheck()
	{
		describeText.text = "デッキ一覧";
		decideButton.gameObject.SetActive(false);
		decideButton.onClick.RemoveAllListeners();
		setActive(true);
	}
	public void openForBattle(bool isRandomMatch = false, bool isTestMatch = false)
	{
		enemySelect = false;
		describeText.text = "使用するデッキを選択してください";
		decideButton.gameObject.SetActive(true);
		decideButton.onClick.RemoveAllListeners();
		if (isRandomMatch)
		{
			decideButton.onClick.AddListener(startRandomMatch);
		}
		else if (isTestMatch)
		{
			decideButton.onClick.AddListener(selectEnemyDeck);
		}
		else
		{
			decideButton.onClick.AddListener(goToEnemySelect);
		}
		setActive(true);
	}

	public void saveDeck()
	{
		List<SaveData.CardComponent> saveComponentList = new List<SaveData.CardComponent>();
		foreach (CardController card in saveCardList)
		{
			saveComponentList.Add(card.createSaveComponent());
		}
		GameManager.instance.saveManager.addDeckData(selectingDeckNumber, "デッキ" + selectingDeckNumber, saveComponentList);
		// 更新後の情報で表示し直す
		updateDeckData();
		selectDeck(selectingDeckNumber);
	}

	public void goToEnemySelect()
	{
		if (deckDataList[selectingDeckNumber] != null)
		{
			GameManager.instance.titleMgr.openBattleMenu(deckDataList[selectingDeckNumber].cards);
		}
	}

	public void selectEnemyDeck()
	{
		describeText.text = "相手側のデッキを選択してください";
		decideButton.gameObject.SetActive(true);
		decideButton.onClick.RemoveAllListeners();
		decideButton.onClick.AddListener(startTestMatch);
		enemySelect = true;
	}

	public void startRandomMatch()
	{
		PlayFabController.getDeckData(startRandomMatch);
	}
	public void startRandomMatch(SaveData.DeckData deckData)
	{
		GameManager.instance.startBattleCoroutine(deckDataList[selectingDeckNumber].cards, deckData.cards);
	}
	public void startTestMatch()
	{
		GameManager.instance.startBattleCoroutine(deckDataList[selectingDeckNumber].cards, deckDataList[enemyDeckNumber].cards);
	}

	public void registerMainDeck()
	{
		if (deckDataList.Count > selectingDeckNumber)
		{
			PlayFabController.updateMainDeckData(deckDataList[selectingDeckNumber]);
		}
		SEManager.instance.playSe("Button");
	}

	public void getRandomDeck()
	{
		PlayFabController.getDeckData(reflectionDeck);
	}

	// デッキ名入力関連 ------------------------------------------------------------------------------------------------------------------------
	public void setActiveInputDeckName(bool active)
	{
		if (active) SEManager.instance.playSe("Button");
		inputDeckNamePanel.SetActive(active);
	}

	public void changeDeckName()
	{
		GameManager.instance.saveManager.changeDeckName(this.selectingDeckNumber, inpuitDeckName.text);
		setActiveInputDeckName(false);
		updateDeckData();
		selectDeck(selectingDeckNumber);
	}
}
