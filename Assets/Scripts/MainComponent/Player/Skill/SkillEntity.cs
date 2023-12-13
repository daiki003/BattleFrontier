using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [System.Serializable]
public class SkillEntity
{
	public string name;
	public int cost;
	[Multiline] public string text;
	public List<AbilityTrait> abilityList = new List<AbilityTrait>();
	public SkillEntity(SkillEntity skill)
	{
		this.name = skill.name;
		this.cost = skill.cost;
		this.text = skill.text;
		for(int i=0; i < skill.abilityList.Count; i++)
		{
			abilityList.Add(skill.abilityList[i]);
		}
	}

}
