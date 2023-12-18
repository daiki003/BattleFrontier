using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildSevenCard : NormalCard
{
    public WildSevenCard(GameObject cardObject, int index) : base(cardObject, 7, Color.white, index)
	{

	}
	public override string getSpritePath()
	{
		return "Images/Card/BattleFrontier/WildSeven";
	}
}
