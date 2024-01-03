using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class CountBase
{
	int multiple = 1;
	int divideRoundUp = 1;
	int divideRoundDown = 1;
	int surplus = -1;
	protected PlayerController player;
	protected PlayerController enemy;
	public CountBase(int multiple = 1, int divideRoundUp = 1, int divideRoundDown = 1, int surplus = -1)
	{
		this.multiple = multiple;
		this.divideRoundUp = divideRoundUp;
		this.divideRoundDown = divideRoundDown;
		this.surplus = surplus;
		if (GameManager.instance.isBattle)
		{
			player = BattleManager.instance.player;
			enemy = BattleManager.instance.enemy;
		}
	}
	public int getActualCount(CardController ownerCard)
	{
		return getActualCount(new AbilityProcessor.ActivateOption(ownerCard: ownerCard));
	}
	public int getActualCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		int baseValue = getCount(option, processOption, activateCount);
		if (this.multiple != 1) return baseValue * this.multiple;
		else if (this.divideRoundUp != 1)
		{
			int remainder = baseValue % this.divideRoundUp == 0 ? 0 : 1;
			return (baseValue / this.divideRoundUp) + remainder;
		}
		else if (this.divideRoundDown != 1)
		{
			return baseValue / this.divideRoundDown;
		}
		else if (this.surplus != -1)
		{
			return baseValue % surplus;
		}
		else return baseValue;
	}
	public virtual int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return 0;
	}
}

public class TargetCount : CountBase
{
	public TargetBase target;
	protected int count;
	protected List<CardController> targetCardList;
	public TargetCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(multiple, divide, divideRoundDown, surplus)
	{
		this.target = AbilityUtility.targetFactory(targetText);
	}
	public void setUpGetCount(AbilityProcessor.ActivateOption option, AbilityProcessor.ProcessOption processOption)
	{
		targetCardList = target.getTargetCards(option, processOption);
		count = 0;
	}
}

// ターゲットが必要ないもの --------------------------------------------------------------------------------------------------------------------------------
public class ConstCount : CountBase
{
	int value;
	public ConstCount(int value, int multiple = 1, int divide = 1, int divideRoundDown = 1) : base(multiple, divide, divideRoundDown)
	{
		this.value = value;
	}

	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return value;
	}

	public void addValue(int value)
	{
		this.value += value;
	}
}

public class ActivatePowerCount : CountBase
{
	public ActivatePowerCount(int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return option.activatePower;
	}
}

public class ActivateCount : CountBase
{
	public ActivateCount(int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return activateCount;
	}
}

// ターゲットが必要なもの --------------------------------------------------------------------------------------------------------------------------------
public class CardCount : TargetCount
{
	public CardCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return target.getTargetCards(option, processOption).Count;
	}
}

public class DistinctCardIdCount : TargetCount
{
	public DistinctCardIdCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		return target.getTargetCards(option, processOption).Select(c => c.model.cardId).Distinct().Count();
	}
}

public class StatusCostCount : TargetCount
{
	public StatusCostCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int cost = 0;
		foreach (CardController card in targetCardList)
		{
			cost += card.model.cost;
		}
		return cost;
	}
}

public class StatusAttackCount : TargetCount
{
	public StatusAttackCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int attack = 0;
		foreach (CardController card in targetCardList)
		{
			attack += card.model.currentAttack;
		}
		return attack;
	}
}

public class StatusHpCount : TargetCount
{
	public StatusHpCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int hp = 0;
		foreach (CardController card in targetCardList)
		{
			hp += card.model.currentHp;
		}
		return hp;
	}
}

public class StatusMaxHpCount : TargetCount
{
	public StatusMaxHpCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int hp = 0;
		foreach (CardController card in targetCardList)
		{
			hp += card.model.currentMaxHp;
		}
		return hp;
	}
}

public class SoulChargeCount : TargetCount
{
	public SoulChargeCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int soulCharge = 0;
		foreach (CardController card in targetCardList)
		{
			soulCharge += card.model.soulCharge;
		}
		return soulCharge;
	}
}

public class ChargeCount : TargetCount
{
	public ChargeCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int chargeCount = 0;
		foreach (CardController card in targetCardList)
		{
			chargeCount += card.model.chargeCount;
		}
		return chargeCount;
	}
}

public class ReleaseCount : TargetCount
{
	public ReleaseCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int releaseCount = 0;
		foreach (CardController card in targetCardList)
		{
			releaseCount += card.model.releaseCount;
		}
		return releaseCount;
	}
}

public class UniqueCounter : TargetCount
{
	public UniqueCounter(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int uniqueCounter = 0;
		foreach (CardController card in targetCardList)
		{
			uniqueCounter += card.model.uniqueCounter;
		}
		return uniqueCounter;
	}
}

public class CustomPowerCount : TargetCount
{
	public CustomPowerCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int customPower = 0;
		foreach (CardController card in targetCardList)
		{
			customPower += card.model.customPower;
		}
		return customPower;
	}
}

public class SpiralCount : TargetCount
{
	public SpiralCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int spiral = 0;
		foreach (CardController card in targetCardList)
		{
			spiral += card.model.spiralCharge;
		}
		return spiral;
	}
}

public class ReturnCount : TargetCount
{
	public ReturnCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int returnCount = 0;
		foreach (CardController card in targetCardList)
		{
			returnCount += card.model.applyInformation.returnCount;
		}
		return returnCount;
	}
}

public class DestroyedMomentCostCount : TargetCount
{
	public DestroyedMomentCostCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int cost = 0;
		foreach (CardController card in targetCardList)
		{
			cost += card.model.destroyedParameter.cost;
		}
		return cost;
	}
}

public class DestroyedMomentAttackCount : TargetCount
{
	public DestroyedMomentAttackCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int attack = 0;
		foreach (CardController card in targetCardList)
		{
			attack += card.model.destroyedParameter.attack;
		}
		return attack;
	}
}

public class DestroyedMomentHpCount : TargetCount
{
	public DestroyedMomentHpCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int hp = 0;
		foreach (CardController card in targetCardList)
		{
			hp += card.model.destroyedParameter.hp;
		}
		return hp;
	}
}

public class BouncedMomentCostCount : TargetCount
{
	public BouncedMomentCostCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int cost = 0;
		foreach (CardController card in targetCardList)
		{
			cost += card.model.bouncedParameter.cost;
		}
		return cost;
	}
}

public class BouncedMomentAttackCount : TargetCount
{
	public BouncedMomentAttackCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int attack = 0;
		foreach (CardController card in targetCardList)
		{
			attack += card.model.bouncedParameter.attack;
		}
		return attack;
	}
}

public class BouncedMomentHpCount : TargetCount
{
	public BouncedMomentHpCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1) : base(targetText, multiple, divide, divideRoundDown, surplus) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		List<CardController> targetCardList = target.getTargetCards(option, processOption);
		int hp = 0;
		foreach (CardController card in targetCardList)
		{
			hp += card.model.bouncedParameter.hp;
		}
		return hp;
	}
}

public class PropertyCount : TargetCount
{
	string propertyText;
	public PropertyCount(string targetText, int multiple = 1, int divide = 1, int divideRoundDown = 1, int surplus = -1, string valueText = "") : base(targetText, multiple, divide, divideRoundDown, surplus)
	{
		propertyText = valueText;
	}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		setUpGetCount(option, processOption);
		foreach (CardController card in targetCardList)
		{
			var propertyList = card.model.allPropertyList.Where(p => p.propertyTrait.propertyType == propertyText);
			foreach (PropertyBase property in propertyList)
			{
				count += property.power.getActualCount(option, processOption, activateCount);
			}
		}
		return count;
	}
}

// 2つのカウントを計算するもの ------------------------------------------------------------------------------------------------------------------------------
public class CalcCount : CountBase
{
	protected CountBase count1;
	protected CountBase count2;
	public CalcCount(CountBase count1, CountBase count2, int multiple, int divide) : base(multiple, divide)
	{
		this.count1 = count1;
		this.count2 = count2;
	}
}

public class SumCount : CalcCount
{
	public SumCount(CountBase count1, CountBase count2, int multiple, int divide) : base(count1, count2, multiple, divide) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		int number1 = count1.getCount();
		int number2 = count2.getCount();
		return number1 + number2;
	}
}

public class SubtractionCount : CalcCount
{
	public SubtractionCount(CountBase count1, CountBase count2, int multiple, int divide) : base(count1, count2, multiple, divide) {}
	public override int getCount(AbilityProcessor.ActivateOption option = null, AbilityProcessor.ProcessOption processOption = null, int activateCount = 0)
	{
		int number1 = count1.getCount(option, processOption, activateCount);
		int number2 = count2.getCount(option, processOption, activateCount);
		return number1 - number2;
	}
}