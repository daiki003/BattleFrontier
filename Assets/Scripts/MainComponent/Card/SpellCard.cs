using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 使うと即座に能力だけ発動するカード
public class SpellCard : CardController
{
    public SpellCard(GameObject cardObject, int index) : base(cardObject, index) { }
    public virtual void OnWhenPlaySkill()
    {
        this.destroy();
    }
}

public class ScoutCard : SpellCard
{
    public ScoutCard(GameObject cardObject, int index) : base(cardObject, index)
    {
        spritePath = "Images/Card/BattleFrontier/Scout";
    }

	public override void OnWhenPlaySkill()
	{
		GameManager.instance.battleMgr.selectPanel.StartSelectCommon(SelectType.SCOUT, selectingCard: this);
        base.OnWhenPlaySkill();
	}
}