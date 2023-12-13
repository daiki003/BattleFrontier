using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

public class CardController : MonoBehaviour
{
	public CardView view; // カードの見た目の処理
	public CardModel model; // カードのデータを処理
	public CardMovement movement; //移動に関することを操作
	public OnClick click;
	public DropPlace drop;
	private PlayerController player;
	private PlayerController enemy;
	public BattleCardCollection ownerCardCollection;
	public BattleCardCollection opponentCardCollection;

	public int index;
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
			return BattleManager.instance.player.model.focusHand ? focusHandSize : unFocusHandSize;
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

	// model,viewのリセット ------------------------------------------------------------------------------------------------------------------------------------------------
	// カードを生成した時に呼ばれる関数
	public virtual void Init(int number, Color color, int index)
	{
		this.index = index;

		// Awakeでやっていたことをここで実行（非アクティブでデッキのカードを作成するので、Awakeが呼ばれないため）
		view = GetComponent<CardView>();
		movement = GetComponent<CardMovement>();
		click = GetComponent<OnClick>();
		drop = GetComponent<DropPlace>();

		if (GameManager.instance.isBattle)
		{
			player = BattleManager.instance.player;
			enemy = BattleManager.instance.enemy;
			ownerCardCollection = player.cardCollection;
			opponentCardCollection = enemy.cardCollection;
		}

		model = new CardModel(); // カードデータを生成
		model.supplementText = DescribeText.supplementText(this);
		view.initIcon(getSpritePath());
		view.Show(this); // 表示
	}

	public void SetToFlagArea(FlagArea area)
	{
		this.targetArea = area;
        view.changeSize(installedSize);
        ownerCardCollection.removeFromHand(this);
		ownerCardCollection.addToField(this, area.fieldIndex);
	}

	public virtual string getSpritePath()
	{
		return "Images/Card/BattleFrontier/default";
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
	public VfxBase destroy()
	{
		bool isEnemy = model.isEnemy;
		ownerCardCollection.addToGrave(this);
		if (model.cardType != CardType.SPELL)
		{
			player.fluctuateDestroyCount(1);
		}

		// フィールドから削除する前に、インデックスを保存しておく
		int fieldIndex = ownerCardCollection.fieldCardList.IndexOf(this);
		int backIndex = ownerCardCollection.fieldCardList.Count - fieldIndex - 1;

		// フィールドカードから削除
		ownerCardCollection.removeFromField(this);

		// 場のカードのソウルチャージを溜める
		if (!isEnemy)
		{
			foreach (CardController fieldCard in player.cardCollection.fieldCardList)
			{
				fieldCard.model.soulCharge++;
			}
		}

		// 破壊された時の能力を発動
		abilityProcessor.addComponent(AbilityTiming.WHEN_DESTROY, this, fieldIndex: fieldIndex, backIndex: backIndex);
		// 破壊に反応して誘発する能力の発動
		if (model.cardType != CardType.SPELL)
		{
			abilityProcessor.addPursuitComponentSingleActivateCard(AbilityTiming.WHEN_DESTROY_OTHER, this, isEnemy: isEnemy);
			abilityProcessor.addPursuitComponentSingleActivateCard(AbilityTiming.WHEN_LEAVE_OTHER, this, isEnemy: isEnemy);
		}

		// カードテキスト更新
		player.updateCardText();

		return new DestroyVfx(this);
	}

	// カードが捨てられた時の処理
	public VfxBase discard()
	{
		player.cardCollection.addToGrave(this);
		player.fluctuateDiscardCount(1);

		// 手札のカードから削除
		ownerCardCollection.removeFromHand(this);

		// 捨てられた時の能力を発動
		abilityProcessor.addComponent(AbilityTiming.DISCARDED, this);

		return new DestroyVfx(this);
	}

	// カードが交換されたときの処理
	public void exchange()
	{
		model.destroyedParameter = new CardStatus(model.cost, model.currentAttack, model.currentHp);
	}

	// クリックタイプの変更
	public void changeClickType(ClickType clickType)
	{
		click.type = clickType;
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
		changeClickType(ClickType.CARD);
	}

	// 判定、取得系 ------------------------------------------------------------------------------------------------------------------------------------------------------------
	// プレイ可能な状態かどうかを判定
	public bool canPlay()
	{
		if (model == null) return false;
		// 敵のターン中はプレイ不可
		if (!BattleManager.instance.canOperationCard) return false;
		// 敵のカードはプレイ不可
		if (model.isEnemy) return false;

		return !BattleManager.instance.stopPlayFlag
			&& isHandCard
			&& BattleManager.instance.canPlayCard;
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

	// 有効な選択スキルを返す
	public SelectComponent getValidSelectComponent(AbilityTiming timing)
	{
		// 選択要素は1タイミングにつき1つの想定
		return model.selectComponent.FirstOrDefault(s => s.selectTiming == timing && s.selectCondition.All(c => c.judgeCondition()));
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
			VfxBase destroyVfx = this.destroy();
			destroyVfx.addToAllBlockList();
			ownerCardCollection.addToGrave(this);
		}
		// プレイする時にコスト変化情報はリセット
		model.applyInformation.eternalApplyValue.costChange = 0;
		model.applyInformation.eternalApplyValue.fixedCost = ApplyInformation.NONE;
		// CoroutineUtility.createAndAddWaitVfx(0.3f);

		// プレイ時の効果を先に処理しておく（プレイ回数を増やす前に処理する）
		abilityProcessor.activateComponent();
		// カードのプレイ回数を増やす
		player.fluctuatePlayCount(1);
		// カードプレイ時に発動する効果を登録
		abilityProcessor.addPursuitComponentSingleActivateCard(AbilityTiming.WHEN_PLAY_OTHER, this, selectedCard: selectCard);
		abilityProcessor.activateComponent();
		player.updateCardText();

		// 使うか検討中なので一旦コメントアウト
		// 同名カードをプレイしたときに自動で強化する処理
		// CardController sameIdCard = ownerCardCollection.fieldCardList.FirstOrDefault(c => c.model.cardId == this.model.cardId && c != this && !c.model.isEvolve);
		// if (sameIdCard != null && !sameIdCard.haveCardProperty<CantEvolveProperty>())
		// {
		// 	// ToDo プレイしたカードと同名の未強化のカードが場にいる場合、そのカードを強化してプレイしたカードは破壊される
		// 	// 強化時効果は発動しない
		// 	VfxBase destroyVfx = this.disappear();
		// 	destroyVfx.addToAllBlockList();
		// 	sameIdCard.evolve(activateWhenPlay: false);
		// }
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
}