using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class AbilityProcessor
{
	private BattleManager battleMgr;
	[NonSerialized] public List<ValidAbility> waitAbilityList = new List<ValidAbility>();

	public class ValidAbility
	{
		public List<AbilityController> abilityList;
		public ActivateOption option;
		public ValidAbility(List<AbilityController> abilityList, ActivateOption component)
		{
			this.abilityList = abilityList;
			this.option = component;
		}
	}

	// アビリティの起動タイミングで設定されるオプション
	public class ActivateOption
	{
		public CardController ownerCard;
		public SkillPanelController ownerSkill;
		public List<CardController> activateCardList;
		public CardController attackedCard;
		public List<CardController> selectCard = new List<CardController>();
		public List<string> choiceCard = new List<string>();
		public bool omitAnimation;
		public int activatePower;

		// 能力発動時のフィールドでの位置、現状when_destroy以外のときは-1が入っている（when_destroy以外はその時点でのIndexを参照すれば良い）
		public int fieldIndex;
		// 能力発動時に、これより後ろに何枚カードがあったか
		public int backIndex;

		public ActivateOption(CardController ownerCard, SkillPanelController ownerSkill = null,
			List<CardController> activateCardList = null, CardController attackedCard = null,
			List<CardController> selectCard = null, List<string> choiceCard = null,
			bool omitAnimation = false, int activatePower = 0, int activateCount = 0, int fieldIndex = -1, int backIndex = -1)
		{
			this.ownerCard = ownerCard;
			this.ownerSkill = ownerSkill;
			this.activateCardList = activateCardList ?? new List<CardController>();
			this.attackedCard = attackedCard;
			this.selectCard = selectCard;
			this.choiceCard = choiceCard;
			this.omitAnimation = omitAnimation;
			this.activatePower = activatePower;
			this.fieldIndex = fieldIndex;
			this.backIndex = backIndex;
		}
	}

	// アビリティの実行中に動的に設定されていくオプション
	public class ProcessOption
	{
		public List<CardController> lastTarget;
		public List<CardController> skillDrewCard;
		public List<CardController> skillSummonCard;
		public bool isShined;

		public ProcessOption(List<CardController> lastTarget = null, int lastAbilityPower = 0)
		{
			if (lastTarget == null)
			{
				this.lastTarget = new List<CardController>();
			}
			else
			{
				this.lastTarget = lastTarget;
			}
			this.skillDrewCard = new List<CardController>();
			this.skillSummonCard = new List<CardController>();
		}
	}

	public AbilityProcessor(BattleManager battleManager)
	{
		battleMgr = battleManager;
	}

	// コンポーネントの追加、実行 ------------------------------------------------------------------------------------------
	public void addComponent(AbilityTiming timing, CardController ownerCard = null, SkillPanelController ownerSkill = null, List<CardController> activateCard = null,
		List<CardController> selectCard = null, List<string> choiceCard = null,
		bool omitAnimation = false, CardController attackedCard = null, int activatePower = 0,
		bool insert = false, int fieldIndex = -1, int backIndex = -1)
	{
		ActivateOption activateOption = new ActivateOption(ownerCard, ownerSkill, activateCard, attackedCard, selectCard, choiceCard, omitAnimation, activatePower: activatePower, fieldIndex: fieldIndex, backIndex: backIndex);
		List<AbilityController> abilityList = getValidAbilityList(timing, activateOption);
		if (abilityList != null && abilityList.Count > 0)
		{
			// 発動が決定した時点で、アビリティの発動回数を増やす
			foreach (AbilityController ability in abilityList)
			{
				ability.activateCount += 1;
			}

			// insertなら、待機中のスキルに割り込んで優先して処理する
			if (insert)
			{
				waitAbilityList.Insert(0, new ValidAbility(abilityList, activateOption));
			}
			else
			{
				waitAbilityList.Add(new ValidAbility(abilityList, activateOption));
			}
		}
	}

	public void addPursuitComponentSingleActivateCard(AbilityTiming timing, CardController activateCard = null, List<CardController> selectedCard = null, int activatePower = 0, bool insert = false, bool isEnemy = false)
	{
		addPursuitComponent(timing, new List<CardController>(){ activateCard }, selectedCard: selectedCard, activatePower: activatePower, insert: insert, isEnemy: isEnemy);
	}

	public void addPursuitComponent(AbilityTiming timing, List<CardController> activateCard = null, List<CardController> selectedCard = null, int activatePower = 0, bool insert = false, bool isEnemy = false)
	{
		if (activateCard == null) activateCard = new List<CardController>();
		// リーダーの能力
		addComponent(timing, activateCard: activateCard);

		// 場の能力
		List<CardController> fieldCardList = new List<CardController>();
		if (isEnemy)
		{
			fieldCardList.AddRange(battleMgr.enemy.cardCollection.fieldCardList);
			fieldCardList.AddRange(battleMgr.player.cardCollection.fieldCardList);
		}
		else
		{
			fieldCardList.AddRange(battleMgr.player.cardCollection.fieldCardList);
			fieldCardList.AddRange(battleMgr.enemy.cardCollection.fieldCardList);
		}
		foreach (CardController fieldCard in fieldCardList)
		{
			if (activateCard.Contains(fieldCard))
			{
				continue;
			}
			addComponent(timing, ownerCard: fieldCard, activateCard: activateCard, selectCard: selectedCard, activatePower: activatePower, insert: insert);
		}
		// 手札の能力
		foreach (CardController handCard in battleMgr.player.cardCollection.handCardList)
		{
			if (activateCard.Contains(handCard))
			{
				continue;
			}
			addComponent(timing, ownerCard: handCard, activateCard: activateCard, selectCard: selectedCard, omitAnimation: true, activatePower: activatePower, insert: insert);
		}
	}

	public void addComponentFromAbility(List<AbilityController> abilityList, CardController ownerCard = null, SkillPanelController ownerSkill = null, List<CardController> activateCard = null,
		List<CardController> selectCard = null, List<string> choiceCard = null,
		bool omitAnimation = false, CardController attackedCard = null, int addEnhance = 0,
		bool insert = false, int fieldIndex = -1, int backIndex = -1)
	{
		ActivateOption activateOption = new ActivateOption(ownerCard, ownerSkill, activateCard, attackedCard, selectCard, choiceCard, omitAnimation, activatePower: addEnhance, fieldIndex: fieldIndex, backIndex: backIndex);
		List<AbilityController> validAbilityList = checkAbilityValidity(abilityList, activateOption);
		if (validAbilityList != null && validAbilityList.Count > 0)
		{
			// 発動が決定した時点で、アビリティの発動回数を増やす
			foreach (AbilityController abilityTrait in validAbilityList)
			{
				abilityTrait.activateCount += 1;
			}
			waitAbilityList.Add(new ValidAbility(validAbilityList, activateOption));
		}
	}

	public void activateComponent()
	{
		while (waitAbilityList.Count > 0)
		{
			ValidAbility abilityList = waitAbilityList.dequeue();
			processAbility(abilityList);
		}
		battleMgr.player.updateCardText();
	}

	// アビリティの実行 ----------------------------------------------------------------------------------------
	private void processAbility(ValidAbility ability)
	{
		ActivateOption activateOption = ability.option;
		ProcessOption processOption = new ProcessOption();

		// アビリティ発動カードが光る演出の登録
		bool omitAnimation = activateOption.omitAnimation || (activateOption.ownerCard != null && !activateOption.ownerCard.isFieldCard);
		ChangeColorVfx beforeAbilityVfx = new ChangeColorVfx(ability.option.ownerCard, effectName: "Activate", waitTime: omitAnimation ? 0.01f : 0.25f, seSkip: omitAnimation);
		beforeAbilityVfx.addToAllBlockList();

		foreach (AbilityController activateAbility in ability.abilityList)
		{
			// 実際のスキルの登録
			activateAbility.callStart(activateOption, processOption, this);
		}
	}

	// 取得関連 ---------------------------------------------------------------------------------------------------------

	// 発動条件を満たしたアビリティを取得
	public List<AbilityController> getValidAbilityList(AbilityTiming timing, ActivateOption option)
	{
		List<AbilityController> abilityList;
		if (option.ownerCard != null) abilityList = new List<AbilityController>(option.ownerCard.model.allAbilityList.Where(a => a.abilityTrait.timing == timing.ToString().ToLower()));
		else if (option.ownerSkill != null) abilityList = new List<AbilityController>(option.ownerSkill.model.abilityList.Where(a => a.abilityTrait.timing == timing.ToString().ToLower()));
		else abilityList = null;
		return checkAbilityValidity(abilityList, option);
	}
	public List<AbilityController> checkAbilityValidity(List<AbilityController> abilityList, ActivateOption option)
	{
		List<AbilityController> validAbilityList = new List<AbilityController>();
		if (abilityList != null)
		{
			foreach (AbilityController ability in abilityList)
			{
				if (ability.judgeCondition(option)) validAbilityList.Add(ability);
			}
		}
		return validAbilityList;
	}
}