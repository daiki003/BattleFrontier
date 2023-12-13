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
	private int DEFAULT_INCREASE_SKILL_POINT = 1;
	private int LATER_INCREASE_SKILL_POINT = 2;
	private int MAX_FIELD = 7;
	public bool isPlayCard;
	public virtual bool isSelf { get { return false; } }

	private AbilityProcessor abilityProcessor { get { return BattleManager.instance.mainAbilityProcessor; } }

	public PlayerController()
	{
		model = new PlayerModel();
		cardCollection = new BattleCardCollection();
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
	public virtual void battleStart()
	{
		resetAllCard();

		changeFocusHand(false, force: true);
		isPlayCard = false;

		// 2戦目以降原因不明で最初のドロー演出が一瞬になるので、とりあえずWaitを入れて誤魔化す
		CoroutineUtility.createAndAddWaitVfx(0.001f);
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
	}

	// カード関連 ---------------------------------------------------------------------------------------------------------------------

	// 山上のカードを引く
	public virtual List<CardController> drawCard(int number, bool isSpecial = false)
	{
		var deck = isSpecial ? BattleManager.instance.specialDeckCardList : BattleManager.instance.normalDeckCardList;
		number = Math.Min(number, deck.Count);

		List<CardController> drawCardList = deck.GetRange(0, number);
		return drawCard(drawCardList, isSpecial);
	}

	// 指定のインデックスのカードを引く
	public virtual List<CardController> drawCardByIndex(List<int> indexList, bool isSpecial = false)
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

	// スキル関連 ------------------------------------------------------------------------------------------------------
	public void addAbility(List<AbilityTrait> abilityList)
	{
		foreach (AbilityTrait ability in abilityList)
		{
			model.abilityList.Add(AbilityUtility.createAbilityController(ability));
		}
	}
	public void advanceQuest(AbilityTiming timing, List<CardController> activateCardList = null, int count = 1)
	{
		foreach (SkillPanelController skill in model.skillList)
		{
			if (skill.model.quest.countTiming == timing)
			{
				skill.model.quest.advanceCount(activateCardList, count);
			}
		}
	}

	// その他行動 ------------------------------------------------------------------------------------------------------------
	// コスト消費
	public void useCost(int cost)
	{
		fluctuateCost(-1 * cost);
		abilityProcessor.addPursuitComponent(AbilityTiming.WHEN_CONSUME_PP, activatePower: cost);
	}
	
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
		this.updateCost();

		// 交換後に再開用データを記録
		BattleManager.instance.recordRecoveryData();
	}

	// 売却
	public void chargeExchangeCount()
	{
		fluctuateExchangeCount(1);
		var vfx = new PlayEffectVfx(BattleManager.instance.trainingPhase.exchangeObject.transform, "Charge");
		vfx.addToAllBlockList();

		this.updateCardText();
		this.updateCost();
	}

	public bool canLevelUp()
	{
		return model.levelUpCost <= model.cost;
	}

	// PPが変化した時に行われる処理
	public void updateCost()
	{
		UpdateCardViewVfx updateCardViewVfx = new UpdateCardViewVfx(cardCollection.handCardList);
		updateCardViewVfx.addToAllBlockList();
	}

	// モデル操作系 ------------------------------------------------------------------------------------------------------------------------------------------------
	public void fluctuateCost(int cost) { model.setCost(Math.Min(Math.Max(0, model.cost + cost), model.maxCost)); }
	public void fluctuateExchangeCount(int soldCount) { model.setExchangeCount(model.exchangeCount + soldCount); }
	public void fluctuatePlayCount(int count) { model.setPlayCount(model.turnPlayCount + count); }
	public void fluctuateDiscardCount(int count) { model.setDiscardCount(model.turnDiscardCount + count); }
	public void fluctuateDestroyCount(int count) { model.setDestroyCount(model.turnDestroyCount + count); }
}
