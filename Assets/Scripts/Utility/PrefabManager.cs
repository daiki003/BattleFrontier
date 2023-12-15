using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class PrefabManager : MonoBehaviour
{
	[SerializeField] GameObject unitCardPrefab;
	[SerializeField] FlagArea flagArea;
	[SerializeField] InputArea inputArea;
	[SerializeField] LevelController levelPrefab;
	[SerializeField] TurnAndEnemy turnAndEnemyPrefab;
	[SerializeField] BonusPanel bonusPrefab;
	[SerializeField] DebugCardName debugCardNamePrefab;
	[SerializeField] BuffLogItem buffLogItemPrefab;

	[SerializeField] BattleMainPanel battleMainPanel;
	[SerializeField] CardListMainPanel cardListMainPanel;
	[SerializeField] List<PanelPrefab> battlePrefabs;
	[SerializeField] List<PanelPrefab> titlePrefabs;
	[SerializeField] List<PanelPrefab> cardListPrefabs;

	public static PrefabManager instance;
	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	// カードを指定の場所に作成する
	public CardController CreateNormalCard(int number, Color color, int index, Transform place)
	{
		GameObject cardObject = Instantiate(unitCardPrefab, place);
		NormalCard card = new NormalCard(cardObject, number, color, index);
		return card;
	}

	// 特殊カードを指定の場所に作成する
	public CardController CreateSpecialCard(string specialName, int index, Transform place)
	{
		switch (specialName)
		{
			case "joker":
				GameObject jokerCardObject = Instantiate(unitCardPrefab, place);
				JokerCard jokerCard = new JokerCard(jokerCardObject, index);
				return jokerCard;
			case "wild_seven":
				GameObject wildSevenCardObject = Instantiate(unitCardPrefab, place);
				WildSevenCard wildSevenCard = new WildSevenCard(wildSevenCardObject, index);
				return wildSevenCard;
			case "fog":
				GameObject fogCardObject = Instantiate(unitCardPrefab, place);
				FlagCard fogCard = new FlagCard(fogCardObject, FlagCardType.Fog, index);
				return fogCard;
			case "mad":
				GameObject madCardObject = Instantiate(unitCardPrefab, place);
				FlagCard madCard = new FlagCard(madCardObject, FlagCardType.Mad, index);
				return madCard;
		}
		GameObject cardObject = Instantiate(unitCardPrefab, place);
		CardController card = new CardController(cardObject, index);
		return card;
	}

	// カード設置領域を指定の場所に作成する
	public FlagArea createArea(Transform place, int index)
	{
		FlagArea area = Instantiate(flagArea, place);
		area.fieldIndex = index;
		return area;
	}

	// テキスト入力領域を指定の場所に作成する
	public InputArea CreateInputArea(Transform place, string text, UnityAction<string, bool> callback)
	{
		InputArea inputArea = Instantiate(this.inputArea, place);
		inputArea.Init(text, callback);
		return inputArea;
	}

	// ボーナスパネルを指定の場所に作成する
	public BonusPanel createBonus(LevelBonusMaster bonusMaster, Transform place)
	{
		BonusPanel bonusPanel = Instantiate(bonusPrefab, place);
		bonusPanel.init(bonusMaster);
		return bonusPanel;
	}

	// レベルパネルを指定の場所に作成する
	public LevelController createLevel(string environmentId, bool isCreateDeck, Transform place)
	{
		LevelController levelPanel = Instantiate(levelPrefab, place);
		levelPanel.init(environmentId, isCreateDeck);
		return levelPanel;
	}

	// ターンごとの敵リストを指定の場所に作成する
	public TurnAndEnemy createTurnAndEnemy(int turn, List<string> cardList, Transform place)
	{
		TurnAndEnemy turnAndEnemy = Instantiate(turnAndEnemyPrefab, place);
		turnAndEnemy.init(turn, cardList);
		return turnAndEnemy;
	}

	public DebugCardName createDebugCardName(string cardName, Transform place)
	{
		DebugCardName debugCardName = Instantiate(debugCardNamePrefab, place);
		debugCardName.init(cardName);
		return debugCardName;
	}
	
	public BuffLogItem createBuffLogItem(Transform place)
	{
		BuffLogItem buffLogItem = Instantiate(buffLogItemPrefab, place);
		return buffLogItem;
	}

	public BattleMainPanel createBattleMainPanel(Transform place)
	{
		BattleMainPanel battleMainPanel = Instantiate(this.battleMainPanel, place);
		return battleMainPanel;
	}

	public CardListMainPanel createCardListMainPanel(Transform place)
	{
		CardListMainPanel cardListMainPanel = Instantiate(this.cardListMainPanel, place);
		return cardListMainPanel;
	}

	public List<PanelPrefab> createBattlePrefabs(Transform place)
	{
		List<PanelPrefab> createdBattlerPrefab = new List<PanelPrefab>();
		foreach (PanelPrefab battlePrefab in battlePrefabs)
		{
			createdBattlerPrefab.Add(Instantiate(battlePrefab, place));
		}
		return createdBattlerPrefab;
	}

	public List<PanelPrefab> createTitlePrefabs(Transform place)
	{
		List<PanelPrefab> createdtitlePrefab = new List<PanelPrefab>();
		foreach (PanelPrefab titlePrefab in titlePrefabs)
		{
			createdtitlePrefab.Add(Instantiate(titlePrefab, place));
		}
		return createdtitlePrefab;
	}
	
	public List<PanelPrefab> createCardListPrefabs(Transform place)
	{
		List<PanelPrefab> createdCardListPrefab = new List<PanelPrefab>();
		foreach (PanelPrefab cardListPrefab in cardListPrefabs)
		{
			createdCardListPrefab.Add(Instantiate(cardListPrefab, place));
		}
		return createdCardListPrefab;
	}
}
