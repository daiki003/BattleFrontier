using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildSevenCard : NormalCard
{
    public void Init(int index)
    {
        base.Init(7, Color.white, index);
    }
	public override string getSpritePath()
	{
		return "Images/Card/BattleFrontier/WildSeven";
	}
}
