using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class DestroyAbility : AbilityController
{
	public DestroyAbility(AbilityTrait abilityTrait, CardController ownerCard, SkillPanelController ownerSkill) : base(abilityTrait, ownerCard, ownerSkill)
	{
		
	}

	public override void startAbility(StartAbilityArgument startAbilityArgument)
	{
		base.startAbility(startAbilityArgument);

		ParallelVfxBlock wholeBlock = new ParallelVfxBlock();
		foreach (CardController targetCard in actualTargetList)
		{
			wholeBlock.addVfx(targetCard.destroy());
		}
		wholeBlock.addToAllBlockList();
	}
}
