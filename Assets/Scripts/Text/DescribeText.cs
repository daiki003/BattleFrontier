using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DescribeText
{
	public static PlayerController player { get { return BattleManager.instance.player; } }

	// 補足文章の作成 ----------------------------------------------------------------------------------------------------------------
	public static string supplementText(CardController card)
	{
		if (!GameManager.instance.isBattle || card.model.supplementTextComponentList.Count == 0)
		{
			return "";
		}

		string supplementText = "";
		foreach (CardModel.SupplementTextComponent supplementTextComponent in card.model.supplementTextComponentList)
		{
			AbilityProcessor.ActivateOption option = new AbilityProcessor.ActivateOption(ownerCard: card);
			supplementText += "(" + supplementTextComponent.supplementTextFirst + supplementTextComponent.supplementTextComponent.getActualCount(option) + supplementTextComponent.supplementTextSecond + ")";
		}
		return supplementText;
	}
}
