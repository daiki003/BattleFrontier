
[System.Serializable]
public class SkillAndQuestData
{
	public string skill;
	public QuestType quest;
	public SkillAndQuestData(string skill, QuestType quest)
	{
		this.skill = skill;
		this.quest = quest;
	}
}
