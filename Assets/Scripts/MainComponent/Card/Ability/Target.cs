using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

[System.Serializable]
public class TargetTrait
{
	public TargetType targetType;
	public string detailFilterText;
}

public class TargetBase
{
	public FilterCollection filterCollection;
	protected List<CardController> targetCardList = new List<CardController>();

	protected PlayerController player;
	protected PlayerController enemy;

	public TargetBase() { }
	public TargetBase(string detailFilterText)
	{
		filterCollection = new FilterCollection(detailFilterText);
		if (GameManager.instance.isBattle)
		{
			player = BattleManager.instance.player;
			enemy = BattleManager.instance.enemy;
		}
	}

	// ownerCardだけを指定してgetTargetCardsを呼びたいとき用
	public List<CardController> getTargetCards(CardController ownerCard)
	{
		return getTargetCards(new AbilityProcessor.ActivateOption(ownerCard));
	}

	public List<CardController> getTargetCards(AbilityProcessor.ActivateOption activateOption = null, AbilityProcessor.ProcessOption processOption = null)
	{
		if (activateOption == null) activateOption = new AbilityProcessor.ActivateOption(ownerCard: player.cardCollection.substanceCard);
		if (processOption == null) processOption = new AbilityProcessor.ProcessOption();
		targetCardList = new List<CardController>();
		targetCardList = getTargetCardList(activateOption, processOption);

		if (filterCollection.optionFilterList.Any(f => f is WithoutSelfFilter))
		{
			// 自分自身を含めないフィルター
			targetCardList = targetCardList.Where(c => c != activateOption.ownerCard).ToList();
		}
		else if (filterCollection.optionFilterList.Any(f => f is OnlySelfFilter))
		{
			// 自分自身のみ返すフィルター
			targetCardList = targetCardList.Where(c => c == activateOption.ownerCard).ToList();
		}
		
		return filteringTarget(targetCardList, activateOption);
	}

	public virtual List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		return new List<CardController>();
	}

	// 対象のカードリストを絞り込み
	public List<CardController> filteringTarget(List<CardController> targetList, AbilityProcessor.ActivateOption activateOption)
	{
		targetList = filterCollection.filtering(targetList, activateOption);
		return targetList;
	}
}

public class NoneTarget : TargetBase
{
	public NoneTarget() : base("") { }
}

public class AttackedTarget : TargetBase
{
	public AttackedTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		if (option.attackedCard != null)
		{
			targetCardList.Add(option.attackedCard);
		}
		return targetCardList;
	}
}

public class SelectTarget : TargetBase
{
	public SelectTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		if (option.selectCard != null)
		{
			targetCardList.AddRange(option.selectCard);
		}
		return targetCardList;
	}
}

public class ChoiceTarget : TargetBase
{
	public ChoiceTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		if (option.selectCard != null)
		{
			targetCardList.AddRange(option.selectCard);
		}
		return targetCardList;
	}
}

public class ActivateCardTarget : TargetBase
{
	public ActivateCardTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		targetCardList.AddRange(option.activateCardList);
		return targetCardList;
	}
}

public class LastTarget : TargetBase
{
	public LastTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		if (processOption.lastTarget != null)
		{
			targetCardList.AddRange(processOption.lastTarget);
		}
		return targetCardList;
	}
}

public class SkillDrewCardTarget : TargetBase
{
	public SkillDrewCardTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		if (processOption.skillDrewCard != null)
		{
			targetCardList.AddRange(processOption.skillDrewCard);
		}
		return targetCardList;
	}
}

public class SkillSummonCardTarget : TargetBase
{
	public SkillSummonCardTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		if (processOption.skillSummonCard != null)
		{
			targetCardList.AddRange(processOption.skillSummonCard);
		}
		return targetCardList;
	}
}

public class SelfTarget : TargetBase
{
	public SelfTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		if (option != null)
		{
			targetCardList.Add(option.ownerCard);
		}
		return targetCardList;
	}
}

public class HandTarget : TargetBase
{
	public HandTarget(string detailFilterText) : base(detailFilterText) { }

	public override List<CardController> getTargetCardList(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		targetCardList.AddRange(player.cardCollection.handCardList);
		return targetCardList;
	}
}