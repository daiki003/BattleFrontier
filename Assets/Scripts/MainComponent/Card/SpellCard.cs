using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 使うと即座に能力だけ発動するカード
public abstract class SpellCard : CardController
{
    public SpellCard(GameObject cardObject, int index) : base(cardObject, index) { }
    public abstract void OnWhenPlaySkill();
}

public class ScoutCard : SpellCard
{
    public ScoutCard(GameObject cardObject, int index) : base(cardObject, index)
    {
        spritePath = "Images/Card/BattleFrontier/Scout";
    }

	public override void OnWhenPlaySkill()
	{
		
	}
}