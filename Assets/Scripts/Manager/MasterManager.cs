using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.Json;

public class CardMaster
{
	public class Accelerate
	{
		public bool canAccelerate;
		public int cost;
		public string accelerateCardId;
	}

	public class SelectCard
	{
		public bool isSelect;
		public string selectTarget;
		public List<string> selectCondition = new List<string>();
	}

	public class ChoiceCard
	{
		public bool isChoice;
		public List<string> choiceCardList = new List<string>();
		public string choiceNumber = "";
		public List<string> choiceCondition = new List<string>();
	}

	public class SelectMasterComponent
	{
		public SelectType selectType;
		public string selectNumber;
		public string selectTiming;
		public List<string> selectCondition = new List<string>();
		public string selectTarget;
		public List<string> choiceCardName;
	}

	public string cardID;
	public string name;
	public CardCategory category;
	public bool isToken;
	public CardType cardType;

	public Tribe tribe;
	public List<string> supplementComponentText = new List<string>();
	public string instantText = "";
	public string text = "";
	public string evolveText = "";
	public List<string> textList = new List<string>();
	public List<string> evolveTextList = new List<string>();

	public CardStatus status;
	public int bootCost;
	
	public Accelerate accelerate = new Accelerate();

	public SelectCard select = new SelectCard();

	public ChoiceCard choice = new ChoiceCard();
	public List<SelectMasterComponent> selectList = new List<SelectMasterComponent>();

	public List<PropertyTrait> propertyList = new List<PropertyTrait>();
	public List<PropertyTrait> evolvePropertyList = new List<PropertyTrait>();
	public List<AbilityTrait> abilityTraitList = new List<AbilityTrait>();
	public List<AbilityTrait> evolveAbilityTraitList = new List<AbilityTrait>();
}

[System.Serializable]
public class EnvironmentMaster
{
	public string environmentId;
	public string name;
	public PlayerEntity player;
	public EnemyEntity enemy;
	public int deckNumber;
}

public class DeckEntity
{
	public string deckId;
	public string name;
	public List<string> deck;
	public string rewardEnvironment;
}

public class LevelBonusMaster
{
	public string bonusId;
	public string bonusName;
	public string bonusText;
	public AbilityTrait ability;
}

public class LevelBonusList
{
	public int level;
	public List<LevelBonusMaster> bonusList;
}

public class KeywordMaster
{
	public string keyword;
	public string title;
	public string describe;
}

public class ClassText
{
	public string classId;
	public string className;
	public string describe;
}

public class MasterManager
{
	public List<CardMaster> cardData = new List<CardMaster>();
	public List<KeywordMaster> keywordData = new List<KeywordMaster>();
	public List<EnvironmentMaster> environmentList = new List<EnvironmentMaster>();
	public List<DeckEntity> deckEntityList = new List<DeckEntity>();
	public List<LevelBonusList> bonusList = new List<LevelBonusList>();
	public List<ClassText> classTextList = new List<ClassText>();
	public bool finishGetMaster;

	public void getAllMasterData()
	{
		if (!GameManager.instance.useLocalResorce)
		{
			PlayFabController.GetTitleData(setMasterData);
		}
		else
		{
			environmentList = PlayFabSimpleJson.DeserializeObject<List<EnvironmentMaster>>(Resources.Load<TextAsset>("Json/Environment").ToString());
		}
	}

	public void setMasterData(List<CardMaster> cardData, List<KeywordMaster> keywordData, List<EnvironmentMaster> environmentList, List<DeckEntity> deckEntityList, List<LevelBonusList> bonusList, List<ClassText> classTextList)
	{
		this.cardData = new List<CardMaster>(cardData);
		this.keywordData = new List<KeywordMaster>(keywordData);
		this.environmentList = new List<EnvironmentMaster>(environmentList);
		this.deckEntityList = new List<DeckEntity>(deckEntityList);
		this.bonusList = new List<LevelBonusList>(bonusList);
		this.classTextList = new List<ClassText>(classTextList);
		finishGetMaster = true;
	}

	// カード取得系 ------------------------------------------------------------------------------------------------------------------------------------------------
	public CardMaster getCardEntity(string cardId)
	{
		CardMaster entity = cardData.FirstOrDefault(x => x.cardID.Equals(cardId));
		return entity;
	}

	public CardMaster getCardEntityByName(string cardName)
	{
		CardMaster master = cardData.FirstOrDefault(x => x.name.Equals(cardName));
		return master;
	}

	// 全カードのIDを取得（トークン以外）
	public List<string> getCardIdListWithoutToken()
	{
		List<string> cardNameList = new List<string>();
		cardNameList.AddRange(cardData.Where(c => !c.isToken).Select(x => x.cardID));
		return cardNameList;
	}

	// 全カードのカード名を取得
	public List<string> getCardName()
	{
		List<string> cardNameList = new List<string>();
		cardNameList.AddRange(cardData.Select(x => x.name));
		return cardNameList;
	}

	// 全カードのカード名を取得（トークン以外）
	public List<string> getCardNameWithoutToken()
	{
		List<string> cardNameList = new List<string>();
		cardNameList.AddRange(cardData.Where(c => !c.isToken).Select(x => x.name));
		return cardNameList;
	}

	// カード名からカードエンティティを取得
	public CardMaster GetCardEntityByName(string cardName)
	{
		return cardData.FirstOrDefault(c => c.name == cardName);
	}

	// カードIDからカードタイプを取得
	public CardType getCardType(string cardId)
	{
		CardMaster entity = getCardEntity(cardId);
		if (entity != null) return entity.cardType;
		else return CardType.UNIT;
	}

	// カードIDからカテゴリを取得
	public CardCategory getCardCategoryById(string cardId)
	{
		CardMaster entity = getCardEntity(cardId);
		if (entity != null) return entity.category;
		else return CardCategory.All;
	}

	// カード名からカテゴリを取得
	public CardCategory getCardCategoryByName(string cardName)
	{
		CardMaster entity = getCardEntityByName(cardName);
		if (entity != null) return entity.category;
		else return CardCategory.All;
	}

	public string getCardIdByName(string cardName)
	{
		CardMaster card = cardData.FirstOrDefault(x => x.name.Equals(cardName));
		if (card != null)
		{
			return card.cardID;
		}
		else
		{
			return cardName;
		}
	}
}
