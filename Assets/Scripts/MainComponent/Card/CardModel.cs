using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardModel
{
	// 基本情報
	public string cardId;
	public string cardName;
	public string instantText;
	public List<string> text = new List<string>();
	public List<string> evolveText = new List<string>();
	public string supplementText;
	public List<string> displayText
	{
		get
		{
			var returnList = new List<string>();
			if (GameManager.instance.isBattle)
			{
				returnList.Add(supplementText);
			}
			if (!string.IsNullOrEmpty(instantText))
			{
				returnList.Add(instantText);
				returnList.Add("---------------------------");
			}
			returnList.AddRange(text);
			return returnList;
		}
	}

	// ステータス
	public int cost
	{
		get { return Math.Max(applyInformation.fixedCost != -1 ? applyInformation.fixedCost : baseCost + applyInformation.costChange, 0); }
	}
	public int currentAttack
	{
		get
		{
			int attack = applyInformation.isEvolve ? baseAttack * 2 : baseAttack;
			if (applyInformation.fixedAttack != -1)
			{
				attack = applyInformation.fixedAttack;
			}
			return attack + applyInformation.attackBuff;
		}
	}
	public int currentHp
	{
		get
		{
			int hp = applyInformation.eternalApplyValue.isEvolve ? baseHp * 2 : baseHp;
			if (applyInformation.fixedHp != -1)
			{
				hp = applyInformation.fixedHp;
			}
			return hp + applyInformation.hpBuff - applyInformation.damage - applyInformation.advanceCount;
		}
	}
	public int currentMaxHp
	{
		get
		{
			int hp = applyInformation.isEvolve ? baseHp * 2 : baseHp;
			if (applyInformation.fixedHp != -1)
			{
				hp = applyInformation.fixedHp;
			}
			return hp + applyInformation.hpBuff;
		}
	}
	public int customPower { get { return applyInformation.customPower; } }
	public int chargeCount { get { return applyInformation.chargeCount; } }
	public int releaseCount { get { return applyInformation.releaseCount; } }
	public int uniqueCounter { get { return applyInformation.uniqueCounter; } }

	public int baseCost { get; private set; }
	public int baseAttack { get; private set; }
	public int baseHp { get; private set; }
	public int bootCost { get; private set; }
	public CardStatus destroyedParameter; // カードが破壊された時のパラメータを保存しておく
	public CardStatus bouncedParameter; // カードが手札に戻った時のパラメータを保存しておく

	// アクセラレート
	public bool canAccelerate;
	public int accelerateCost;
	public string accelerateCardId;

	// 対象選択
	public List<SelectComponent> selectComponent = new List<SelectComponent>();

	// アビリティとプロパティ
	public List<PropertyBase> propertyList = new List<PropertyBase>();
	public List<PropertyBase> evolvePropertyList = new List<PropertyBase>();
	public List<AbilityController> abilityList = new List<AbilityController>();
	public List<AbilityController> evolveAbilityList = new List<AbilityController>();
	public List<PropertyBase> allPropertyList
	{
		get
		{
			List<PropertyBase> properties = new List<PropertyBase>();
			properties.AddRange(propertyList);
			return properties;
		}
	}
	public List<AbilityController> allAbilityList
	{
		get
		{
			List<AbilityController> abilities = new List<AbilityController>();
			abilities.AddRange(abilityList);
			return abilities;
		}
	}

	// カードの保持する回数系
	public int soulCharge; // ソウルチャージの数

	// 変身情報
	public CardController originalCard; // 変身元のカード
	public CardController metamorphoseCard; // 変身先のカード

	// その他
	public ApplyInformation applyInformation; // 付与されている状態など（受けているダメージも含む）
	public bool isSelected;
	public bool isSelectTarget;
	public bool isShowCard = false;
	public CardType cardType;
	public Tribe tribe;
	public int canAttackCount; // 残り攻撃可能回数
	public int maxAttackCount; // 最大攻撃可能回数
	public bool isEnemy; // 敵のカードかどうか
	public int spiralCharge;
	public int nextAbilityIndex;
	public bool alreadyBoot;

	// 補足テキスト用構成要素
	public class SupplementTextComponent
	{
		public CountBase supplementTextComponent = new CountBase(multiple: 1, divideRoundUp: 1);
		public string supplementTextFirst = "";
		public string supplementTextSecond = "";
		public SupplementTextComponent(string baseText)
		{
			var firstSplit = baseText.Split('{');
			supplementTextFirst = firstSplit[0];
			var secondSplit = firstSplit[1].Split('}');
			supplementTextComponent = AbilityUtility.countFactory(secondSplit[0]);
			supplementTextSecond = secondSplit[1];
		}
	}
	public List<SupplementTextComponent> supplementTextComponentList = new List<SupplementTextComponent>();
	public CountBase supplementTextComponent = new CountBase(multiple: 1, divideRoundUp: 1);
	public string supplementTextFirst = "";
	public string supplementTextSecond = "";

	public CardModel()
	{
		
	}
	public CardModel(CardMaster cardMaster, bool isRevival = false, bool isTransform = false)
	{
		initModel(cardMaster, isRevival);
		if (!isTransform)
		{
			applyInformation = new ApplyInformation();
		}
	}

	public void initModel(CardMaster cardMaster, bool isRevival = false)
	{
		// 基本情報
		this.cardId = cardMaster.cardID;
		this.cardName = cardMaster.name;
		this.instantText = cardMaster.instantText;
		if (cardMaster.textList.Count > 0)
		{
			this.text.AddRange(cardMaster.textList);
		}
		else
		{
			this.text.Add(cardMaster.text);
		}
		if (cardMaster.evolveTextList.Count > 0)
		{
			this.evolveText.AddRange(cardMaster.evolveTextList);
		}
		else
		{
			this.evolveText.Add(cardMaster.evolveText);
		}

		// ステータス
		baseCost = cardMaster.status.cost;
		baseAttack = cardMaster.status.attack;
		baseHp = cardMaster.status.hp;
		bootCost = cardMaster.bootCost;

		// アクセラレート
		canAccelerate = cardMaster.accelerate.canAccelerate;
		accelerateCost = cardMaster.accelerate.cost;
		accelerateCardId = cardMaster.accelerate.accelerateCardId;

		// 選択リスト
		foreach (CardMaster.SelectMasterComponent selectMasterComponent in cardMaster.selectList)
		{
			selectComponent.Add(new SelectComponent(selectMasterComponent));
		}

		isSelected = false;
		cardType = cardMaster.cardType;
		tribe = cardMaster.tribe;
		propertyList = new List<PropertyBase>();
		foreach (PropertyTrait propertyTrait in cardMaster.propertyList)
		{
			if (!isRevival || propertyTrait.propertyType != "revival")
			{
				propertyList.Add(AbilityUtility.propertyFactory(propertyTrait));
			}
		}
		if (cardMaster.evolvePropertyList.Count == 0)
		{
			evolvePropertyList = new List<PropertyBase>(propertyList);
		}
		else
		{
			foreach (PropertyTrait propertyTrait in cardMaster.evolvePropertyList)
			{
				if (!isRevival || propertyTrait.propertyType != "revival")
				{
					evolvePropertyList.Add(AbilityUtility.propertyFactory(propertyTrait));
				}
			}
		}
		abilityList = new List<AbilityController>();

		foreach (string supplementComponentText in cardMaster.supplementComponentText)
		{
			if (supplementComponentText.Contains('{') && supplementComponentText.Contains('}'))
			{
				supplementTextComponentList = new List<SupplementTextComponent>();
				supplementTextComponentList.Add(new SupplementTextComponent(supplementComponentText));
			}
		}
	}
}
