using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillPanelEntity", menuName = "Create SkillPanelEntity")]

public class SkillPanelEntity : ScriptableObject
{
	public string skillPanelId;
	public string skillName;
	public Sprite icon;
	public CardCategory category;
	public int questLevel = 1;
	[Multiline] public string text;
	public List<AbilityTrait> abilityList;
}

