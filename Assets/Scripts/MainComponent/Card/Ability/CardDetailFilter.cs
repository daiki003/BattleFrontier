using System.Runtime.InteropServices.ComTypes;
using System.Globalization;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class FilterCollection
{
	public List<DetailFilter> detailFilterList = new List<DetailFilter>();
	public SelectFilter selectFilter = new AllFilter();
	public List<OptionFilter> optionFilterList = new List<OptionFilter>();
	public FilterCollection(string filterText)
	{
		if (filterText == null) return;
		string[] partTextList = filterText.Split('&');
		foreach (string partText in partTextList)
		{
			CardFilter cardFilter = AbilityUtility.cardFilterFactory(partText);
			if (cardFilter is DetailFilter)
			{
				detailFilterList.Add((DetailFilter)cardFilter);
			}
			else if (cardFilter is SelectFilter)
			{
				selectFilter = (SelectFilter)cardFilter;
			}
			else if (cardFilter is OptionFilter)
			{
				optionFilterList.Add((OptionFilter)cardFilter);
			}
		}
	}

	public List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		List<CardController> returnList = new List<CardController>(cardList);
		foreach (DetailFilter detailFilter in detailFilterList)
		{
			returnList = detailFilter.filtering(returnList, activateOption);
		}
		returnList = selectFilter.filtering(returnList, activateOption);

		return returnList;
	}
}

public class CardFilter
{

}

public class DetailFilter : CardFilter
{
	public DetailFilter()
	{

	}

	public virtual List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList;
	}
}


// StringValue系 ------------------------------------------------------------------------------------------------------------------------
public class StringValueFilter : DetailFilter
{
	public string value;
	public bool equal;
	public StringValueFilter(string value, string ope) : base()
	{
		this.value = value;
		this.equal = ope == "=";
	}
}

public class CardIdFilter : StringValueFilter
{
	public CardIdFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		List<string> cardIdList = value.Split(':').ToList();
		return cardList.Where(c => (cardIdList.Contains(c.model.cardId)) == equal).ToList();
	}
}

public class CardTypeFilter : StringValueFilter
{
	public CardTypeFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Where(c => (c.model.cardType.ToString().ToLower() == this.value) == equal).ToList();
	}
}

public class TribeFilter : StringValueFilter
{
	public TribeFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Where(c => (c.model.tribe.ToString().ToLower() == this.value) == equal).ToList();
	}
}

public class CardStateFilter : StringValueFilter
{
	public CardStateFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Where(c => (c.cardState.ToString().ToLower() == this.value) == equal).ToList();
	}
}

public class AbilityTimingFilter : StringValueFilter
{
	public AbilityTimingFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Where(c => c.model.abilityList.Any(a => (a.abilityTrait.timing.ToString().ToLower() == value) == equal)).ToList();
	}
}

// IntValue系 ------------------------------------------------------------------------------------------------------------------------
public class IntValueFilter : DetailFilter
{
	protected int value;
	protected Func<int, int, bool> compareFunc;
	public IntValueFilter(string value, string ope) : base()
	{
		this.value = int.Parse(value);
		this.compareFunc = getCompareFunc(ope);
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

public class CostFilter : IntValueFilter
{
	public CostFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Where(c => compareFunc(c.model.cost, value)).ToList();
	}
}

public class AttackFilter : IntValueFilter
{
	public AttackFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Where(c => compareFunc(c.model.currentAttack, value)).ToList();
	}
}

public class HpFilter : IntValueFilter
{
	public HpFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Where(c => compareFunc(c.model.currentHp, value)).ToList();
	}
}

public class SoulChargeFilter : IntValueFilter
{
	public SoulChargeFilter(string value, string ope) : base(value, ope) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Where(c => compareFunc(c.model.soulCharge, value)).ToList();
	}
}

// BoolValue系 ------------------------------------------------------------------------------------------------------------------------------------
public class BoolValueFilter : DetailFilter
{
	protected bool value;
	public BoolValueFilter(string value) : base()
	{
		this.value = value == "true";
	}
}

public class EnemyFilter : BoolValueFilter
{
	public EnemyFilter(string value) : base(value) { }

	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		bool isEnemyOwner = activateOption != null ? activateOption.ownerCard.model.isEnemy : false;
		return cardList.Where(c => (c.model.isEnemy != isEnemyOwner) == value).ToList();
	}
}

// 対象選択系 ---------------------------------------------------------------------------------------------------------------------------------------
public class SelectFilter : CardFilter
{
	protected int number;
	public SelectFilter(string number) : base()
	{
		if (!int.TryParse(number, out int a)) Debug.Log(number);
		this.number = int.Parse(number);
	}

	public virtual List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList;
	}
}

public class AllFilter : SelectFilter
{
	public AllFilter() : base("0") { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList;
	}
}

public class RandomFilter : SelectFilter
{
	public RandomFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		if (cardList.Count <= number)
		{
			return cardList;
		}
		else
		{
			List<int> indexList = RandomUtility.random(cardList.Count, number);
			List<CardController> returnList = new List<CardController>();
			foreach (int index in indexList)
			{
				returnList.Add(cardList[index]);
			}
			return returnList;
		}
	}
}

public class RandomWithoutSelfFilter : RandomFilter
{
	public RandomWithoutSelfFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		List<CardController> candidateCardList = cardList.Where(c => c != activateOption.ownerCard).ToList();
		return base.filtering(candidateCardList, activateOption);
	}
}

public class MinHpFilter : SelectFilter
{
	public MinHpFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		cardList = cardList.OrderBy(c => c.model.currentHp).ToList();
		return cardList.Take(number).ToList();
	}
}

public class MaxHpFilter : SelectFilter
{
	public MaxHpFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		cardList = cardList.OrderByDescending(c => c.model.currentHp).ToList();
		return cardList.Take(number).ToList();
	}
}

public class MinAttackFilter : SelectFilter
{
	public MinAttackFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		cardList = cardList.OrderBy(c => c.model.currentAttack).ToList();
		return cardList.Take(number).ToList();
	}
}

public class MaxAttackFilter : SelectFilter
{
	public MaxAttackFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		cardList = cardList.OrderByDescending(c => c.model.currentAttack).ToList();
		return cardList.Take(number).ToList();
	}
}

public class MaxCostFilter : SelectFilter
{
	public MaxCostFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		cardList = cardList.OrderByDescending(c => c.model.cost).ToList();
		return cardList.Take(number).ToList();
	}
}

public class OldestFilter : SelectFilter
{
	public OldestFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		return cardList.Take(number).ToList();
	}
}

public class NewestFilter : SelectFilter
{
	public NewestFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		cardList.Reverse();
		return cardList.Take(number).ToList();
	}
}

public class IndexFilter : SelectFilter
{
	public IndexFilter(string value) : base(value) { }
	public override List<CardController> filtering(List<CardController> cardList, AbilityProcessor.ActivateOption activateOption = null)
	{
		int index = number;
		List<CardController> returnList = new List<CardController>();
		if (index < cardList.Count)
		{
			returnList.Add(cardList[index]);
		}
		return returnList;
	}
}

// 単体系 ---------------------------------------------------------------------------------------------------------------------------------------
public class OptionFilter : CardFilter
{

}

public class OnlySelfFilter : OptionFilter
{

}

public class WithoutSelfFilter : OptionFilter
{

}

public class ThisTurnFilter : OptionFilter
{

}

public class LastTurnFilter : OptionFilter
{

}

public class BattlePhaseFilter : OptionFilter
{

}