using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class AbilityPreprocess
{
	public abstract void start(AbilityProcessor.ActivateOption option);
	public abstract bool isRight(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption, int activateCount);
}

public class NonePreprocess : AbilityPreprocess
{
	public override void start(AbilityProcessor.ActivateOption option)
	{

	}

	public override bool isRight(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption, int activateCount)
	{
		return true;
	}
}

public class PreprocessCondition : AbilityPreprocess
{
	ConditionBase condition;
	public PreprocessCondition(string option)
	{
		condition = AbilityUtility.conditionFactory(option);
	}

	public override void start(AbilityProcessor.ActivateOption option)
	{

	}

	public override bool isRight(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption, int activateCount)
	{
		return condition.judgeCondition(option, processOption, activateCount);
	}
}