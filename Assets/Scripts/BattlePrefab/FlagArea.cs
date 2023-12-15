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
	public GameObject flag;
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
		SkillPanelController skill = eventData.pointerDrag.GetComponent<SkillPanelController>(); // ドラッグしてきた情報からSkillMovementを取得
		if (!GameManager.instance.canOperationCard) return;
		SetCard(movement.card);
	}

	public void SetCard(CardController card, bool isSelf = true)
	{
		if (card == null)
		{
			return;
		}

		// 列に対して使うカードなら列に追加する
		if (card is FlagCard flagCard)
		{
			card.SetToFlagArea(this, flagCardArea);
			flagCardList.Add(flagCard);
		}
		else if (card is NormalCard normalCard && CanSetCard(isSelf))
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

		BattleManager.instance.player.isPlayCard = true;
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

    public void ChangeAreaColor(bool isShine)
    {
		// カードが3枚以上ある場合は光らせない
		if (!CanSetCard(isSelf: true))
		{
			isShine = false;
		}
        playArea.gameObject.GetComponent<Image>().color = isShine ? new Color32(255, 200, 0, 100) : new Color32(50, 50, 50, 100);
    }

	private bool CanSetCard(bool isSelf)
	{
		if (isSelf)
		{
			return allyInstalledCardList.Count < cardSlot && !BattleManager.instance.player.isPlayCard;
		}
		else
		{
			return enemyInstalledCardList.Count < cardSlot;
		}
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
		for (int i = 0; i < sortedCardList.Count; i++)
		{
			if (threeCardList.All(c => c.number == sortedCardList[i].number || c.number == -1 || sortedCardList[i].number == -1))
			{
				threeCardList.Add(sortedCardList[i]);
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
			if (flushCardList.All(c => c.cardColor == sortedCardList[i].cardColor))
			{
				flushCardList.Add(sortedCardList[i]);
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
		var cardListWithoutJoker = sortedCardList.Where(c => c.number != -1).ToList();
		var streatCardList = new List<NormalCard>();
		for (int i = 0; i < cardListWithoutJoker.Count; i++)
		{
			var lastCard = streatCardList.LastOrDefault();
			if (lastCard == null || 
				sortedCardList[i].number == (lastCard.number + 1) ||
				(containJoker && sortedCardList[i].number == (lastCard.number + 2)))
			{
				streatCardList.Add(sortedCardList[i]);
			}
			else
			{
				return false;
			}
		}
		return true;
	}
}
