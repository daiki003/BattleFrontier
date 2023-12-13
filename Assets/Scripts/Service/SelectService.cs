using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum SelectType
{
	NONE,
	SELECT_TARGET,
	CHOICE,
	CHANGE_HAND,
	SELECT_BONUS
}

public class SelectComponent
{
	public SelectType selectType;
	public AbilityTiming selectTiming;
	public CountBase selectNumber;
	public List<ConditionBase> selectCondition = new List<ConditionBase>();
	public TargetBase selectTarget;
	public List<string> choiceCardName;
	public SelectComponent(SelectType selectType)
	{
		this.selectType = selectType;
		// 手札交換の場合、手札のカードを対象にする
		if (selectType == SelectType.CHANGE_HAND)
		{
			selectTarget = AbilityUtility.targetFactory("field");
		}
		BattleManager.instance.player.cardCollection.handCardList.Where(c => c.canExchange()).ToList();
	}
	public SelectComponent(CardMaster.SelectMasterComponent selectMasterComponent)
	{
		selectType = selectMasterComponent.selectType;
		Enum.TryParse(selectMasterComponent.selectTiming.ToUpper(), out selectTiming);
		selectNumber = AbilityUtility.countFactory(selectMasterComponent.selectNumber);
		foreach (string conditionText in selectMasterComponent.selectCondition)
		{
			selectCondition.Add(AbilityUtility.conditionFactory(conditionText));
		}
		selectTarget = AbilityUtility.targetFactory(selectMasterComponent.selectTarget);
		choiceCardName = new List<string>(selectMasterComponent.choiceCardName);
	}
	
	public bool isValid(AbilityTiming timing)
	{
		return selectTiming == timing && selectCondition.All(c => c.judgeCondition());
	}
}

public class SelectService : PanelPrefab
{
	[SerializeField] GameObject selectService;
	[SerializeField] GameObject selectPanel;
	[SerializeField] public Transform selectCardPlace;
	[SerializeField] GridLayoutGroup selectGrid;
	[SerializeField] Text selectText;
	[SerializeField] Button decide;
	[System.NonSerialized] public CardController selectingCard;
	[System.NonSerialized] public SelectType selectType;
	[System.NonSerialized] public SelectComponent selectComponent;
	List<CardController> selectCardList = new List<CardController>();
	List<CardController> targetCardList = new List<CardController>(); // 選択されたカードリスト
	List<SkillAndQuest> selectQuestList = new List<SkillAndQuest>();
	List<BonusPanel> selectBonusList = new List<BonusPanel>();
	private int selectCardNumber = 0;
	private bool flexible; // 選択枚数がselectCardNumber以下の枚数で自由に選べるかどうか
	private List<int> selectedCardIndex = new List<int>();
	private string originalCardId;
	private bool isEvolve;
	private BattleManager battleMgr;
	private Vector2 cardCellSize = new Vector2(200, 240);
	private Vector2 bonusCellSize = new Vector2(350, 450);

	public override void setup()
	{
		close();
	}

	public override void close()
	{
		selectService.SetActive(false);
	}

	public override void onClickExtra()
	{
		switch (selectType)
		{
			case SelectType.SELECT_TARGET:
				BattleManager.instance.selectPanel.cancelSelect();
				break;
			case SelectType.CHANGE_HAND:
				BattleManager.instance.selectPanel.cancelChangeHand();
				break;
			case SelectType.CHOICE:
				BattleManager.instance.selectPanel.cancelChoice();
				break;
		}
	}


	// 対象選択関連 ---------------------------------------------------------------------------------------------------------------------------
	public void startSelect()
	{
		this.selectCardList = selectComponent.selectTarget.getTargetCards(this.selectingCard);
		// 選択対象のカードが存在しなければ、選択をやめる
		if (selectCardList.Count == 0)
		{
			if (selectingCard.model.cardType == CardType.SPELL)
			{
				cancelSelect();
			}
			else
			{
				playWithoutSelect();
				return;	
			}
		}
		this.selectService.SetActive(true);
		this.selectPanel.SetActive(true);
		this.selectCardNumber = selectComponent.selectNumber.getActualCount(selectingCard);
		this.flexible = false;
		this.selectText.text = "対象を選択してください（" + selectCardNumber + "枚）";
		this.selectGrid.cellSize = cardCellSize;

		moveToSelect();
	}

	public void endSelect()
	{
		setAndReturnSelectedCard(false);

		// カードプレイ禁止状態を解除
		BattleManager.instance.stopPlayFlag = false;

		// ターゲットリストを対象に、カードを使用
		battleMgr.player.useCost(selectingCard.model.cost);
		selectingCard.play(targetCardList);

		// 選択対象になった時の能力発動
		foreach (CardController card in targetCardList)
		{
			battleMgr.mainAbilityProcessor.addComponent(AbilityTiming.WHEN_SELECTED, ownerCard: card, activateCard: new List<CardController>(){ selectingCard });
		}
		battleMgr.mainAbilityProcessor.activateComponent();

		battleMgr.deleteDescribe();
		this.selectType = SelectType.NONE;
		this.selectService.SetActive(false);
	}

	public void playWithoutSelect()
	{
		//コストを使用し、カードプレイ禁止状態を解除
		battleMgr.player.useCost(selectingCard.model.cost);
		BattleManager.instance.stopPlayFlag = false;

		// 対象を取らずに、カードを使用
		selectingCard.play();
	}

	public void cancelSelect()
	{
		setAndReturnSelectedCard(true);
		this.selectingCard.transform.SetParent(battleMgr.mainPanel.playerHand);
		battleMgr.player.cardCollection.alignment(isHand: !isEvolve, isField: isEvolve);
		selectingCard.view.Show(new CardView.DisplayComponent(selectingCard));
		battleMgr.deleteDescribe();
		this.selectType = SelectType.NONE;
		this.selectService.SetActive(false);
		// カードプレイ禁止状態を解除
		BattleManager.instance.stopPlayFlag = false;
	}

	// 手札交換関連 ---------------------------------------------------------------------------------------------------------------------------
	public void startChangeHand()
	{
		this.selectCardList = selectComponent.selectTarget.getTargetCards(BattleManager.instance.player.cardCollection.substanceCard);
		this.selectService.SetActive(true);
		this.selectPanel.SetActive(true);
		this.selectCardNumber = BattleManager.instance.player.cardCollection.handCardList.Count;
		this.flexible = true;
		this.selectText.text = "交換するカードを選択してください";
		this.selectGrid.cellSize = cardCellSize;

		moveToSelect();
	}

	public void endChangeHand()
	{
		// 選択されたカードリストの格納先
		targetCardList = new List<CardController>();

		setAndReturnSelectedCard(false);

		// ターゲットリストのカードを全て交換
		BattleManager.instance.player.exchange(targetCardList);

		battleMgr.deleteDescribe();
		this.selectType = SelectType.NONE;
		this.selectService.SetActive(false);
		BattleManager.instance.stopPlayFlag = false;
	}

	public void cancelChangeHand()
	{
		setAndReturnSelectedCard(true);
		battleMgr.deleteDescribe();
		this.selectType = SelectType.NONE;
		this.selectService.SetActive(false);
		BattleManager.instance.stopPlayFlag = false;
	}

	// チョイス関連 --------------------------------------------------------------------------------------------------------------------------

	public void endChoice()
	{
		// 選択されたカードリストの格納先
		List<string> choiceCardList = new List<string>();

		foreach (CardController card in selectCardList)
		{
			if (card.model.isSelected) choiceCardList.Add(card.model.cardId);
			Destroy(card.gameObject);
		}

		// カードプレイ禁止状態を解除
		BattleManager.instance.stopPlayFlag = false;

		// ターゲットリストを対象に、カードを使用
		battleMgr.player.useCost(selectingCard.model.cost);
		selectingCard.play(choiceCard: choiceCardList);

		battleMgr.deleteDescribe();
		this.selectType = SelectType.NONE;
		this.selectService.SetActive(false);
	}

	public void cancelChoice()
	{
		foreach (CardController card in selectCardList)
		{
			Destroy(card.gameObject);
		}
		this.selectingCard.transform.SetParent(battleMgr.mainPanel.playerHand);
		battleMgr.player.cardCollection.alignment(isHand: !isEvolve, isField: isEvolve);
		selectingCard.view.Show(new CardView.DisplayComponent(selectingCard));
		battleMgr.deleteDescribe();
		this.selectType = SelectType.NONE;
		this.selectService.SetActive(false);
		// カードプレイ禁止状態を解除
		BattleManager.instance.stopPlayFlag = false;
	}

	// 共通 -----------------------------------------------------------------------------------------------------------------------------
	public void startSelectCommon(SelectComponent selectComponent, CardController selectingCard = null, string originalCardId = null, bool isEvolve = false, int level = 0)
	{
		this.selectType = selectComponent.selectType;
		this.selectComponent = selectComponent;
		this.isEvolve = isEvolve;
		this.selectingCard = selectingCard;
		this.originalCardId = originalCardId;
		switch (selectType)
		{
			case SelectType.SELECT_TARGET:
				startSelect();
				break;
			case SelectType.CHANGE_HAND:
				startChangeHand();
				break;
		}
	}
	
	
	// 決定ボタン押下時の処理
	public void decideButton()
	{
		switch (selectType)
		{
			case SelectType.SELECT_TARGET:
				endSelect();
				break;
			case SelectType.CHANGE_HAND:
				endChangeHand();
				break;
			case SelectType.CHOICE:
				endChoice();
				break;
		}
	}

	// 非表示ボタン押下時の処理
	public void hiddenButton()
	{
		selectPanel.SetActive(!selectPanel.activeSelf);
	}

	// 選択されているカードの枚数を取得
	public int selectedCardNumber()
	{
		int count = 0;
		switch (selectType)
		{
			case SelectType.SELECT_TARGET:
			case SelectType.CHOICE:
				foreach (CardController card in selectCardList)
				{
					if (card.model.isSelected) count++;
				}
				return count;
			case SelectType.SELECT_BONUS:
				foreach (BonusPanel bonus in selectBonusList)
				{
					if (bonus.selected) count++;
				}
				return count;
			default:
				return count;
		}
	}

	// 選択可能枚数が1枚の場合、新たにカードを選択すると元々選択されていたカードの選択が外れる
	public void removeSelect()
	{
		switch (selectType)
		{
			case SelectType.SELECT_TARGET:
			case SelectType.CHOICE:
				foreach (CardController card in selectCardList)
				{
					card.changeSelected(false);
				}
				break;
			case SelectType.SELECT_BONUS:
				foreach (BonusPanel bonus in selectBonusList)
				{
					bonus.setSelected(false);
				}
				break;
		}
	}

	// カードの追加選択が可能か判定
	public bool judgeCanSelect()
	{
		if (flexible) return true;
		
		if (this.selectCardNumber == 1) removeSelect();
		else if (selectedCardNumber() >= this.selectCardNumber) return false;
		return true;

	}

	// 選択する対象となるカードリストを決定
	public void setSelectedCard()
	{
		switch (selectType)
		{
			case SelectType.CHANGE_HAND:
				selectCardList = BattleManager.instance.player.cardCollection.handCardList.Where(c => c.canExchange()).ToList();
				break;
		}
	}

	// 選択ゾーンにカードを移動する
	public void moveToSelect()
	{
		setSelectedCard();
		selectedCardIndex = new List<int>();
		// 選択ゾーンに移動する前の親要素の中でのindexを記録
		foreach (CardController card in this.selectCardList)
		{
			int cardIndex = card.transform.GetSiblingIndex();
			selectedCardIndex.Add(cardIndex);
		}
		foreach (CardController card in this.selectCardList)
		{
			card.model.isSelectTarget = true;
			card.transform.SetParent(this.selectCardPlace, false);
			card.changeClickType(ClickType.SELECTED_CARD);
			card.view.changeSize(card.selectCardSize);
		}
	}

	// 選択されるカードの元の親を特定
	public Transform setCardParent(TargetBase target)
	{
		if (target is HandTarget) return battleMgr.mainPanel.playerHand;
		else return battleMgr.mainPanel.playerHand;
	}

	// 選択候補のカードを元の場所に戻す
	public void setAndReturnSelectedCard(bool cancel)
	{
		targetCardList = new List<CardController>();

		// 選択候補のカードの元の場所を設定
		Transform parent = battleMgr.mainPanel.playerHand;
		if (selectType == SelectType.SELECT_TARGET)
		{
			parent = setCardParent(selectComponent.selectTarget);
		}

		// 選択候補のカードを元の場所に戻し、選択されていたカードをリストに格納
		for (int i = 0; i < selectCardList.Count; i++)
		{
			if (selectCardList[i].model.isSelected && !cancel)
			{
				targetCardList.Add(selectCardList[i]);
			}
			selectCardList[i].model.isSelectTarget = false;
			returnOneCard(selectCardList[i], parent, selectedCardIndex[i]);
			selectCardList[i].view.Show(selectCardList[i]);
		}
	}

	// 選択候補のカード1枚を元の場所に戻す処理
	private void returnOneCard(CardController selectedCard, Transform parent, int index)
	{
		selectedCard.cancelSelected();
		if (parent != null)
		{
			selectedCard.transform.SetParent(parent, false);
			if (selectCardList.Count < index) selectedCard.transform.SetAsLastSibling();
			else selectedCard.transform.SetSiblingIndex(index);
		}
	}

	public void start(BattleManager battleMgr)
	{
		this.battleMgr = battleMgr;
	}

	public void update()
	{
		if (selectCardList.Count > 0 || selectBonusList.Count > 0)
		{
			this.decide.interactable = (selectedCardNumber() == this.selectCardNumber || flexible);
		}
	}
}
