using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class SelfPlayerController : PlayerController
{
    public override bool isSelf { get { return true; } }
    public SelfPlayerController() : base()
    {
		cardCollection.ObserveEveryValueChanged(cardCollection => cardCollection.handCardList.Count).Subscribe(_ => refreshCardSpace());
    }

	public override void battleStart()
	{
		base.battleStart();
		// ドロー処理は指定されたインデックスのカードを引く
		this.drawCardByIndex(GameManager.instance.networkManager.GetPlayerFirstHand());
	}

	public override List<CardController> drawCard(int number, bool isSpecial = false)
	{
		List<CardController> drawCardList = base.drawCard(number, isSpecial);
		var parameter = new NetworkParameter();
		parameter.indexList = drawCardList.Select(c => c.index).ToArray();
		parameter.isSpecial = isSpecial;
		parameter.isSelf = false;
		GameManager.instance.networkManager.SendAction(NetworkOperationType.DRAW, parameter);
		return drawCardList;
	}

    // デッキから指定のカードを引く
	public override List<CardController> drawCard(List<CardController> cardList, bool isSpecial = false)
	{
		List<CardController> drawList = base.drawCard(cardList);
		DrawVfx drawVfx = new DrawVfx(drawList, BattleManager.instance.mainPanel.deck);
		drawVfx.addToAllBlockList();
		return drawList;
	}

	public override void TurnStart()
	{
		base.TurnStart();
		AfterSelfTurnStartVfx afterTurnStartVfx = new AfterSelfTurnStartVfx();
		afterTurnStartVfx.addToAllBlockList();
	}
}
