using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillAndQuest : MonoBehaviour
{
	[SerializeField] Image skillIcon;
	[SerializeField] Text questText, nameText;
	[SerializeField] GameObject selectedPanel;
	public SkillPanelEntity skill;
	public QuestEntity quest;
	
	public bool selectedFlag;

	public void init()
	{
		skillIcon.sprite = null;
		questText.text = "";
		nameText.text = "";
	}
	public void init(SkillPanelEntity skill, QuestEntity quest)
	{
		skillIcon.sprite = null;
		questText.text = "";
		nameText.text = "";
		setSkill(skill);
		setQuest(quest);
	}
	public void setSkill(SkillPanelEntity skillEntity)
	{
		skillIcon.sprite = skillEntity.icon;
		this.skill = skillEntity;
		this.nameText.text = this.skill.skillName;
	}
	public void setQuest(QuestEntity questEntity)
	{
		questText.text = questEntity.getQuestText(skill != null ? skill.questLevel : 1);
		this.quest = questEntity;
	}

	// 選択状態を切り替え
	public void switchSelected()
	{
		if (!BattleManager.instance.selectPanel.judgeCanSelect() && !selectedFlag) return;
		changeSelected(!selectedFlag);
	}
	public void changeSelected(bool selected)
	{
		selectedFlag = selected;
		selectedPanel.SetActive(selected);
	}
}
