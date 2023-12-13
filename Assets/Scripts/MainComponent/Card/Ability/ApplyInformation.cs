using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ApplyValue
{
	public int costChange;
	public int attackBuff;
	public int hpBuff;
	public int advancedCount;
	public int fixedCost = -1;
	public int fixedAttack = -1;
	public int fixedHp = -1;
	public int customPower;
	public int returnCount;
	public int chargeCount;
	public int releaseCount;
	public int uniqueCounter;
	public int damage;
	public bool isEvolve;

	public ApplyValue() {}
	public ApplyValue(ApplyValue originalApplyValue)
	{
		costChange = originalApplyValue.costChange;
		attackBuff = originalApplyValue.attackBuff;
		hpBuff = originalApplyValue.hpBuff;
		advancedCount = originalApplyValue.advancedCount;
		fixedCost = originalApplyValue.fixedCost;
		fixedAttack = originalApplyValue.fixedAttack;
		fixedHp = originalApplyValue.fixedHp;
		customPower = originalApplyValue.customPower;
		returnCount = originalApplyValue.returnCount;
		chargeCount = originalApplyValue.chargeCount;
		damage = originalApplyValue.damage;
		isEvolve = originalApplyValue.isEvolve;
	}

	public void reset()
	{
		damage = 0;
		attackBuff = 0;
		hpBuff = 0;
		costChange = 0;
		advancedCount = 0;
		fixedCost = -1;
		fixedAttack = -1;
		fixedHp = -1;
		customPower = 0;
		returnCount = 0;
		chargeCount = 0;
		isEvolve = false;
	}
}

[System.Serializable]
public class ApplyInformation
{
	// 永続的な能力変化
	public ApplyValue eternalApplyValue = new ApplyValue();
	// 一時的な能力変化（主に戦闘中）
	public ApplyValue tempApplyValue = new ApplyValue();
	public int damage { get { return tempApplyValue.damage; } }
	public int costChange { get { return eternalApplyValue.costChange; } }
	public int attackBuff { get { return eternalApplyValue.attackBuff + tempApplyValue.attackBuff; } }
	public int hpBuff { get { return eternalApplyValue.hpBuff + tempApplyValue.hpBuff; } }
	public int advanceCount { get { return eternalApplyValue.advancedCount + tempApplyValue.advancedCount; } }
	public int fixedCost { get { return eternalApplyValue.fixedCost; } }
	public int fixedAttack { get { return tempApplyValue.fixedAttack != -1 ? tempApplyValue.fixedAttack : eternalApplyValue.fixedAttack; } }
	public int fixedHp { get { return tempApplyValue.fixedHp != -1 ? tempApplyValue.fixedHp : eternalApplyValue.fixedHp; } }
	public int customPower { get { return eternalApplyValue.customPower + tempApplyValue.customPower; } }
	public int returnCount { get { return eternalApplyValue.returnCount + tempApplyValue.returnCount; } }
	public int chargeCount { get { return GameManager.instance.isBattlePhase ? tempApplyValue.chargeCount : eternalApplyValue.chargeCount; } }
	public int releaseCount { get { return GameManager.instance.isBattlePhase ? tempApplyValue.releaseCount : eternalApplyValue.releaseCount; } }
	public int uniqueCounter { get { return tempApplyValue.uniqueCounter + eternalApplyValue.uniqueCounter; } }
	public bool isEvolve { get { return eternalApplyValue.isEvolve || tempApplyValue.isEvolve; } }

	// アビリティとプロパティ
	public List<PropertyBase> attachedPropertyList = new List<PropertyBase>();
	public List<PropertyBase> tempAttachedPropertyList = new List<PropertyBase>();
	public List<PropertyBase> tempRemovedPropertyList = new List<PropertyBase>();
	public List<AttachPropertyData> attachedPropertyDataList = new List<AttachPropertyData>(); // 保存用

	public List<AbilityController> attachedAbilityList = new List<AbilityController>();
	public List<int> removedAbilityIndexList = new List<int>();
	public List<AbilityController> tempAttachedAbilityList = new List<AbilityController>();
	public List<int> tempRemovedAbilityIndexList = new List<int>();
	public List<AttachAbilityData> attachedAbilityDataList = new List<AttachAbilityData>(); // 保存とかかっている効果表示用
	public List<AttachAbilityData> actualAttachAbility
	{
		get
		{
			List<AttachAbilityData> returnList = new List<AttachAbilityData>();
			returnList.AddRange(attachedAbilityDataList.Where(abilityBlock => !removedAbilityIndexList.Contains(abilityBlock.index)));
			return returnList;
		}
	}

	public const int NONE = -1;

	[System.Serializable]
	public class AttachPropertyData
	{
		public PropertyTrait propertyTrait;
		public string propertyText;
		public string ownerCardId;
		public bool isTemp;
		public bool displayTanzaku;
		public AttachPropertyData() {}
		public AttachPropertyData(PropertyBase property, CardController ownerCard, bool isTemp = false, bool displayTanzaku = true)
		{
			this.propertyTrait = property.propertyTrait;
			this.propertyText = property.displayText;
			this.ownerCardId = ownerCard.model.cardId;
			this.isTemp = isTemp;
			this.displayTanzaku = displayTanzaku;
		}
	}
	
	[System.Serializable]
	public class AttachAbilityData
	{
		public int index;
		public List<AbilityTrait> attachAbilityList;
		public string ownerCardId;
		public string attachText;
		public bool isTemp;
		public AttachAbilityData()
		{
			attachAbilityList = new List<AbilityTrait>();
		}
		public AttachAbilityData(List<AbilityTrait> attachAbilityList, CardController ownerCard, string attachText, bool isTemp = false)
		{
			this.attachAbilityList = attachAbilityList;
			this.ownerCardId = ownerCard.model.cardId;
			this.attachText = attachText;
			this.isTemp = isTemp;
		}
	}

	[System.Serializable]
	public class SaveComponent
	{
		public ApplyValue eternalApplyValue;
		public List<AttachPropertyData> attachPropertyList;
		public List<AttachAbilityData> attachAbilityList;
		public SaveComponent()
		{
			this.attachPropertyList = new List<AttachPropertyData>();
			this.attachAbilityList = new List<AttachAbilityData>();
		}
		public SaveComponent(ApplyValue eternalApplyValue, List<AttachPropertyData> attachPropertyList, List<AttachAbilityData> attachAbilityList)
		{
			this.eternalApplyValue = eternalApplyValue;
			this.attachPropertyList = attachPropertyList;
			this.attachAbilityList = attachAbilityList;
		}
	}

	// 保存用にSaveComponentを作成
	public SaveComponent createSaveComponent()
	{
		SaveComponent saveComponent = new SaveComponent(eternalApplyValue, attachedPropertyDataList, attachedAbilityDataList);
		return saveComponent;
	}
	// 保存用のSaveComponentからApplyInformationを復元
	public void restoration(SaveComponent saveComponent, CardController ownerCard)
	{
		this.eternalApplyValue = saveComponent.eternalApplyValue;
		this.attachedPropertyDataList.AddRange(saveComponent.attachPropertyList);
		this.attachedAbilityDataList.AddRange(saveComponent.attachAbilityList);

		// テキストから実際のプロパティを作成
		foreach (AttachPropertyData propertyData in saveComponent.attachPropertyList)
		{
			attachedPropertyList.Add(AbilityUtility.propertyFactory(propertyData.propertyTrait));
		}
		
		// AbilityuTraitから実際のアビリティを作成
		foreach (AttachAbilityData abilityBlock in saveComponent.attachAbilityList)
		{
			foreach (AbilityTrait abilityTrait in abilityBlock.attachAbilityList)
			{
				AbilityController ability = AbilityUtility.createAbilityController(abilityTrait, ownerCard);
				attachedAbilityList.Add(ability);
			}
		}
	}

	// ステータスの付与値を個別に設定 --------------------------------------------------------------------------------------------------------------
	private ApplyValue getTargetApplyValue(bool isEternal)
	{
		return BattleManager.instance.isBattlePhase && !isEternal ? tempApplyValue : eternalApplyValue;
	}

	public void addDamage(int damage)
	{
		this.tempApplyValue.damage += damage;
		if (this.tempApplyValue.damage <= 0)
		{
			this.tempApplyValue.damage = 0;
		}
	}

	public void fluctuateCost(int costChange)
	{
		this.eternalApplyValue.costChange += costChange;
	}

	public void addAttackBuff(int attackBuff, bool eternal = false)
	{
		var targetApplyValue = getTargetApplyValue(eternal);
		targetApplyValue.attackBuff += attackBuff;
	}

	public void addHpBuff(int hpBuff, bool eternal = false)
	{
		var targetApplyValue = getTargetApplyValue(eternal);
		targetApplyValue.hpBuff += hpBuff;
	}

	public void setCost(int setCost)
	{
		// これまでのバフは全て取り除く
		eternalApplyValue.costChange = 0;
		eternalApplyValue.fixedCost = setCost;
	}

	public void setAttack(int setAttack, bool eternal = false)
	{
		var targetApplyValue = getTargetApplyValue(eternal);
		// これまでのバフは全て取り除く
		targetApplyValue.attackBuff = 0;
		targetApplyValue.fixedAttack = setAttack;
	}

	public void setHp(int setHp, bool eternal = false)
	{
		var targetApplyValue = getTargetApplyValue(eternal);
		// これまでのバフは全て取り除く
		targetApplyValue.hpBuff = 0;
		targetApplyValue.fixedHp = setHp;
	}

	public void fluctuateCustomPower(int power)
	{
		this.eternalApplyValue.customPower += power;
	}

	public void fluctuateReturnCount(int count)
	{
		this.eternalApplyValue.returnCount += count;
	}

	public void addChargeCount(int count)
	{
		var targetApplyValue = getTargetApplyValue(isEternal: false);
		targetApplyValue.chargeCount += count;
	}

	public void resetChargeCount()
	{
		var targetApplyValue = getTargetApplyValue(isEternal: false);
		targetApplyValue.chargeCount = 0;
	}

	public void addReleaseCount(int count)
	{
		var targetApplyValue = getTargetApplyValue(isEternal: false);
		targetApplyValue.releaseCount += count;
	}

	public void addUniqueCounter(int count)
	{
		var targetApplyValue = getTargetApplyValue(isEternal: false);
		targetApplyValue.uniqueCounter += count;
	}

	// プロパティやアビリティの付与 --------------------------------------------------------------------------------------------------------------
	public void attachProperty(PropertyBase property, CardController ownerCard, bool displayTanzaku)
	{
		if (BattleManager.instance.isBattlePhase)
		{
			tempAttachedPropertyList.Add(property);
			ActionVfx actionVfx = new ActionVfx(() =>
			{
				attachedPropertyDataList.Add(new AttachPropertyData(property, ownerCard, isTemp: true, displayTanzaku));
			});
		}
		else
		{
			attachedPropertyList.Add(property);
			attachedPropertyDataList.Add(new AttachPropertyData(property, ownerCard, displayTanzaku: displayTanzaku));
		}
	}

	public void attachProperty(List<PropertyBase> propertyList)
	{
		if (BattleManager.instance.isBattlePhase)
		{
			tempAttachedPropertyList.AddRange(propertyList);
		}
		else
		{
			attachedPropertyList.AddRange(propertyList);
		}
	}

	public void removeProperty(List<PropertyBase> property)
	{
		tempRemovedPropertyList.AddRange(property);
	}

	public void attachAbility(AbilityController attachAbility, CardController ownerCard, string attachText)
	{
		attachedAbilityList.Add(attachAbility);
	}

	public void attachAbility(List<AbilityTrait> attachAbilityList, CardController targetCard, CardController ownerCard, string attachText)
	{
		List<AbilityController> abilityControllerList = new List<AbilityController>();
		foreach (AbilityTrait abilityTrait in attachAbilityList)
		{
			AbilityController ability = AbilityUtility.createAbilityController(abilityTrait, targetCard);
			abilityControllerList.Add(ability);
		}
		if (BattleManager.instance.isBattlePhase)
		{
			tempAttachedAbilityList.AddRange(abilityControllerList);
			ActionVfx actionVfx = new ActionVfx(() =>
			{
				attachedAbilityDataList.Add(new AttachAbilityData(attachAbilityList, ownerCard, attachText, isTemp: true));
			});
			actionVfx.addToAllBlockList();
		}
		else
		{
			attachedAbilityList.AddRange(abilityControllerList);
			attachedAbilityDataList.Add(new AttachAbilityData(attachAbilityList, ownerCard, attachText));
		}
	}

	public void removeAbility(List<int> abilityIndexList)
	{
		if (BattleManager.instance.isBattlePhase)
		{
			tempRemovedAbilityIndexList.AddRange(abilityIndexList);
		}
		else
		{
			removedAbilityIndexList.AddRange(abilityIndexList);
		}
	}

	// 付与値のリセット -------------------------------------------------------------------------------------------------------------
	public void resetTempApply()
	{
		this.tempApplyValue.reset();
		tempAttachedPropertyList = new List<PropertyBase>();
		tempRemovedPropertyList = new List<PropertyBase>();
		tempAttachedAbilityList = new List<AbilityController>();
		tempRemovedAbilityIndexList = new List<int>();
		ActionVfx actionVfx = new ActionVfx(() =>
		{
			attachedAbilityDataList.RemoveAll(a => a.isTemp);
		});
		actionVfx.addToAllBlockList();
	}

	// コピー
	public ApplyInformation copyInformation()
	{
		ApplyInformation applyInformation = new ApplyInformation();
		applyInformation.eternalApplyValue = new ApplyValue(this.eternalApplyValue);
		applyInformation.tempApplyValue = new ApplyValue(this.tempApplyValue);

		applyInformation.attachedPropertyList = new List<PropertyBase>(this.attachedPropertyList);
		applyInformation.tempAttachedPropertyList = new List<PropertyBase>(this.tempAttachedPropertyList);
		applyInformation.tempRemovedPropertyList = new List<PropertyBase>(this.tempRemovedPropertyList);
		applyInformation.attachedAbilityList = new List<AbilityController>(this.attachedAbilityList);
		applyInformation.removedAbilityIndexList = new List<int>(this.removedAbilityIndexList);
		applyInformation.tempAttachedAbilityList = new List<AbilityController>(this.tempAttachedAbilityList);
		applyInformation.tempRemovedAbilityIndexList = new List<int>(this.tempRemovedAbilityIndexList);
		applyInformation.attachedAbilityDataList = new List<AttachAbilityData>(this.attachedAbilityDataList);

		return applyInformation;
	}
}
