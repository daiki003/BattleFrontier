using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCard : CardController
{
    public int number;
    public Color cardColor;
    public NormalCard(GameObject cardObject, int number, Color color, int index) : base(cardObject, index)
    {
        this.number = number;
        this.cardColor = color;
    }
	public override string getSpritePath()
	{
		return "Images/Card/BattleFrontier/" + number;
	}
}
