using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerCard : NormalCard
{
    public JokerCard(GameObject cardObject, int index) : base(cardObject, -1, Color.white, index)
	{

	}
	public override string getSpritePath()
	{
		return "Images/Card/BattleFrontier/Joker";
	}
}
