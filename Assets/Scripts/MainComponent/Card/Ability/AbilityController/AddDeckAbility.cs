using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class AddDeckAbility : AbilityController
{
	public AddDeckAbility(AbilityTrait abilityTrait, CardController ownerCard, SkillPanelController ownerSkill) : base(abilityTrait, ownerCard, ownerSkill)
	{
		
	}
	
	public override void startAbility(StartAbilityArgument startAbilityArgument)
	{
		base.startAbility(startAbilityArgument);

		string cardName = cardId.FirstOrDefault();
		int number = this.power.getActualCount(startAbilityArgument.activateOption, startAbilityArgument.processOption);

		List<CardController> addCardList = new List<CardController>();
		startAbilityArgument.activateOption.ownerCard.ownerCardCollection.shuffleDeck();
		AddToDeckVfx addToDeckVfx = new AddToDeckVfx(addCardList);
		addToDeckVfx.addToAllBlockList();
		startAbilityArgument.processOption.skillDrewCard = addCardList;
	}
}
