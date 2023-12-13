using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestEntity", menuName = "Create QuestEntity")]

[System.Serializable]
public class QuestEntity : ScriptableObject
{
	public QuestType questType;
	public AbilityTiming countTiming;
	public string optionText;
	public List<int> standardValue;
	public string beforeText;
	public string afterText;

	public string getQuestText(int questLevel = 1)
	{
		return beforeText + (beforeText == "" ? "" : "\n") + standardValue[questLevel - 1] + afterText;
	}
}
