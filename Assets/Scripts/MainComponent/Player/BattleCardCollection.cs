using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardComponent
{
	public int number;
	public Color color;
	public CardComponent() { }
	public CardComponent(int number, Color color)
	{
		this.number = number;
		this.color = color;
	}
}

public class BattleCardCollection
{
	private int MAX_FIELD = 7;

	public Transform fieldTransform;
	public List<CardComponent> masterDeck = new List<CardComponent>();

	// 実際のカードリスト ------------------------------------------------------------------------------------------------------------------------------
	public CardController substanceCard { get; set; } // OwnerCardのいないスキル発動用のカード
	public List<CardController> handCardList { get; set; } = new List<CardController>(); // 手札のカードリスト
	public List<List<CardController>> fieldCardListList { get; set; } = new List<List<CardController>>(); // フィールドのカードリスト
	
	public List<CardController> fieldCardList // フィールドのカードリスト1枚1枚
	{
		get
		{
			var cardList = new List<CardController>();
			for (int i = 0; i < fieldCardListList.Count; i++)
			{
				foreach (CardController card in fieldCardListList[i])
				{
					cardList.Add(card);
				}
			}
			return cardList;
		}
	}
	public List<CardController> deckCardList = new List<CardController>(); // デッキのカードリスト
	public List<CardController> graveCardList = new List<CardController>(); // 手札、フィールド、デッキ以外のカードリスト

	// リセット系 ------------------------------------------------------------------------------------------------------------------------------

	public void resetAllCard()
	{
		resetHandCard();
		resetFieldCard();
		resetGrave();
	}

	public void resetHandCard()
	{
		handCardList = new List<CardController>();
	}
	public void resetFieldCard()
	{
		fieldCardListList = new List<List<CardController>>();
		for (int i = 0; i < BattleManager.FLAG_AREA_NUMBER; i++)
		{
			fieldCardListList.Add(new List<CardController>());
		}
	}
	public void resetGrave()
	{
		graveCardList = new List<CardController>();
	}

	// カード追加系 ------------------------------------------------------------------------------------------------------------------------------
	public void AddToField(CardController card, int fieldIndex)
	{
		fieldCardListList[fieldIndex].Add(card);
	}

	public void AddToHand(CardController card)
	{
		handCardList.Add(card);
	}

	public void AddToDeckTop(CardController card)
	{
		deckCardList.Insert(0, card);
	}

	public void AddToGrave(CardController destroyedCard)
	{
		graveCardList.Add(destroyedCard);
	}

	// カード削除系 ------------------------------------------------------------------------------------------------------------------------------

	public void removeFromHand(CardController card)
	{
		handCardList.Remove(card);
	}

	// その他 ------------------------------------------------------------------------------------------------------------------------------
	// デッキをシャッフルする
	public void shuffleDeck()
	{
		// 整数 n の初期値はデッキの枚数
		int n = deckCardList.Count;

		// nが1より小さくなるまで繰り返す
		while (n > 1)
		{
			n--;

			// kは 0 〜 n+1 の間のランダムな値
			int k = RandomUtility.random(n + 1);

			// k番目のカードをtempに代入
			CardController temp = deckCardList[k];
			deckCardList[k] = deckCardList[n];
			deckCardList[n] = temp;
		}
	}

	// カードを場に出す
	public void summonCard(CardController summonCard, int fieldIndex = -1)
	{
		this.summonCard(new List<CardController>(){ summonCard }, fieldIndex);
	}

	// カードを場に出す
	public void summonCard(List<CardController> summonCardList, int backIndex = 0)
	{
		SeriesVfxBlock wholeBlock = new SeriesVfxBlock();
		ParallelVfxBlock summonBlock = new ParallelVfxBlock();

		for (int i = 0; i < summonCardList.Count; i++)
		{
			CardController summonCard = summonCardList[i];
			summonCard.sortie();
		}
		wholeBlock.addVfx(summonBlock);
		wholeBlock.addToAllBlockList();
	}

	public void alignment(List<CardController> cardList)
	{
		foreach (CardController card in cardList)
		{
			card.cardObject.transform.SetAsLastSibling();
		}
	}

	public void Update()
	{
		foreach (CardController handCard in handCardList)
		{
			handCard.Update();
		}

		foreach (CardController fieldCard in fieldCardList)
		{
			fieldCard.Update();
		}
	}
}
