using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

public class CardController
{
	public GameObject cardObject; // カードのオブジェクト
	public CardView view; // カードの見た目の処理
	public CardModel model; // カードのデータを処理
	private PlayerController player;
	private PlayerController enemy;
	public BattleCardCollection ownerCardCollection;
	public BattleCardCollection opponentCardCollection;

	public int index;
	public string spritePath;
	public FlagArea targetArea;
	public Vector3 focusHandSize = new Vector3(1.4f, 1.4f, 1f);
	public Vector3 unFocusHandSize = new Vector3(0.85f, 0.85f, 1f);
	public Vector3 installedSize = new Vector3(1f, 1f, 1f);
	public Vector3 fieldCardSize = new Vector3(1.2f, 1.2f, 1f);
	public Vector3 centerCardSize = new Vector3(1.8f, 1.8f, 1f);
	public Vector3 selectCardSize = new Vector3(1.0f, 1.0f, 1f);
	// 手札のカードの大きさを返す
	public Vector3 handCardSize
	{
		get
		{
			return BattleManager.instance.mainPanel.focusHand ? focusHandSize : unFocusHandSize;
		}
	}

	public bool isDead // 破壊されているかどうか
	{
		get { return ownerCardCollection.graveCardList.Contains(this); }
	}
	public bool isFieldCard // フィールドのカードかどうか
	{
		get { return ownerCardCollection.fieldCardListList.Any(f => f.Contains(this)); }
	}
	public bool isHandCard // 手札のカードかどうか
	{
		get { return ownerCardCollection.handCardList.Contains(this); }
	}
	public CardState cardState
	{
		get
		{
			if (!GameManager.instance.isBattle)
			{
				return CardState.NONE;
			}

			if (isHandCard)
			{
				return CardState.HAND;
			}
			else if (isFieldCard)
			{
				return CardState.FIELD;
			}
			else if (isDead)
			{
				return CardState.GRAVE;
			}
			else
			{
				return CardState.NONE;
			}
		}
	}

	public bool duringSelected;

	// model,viewのリセット ------------------------------------------------------------------------------------------------------------------------------------------------
	public CardController(GameObject cardObject, int index)
	{
		this.index = index;

		// Awakeでやっていたことをここで実行（非アクティブでデッキのカードを作成するので、Awakeが呼ばれないため）
		this.cardObject = cardObject; 
		view = cardObject.GetComponent<CardView>();
		var movement = cardObject.GetComponent<CardMovement>();
		movement.card = this;
		var click = cardObject.GetComponent<OnClick>();
		click.card = this;

		if (GameManager.instance.isBattle)
		{
			player = BattleManager.instance.player;
			enemy = BattleManager.instance.enemy;
			ownerCardCollection = player.cardCollection;
			opponentCardCollection = enemy.cardCollection;
		}

		model = new CardModel(); // カードデータを生成
		model.supplementText = DescribeText.supplementText(this);
	}

	public void InitView()
	{
		view.initIcon(spritePath);
		view.Show(this);
	}

	public void SetToFlagArea(FlagArea area, Transform setArea)
	{
		cardObject.transform.SetParent(setArea);
		this.targetArea = area;
        view.changeSize(installedSize);
        ownerCardCollection.removeFromHand(this);
		ownerCardCollection.AddToField(this, area.fieldIndex);
	}

	// 以下現在未使用 -------------------------------------------------------------------------------------------------------------------------------------------------
	private AbilityProcessor abilityProcessor { get { return BattleManager.instance.mainAbilityProcessor; } }

	public SaveData.CardComponent createSaveComponent()
	{
		return new SaveData.CardComponent(model.cardId, model.applyInformation);
	}

	// カードテキスト更新
	public void updateText()
	{
		model.supplementText = DescribeText.supplementText(this);
	}

	// 破壊などされた時の処理 -----------------------------------------------------------------------------------------------------------------------------
	// カードが破壊された時の処理
	public void destroy()
	{
		ownerCardCollection.removeFromHand(this);
		ownerCardCollection.AddToGrave(this);

		// カードテキスト更新
		player.updateCardText();
		DestroyVfx destroyVfx = new DestroyVfx(this);
		destroyVfx.addToAllBlockList();
	}

	public void ReturnToDeck()
	{
		ownerCardCollection.removeFromHand(this);
		ownerCardCollection.AddToDeckTop(this);

		AddToDeckVfx addToDeckVfx = new AddToDeckVfx(new List<CardController>(){ this });
		addToDeckVfx.addToAllBlockList();
	}

	// カードが交換されたときの処理
	public void exchange()
	{
		model.destroyedParameter = new CardStatus(model.cost, model.currentAttack, model.currentHp);
	}

	// 選択状態を切り替え
	public void switchSelected()
	{
		if (!BattleManager.instance.selectPanel.judgeCanSelect() && !model.isSelected) return;
		changeSelected(!model.isSelected);
	}

	// 選択状態を設定
	public void changeSelected(bool selected)
	{
		model.isSelected = selected;
	}

	// 選択状態を解除
	public void cancelSelected()
	{
		model.isSelected = false;
		duringSelected = false;
	}

	// 判定、取得系 ------------------------------------------------------------------------------------------------------------------------------------------------------------
	// プレイ可能な状態かどうかを判定
	public bool canPlay()
	{
		var battleMgr = BattleManager.instance;
		return !battleMgr.stopPlayFlag && isHandCard && battleMgr.canOperationCard && battleMgr.selectPanel.currentPhase != SelectPhase.DRAW;
	}

	// 交換可能な状態かどうかを判定
	public bool canExchange()
	{
		// バトル中以外は交換不可
		if (!GameManager.instance.isBattle)
		{
			return false;
		}

		if (model == null || model.isEnemy) return false;

		return isHandCard;
	}

	// カードをプレイしたときの処理 --------------------------------------------------------------------------------------------------------------
	public void play(List<CardController> selectCard = null, List<string> choiceCard = null, bool isWait = false)
	{
		OnPlayMotionVfx onPlayMotionVfx = new OnPlayMotionVfx(this, isWait);
		onPlayMotionVfx.addToAllBlockList();
		onPlayProcess(selectCard, choiceCard);

		// プレイ時処理が終わった段階での再開用データを記録
		BattleManager.instance.recordRecoveryData();
	}

	public void onPlayProcess(List<CardController> selectCard = null, List<string> choiceCard = null)
	{
		ownerCardCollection.removeFromHand(this);

		// プレイ時の効果発動（選択が必要なカードで選択ができなかった場合、効果は発動しない）
		abilityProcessor.addComponent(AbilityTiming.WHEN_PLAY, this, selectCard: selectCard, choiceCard: choiceCard);
		// 場に出た時の処理
		if (model.cardType != CardType.SPELL)
		{
			ownerCardCollection.summonCard(this);
		}
		else
		{
			this.destroy();
			ownerCardCollection.AddToGrave(this);
		}
		// プレイする時にコスト変化情報はリセット
		model.applyInformation.eternalApplyValue.costChange = 0;
		model.applyInformation.eternalApplyValue.fixedCost = ApplyInformation.NONE;
		// CoroutineUtility.createAndAddWaitVfx(0.3f);

		// プレイ時の効果を先に処理しておく（プレイ回数を増やす前に処理する）
		abilityProcessor.activateComponent();
		// カードプレイ時に発動する効果を登録
		abilityProcessor.addPursuitComponentSingleActivateCard(AbilityTiming.WHEN_PLAY_OTHER, this, selectedCard: selectCard);
		abilityProcessor.activateComponent();
		player.updateCardText();
	}

	// カードが場に出た時の処理
	public void sortie(bool onlyBattle = false)
	{
		// 召喚時の効果発動（バトルのみの場合の最初のセット時は発動しない）
		if (!onlyBattle)
		{
			abilityProcessor.addComponent(AbilityTiming.WHEN_SUMMON, this);
			abilityProcessor.addPursuitComponentSingleActivateCard(AbilityTiming.WHEN_SUMMON_OTHER, this, isEnemy: model.isEnemy);
		}
	}

	public void Update()
	{
		view.activeCursor.SetActive(canPlay() && BattleManager.instance.movingCard == null);
	}
}