using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[Serializable]
public class PlayerValiableParameter
{
	public int hp;
	public int shield;
	public int cost;
	public int maxCost;
	public int levelUpCost;
	public int turnPlayCount;
	public int turnDiscardCount;
	public int turnDestroyCount;
	public int skillPoint;
	public int exchangeCount;
	public bool focusHand;
	public bool turnFirstExchange;
	public int backWaterCount;
}

[Serializable]
public class PlayerModel
{	
	// バトル中に変化するもの --------------------------------------------------------------------------------------------------------------
	public PlayerValiableParameter valiableParameter = new PlayerValiableParameter();
	public int hp { get { return valiableParameter.hp; }  }
	public int shield { get { return valiableParameter.shield; }  }
	public int cost { get { return valiableParameter.cost; }  }
	public int maxCost { get { return valiableParameter.maxCost; }  }
	public int levelUpCost { get { return valiableParameter.levelUpCost; }  }
	public int turnPlayCount { get { return valiableParameter.turnPlayCount; }  }
	public int turnDiscardCount { get { return valiableParameter.turnDiscardCount; }  }
	public int turnDestroyCount { get { return valiableParameter.turnDestroyCount; }  }
	public int skillPoint { get { return valiableParameter.skillPoint; }  }
	public int exchangeCount { get { return valiableParameter.exchangeCount; }  }
	public bool focusHand { get { return valiableParameter.focusHand; }  }
	public bool turnFirstExchange { get { return valiableParameter.turnFirstExchange; }  }
	public int backWaterCount { get { return valiableParameter.backWaterCount; }  }


	// リーダーの持つ能力 ----------------------------------------------------------------------------------------------------------------------------
	public List<SkillAndQuestData> masterSkill = new List<SkillAndQuestData>();
	public List<SkillPanelController> skillList = new List<SkillPanelController>();
	public List<AbilityController> abilityList = new List<AbilityController>();
	public List<PropertyBase> propertyList = new List<PropertyBase>();

	public PlayerModel() {}

	// set ----------------------------------------------------------------------------------------------------------------------
	public void setValiableParameter(PlayerValiableParameter parameter) { valiableParameter = parameter; }
	public void setHp(int hp) { valiableParameter.hp = hp; }
	public void setShield(int shield) { valiableParameter.shield = shield; }
	public void setCost(int cost) { valiableParameter.cost = cost; }
	public void setMaxCost(int maxCost) { valiableParameter.maxCost = maxCost; }
	public void setLevelUpCost(int levelUpCost) { valiableParameter.levelUpCost = levelUpCost; }
	public void setSkillPoint(int skillPoint) { valiableParameter.skillPoint = skillPoint; }
	public void setExchangeCount(int soldCount) { valiableParameter.exchangeCount = soldCount; }
	public void setPlayCount(int count) { valiableParameter.turnPlayCount = count; }
	public void setDiscardCount(int count) { valiableParameter.turnDiscardCount = count; }
	public void setDestroyCount(int count) { valiableParameter.turnDestroyCount = count; }
	public void setFocusHand(bool focus) { valiableParameter.focusHand = focus; }
	public void setTurnFirstExchange(bool isFirst) { valiableParameter.turnFirstExchange = isFirst; }
	public void setBackWaterCount(int count) { valiableParameter.backWaterCount = count; }

	public void setMasterSkill(List<SkillAndQuestData> skill) { this.masterSkill = skill; }
}