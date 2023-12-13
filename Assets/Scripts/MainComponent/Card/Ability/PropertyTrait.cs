using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropertyTrait
{
	public string propertyType;
	public string power;
	public List<string> condition = new List<string>();
	public string option = "";

	public PropertyTrait() {}

	public PropertyTrait(string type, string power, List<string> condition, string option)
	{
		this.propertyType = type;
		this.power = power;
		this.condition = new List<string>(condition);
		this.option = option;
	}
}

public class PropertyBase
{
	public PropertyTrait propertyTrait;
	public virtual string displayText { get {return "";} }
	public CountBase power;
	public List<ConditionBase> conditionList;
	public bool removeAfterAttack;

	public PropertyBase(PropertyTrait propertyTrait)
	{
		this.propertyTrait = propertyTrait;
		power = AbilityUtility.countFactory(propertyTrait.power);
		conditionList = new List<ConditionBase>();
		foreach (string partText in propertyTrait.condition)
		{
			if (!string.IsNullOrEmpty(partText))
			{
				conditionList.Add(AbilityUtility.conditionFactory(partText));
			}
		}

		string[] optionList = AbilityUtility.splitOption(propertyTrait.option);
		foreach (string option in optionList)
		{
			if (string.IsNullOrEmpty(option)) continue;

			removeAfterAttack |= AbilityUtility.parseOptionBool(option, "remove_after_attack");
		}
	}

	public bool isValid(CardController ownerCard)
	{
		AbilityProcessor.ActivateOption activateOption = new AbilityProcessor.ActivateOption(ownerCard: ownerCard);
		foreach (ConditionBase condition in conditionList)
		{
			if (!condition.judgeCondition(activateOption)) return false;
		}
		return true;
	}
}

public class PhantomProperty : PropertyBase
{
	public override string displayText { get { return "【幻影】"; } }
	public PhantomProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class DisappearProperty : PropertyBase
{
	public override string displayText { get { return "【消滅】"; } }
	public DisappearProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class WholeAttackProperty : PropertyBase
{
	public override string displayText { get { return "【全体攻撃】"; } }
	public WholeAttackProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class AddAttackCountProperty : PropertyBase
{
	public override string displayText { get { return "攻撃回数追加"; } }
	public AddAttackCountProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class PhantomAttackProperty : PropertyBase
{
	public override string displayText { get { return "攻撃した後消滅する"; } }
	public PhantomAttackProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class SpiralCostProperty : PropertyBase
{
	public SpiralCostProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class CustomPowerProperty : PropertyBase
{
	public CustomPowerProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// ファイアボール
public class FireballProperty : PropertyBase
{
	public override string displayText { get { return "『ファイアボール』"; } }
	public FireballProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class EnhanceFireballProperty : PropertyBase
{
	public override string displayText { get { return "自分の『ファイアボール』のダメージを1増やす"; } }
	public EnhanceFireballProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class SplitFireballProperty : PropertyBase
{
	public override string displayText { get { return "これの『ファイアボール』は追加で1体の対象にダメージを与える"; } }
	public SplitFireballProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 守護
public class GuardProperty : PropertyBase
{
	public override string displayText { get { return "【守護】"; } }
	public GuardProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 隠密
public class HidingProperty : PropertyBase
{
	public override string displayText { get { return "【隠密】"; } }
	public HidingProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 王者の加護
public class KingProtectionProperty : PropertyBase
{
	public override string displayText { get { return "【王者の加護】"; } }
	public KingProtectionProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 復活
public class RevivalProperty : PropertyBase
{
	public override string displayText { get { return "【復活】"; } }
	public RevivalProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 交換不可
public class CantExchangeProperty : PropertyBase
{
	public override string displayText { get { return "【交換不可】"; } }
	public CantExchangeProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 強化不可
public class CantEvolveProperty : PropertyBase
{
	public override string displayText { get { return "【強化不可】"; } }
	public CantEvolveProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 攻撃ダメージを受けない
public class AttackBarrierProperty : PropertyBase
{
	public override string displayText { get { return "攻撃ダメージを受けない"; } }
	public AttackBarrierProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 能力ダメージを受けない
public class AbilityBarrierProperty : PropertyBase
{
	public override string displayText { get { return "能力ダメージを受けない"; } }
	public AbilityBarrierProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// ファントムシールド
public class PhantomShieldProperty : PropertyBase
{
	public override string displayText { get { return "【ファントムシールド】"; } }
	public PhantomShieldProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

// 受けるX以上のダメージをYにする
public class DamageCutProperty : PropertyBase
{
	public DamageCutProperty(PropertyTrait propertyTrait) : base(propertyTrait)
	{
		
	}
}

// 受けるダメージを減らす
public class DamageReduceProperty : PropertyBase
{
	public override string displayText { get { return "受けるダメージを" + power.getActualCount() + "減らす"; } }
	public DamageReduceProperty(PropertyTrait propertyTrait) : base(propertyTrait)
	{
		
	}
}

// 根性
public class GutsProperty : PropertyBase
{
	public override string displayText { get { return "【根性：" + power.getActualCount() + "】"; } }
	public GutsProperty(PropertyTrait propertyTrait) : base(propertyTrait)
	{
		
	}
}

public class IndependentProperty : PropertyBase
{
	public IndependentProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class SkipAttackProperty : PropertyBase
{
	public override string displayText { get { return "攻撃しない"; } }
	public SkipAttackProperty(PropertyTrait propertyTrait) : base(propertyTrait)
	{
		
	}
}

public class WhenDestroyAgainProperty : PropertyBase
{
	public override string displayText { get { return "自分の破壊時能力は追加で1回発動する"; } }
	public WhenDestroyAgainProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class WhenTurnStartAgainProperty : PropertyBase
{
	public override string displayText { get { return "自分のターン開始時能力は追加で1回発動する"; } }
	public WhenTurnStartAgainProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class CommandAgainProperty : PropertyBase
{
	public override string displayText { get { return "自分の『司令』能力は追加で1回発動する"; } }
	public CommandAgainProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class WhenTurnStartAgainOnlySelfProperty : PropertyBase
{
	public override string displayText { get { return "このカードのターン開始時能力は追加で1回発動する"; } }
	public WhenTurnStartAgainOnlySelfProperty(PropertyTrait propertyTrait) : base(propertyTrait) { }
}

public class DealMultipleDamage : PropertyBase
{
	public override string displayText { get { return "与えるダメージが" + power + "倍になる"; } }
	public DealMultipleDamage(PropertyTrait propertyTrait) : base(propertyTrait) { }
}