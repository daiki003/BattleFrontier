using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCard : CardController
{
    public int number;
    public Color cardColor;
    public override void Init(int number, Color color, int index)
    {
        this.number = number;
        this.cardColor = color;
        base.Init(number, color, index);
    }
	public override string getSpritePath()
	{
		return "Images/Card/BattleFrontier/" + number;
	}
}
