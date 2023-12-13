using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public abstract class VfxBase
{
	public IEnumerator coroutine;
	public int waitAfter;
	public bool isEnd;
	public string logText;

	public VfxBase(string logText = "")
	{
		this.logText = logText;
	}

	public virtual void addToAllBlockList()
	{
		if (BattleManager.instance != null)
		{
			GameManager.instance.coroutineProcessor.allVfxList.Add(this);
		}
	}

	public virtual bool getIsEnd()
	{
		return isEnd;
	}
}

public class SingleVfx : VfxBase
{
	public SingleVfx(string logText = "") : base(logText)
	{

	}

	public override void addToAllBlockList()
	{
		if (coroutine == null)
		{
			return;
		}
		base.addToAllBlockList();
	}
}

public class TweenVfx : VfxBase
{
	public float afterWaitTime = 0f;
	public Action tweenAction;

	public TweenVfx(string logText = "") : base(logText)
	{

	}

	// ここにSequenceを入れて実行すると、自動で待ち時間とendTweenが設定される
	protected void createTweenAction(Sequence sequence)
	{
		sequence
			.AppendInterval(afterWaitTime)
			.AppendCallback(() => endTween());
	}

	public void endTween()
	{
		isEnd = true;
	}

	public override void addToAllBlockList()
	{
		if (tweenAction == null)
		{
			return;
		}
		base.addToAllBlockList();
	}
}

public class ActionVfx : VfxBase
{
	public List<Action> actionList = new List<Action>();

	public ActionVfx(string logText = "") : base(logText)
	{
		actionList = new List<Action>();
	}
	public ActionVfx(Action action, string logText = "") : base(logText)
	{
		this.actionList = new List<Action>() { action };
	}
	public ActionVfx(List<Action> actionList, string logText = "") : base(logText)
	{
		this.actionList = actionList;
	}

	public override void addToAllBlockList()
	{
		if (actionList.Count <= 0)
		{
			return;
		}
		base.addToAllBlockList();
	}
}

public class WaitVfx : VfxBase
{
	public float waitTime;

	public WaitVfx(float waitTime, string logText = "") : base(logText)
	{
		this.waitTime = waitTime;
	}
}

public class VfxBlock : VfxBase
{
	public List<VfxBase> vfxList = new List<VfxBase>();

	public void addVfx(VfxBase vfx)
	{
		if (vfx != null)
		{
			vfxList.Add(vfx);
		}
	}

	public override void addToAllBlockList()
	{
		if (vfxList.Count <= 0)
		{
			return;
		}
		base.addToAllBlockList();
	}

	public override bool getIsEnd()
	{
		return vfxList.All(v => v.getIsEnd());
	}
}

public class SeriesVfxBlock : VfxBlock
{

}

public class ParallelVfxBlock : VfxBlock
{

}

// カードの移動系 -----------------------------------------------------------------------------------------------------------------------------------------------------------
// カードを手札に加える演出
public class DrawVfx : TweenVfx
{
	public class DrawCardComponent
	{
		public CardController card;
		public CardView.DisplayComponent displayComponent;

		public DrawCardComponent(CardController card)
		{
			this.card = card;
			this.displayComponent = new CardView.DisplayComponent(card);
		}
	}

	public DrawVfx(List<CardController> drawCard, Transform firstTransform, bool seSkip = false, float afterWaitTime = 0.2f) : base()
	{
		List<DrawCardComponent> drawCardList = new List<DrawCardComponent>();
		foreach (CardController card in drawCard)
		{
			drawCardList.Add(new DrawCardComponent(card));
		}
		this.afterWaitTime = afterWaitTime;
		this.tweenAction = () => createTweenAction(draw(drawCardList, firstTransform));
	}

	public Sequence draw(List<DrawCardComponent> drawCardList, Transform firstTransform)
	{
		SEManager.instance.playSe("Draw");

		Sequence allSequence = DOTween.Sequence();
		allSequence.AppendCallback(() =>
		{
			BattleManager.instance.mainPanel.changeCenterSpacing(drawCardList.Count > 5 ? 0 : 70);
		});

		for (int i = 0; i < drawCardList.Count; i++)
		{
			allSequence.Join(drawOneCard(drawCardList[i], firstTransform, drawCardList.Count, drawIndex: i));
		}

		allSequence
			.AppendCallback(() =>
			{
				SEManager.instance.playSe("InHand");
			});
		
		return allSequence;
	}

	// カード1枚のドロー演出
	public Sequence drawOneCard(DrawCardComponent drawCard, Transform firstTransform, int drawNumber, int drawIndex)
	{
		Transform cardParent = firstTransform;
		Transform cardTransform = drawCard.card.transform;
		Transform handTransform = BattleManager.instance.mainPanel.playerHand;

		// 手札の一番右のカードを取得
		CardController handCard = handTransform.GetComponentsInChildren<CardController>().LastOrDefault();
		int handCardCount = handTransform.GetComponentsInChildren<CardController>().Count();
		Vector3 targetPosition = new Vector3(handTransform.position.x + getHandPosition(handCardCount + drawNumber, handCardCount + drawIndex), handTransform.position.y, handTransform.position.z);

		// カードを初期位置に移動後、一度親をキャンバスにする
		cardTransform.SetParent(cardParent);
		cardTransform.localPosition = new Vector3(0, 0, 0);
		cardTransform.SetParent(cardParent.parent);

		Sequence drawSequence = DOTween.Sequence();

		// カードをセンターに移動
		float moveTime = firstTransform != BattleManager.instance.mainPanel.center ? 0.1f : 0.01f;
		drawSequence
			.Append(cardTransform.DOMove(BattleManager.instance.mainPanel.center.position, moveTime))
			.Join(cardTransform.DOScale(drawCard.card.centerCardSize, moveTime))
			.AppendCallback(() =>
			{
				cardTransform.SetParent(BattleManager.instance.mainPanel.center);
				drawCard.card.view.Show(drawCard.displayComponent, forceOther: true);
			});


		// 手札に加わる予定のカードならカードを手札に移動
		// 手札から溢れる予定のカードなら移動演出はなしで墓場に移動
		drawSequence.AppendInterval(0.3f);
		if (drawCard.card.isHandCard)
		{
			drawSequence.Append(cardTransform.DOMove(targetPosition, 0.1f))
				.Join(cardTransform.DOScale(drawCard.card.handCardSize, 0.1f))
				.AppendCallback(() =>
				{
					cardTransform.localScale = drawCard.card.handCardSize;
					cardParent = handTransform;
					cardTransform.SetParent(cardParent);
				});
		}
		else if (drawCard.card.isDead)
		{
			drawSequence.AppendCallback(() => Effect.instance.generateEffect("Destroy", cardTransform))
				.AppendInterval(0.1f)
				.AppendCallback(() => SEManager.instance.destroy())
				.AppendInterval(0.2f)
				.AppendCallback(() => drawCard.card.transform.SetParent(BattleManager.instance.mainPanel.banishArea));
		}

		drawSequence.AppendCallback(() => drawCard.card.view.Show(drawCard.displayComponent));

		return drawSequence;
	}

	// 手札の枚数と位置から移動後の手札の座標を計算
	public int getHandPosition(int cardNumber, int index)
	{
		int spacing = BattleManager.instance.player.getHandSpacing(cardNumber) + 200;
		List<int> positionList = new List<int>();
		int firstPosition = -1 * spacing * (cardNumber - 1) / 2;
		for (int i = 0; i < cardNumber; i++)
		{
			positionList.Add(firstPosition + spacing * i);
		}
		return positionList[index];
	}
}

// カードを場に出す演出
public class SummonVfx : TweenVfx
{
	CardController summonCard;
	Transform fieldTransform;
	bool seSkip;

	public SummonVfx(CardController card, Transform fieldTransform, bool seSkip = false, int fieldIndex = -1) : base()
	{
		this.summonCard = card;
		this.fieldTransform = fieldTransform;
		this.seSkip = seSkip;

		afterWaitTime = 0.4f;
		var displayComponent = new CardView.DisplayComponent(card);
		tweenAction = () => createTweenAction(summon(card, displayComponent, seSkip, fieldIndex));
	}

	public Sequence summon(CardController summonCard, CardView.DisplayComponent displayComponent, bool seSkip = false, int fieldIndex = -1)
	{
		Sequence sequence = DOTween.Sequence();
		sequence
			.AppendCallback(() =>
			{
				// 最初の一枚だけSEを鳴らす
				Effect.instance.generateEffect("Summon", summonCard.view.transform);
				if (!seSkip) SEManager.instance.playSe("InHand");

				Transform cardTransform = summonCard.transform;

				// 親をフィールドにする
				cardTransform.SetParent(fieldTransform);
				cardTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
				if (fieldIndex != -1)
				{
					cardTransform.SetSiblingIndex(fieldIndex);
				}

				summonCard.view.Show(displayComponent);
			});
		
		return sequence;
	}
}

// カードを破壊する演出
public class DestroyVfx : SingleVfx
{
	CardController destroyedCard;
	bool omitEffect;

	public DestroyVfx(CardController card, bool omitEffect = false) : base()
	{
		this.destroyedCard = card;
		this.omitEffect = omitEffect;

		coroutine = destroy(card, new CardView.DisplayComponent(card), omitEffect);
	}

	public IEnumerator destroy(CardController card, CardView.DisplayComponent displayComponent, bool omitEffect = false)
	{
		if (card.model.cardType != CardType.SPELL && !omitEffect)
		{
			yield return new WaitForSeconds(0.2f);
			Effect.instance.generateEffect("Destroy", card.view.transform);
			yield return new WaitForSeconds(0.1f);
			SEManager.instance.destroy();
			yield return new WaitForSeconds(0.4f);
		}
		// 親を墓場にして、表示を消す
		card.transform.SetParent(BattleManager.instance.mainPanel.banishArea);

		card.view.Show(displayComponent);
	}
}

// カードをデッキに加える演出
public class AddToDeckVfx : TweenVfx
{
	public AddToDeckVfx(List<CardController> drawCard) : base()
	{
		this.tweenAction = () => addToDeck(drawCard);
	}

	public void addToDeck(List<CardController> addCardList)
	{
		SEManager.instance.playSe("Draw");

		Sequence allSequence = DOTween.Sequence();
		allSequence.AppendCallback(() =>
		{
			BattleManager.instance.mainPanel.changeCenterSpacing(addCardList.Count > 5 ? 0 : 70);
		});
		foreach (CardController drawCard in addCardList)
		{
			allSequence.Join(addToDeckOneCard(drawCard));
		}

		allSequence.AppendCallback(() =>
		{
			SEManager.instance.playSe("InHand");
			endTween();
		});
	}

	// カード1枚をデッキに加える演出
	public Sequence addToDeckOneCard(CardController addToDeckCard)
	{
		Transform cardTransform = addToDeckCard.transform;

		// カードをセンターに移動
		cardTransform.SetParent(BattleManager.instance.mainPanel.center);
		cardTransform.localPosition = new Vector3(0, 0, 0);
		cardTransform.localScale = addToDeckCard.centerCardSize;

		Sequence addToDeckSequence = DOTween.Sequence();

		// カードをデッキに移動
		addToDeckSequence.AppendInterval(0.35f)
			.Append(cardTransform.DOMove(BattleManager.instance.mainPanel.deck.position, 0.2f))
			.AppendCallback(() =>
			{
				cardTransform.SetParent(BattleManager.instance.mainPanel.invisibleArea);
			});

		return addToDeckSequence;
	}
}


// カード、プレイヤーの見た目変化系 ------------------------------------------------------------------------------------------------------------------------------------------------------
public class EvolveVfx : SingleVfx
{
	public EvolveVfx(CardController card, bool omitEffect = false) : base()
	{
		coroutine = evolve(card, new CardView.DisplayComponent(card), omitEffect);
	}

	public IEnumerator evolve(CardController card, CardView.DisplayComponent displayComponent, bool omitEffect = false)
	{
		if (card.model.cardType != CardType.SPELL && !omitEffect)
		{
			yield return new WaitForSeconds(0.1f);
			Effect.instance.generateEffect("Evolve", card.view.transform);
			SEManager.instance.playSe("Buff");
			yield return new WaitForSeconds(0.2f);
		}
		card.view.Show(displayComponent);
	}
}

public class UpdateCardViewVfx : ActionVfx
{
	public UpdateCardViewVfx(CardController targetCard, bool isChangeSize = false) : base()
	{
		var displayComponent = new CardView.DisplayComponent(targetCard);
		actionList.Add(updateView(targetCard, displayComponent, isChangeSize));
	}
	public UpdateCardViewVfx(List<CardController> targetCardList, bool isChangeSize = false) : base()
	{
		foreach (CardController targetCard in targetCardList)
		{
			var displayComponent = new CardView.DisplayComponent(targetCard);
			actionList.Add(updateView(targetCard, displayComponent, isChangeSize));
		}
	}

	public Action updateView(CardController targetCard, CardView.DisplayComponent displayComponent, bool isChangeSize)
	{
		return () =>
		{
			targetCard.view.Show(displayComponent, isChangeSize: isChangeSize);
		};
	}
}

public class BeforeUnitAttackVfx : TweenVfx
{
	private float beforeAttackTime = 0.5f;
	private Vector3 defaultSize = new Vector3(1.2f, 1.2f, 1.2f);
	private Vector3 attackerSize = new Vector3(1.4f, 1.4f, 1.4f);
	public BeforeUnitAttackVfx(CardController attackCard) : base()
	{
		this.tweenAction = () => attack(attackCard);
	}

	public void attack(CardController attackCard)
	{
		Transform attackerTransform = attackCard.transform;
		Sequence attackSequence = DOTween.Sequence();
		attackSequence
			.AppendInterval(0.1f)
			.Append(attackerTransform.DOScale(attackerSize, beforeAttackTime))
			.AppendCallback(() => endTween());
	}
}

public class UnitAttackVfx : TweenVfx
{
	private float baseTime = 0.25f;
	private Vector3 defaultSize = new Vector3(1.2f, 1.2f, 1.2f);
	private Vector3 attackerSize = new Vector3(1.4f, 1.4f, 1.4f);
	public UnitAttackVfx(CardController attackCard, CardController targetCard, int attackDamage, int counterDamage) : base()
	{
		CardView.DisplayComponent attackerDisplayComponent = new CardView.DisplayComponent(attackCard);
		CardView.DisplayComponent targetDisplayComponent = new CardView.DisplayComponent(targetCard);
		this.tweenAction = () => attack(attackCard, targetCard, attackDamage, counterDamage, attackerDisplayComponent, targetDisplayComponent);
	}

	public void attack(CardController attackCard, CardController targetCard, int attackDamage, int counterDamage, CardView.DisplayComponent attacerDisplayComponent, CardView.DisplayComponent targetDisplayComponent)
	{
		Transform attackerTransform = attackCard.transform;
		Transform targetTransform = targetCard.transform;
		float distance = Vector3.Distance(attackerTransform.position, targetTransform.position);
		float moveTime = baseTime;
		Vector3 firstPosition = attackerTransform.position;
		Sequence attackSequence = DOTween.Sequence();
		Transform firstTransform = attackerTransform.parent;

		attackSequence.AppendCallback(() => attackCard.view.setSorting(true))
			.Append(attackerTransform.DOMove(targetTransform.position, moveTime).SetEase(Ease.InCubic))
			.AppendCallback(() =>
			{
				targetCard.view.Show(targetDisplayComponent, isChangeSize: false);
				attackCard.view.Show(attacerDisplayComponent, isChangeSize: false);
				SEManager.instance.playSe("Blow");
			})
			.Append(attackerTransform.DOMove(firstPosition, moveTime).SetEase(Ease.OutCubic))
			.Join(attackerTransform.DOScale(defaultSize, moveTime))
			.AppendCallback(() => 
			{
				attackCard.view.setSorting(false);
				endTween();
			});
	}
}

public class EndUnitAttackVfx : TweenVfx
{
	private float cancelAttackTime = 0.3f;
	private Vector3 defaultSize = new Vector3(1.2f, 1.2f, 1.2f);
	public 	EndUnitAttackVfx(CardController attackCard) : base()
	{
		this.tweenAction = () => attack(attackCard);
	}

	public void attack(CardController attackCard)
	{
		Transform attackerTransform = attackCard.transform;
		if (attackerTransform.localScale == defaultSize)
		{
			endTween();
			return;
		}
		Vector3 firstPosition = attackerTransform.position;
		Sequence attackSequence = DOTween.Sequence();
		attackSequence.Append(attackerTransform.DOScale(defaultSize, cancelAttackTime))
			.AppendCallback(() =>
			{
				endTween();
			});
	}
}


// エフェクト表示系 ----------------------------------------------------------------------------------------------------------------------------------------------------------------
public class LeaderAttackVfx : SingleVfx
{
	public LeaderAttackVfx(string effectName) : base()
	{
		coroutine = singleAttack(effectName);
	}

	public IEnumerator singleAttack(string effectName)
	{
		Effect.instance.playerFixedEffect(effectName);
		SEManager.instance.playSe(effectName);
		yield return null;
	}
}

public class WholeAttackVfx : SingleVfx
{
	public WholeAttackVfx(string effectName, bool seSkip = false) : base()
	{
		coroutine = wholeAttack(effectName);
	}

	public IEnumerator wholeAttack(string effectName)
	{
		Effect.instance.wholeAttack(effectName);
		yield return new WaitForSeconds(0.2f);
		SEManager.instance.playSe(effectName);
	}
}

public class PlayEffectVfx : SingleVfx
{
	private List<string> linearMovementEffectName = new List<string>()
	{
		"Fireball"
	};

	public PlayEffectVfx(Transform targetTransform, string effectName, bool seSkip = false, Transform firstTransform = null, float waitTimeOverride = -1f) : base()
	{
		coroutine = playEffect(targetTransform, effectName, seSkip, firstTransform, waitTimeOverride);
	}

	public IEnumerator playEffect(Transform targetTransform, string effectName, bool seSkip, Transform firstTransform, float waitTimeOverride)
	{
		float efectTime = Effect.instance.generateEffect(effectName, targetTransform, firstTransform);
		bool isLinear = linearMovementEffectName.Contains(effectName);
		if (!isLinear && !seSkip)
		{
			SEManager.instance.playSe(effectName);
		}
		float waitTime = waitTimeOverride >= 0 ? waitTimeOverride : efectTime;
		yield return new WaitForSeconds(Math.Max(waitTime, 0f));
		// 線形に飛んでいくエフェクトの場合、着弾後にseを鳴らす
		if (isLinear && !seSkip)
		{
			SEManager.instance.playSe(effectName);
		}
	}
}

public class ChangeColorVfx : TweenVfx
{
	private Dictionary<string, Color> effectColorDic = new Dictionary<string, Color>()
	{
		{"SpiralCharge", Color.cyan},
		{"Activate", Color.yellow},
		{"CostChange", Color.green}
	};

	public ChangeColorVfx(CardController targetCard, string effectName, float waitTime = 0f, bool seSkip = false) : base()
	{
		this.tweenAction = () => changeColor(targetCard, effectName, waitTime, seSkip);
	}

	public void changeColor(CardController targetCard, string effectName, float waitTime, bool seSkip)
	{
		Sequence sequence = DOTween.Sequence();
		sequence
			.AppendCallback(() => 
			{
				if (!seSkip)
				{
					SEManager.instance.playSe(effectName);
				}
			})
			.AppendInterval(waitTime)
			.AppendCallback(() => endTween());
	}
}

public class GameSetVfx : SingleVfx
{
	public GameSetVfx(bool isSelfWin) : base()
	{
		coroutine = gameSet(isSelfWin);
	}

	public IEnumerator gameSet(bool isSelfWin)
	{
		yield return new WaitForSeconds(1.0f);
		BattleManager.instance.showResultPanel(isSelfWin);
		GameManager.instance.coroutineProcessor.stopAllCoroutine();
	}
}

// カードプレイ関連 --------------------------------------------------------------------------------------------------------------------------
public class WaitSelectVfx : ActionVfx
{
	CardController card;

	public WaitSelectVfx(CardController card, SelectComponent selectComponent, int index, string originalCardId = null, bool isEvolve = false) : base()
	{
		this.card = card;
		actionList.Add(waitSelect(card, selectComponent, index, originalCardId, isEvolve));
	}

	// 他のカードがプレイ中なら、カードのプレイを待機（選択、チョイス）
	public Action waitSelect(CardController card, SelectComponent selectComponent, int index, string originalCardId, bool isEvolve)
	{
		return () =>
		{
			BattleManager.instance.showDescribe(this);
			BattleManager.instance.selectPanel.startSelectCommon(selectComponent, selectingCard: card, originalCardId: originalCardId, isEvolve: isEvolve);
		};
	}
}

public class OnPlayMotionVfx : TweenVfx
{
	CardController card;

	public OnPlayMotionVfx(CardController card, bool isWait) : base()
	{
		this.card = card;
		this.tweenAction = () => onPlayMotion(card, isWait);
	}

	public void onPlayMotion(CardController card, bool isWait)
	{
		Sequence sequence = DOTween.Sequence();
		float moveTime = isWait ? 0.1f : 0.01f;
		sequence
			.Append(card.transform.DOMove(BattleManager.instance.mainPanel.center.position, moveTime))
			.Join(card.transform.DOMove(BattleManager.instance.mainPanel.center.position, moveTime))
			.Join(card.transform.DOScale(card.centerCardSize, moveTime))
			.AppendCallback(() => 
			{
				card.transform.SetParent(BattleManager.instance.mainPanel.playArea, false);
				SEManager.instance.playSe("PlayCard");
			})
			.AppendInterval(0.5f)
			.AppendCallback(() => endTween());
	}
}

// ターン制御関連 --------------------------------------------------------------------------------------------------------------
public class TurnPanelVfx : SingleVfx
{
	public TurnPanelVfx(bool isSelfTurn) : base()
	{
		coroutine = displayTurnPanel(isSelfTurn);
	}

	public IEnumerator displayTurnPanel(bool isSelfTurn)
	{
		BattleManager.instance.turnPanel.setTurnPanel(active: true, isSelfTurn: isSelfTurn);
		yield return new WaitForSeconds(1.2f);
		BattleManager.instance.turnPanel.close();
		yield return new WaitForSeconds(0.5f);
	}
}

public class AfterSelfTurnStartVfx : ActionVfx
{
	public AfterSelfTurnStartVfx() : base()
	{
		actionList.Add(changeCanOperation());
	}

	public Action changeCanOperation()
	{
		return () =>
		{
			BattleManager.instance.canOperationCard = true;
		};
	}
}

public class ChangeTurnVfx : ActionVfx
{
	public ChangeTurnVfx() : base()
	{
		actionList.Add(turnCalc());
	}

	public Action turnCalc()
	{
		return () =>
		{
			BattleManager.instance.turnStart();
		};
	}
}