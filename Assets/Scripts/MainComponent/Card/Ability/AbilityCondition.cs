using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class AbilityCondition
{
	public string countText = "";
	public string target = "";
}

public class ConditionBase
{
	public virtual bool judgeCondition(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return true;
	}
}

public class CountCondition: ConditionBase
{
	public CountBase count;
	protected CountBase value;
	protected Func<int, int, bool> compareFunc;
	private List<char> opeList = new List<char>() { '=', '<', '>', '!' };
	public CountCondition(string conditionText) : base()
	{
		// 左辺は{}で囲まれている前提
		int firstStart = conditionText.IndexOf("{");
		int firstEnd = conditionText.IndexOf("}");
		string leftCountText = conditionText.Substring(firstStart, firstEnd - firstStart).Trim('{', '}');

		conditionText = conditionText.Substring(firstEnd + 1);
		string ope = "";
		while (opeList.Contains(conditionText.First()))
		{
			ope += conditionText.First();
			conditionText = conditionText.Remove(0, 1);
		}

		string rightCountText = conditionText.Trim('{', '}');

		this.count = AbilityUtility.countFactory(leftCountText);
		this.value = AbilityUtility.countFactory(rightCountText);
		this.compareFunc = getCompareFunc(ope);
	}

	public override bool judgeCondition(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return compareFunc(count.getActualCount(option, processOption, activateCount), value.getActualCount(option, processOption, activateCount));
	}

	public Func<int, int, bool> getCompareFunc(string ope)
	{
		switch (ope)
		{
			case "=":
				return (a, b) => { return a == b; };
			case "!=":
				return (a, b) => { return a != b; };
			case ">":
				return (a, b) => { return a > b; };
			case "<":
				return (a, b) => { return a < b; };
			case ">=":
				return (a, b) => { return a >= b; };
			case "<=":
				return (a, b) => { return a <= b; };
			default:
				return (a, b) => { return true; };
		}
	}
}

public class PlaceCondition : ConditionBase
{
	public CardState targetState;
	public PlaceCondition(string conditionText) : base()
	{
		Enum.TryParse(conditionText.ToUpper(), out targetState);
	}

	public override bool judgeCondition(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return option.ownerCard.cardState == targetState;
	}
}

public class IsBattleCondition : ConditionBase
{
	public bool isBattle;
	public IsBattleCondition(string value) : base()
	{
		isBattle = Boolean.Parse(value);
	}
	public override bool judgeCondition(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return GameManager.instance.isBattlePhase == isBattle;
	}
}