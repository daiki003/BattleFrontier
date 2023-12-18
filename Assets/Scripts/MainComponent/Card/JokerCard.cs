using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerCard : NormalCard
{
    public JokerCard(GameObject cardObject, int index) : base(cardObject, -1, Color.white, index)
	{
		spritePath = "Images/Card/BattleFrontier/Joker";
	}
	public override string getSpritePath()
	{
		return "Images/Card/BattleFrontier/Joker";
	}
}
