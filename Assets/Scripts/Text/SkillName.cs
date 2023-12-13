using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillName
{
	List<SkillPanelEntity> skillEntityList = new List<SkillPanelEntity>();
	List<QuestEntity> questList = new List<QuestEntity>();
	Dictionary<CardCategory, List<SkillPanelEntity>> skillEntityListEachCategory = 
											new Dictionary<CardCategory, List<SkillPanelEntity>>();

	public static SkillName instance;
	public void init()
	{
		if (instance == null)
		{
			instance = this;
		}
	}
	public SkillPanelEntity getSkillEntity(string skillId)
	{
		SkillPanelEntity entity = skillEntityList.FirstOrDefault(x => x.skillPanelId.Equals(skillId));
		return entity;
	}

	// スキルをランダムに取得
	public SkillPanelEntity getRandomSkillEntity()
	{
		int randomIndex = RandomUtility.random(skillEntityList.Count);
		return skillEntityList[randomIndex];
	}

	// 指定したカテゴリの全カードのIDを取得
	public List<string> getSkillIdListByCategory(CardCategory category)
	{
		List<string> skillNameList = new List<string>();
		skillNameList.AddRange(skillEntityListEachCategory[category].Select(x => x.skillPanelId));
		return skillNameList;
	}

	// トークンと敵以外の全カードのIDを取得
	public List<string> getSkillIdListWithoutToken()
	{
		List<string> skillNameList = new List<string>();
		foreach (CardCategory category in Enum.GetValues(typeof(CardCategory)))
		{
			if (category != CardCategory.Token && category != CardCategory.Enemy && category != CardCategory.Boss)
			{
				skillNameList.AddRange(getSkillIdListByCategory(category));
			}
		}
		return skillNameList;
	}
}
