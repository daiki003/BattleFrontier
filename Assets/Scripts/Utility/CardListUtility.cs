using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardListUtility
{
    // カードリストの単純な数値の高さを返す
    public static int GetTotalPower(List<NormalCard> cardList)
    {
        int power = 0;
        foreach (NormalCard card in cardList)
        {
            // ジョーカーの場合、10として計算
            if (card.number == -1)
            {
                power += 10;
            }
            else
            {
                power += card.number;
            }
        }
        return power;
    }
}
