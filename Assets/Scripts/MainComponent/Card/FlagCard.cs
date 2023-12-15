using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlagCardType
{
    Fog,
    Mad
}

public class FlagCard : CardController
{
    public FlagCardType flagCardType;
    public FlagCard(GameObject cardObject, FlagCardType flagCardType, int index) : base(cardObject, index)
    {
        this.flagCardType = flagCardType;
    }

    public override string getSpritePath()
	{
		return "Images/Card/BattleFrontier/" + flagCardType;
	}
}
