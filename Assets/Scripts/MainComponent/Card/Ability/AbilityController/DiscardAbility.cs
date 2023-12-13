using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class DiscardAbility : AbilityController
{
	public DiscardAbility(AbilityTrait abilityTrait, CardController ownerCard, SkillPanelController ownerSkill) : base(abilityTrait, ownerCard, ownerSkill)
	{
		
	}

	public override void startAbility(StartAbilityArgument startAbilityArgument)
	{
		base.startAbility(startAbilityArgument);

		ParallelVfxBlock wholeBlock = new ParallelVfxBlock();
		foreach (CardController targetCard in actualTargetList)
		{
			wholeBlock.addVfx(targetCard.discard());
		}
		// 他のカードが捨てられた時に誘発する能力を発動
		mainAbilityProcessor.addPursuitComponent(AbilityTiming.WHEN_DISCARD, actualTargetList);
		wholeBlock.addToAllBlockList();
	}
}
