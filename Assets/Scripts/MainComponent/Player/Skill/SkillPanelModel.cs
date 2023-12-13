using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanelModel
{
	public string skillPanelId;
	public string skillName;
	public Sprite icon;
	public int level;
	public int questLevel;
	public CardCategory category;
	public string text;
	public List<AbilityController> abilityList = new List<AbilityController>();
	public Quest quest;
	public bool isActive;
	public bool afterActivate;

	public SkillPanelModel(SkillPanelEntity skillEntity)
	{
		this.skillPanelId = skillEntity.skillPanelId;
		this.skillName = skillEntity.skillName;
		this.icon = skillEntity.icon;
		this.level = 0;
		this.questLevel = skillEntity.questLevel;
		this.category = skillEntity.category;
		this.text = skillEntity.text;
		this.isActive = !GameManager.instance.isBattle;
		this.abilityList = new List<AbilityController>();
	}
}