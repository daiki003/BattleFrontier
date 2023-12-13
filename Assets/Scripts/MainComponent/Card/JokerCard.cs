using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerCard : NormalCard
{
    public void Init(int index)
    {
        base.Init(-1, Color.white, index);
    }
	public override string getSpritePath()
	{
		return "Images/Card/BattleFrontier/Joker";
	}
}
