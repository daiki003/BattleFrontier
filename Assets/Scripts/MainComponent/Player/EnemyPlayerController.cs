using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyPlayerController : PlayerController
{
    public override bool isSelf { get { return false; } }
    public EnemyPlayerController() : base()
    {

    }

	public override void battleStart()
	{
		base.battleStart();
		// ドロー処理は指定されたインデックスのカードを引く
		this.drawCardByIndex(GameManager.instance.networkManager.GetEnemyFirstHand());
	}

	public override void TurnStart()
	{
		base.TurnStart();
	}

	public void AISetCard()
	{
		int rondomNumber = RandomUtility.random(cardCollection.handCardList.Count);
		var validflagAreaList = BattleManager.instance.flagAreaList.Where(f => f.enemyInstalledCardList.Count < 3).ToList();
		int fieldIndex = RandomUtility.random(validflagAreaList.Count);
		SetCard(cardCollection.handCardList[rondomNumber].index, fieldIndex);
		drawCard(1);
		BattleManager.instance.changeTurn();
	}

	public void SetCard(int cardIndex, int fieldIndex)
	{
		int playHandIndex = RandomUtility.random(cardCollection.handCardList.Count);
		var playCard = cardCollection.handCardList.FirstOrDefault(c => c.index == cardIndex);
		if (playCard != null)
		{
			cardCollection.removeFromHand(playCard);
			var vfx = new ActionVfx(() => BattleManager.instance.flagAreaList[fieldIndex].SetCard(playCard, isSelf: false));
			vfx.addToAllBlockList();
			CoroutineUtility.createAndAddWaitVfx(1.0f);
		}
		else
		{
			Debug.LogError("NotFoundCard index:" + cardIndex);
		}
	}

	public void GetFlag(int fieldIndex)
	{
		BattleManager.instance.flagAreaList[fieldIndex].MoveFlag(isSelf: false);
	}

	public override List<CardController> drawCard(int number, bool isSpecial = false)
	{
		List<CardController> drawCardList = base.drawCard(number, isSpecial);
		var parameter = new NetworkParameter();
		parameter.indexList = drawCardList.Select(c => c.index).ToArray();
		parameter.isSpecial = isSpecial;
		parameter.isSelf = true;
		GameManager.instance.networkManager.SendAction(NetworkOperationType.DRAW, parameter);
		return drawCardList;
	}
}
