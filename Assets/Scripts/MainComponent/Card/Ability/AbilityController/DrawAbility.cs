using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class DrawAbility : AbilityController
{
	string deckDrawId;

	public DrawAbility(AbilityTrait abilityTrait, CardController ownerCard, SkillPanelController ownerSkill) : base(abilityTrait, ownerCard, ownerSkill)
	{
		string[] optionList = splitOption(abilityTrait.option);
		foreach (string optionText in optionList)
		{
			if (string.IsNullOrEmpty(optionText)) continue;

			if (tryGetOptionValueString(optionText, "deck_draw_id", out string drawIdValue))
			{
				deckDrawId = drawIdValue;	
			}
		}
	}

	public override void startAbility(StartAbilityArgument startAbilityArgument)
	{
		if (ownerCard != null && ownerCard.model.isEnemy) return;
		base.startAbility(startAbilityArgument);

		List<CardController> drawList = new List<CardController>();
		drawList = player.drawCard(power.getActualCount(startAbilityArgument.activateOption, startAbilityArgument.processOption));

		startAbilityArgument.processOption.skillDrewCard = drawList;
	}
}
