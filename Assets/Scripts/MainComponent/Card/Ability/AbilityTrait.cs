using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class AbilityTrait
{
	public string abilityType;
	public string timing;
	public List<string> condition = new List<string>();
	public string target = "";
	public string option = "";
	public string preprocess = "";
	public string effectName = "";

	public AbilityTrait() {}

	public AbilityTrait(string type, string timing, List<string> condition, string target, string option, string effectName = "")
	{
		this.abilityType = type;
		this.timing = timing;
		this.condition = new List<string>(condition);
		this.target = target;
		this.option = option;
		this.effectName = effectName;
	}
}