using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class FlagArea : MonoBehaviour, IDropHandler
{
    public Transform playArea;
    public Transform allyArea;
    public Transform enemyArea;
	public Transform flagCardArea;
	public Transform firstFlagPosition;
	public Transform allyFlagPosition;
	public Transform enemyFlagPosition;
	public GameObject flag, fogIcon, madIcon;
    public int fieldIndex;
	// カードを置ける最大枚数（Madがあれば4枚、なければ3枚）
	public int cardSlot { get { return flagCardList.Any(c => c.flagCardType == FlagCardType.Mad) ? 4 : 3; } }
	public List<NormalCard> allyInstalledCardList = new List<NormalCard>();
	public List<NormalCard> enemyInstalledCardList = new List<NormalCard>();
	public bool isPlayerArea;
	public bool isEnemyArea;
	public List<FlagCard> flagCardList = new List<FlagCard>();

	public void OnDrop(PointerEventData eventData) // ドロップされた時に行う処理
	{
		CardMovement movement = eventData.pointerDrag.GetComponent<CardMovement>(); // ドラッグしてきた情報からCardMovementを取得
		if (!movement.isDraggable)
		{
			return;
		}
		SkillPanelController skill = eventData.pointerDrag.GetComponent<SkillPanelController>(); // ドラッグしてきた情報からSkillMovementを取得
		if (!GameManager.instance.canOperationCard) return;
		SetCard(movement.card, GameManager.instance.battleMgr.isSelfTurn);
	}

	public void SetCard(CardController card, bool isSelf = true)
	{
		if (card == null || !CanSetCard(card, isSelf))
		{
			return;
		}

		// 列に対して使うカードなら列に追加する
		if (card is FlagCard flagCard)
		{
			card.SetToFlagArea(this, flagCardArea);
			flagCardList.Add(flagCard);
			if (flagCard.flagCardType == FlagCardType.Fog)
			{
				fogIcon.SetActive(true);
			}
			else if (flagCard.flagCardType == FlagCardType.Mad)
			{
				madIcon.SetActive(true);
			}
		}
		else if (card is NormalCard normalCard)
		{
			card.SetToFlagArea(this, isSelf ? allyArea : enemyArea);
			if (isSelf)
			{
				allyInstalledCardList.Add(normalCard);
			}
			else
			{
				enemyInstalledCardList.Add(normalCard);
			}
		}
		else
		{
			return;
		}

		if (isSelf)
		{
			BattleManager.instance.player.isPlayCard = true;
		}
		else
		{
			BattleManager.instance.enemy.isPlayCard = true;
		}
		SEManager.instance.playSe("InHand");

		// 自分側なら、相手に通信を送る
		if (isSelf)
		{
			var parameter = new NetworkParameter();
			parameter.cardIndex = card.index;
			parameter.fieldIndex = fieldIndex;
			GameManager.instance.networkManager.SendAction(NetworkOperationType.PLAY_CARD, parameter);
		}
	}

    public void ChangeAreaColor(CardController card, bool isShine)
    {
		// カードが設置できない場合は光らせない
		if (isShine && !CanSetCard(card, GameManager.instance.battleMgr.isSelfTurn))
		{
			isShine = false;
		}
        playArea.gameObject.GetComponent<Image>().color = isShine ? new Color32(255, 200, 0, 100) : new Color32(50, 50, 50, 100);
    }

	// 対象のカードがセット可能かどうか
	private bool CanSetCard(CardController card, bool isSelf)
	{
		var battleMgr = GameManager.instance.battleMgr;
		var player = isSelf ? battleMgr.player : battleMgr.enemy;
		var cardList = isSelf ? allyInstalledCardList : enemyInstalledCardList;

		// 通常カードなら、規定の枚数カードが存在していたらプレイ不可
		// 旗に置くカードなら、同じ種類のカードがすでに置かれていたらプレイ不可
		bool isFull = ((card is NormalCard) && cardList.Count >= cardSlot) || ((card is FlagCard flagCard) && flagCardList.Any(c => c.flagCardType == flagCard.flagCardType));
		return !isFull && (!player.isPlayCard || battleMgr.isTestBattle);
	}

	public void MoveFlag(bool isSelf)
	{
		if (isSelf)
		{
			var parameter = new NetworkParameter();
			parameter.fieldIndex = this.fieldIndex;
			GameManager.instance.networkManager.SendAction(NetworkOperationType.MOVE_FLAG, parameter);
			flag.transform.parent = allyFlagPosition;
			isPlayerArea = true;
		}
		else
		{
			flag.transform.parent = enemyFlagPosition;
			isEnemyArea = true;
		}
		BattleManager.instance.CheckResult();
	}

	// どちらが旗を取る権利があるかを判定
	// どちらもなければ0、自分側なら1、相手側なら-1
	public int JudgeArea()
	{
		if (CalculateAreaPower(allyInstalledCardList) == -1 || CalculateAreaPower(enemyInstalledCardList) == -1)
		{
			return 0;
		}
		if (CalculateAreaPower(allyInstalledCardList) > CalculateAreaPower(enemyInstalledCardList))
		{
			return 1;
		}
		else if (CalculateAreaPower(allyInstalledCardList) < CalculateAreaPower(enemyInstalledCardList))
		{
			return -1;
		}
		return 0;
	}

	public int CalculateAreaPower(List<NormalCard> actualCardList)
	{
		// カードが最大枚数以下なら無効値
		if (actualCardList.Count < cardSlot)
		{
			return -1;
		}

		// 計算用のリストに変換
		var cardList = new List<NormalCard>(actualCardList).OrderBy(c => c.number).ToList();
		int point = 0;

		// 数字の強さを判定
		for (int i = 0; i < cardList.Count; i++)
		{
			point += cardList[i].number;
		}

		// 霧カードが置かれている場合、数値の強さのみで判定する
		if (flagCardList.Any(c => c.flagCardType == FlagCardType.Fog))
		{
			return point;
		}
		
		// ストレートフラッシュを判定
		if (IsFlush(cardList) && IsStreat(cardList))
		{
			point += 100000;
		}

		// 3カードを判定
		if (IsThreeCard(cardList))
		{
			point += 10000;
		}

		// フラッシュを判定
		if (IsFlush(cardList))
		{
			point += 1000;
		}

		// ストレートを判定
		if (IsStreat(cardList))
		{
			point += 100;
		}
		return point;
	}

	// 3カードかどうか
	public bool IsThreeCard(List<NormalCard> sortedCardList)
	{
		var cardListWithoutJoker = sortedCardList.Where(c => c.number != -1).ToList();
		var threeCardList = new List<NormalCard>();
		for (int i = 0; i < cardListWithoutJoker.Count; i++)
		{
			if (threeCardList.All(c => c.number == cardListWithoutJoker[i].number || c.number == -1 || cardListWithoutJoker[i].number == -1))
			{
				threeCardList.Add(cardListWithoutJoker[i]);
			}
			else
			{
				return false;
			}
		}
		return true;
	}

	// フラッシュかどうか
	public bool IsFlush(List<NormalCard> sortedCardList)
	{
		var cardListWithoutJoker = sortedCardList.Where(c => c.cardColor != Color.white).ToList();
		var flushCardList = new List<NormalCard>();
		for (int i = 0; i < cardListWithoutJoker.Count; i++)
		{
			if (flushCardList.All(c => c.cardColor == cardListWithoutJoker[i].cardColor))
			{
				flushCardList.Add(cardListWithoutJoker[i]);
			}
			else
			{
				return false;
			}
		}
		return true;
	}

	// ストレートかどうか
	public bool IsStreat(List<NormalCard> sortedCardList)
	{
		bool containJoker = sortedCardList.Any(c => c.number == -1);
		bool useJoker = false;
		var cardListWithoutJoker = sortedCardList.Where(c => c.number != -1).ToList();
		var streatCardList = new List<NormalCard>();
		for (int i = 0; i < cardListWithoutJoker.Count; i++)
		{
			var lastCard = streatCardList.LastOrDefault();
			if (lastCard == null || 
				cardListWithoutJoker[i].number == (lastCard.number + 1))
			{
				streatCardList.Add(cardListWithoutJoker[i]);
			}
			else if (containJoker && !useJoker && cardListWithoutJoker[i].number == (lastCard.number + 2))
			{
				streatCardList.Add(cardListWithoutJoker[i]);
				useJoker = true;
			}
			else
			{
				return false;
			}
		}
		return true;
	}
}
