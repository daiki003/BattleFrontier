using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class AbilityController
{
	public AbilityTrait abilityTrait;

	public AbilityTiming timing;
	public List<ConditionBase> conditionList;
	public TargetBase target;
	protected List<CardController> actualTargetList;
	public List<AbilityPreprocess> preprocessList;
	protected string effectName;

	// 共通オプション
	protected CountBase power = new ConstCount(0);
	protected CountBase repeatCount = new ConstCount(1);
	protected CountBase limitUpperCount = new ConstCount(10000);
	protected List<string> cardId = new List<string>();
	protected bool skipAnimation;
	protected bool skipSe;
	public bool allowDeadTarget;
	public float waitTime = 0f;
	protected int actualPower
	{
		get
		{
			return Math.Min(power.getActualCount(ownerCard), limitUpperCount.getActualCount(ownerCard));
		}
	}

	protected PlayerController player;
	protected PlayerController enemy;
	public int activateCount;

	public int index;
	public CardController ownerCard = null;
	public SkillPanelController ownerSkill = null;

	protected AbilityProcessor mainAbilityProcessor { get { return BattleManager.instance.mainAbilityProcessor; } }

	// オーナーが墓場にいても発動するタイミング
	public List<string> graveTiming = new List<string>()
	{
		"when_destroy",
		"discarded",
		"solded"
	};

	public AbilityController(AbilityTrait abilityTrait, CardController ownerCard = null, SkillPanelController ownerSkill = null)
	{
		this.abilityTrait = abilityTrait;

		Enum.TryParse(abilityTrait.timing.ToUpper(), out timing);

		conditionList = new List<ConditionBase>();
		foreach (string conditionText in abilityTrait.condition)
		{
			if (!string.IsNullOrEmpty(conditionText))
			{
				conditionList.Add(AbilityUtility.conditionFactory(conditionText));
			}
		}
		// 誘発系のタイミングで場所の設定がされていなければ、デフォルトでフィールドを指定しておく
		if (AbilityUtility.pursuitTiming.Contains(timing) && !conditionList.Any(c => c is PlaceCondition))
		{
			conditionList.Add(AbilityUtility.conditionFactory("place=field"));
		}

		this.target = AbilityUtility.targetFactory(abilityTrait.target);

		preprocessList = new List<AbilityPreprocess>();
		string[] preprocessParts = abilityTrait.preprocess.Split(',');
		for (int i = 0; i < preprocessParts.Length; i++)
		{
			this.preprocessList.Add(AbilityUtility.preprocessFactory(preprocessParts[i]));
		}

		// 共通オプションのセット
		string[] optionList = splitOption(abilityTrait.option);
		foreach (string option in optionList)
		{
			if (string.IsNullOrEmpty(option)) continue;

			if (tryGetOptionValueInt(option, "power", out CountBase powerValue))
			{
				power = powerValue;
			}
			if (tryGetOptionValueInt(option, "repeat_count", out CountBase repeatCountValue))
			{
				repeatCount = repeatCountValue;
			}
			if (tryGetOptionValueInt(option, "limit_upper_count", out CountBase limitUpperValue))
			{
				limitUpperCount = limitUpperValue;
			}
			if (tryGetOptionValueStringList(option, "card_id", out List<string> cardIdValue))
			{
				cardId = cardIdValue;
			}
			if (tryGetOptionValueString(option, "wait_time", out string waitTimeValue))
			{
				waitTime = float.Parse(waitTimeValue);
			}
			skipAnimation |= parseOptionBool(option, "skip_animation");
			skipSe |= parseOptionBool(option, "skip_se");
			allowDeadTarget |= parseOptionBool(option, "allow_dead_target");
		}

		if (abilityTrait.effectName != "")
		{
			effectName = abilityTrait.effectName;
		}

		if (GameManager.instance.isBattle)
		{
			player = BattleManager.instance.player;
			enemy = BattleManager.instance.enemy;
		}
		this.ownerCard = ownerCard;
		this.ownerSkill = ownerSkill;
		if (ownerCard != null)
		{
			this.index = ownerCard.model.nextAbilityIndex;
			ownerCard.model.nextAbilityIndex++;
		}
	}

	public void callStart(AbilityProcessor.ActivateOption activateOption, AbilityProcessor.ProcessOption processOption, AbilityProcessor abilityProcessor)
	{
		// 墓場にいても発動するタイミングではなく、オーナーが墓場にいる場合は発動を中止する
		if (!graveTiming.Contains(abilityTrait.timing) && activateOption.ownerCard != null && activateOption.ownerCard.model.cardType != CardType.SPELL && activateOption.ownerCard.isDead)
		{
			return;
		}

		foreach (AbilityPreprocess preprocess in preprocessList)
		{
			preprocess.start(activateOption);
			// １つでも条件を満たさないpreprocessがあれば発動を中止する
			if (!preprocess.isRight(activateOption, processOption, activateCount))
			{
				return;
			}
		}

		int repeatCount = this.repeatCount.getActualCount(activateOption, processOption);
		for (int i = 0; i < repeatCount; i++)
		{
			// ターゲットを取得
			actualTargetList = target.getTargetCards(activateOption, processOption);

			// ターゲットが死亡している場合は基本的に対象から除く
			if (!allowDeadTarget)
			{
				actualTargetList.RemoveAll(t => t.isDead);
			}

			// ターゲットが確定したので、lastTargetに設定
			if (actualTargetList != null) processOption.lastTarget = new List<CardController>(actualTargetList);

			// ターゲットが必要ない、またはターゲットが正常に設定されているなら、アビリティを実行する
			if (target is NoneTarget || (actualTargetList != null && !actualTargetList.Contains(null)))
			{
				// アビリティの実際の処理
				startAbility(new StartAbilityArgument(activateOption, processOption, abilityProcessor));
			}
		}

		CoroutineUtility.createAndAddWaitVfx(waitTime);
	}

	public class StartAbilityArgument
	{
		public AbilityProcessor.ActivateOption activateOption;
		public AbilityProcessor.ProcessOption processOption;
		public AbilityProcessor abilityProcessor;
		public StartAbilityArgument(AbilityProcessor.ActivateOption activateOption, AbilityProcessor.ProcessOption processOption, AbilityProcessor abilityProcessor)
		{
			this.activateOption = activateOption;
			this.processOption = processOption;
			this.abilityProcessor = abilityProcessor;
		}
	}

	public virtual void startAbility(StartAbilityArgument startAbilityArgument)
	{

	}

	public bool judgeCondition(AbilityProcessor.ActivateOption option)
	{
		foreach (ConditionBase abilityCondition in conditionList)
		{
			if (!abilityCondition.judgeCondition(option, activateCount: activateCount)) return false;
		}
		return true;
	}

	public void resetActivateCount()
	{
		activateCount = 0;
	}

	// オプション関連 ------------------------------------------------------------------------------------------------------------------------------------------------
	public bool tryGetOptionValueString(string optionText, string keyword, out string value)
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

	public bool tryGetOptionValueStringList(string optionText, string keyword, out List<string> value)
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

	public bool tryGetOptionValueInt(string optionText, string keyword, out CountBase value)
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

	public bool parseOptionBool(string optionText, string keyword)
	{
		return optionText == keyword;
	}

	public string[] splitOption(string baseText)
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
