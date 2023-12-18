using System.Linq.Expressions;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class AbilityUtility
{
	// 何かのアクションに反応して発動するタイミング
	public static List<AbilityTiming> pursuitTiming = new List<AbilityTiming>()
	{
		AbilityTiming.TURN_START,
		AbilityTiming.ENEMY_TURN_START,
		AbilityTiming.TURN_END,
		AbilityTiming.ENEMY_TURN_END,
		AbilityTiming.WHEN_PLAY_OTHER,
		AbilityTiming.WHEN_SUMMON_OTHER,
		AbilityTiming.WHEN_DESTROY_OTHER,
		AbilityTiming.WHEN_RETURN_OTHER,
		AbilityTiming.WHEN_RETURN_ACTIVATE,
		AbilityTiming.WHEN_DRAW_OTHER,
		AbilityTiming.WHEN_GET_SHIELD,
		AbilityTiming.WHEN_ATTACK_OTHER,
		AbilityTiming.WHEN_LEAVE_OTHER,
		AbilityTiming.WHEN_DISCARD,
		AbilityTiming.WHEN_RALLY_OTHER,
		AbilityTiming.WHEN_LEADER_DAMAGE,
		AbilityTiming.WHEN_CREATE_OTHER,
		AbilityTiming.WHEN_SOLD_OTHER
	};

	public static PropertyBase propertyFactory(PropertyTrait propertyTrait)
	{
		switch (propertyTrait.propertyType)
		{
			case "phantom": return new PhantomProperty(propertyTrait);
			case "disappear": return new DisappearProperty(propertyTrait);
			case "whole_attack": return new WholeAttackProperty(propertyTrait);
			case "add_attack_count": return new AddAttackCountProperty(propertyTrait);
			case "phantom_attack": return new PhantomAttackProperty(propertyTrait);
			case "spiral_cost": return new SpiralCostProperty(propertyTrait);
			case "custom_power": return new CustomPowerProperty(propertyTrait);
			case "fireball": return new FireballProperty(propertyTrait);
			case "enhance_fireball": return new EnhanceFireballProperty(propertyTrait);
			case "split_fireball": return new SplitFireballProperty(propertyTrait);
			case "guard": return new GuardProperty(propertyTrait);
			case "hiding": return new HidingProperty(propertyTrait);
			case "king_protection": return new KingProtectionProperty(propertyTrait);
			case "revival": return new RevivalProperty(propertyTrait);
			case "cant_exchange": return new CantExchangeProperty(propertyTrait);
			case "cant_evolve": return new CantEvolveProperty(propertyTrait);
			case "attack_barrier": return new AttackBarrierProperty(propertyTrait);
			case "ability_barrier": return new AbilityBarrierProperty(propertyTrait);
			case "phantom_shield": return new PhantomShieldProperty(propertyTrait);
			case "damage_cut": return new DamageCutProperty(propertyTrait);
			case "damage_reduce": return new DamageReduceProperty(propertyTrait);
			case "guts": return new GutsProperty(propertyTrait);
			case "independent": return new IndependentProperty(propertyTrait);
			case "skip_attack": return new SkipAttackProperty(propertyTrait);
			case "when_destroy_again": return new WhenDestroyAgainProperty(propertyTrait);
			case "when_turn_start_again": return new WhenTurnStartAgainProperty(propertyTrait);
			case "command_again": return new CommandAgainProperty(propertyTrait);
			case "when_turn_start_again_only_self": return new WhenTurnStartAgainOnlySelfProperty(propertyTrait);
			case "deal_multiple_damage": return new DealMultipleDamage(propertyTrait);
			default: return new PropertyBase(propertyTrait);
		}
	}

	public static AbilityController createAbilityController(AbilityTrait abilityTrait, CardController ownerCard = null, SkillPanelController ownerSkill = null)
	{  
		switch ((AbilityType)Enum.Parse(typeof(AbilityType), abilityTrait.abilityType.ToUpper()))
		{
			case AbilityType.DRAW: return new DrawAbility(abilityTrait, ownerCard, ownerSkill);
			case AbilityType.ADD_DECK: return new AddDeckAbility(abilityTrait, ownerCard, ownerSkill);
			default: return null;
		}
	}

	public static AbilityController createRevivalAbility(CardController ownerCard)
	{
		AbilityTrait revivalAbility = new AbilityTrait(
			type : "summon",
			timing : "when_destroy",
			condition : new List<string>(),
			target : "",
			option : "revival,power=1"
		);
		return createAbilityController(revivalAbility, ownerCard);
	}

	public static AbilityController createFireballAbility(CardController ownerCard, int power, int repeat, int split)
	{
		AbilityTrait revivalAbility = new AbilityTrait(
			type : "damage",
			timing : "when_play",
			condition : new List<string>(),
			target : "enemy_field|random=" + split,
			option : "power=" + power + ",repeat_count=" + repeat,
			effectName: "Fireball"
		);
		return createAbilityController(revivalAbility, ownerCard);
	}

	public static List<AbilityController> createAbilityControllerList(List<AbilityTrait> abilityTraitList)
	{
		List<AbilityController> abilityList = new List<AbilityController>();
		foreach (AbilityTrait abilityTrait in abilityTraitList)
		{
			abilityList.Add(createAbilityController(abilityTrait));
		}
		return abilityList;
	}

	public static ConditionBase conditionFactory(string conditionText)
	{
		if (conditionText.StartsWith("{"))
		{
			return new CountCondition(conditionText);
		}

		string conditionName = conditionText.Substring(0, conditionText.IndexOf("="));
		string conditionValue = conditionText.Substring(conditionText.IndexOf("=") + 1);
		switch (conditionName)
		{
			case "place": return new PlaceCondition(conditionValue);
			case "is_battle": return new IsBattleCondition(conditionValue);
		}
	
		return new ConditionBase();
	}

	public static TargetBase targetFactory(string targetText)
	{
		if (string.IsNullOrEmpty(targetText)) return new NoneTarget();
		
		string[] partTargetText = targetText.Split('|');
		string targetType = partTargetText[0].ToUpper();
		string detailFilterText = "";
		if (partTargetText.Length > 1)
		{
			detailFilterText = partTargetText[1];
		}
		if (targetType == "") return new NoneTarget();
		return new NoneTarget();
	}

	public static CardFilter cardFilterFactory(string filterText)
	{
		// 記号を抽出
		Match match = Regex.Match(filterText, "[!=<>]+");
		string[] partText = filterText.Split(match.Value.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		string value = partText.Length < 2 ? "" : partText[1];
		if (partText.Length == 0) return new DetailFilter();

		switch (partText[0])
		{
			// DetailFilter
			case "card_id": return new CardIdFilter(value, match.Value);
			case "card_type": return new CardTypeFilter(value, match.Value);
			case "unit": return new CardTypeFilter("unit", "=");
			case "tribe": return new TribeFilter(value, match.Value);
			case "state": return new CardStateFilter(value, match.Value);
			case "ability_timing": return new AbilityTimingFilter(value, match.Value);
			case "cost": return new CostFilter(value, match.Value);
			case "attack": return new AttackFilter(value, match.Value);
			case "hp": return new HpFilter(value, match.Value);
			case "soul_charge": return new SoulChargeFilter(value, match.Value);
			case "is_enemy": return new EnemyFilter(value);

			// SelectFilter
			case "random": return new RandomFilter(value);
			case "random_without_self": return new RandomWithoutSelfFilter(value);
			case "min_hp": return new MinHpFilter(value);
			case "max_hp": return new MaxHpFilter(value);
			case "min_attack": return new MinAttackFilter(value);
			case "max_attack": return new MaxAttackFilter(value);
			case "max_cost": return new MaxCostFilter(value);
			case "oldest": return new OldestFilter(value);
			case "newest": return new NewestFilter(value);
			case "index": return new IndexFilter(value);

			// OptionFilter
			case "this_turn": return new ThisTurnFilter();
			case "last_turn": return new LastTurnFilter();
			case "battle_phase": return new BattlePhaseFilter();
			case "only_self": return new OnlySelfFilter();
			case "without_self": return new WithoutSelfFilter();
			default: return new CardFilter();
		}
	}

	public static CountBase countFactory(string countText)
	{
		if (string.IsNullOrEmpty(countText)) return new CountBase(multiple: 1, divideRoundUp: 1);

		// 倍率の設定
		int multiple = 1;
		int divideRoundUp = 1;
		int divideRoundDown = 1;
		int surplus = -1;
		Debug.Log(countText);
		Match multipleMatch = Regex.Match(countText, "[*/%]+");
		string[] countPartText = countText.Split(multipleMatch.Value.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		if (countPartText.Length > 1)
		{
			countText = countPartText[0];
			if (multipleMatch.Value == "*")
			{
				multiple = int.Parse(countPartText[1]);
			}
			else if (multipleMatch.Value == "//")
			{
				divideRoundDown = int.Parse(countPartText[1]);
			}
			else if (multipleMatch.Value == "/")
			{
				divideRoundUp = int.Parse(countPartText[1]);
			}
			else
			{
				surplus = int.Parse(countPartText[1]);
			}
		}

		// 足し算か引き算なら、２つのカウントを作ってそれをCalcCountに入れる
		Match calcMatch = Regex.Match(countText, "[+-]");
		string[] sumPartText = countText.Split(calcMatch.Value.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		if (sumPartText.Length > 1)
		{
			switch (calcMatch.Value)
			{
				case "+":
					return new SumCount(countFactory(sumPartText[0]), countFactory(sumPartText[1]), multiple, divideRoundUp);
				case "-":
					return new SubtractionCount(countFactory(sumPartText[0]), countFactory(sumPartText[1]), multiple, divideRoundUp);
			}
		}

		// テキストに=>があれば、左側をターゲット、右側を数え方にする
		// なければテキストそのままを数え方にする
		string[] arrow = {"=>"};
		string[] enhancePartText = countText.Split(arrow, StringSplitOptions.RemoveEmptyEntries);
		string countTypeText = "";
		string targetText = "";
		if (enhancePartText.Length > 1)
		{
			targetText = enhancePartText[0];
			countTypeText = enhancePartText[1];
		}
		else
		{
			countTypeText = enhancePartText[0];
		}

		// 数値に変換できるならConstCountを返す
		if (int.TryParse(countTypeText, out int value))
		{
			return new ConstCount(value, multiple, divideRoundUp);
		}

		// カウントタイプに=が含まれる場合、右をvalueとする
		string valueText = "";
		if (countTypeText.Contains("="))
		{
			string[] countTextSplit = countTypeText.Split('=');
			countTypeText = countTextSplit[0];
			valueText = countTextSplit[1];
		}

		string countType = countTypeText.ToUpper();
		switch ((CountType)Enum.Parse(typeof(CountType), countType))
		{
			// ターゲットが必要ないカウント
			case CountType.PP: return new PpCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.MAX_PP: return new MaxPpCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.LEADER_HP: return new LeaderHpCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.SHIELD: return new ShieldCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.TURN_PLAY_COUNT: return new TurnPlayCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.TURN_DISCARD_COUNT: return new TurnDiscardCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.TURN_DESTROY_COUNT: return new TurnDestroyCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.ACTIVATE_POWER: return new ActivatePowerCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.ACTIVATE_COUNT: return new ActivateCount(multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.BACK_WATER: return new BackWaterCount(multiple, divideRoundUp, divideRoundDown, surplus);

			// ターゲットが必要なカウント
			case CountType.CARD_COUNT: return new CardCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.DISTINCT_CARD_ID_COUNT: return new DistinctCardIdCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.COST: return new StatusCostCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.ATTACK: return new StatusAttackCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.HP: return new StatusHpCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.MAX_HP: return new StatusMaxHpCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.SOUL_CHARGE: return new SoulChargeCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.CHARGE_COUNT: return new ChargeCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.RELEASE_COUNT: return new ReleaseCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.UNIQUE_COUNTER: return new UniqueCounter(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.CUSTOM_POWER: return new CustomPowerCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.SPIRAL: return new SpiralCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.RETURN_COUNT: return new ReturnCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.DESTROYED_MOMENT_COST: return new DestroyedMomentCostCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.DESTROYED_MOMENT_ATTACK: return new DestroyedMomentAttackCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.DESTROYED_MOMENT_HP: return new DestroyedMomentHpCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.BOUNCED_MOMENT_COST: return new BouncedMomentCostCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.BOUNCED_MOMENT_ATTACK: return new BouncedMomentAttackCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.BOUNCED_MOMENT_HP: return new BouncedMomentHpCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus);
			case CountType.PROPERTY_COUNT: return new PropertyCount(targetText, multiple, divideRoundUp, divideRoundDown, surplus, valueText);

			default: return new CountBase(multiple, divideRoundUp);
		}
	}

	public static AbilityPreprocess preprocessFactory(string preprocessText)
	{
		if (!preprocessText.Contains("="))
		{
			return new NonePreprocess();
		}

		string preprocessName = preprocessText.Substring(0, preprocessText.IndexOf("="));
		List<string> preprocessValue = preprocessText.Substring(preprocessText.IndexOf("=") + 1).Split(':').ToList();

		switch (preprocessName)
		{
			// DetailFilter
			case "preprocess_condition": return new PreprocessCondition(preprocessValue[0]);
			
			default: return new NonePreprocess();
		}
	}

	// オプション関連 ----------------------------------------------------------------------------------------------------------------
	public static bool tryGetOptionValueString(string optionText, string keyword, out string value)
	{
		if (!optionText.Contains("="))
		{
			value = "";
			return false;
		}

		// 最初の=で分割
		string optionName = optionText.Substring(0, optionText.IndexOf("="));
		string optionValue = optionText.Substring(optionText.IndexOf("=") + 1);
		if (optionName != keyword)
		{
			value = "";
			return false;
		}

		value = optionValue;
		return true;
	}

	public static bool tryGetOptionValueStringList(string optionText, string keyword, out List<string> value)
	{
		value = new List<string>();
		if (!optionText.Contains("="))
		{
			return false;
		}

		string optionName = optionText.Substring(0, optionText.IndexOf("="));
		string optionValue = optionText.Substring(optionText.IndexOf("=") + 1);
		if (optionName != keyword)
		{
			return false;
		}

		string[] valueList = optionValue.Split(':');
		foreach (string partValue in valueList)
		{
			value.Add(partValue);
		}
		return true;
	}

	public static bool tryGetOptionValueInt(string optionText, string keyword, out CountBase value)
	{
		if (!optionText.Contains("="))
		{
			value = new CountBase();
			return false;
		}

		string optionName = optionText.Substring(0, optionText.IndexOf("="));
		string optionValue = optionText.Substring(optionText.IndexOf("=") + 1);
		if (optionName != keyword)
		{
			value = new CountBase();
			return false;
		}

		value = AbilityUtility.countFactory(optionValue);
		return true;
	}

	public static bool parseOptionBool(string optionText, string keyword)
	{
		return optionText == keyword;
	}

	public static string[] splitOption(string baseText)
	{
		// attach_ability内部のoptionの,に引っかからないように特殊な処理を行う
		if (baseText.Contains("attach_ability="))
		{
			int openCount = 0;
			int parseStartIndex = 0;
			List<string> context = new List<string>();
			int i;

			for (i = 0; i < baseText.Length; i++)
			{
				if (openCount == 0 && baseText[i] == ',')
				{
					context.Add(baseText.Substring(parseStartIndex == 0 ? 0 : parseStartIndex + 1, i - (parseStartIndex == 0 ? 0 : parseStartIndex + 1)));
					parseStartIndex = i;
				}

				if (baseText[i] == '[')
				{
					openCount ++;
				}
				if (baseText[i] == ']')
				{
					openCount --;
				}
			}

			if (baseText[parseStartIndex] == ',')
			{
				context.Add(baseText.Substring(parseStartIndex + 1, i - (parseStartIndex + 1)));
			}
			else
			{
				context.Add(baseText.Substring(parseStartIndex, i));
			}
			return context.ToArray();
		}
		else
		{
			return baseText.Split(',');
		}
	}
}
