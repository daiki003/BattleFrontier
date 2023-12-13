using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFieldController : MonoBehaviour
{
    public List<CardController> fieldCardList;
    public BattleFieldController()
    {
        fieldCardList = new List<CardController>();
    }

    public void addFieldCard(CardController card, int fieldIndex = -1)
	{
		if (fieldIndex != -1 && fieldCardList.Count > fieldIndex)
		{
			fieldCardList.Insert(fieldIndex, card);
		}
		else
		{
			fieldCardList.Add(card);
		}
	}
}
