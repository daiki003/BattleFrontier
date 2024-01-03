using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerController
{
	public BattleCardCollection cardCollection; // カード関連のデータを処理
	public bool isPlayCard;
	public bool isSelf;
	public List<int> firstDrawIndex = new List<int>();

	private AbilityProcessor abilityProcessor { get { return BattleManager.instance.mainAbilityProcessor; } }

	public PlayerController(bool isSelf, List<int> firstDrawIndex)
	{
		cardCollection = new BattleCardCollection();
		this.isSelf = isSelf;
		this.firstDrawIndex = firstDrawIndex;
	}

	public void Update()
	{
		cardCollection.Update();
	}

	// reset --------------------------------------------------------------------------------------------------
	public void resetAllCard()
	{
		cardCollection.resetAllCard();
		this.resetHandAndField();
	}

	public void resetHandAndField()
	{
		BattleManager.instance.mainPanel.playerHand.resetCard();
	}

	// バトル開始時の処理
	public void battleStart()
	{
		resetAllCard();
		isPlayCard = false;

		// 2戦目以降原因不明で最初のドロー演出が一瞬になるので、とりあえずWaitを入れて誤魔化す
		CoroutineUtility.createAndAddWaitVfx(0.001f);

		// ドロー処理は指定されたインデックスのカードを引く
		this.DrawCardByIndex(firstDrawIndex);
	}

	// バトル再開時の処理
	public void battleRecovery(RecoveryData recoveryData)
	{
		resetAllCard();

		updateCardText();
	}

	public virtual void TurnStart()
	{
		isPlayCard = false;
	}

	// カード関連 ---------------------------------------------------------------------------------------------------------------------

	// 山上のカードを引く
	public virtual List<CardController> drawCard(int number, bool isSpecial = false)
	{
		var deck = isSpecial ? BattleManager.instance.specialDeckCardList : BattleManager.instance.normalDeckCardList;
		number = Math.Min(number, deck.Count);

		List<CardController> drawCardList = deck.GetRange(0, number);

		// オンラインなら通信を送る
		if (GameManager.instance.battleMgr.isOnline)
		{
			var parameter = new NetworkParameter();
			parameter.indexList = drawCardList.Select(c => c.index).ToArray();
			parameter.isSpecial = isSpecial;
			// 相手に送るパラメータなので、isSelfは逆にする
			parameter.isSelf = !isSelf;
			GameManager.instance.networkManager.SendAction(NetworkOperationType.DRAW, parameter);
		}

		return drawCard(drawCardList, isSpecial);
	}

	// 指定のインデックスのカードを引く
	public virtual List<CardController> DrawCardByIndex(List<int> indexList, bool isSpecial = false)
	{
		var deck = isSpecial ? BattleManager.instance.specialDeckCardList : BattleManager.instance.normalDeckCardList;
		List<CardController> drawCardList = new List<CardController>();
		foreach (int index in indexList)
		{
			drawCardList.Add(deck.FirstOrDefault(c => c.index == index));
		}
		return drawCard(drawCardList, isSpecial);
	}

	// デッキから指定のカードを引く
	public virtual List<CardController> drawCard(List<CardController> cardList, bool isSpecial = false)
	{
		var deck = isSpecial ? BattleManager.instance.specialDeckCardList : BattleManager.instance.normalDeckCardList;
		List<CardController> drawList = new List<CardController>();
		for (int i = 0; i < cardList.Count; i++)
		{
			if (cardList[i] == null || !deck.Any(s => s == cardList[i]))
			{
				continue;
			}
			else
			{
				deck.Remove(cardList[i]);
			}
			CardController drawCard = cardList[i];
			cardCollection.AddToHand(drawCard);
			drawList.Add(drawCard);
		}

		// 自分側のみドロー演出を再生
		if (isSelf)
		{
			DrawVfx drawVfx = new DrawVfx(drawList, BattleManager.instance.mainPanel.deck);
			drawVfx.addToAllBlockList();
		}

		return drawList;
	}

	// 手札、フィールドのカードとスキルクエストカウントのテキストを更新
	public void updateCardText()
	{
		// カードテキスト更新
		List<CardController> cardList = new List<CardController>(cardCollection.handCardList);
		cardList.AddRange(cardCollection.fieldCardList);
		foreach (CardController card in cardList)
		{
			card.updateText();
		}
	}

	// その他行動 ------------------------------------------------------------------------------------------------------------
	
	// カード交換
	public void exchange(List<CardController> exchangeCards)
	{
		foreach (CardController card in exchangeCards)
		{
			cardCollection.removeFromHand(card);
			card.exchange();
		}
		AddToDeckVfx addToDeckVfx = new AddToDeckVfx(exchangeCards);
		addToDeckVfx.addToAllBlockList();
		CoroutineUtility.createAndAddWaitVfx(0.3f);

		// 交換ポイント消費
		this.drawCard(exchangeCards.Count);

		this.updateCardText();

		// 交換後に再開用データを記録
		BattleManager.instance.recordRecoveryData();
	}
}
