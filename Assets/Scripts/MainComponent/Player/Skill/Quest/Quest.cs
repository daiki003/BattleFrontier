using System.ComponentModel.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Quest
{
	public QuestType questType;
	public AbilityTiming countTiming;
	public string optionText;
	public List<int> standardValue;
	public string beforeText;
	public string afterText;
	public bool isActive;
	public int questCount;
	FilterCollection cardFilterCollection;

	public Quest(QuestEntity quest)
	{
		this.countTiming = quest.countTiming;
		this.questType = quest.questType;
		this.standardValue = quest.standardValue;
		this.beforeText = quest.beforeText;
		this.afterText = quest.afterText;
		cardFilterCollection = new FilterCollection(quest.optionText);
	}

	public string getQuestText(int questLevel = 1)
	{
		return beforeText + (beforeText == "" ? "" : "\n") + standardValue[questLevel - 1] + afterText;
	}

	public void advanceCount(List<CardController> activateCardList = null, int count = 1)
	{
		if (activateCardList == null) questCount += count;
		else
		{
			activateCardList = cardFilterCollection.filtering(activateCardList);
			if (activateCardList.Count > 0) questCount += count;
		}
	}

	public void resetQuest()
	{
		questCount = 0;
	}
}
