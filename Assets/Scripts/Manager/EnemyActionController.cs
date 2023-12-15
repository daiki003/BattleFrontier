using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 相手起因で発生する処理を管理する
public class EnemyActionController
{
    PlayerController selfPlayer;
    PlayerController enemyPlayer;

    public EnemyActionController(PlayerController selfPlayer, PlayerController enemyPlayer)
    {
        this.selfPlayer = selfPlayer;
        this.enemyPlayer = enemyPlayer;
    }

    // 指定されたカードを引く
    public void Draw(List<int> indexList, bool isSelf, bool isSpecial)
    {
        var player = isSelf ? selfPlayer: enemyPlayer;
        player.DrawCardByIndex(indexList, isSpecial);
    }
    
    public void SetCard(int cardIndex, int fieldIndex)
	{
        var cardCollection = enemyPlayer.cardCollection;
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
}
