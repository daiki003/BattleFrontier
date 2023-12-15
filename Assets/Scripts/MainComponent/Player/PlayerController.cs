using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerController
{
	public PlayerModel model; // データを処理
	public BattleCardCollection cardCollection; // カード関連のデータを処理
	public bool isPlayCard;
	public bool isSelf;
	public List<int> firstDrawIndex = new List<int>();

	private AbilityProcessor abilityProcessor { get { return BattleManager.instance.mainAbilityProcessor; } }

	public PlayerController(bool isSelf, List<int> firstDrawIndex)
	{
		model = new PlayerModel();
		cardCollection = new BattleCardCollection();
		this.isSelf = isSelf;
		this.firstDrawIndex = firstDrawIndex;
	}

	public void update()
	{
		
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

		changeFocusHand(false, force: true);
		isPlayCard = false;

		// 2戦目以降原因不明で最初のドロー演出が一瞬になるので、とりあえずWaitを入れて誤魔化す
		CoroutineUtility.createAndAddWaitVfx(0.001f);

		// ドロー処理は指定されたインデックスのカードを引く
		this.DrawCardByIndex(firstDrawIndex);
	}

	// バトル再開時の処理
	public void battleRecovery(RecoveryData recoveryData)
	{
		model.setValiableParameter(recoveryData.player.valiableParameter);
		resetAllCard();

		changeFocusHand(false, force: true);

		updateCardText();
	}

	public virtual void TurnStart()
	{
		isPlayCard = false;
		TurnPanelVfx turnPanelVfx = new TurnPanelVfx(isSelfTurn: isSelf);
		turnPanelVfx.addToAllBlockList();
		if (isSelf)
		{
			var afterTurnStartVfx = new ActionVfx(() => BattleManager.instance.canOperationCard = true);
			afterTurnStartVfx.addToAllBlockList();
		}
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
			cardCollection.addToHand(drawCard);
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

		// スキルクエストカウント更新
		foreach (SkillPanelController skill in model.skillList)
		{
			skill.updateQuestCount();
		}
	}

	// 手札の枚数に応じてカードのスペースを変更する
	public void refreshCardSpace()
	{
		List<CardController> playerHandCardList = cardCollection.handCardList;
		BattleManager.instance.mainPanel.holizontalLayoutGroup.spacing = getHandSpacing(playerHandCardList.Count);
	}

	public int getHandSpacing(int cardNumber)
	{
		if (cardNumber < 8)
		{
			return model.focusHand ? 50 : -50;
		}
		else if (cardNumber == 8)
		{
			return model.focusHand ? 10 : -70;
		}
		else if (cardNumber == 9)
		{
			return model.focusHand ? -10 : -80;
		}
		else
		{
			return model.focusHand ? -25 : -100;
		}
	}

	// 手札の拡大、縮小
	public void changeFocusHand(bool focus, bool force = false)
	{
		if (model.focusHand == focus && !force) return;

		model.setFocusHand(focus);
		Vector2 focusSize = new Vector2(2000, 370);
		Vector2 unFocusSize = new Vector2(1250, 300);
		Vector3 focusPosition = new Vector3(110, 150, 0);
		Vector3 unFocusPosition = new Vector3(500, 100, 0);

		RectTransform handTransform = (RectTransform)BattleManager.instance.mainPanel.playerHand;
		handTransform.sizeDelta = focus ? focusSize : unFocusSize;
		handTransform.anchoredPosition = focus ? focusPosition : unFocusPosition;

		foreach (CardController card in cardCollection.handCardList)
		{
			card.view.changeSize(focus ? card.focusHandSize : card.unFocusHandSize);
		}
		refreshCardSpace();
		if (!force) SEManager.instance.playSe("Draw");
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
		model.setTurnFirstExchange(false);
		this.drawCard(exchangeCards.Count);

		this.updateCardText();

		// 交換後に再開用データを記録
		BattleManager.instance.recordRecoveryData();
	}
}
