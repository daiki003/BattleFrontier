using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class FlagArea : MonoBehaviour
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

	public void DropCard(CardController card)
	{
		var movement = card.cardObject.GetComponent<CardMovement>();
		if (!movement.isDraggable)
		{
			return;
		}
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
		// 現状のパワーが相手の理論上作れるパワーを上回っていたら、旗を取れる
		if (CalculateAreaPower(allyInstalledCardList) > CalculateTheoreticallyAreaPower(enemyInstalledCardList))
		{
			return 1;
		}
		else if (CalculateTheoreticallyAreaPower(allyInstalledCardList) < CalculateAreaPower(enemyInstalledCardList))
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

		// 霧カードが置かれている場合、数値の強さのみで判定する
		if (flagCardList.Any(c => c.flagCardType == FlagCardType.Fog))
		{
			return CardListUtility.GetTotalPower(cardList);
		}

		var isFlushData = IsFlush(cardList);
		var isStraightData = IsStraight(cardList);
		var isThreeCardData = IsThreeCard(cardList);

		// ストレートフラッシュを判定
		if (isFlushData.isFlush && isStraightData.isStraight)
		{
			return 100000 + isStraightData.power;
		}

		// 3カードを判定
		if (isThreeCardData.isThreeCard)
		{
			return 10000 + isThreeCardData.power;
		}

		// フラッシュを判定
		if (isFlushData.isFlush)
		{
			return 1000 + isFlushData.power;
		}

		// ストレートを判定
		if (isStraightData.isStraight)
		{
			return 100 + isStraightData.power;
		}
		return CardListUtility.GetTotalPower(cardList);
	}

	// 3カードかどうか
	public (bool isThreeCard, int power) IsThreeCard(List<NormalCard> sortedCardList)
	{
		var baseCard = sortedCardList.FirstOrDefault(c => c.number != -1);
		if (baseCard == null)
		{
			// ジョーカーのみの場合、パワーは最大の10*枚数分で返す
			return (true, 10 * sortedCardList.Count);
		}
		int power = baseCard.number * sortedCardList.Count;
		bool isThreeCard = sortedCardList.All(c => c.number == baseCard.number || c.number == -1);
		return (isThreeCard, power);
	}

	// フラッシュかどうか
	public (bool isFlush, int power) IsFlush(List<NormalCard> sortedCardList)
	{
		var baseCard = sortedCardList.FirstOrDefault(c => c.cardColor != Color.white);
		if (baseCard == null)
		{
			// ジョーカーのみの場合、パワーは最大の10*枚数分で返す
			return (true, 10 * sortedCardList.Count);
		}
		bool isFlush = sortedCardList.All(c => c.cardColor == baseCard.cardColor || c.cardColor == Color.white);
		int power = 0;
		for (int i = 0; i < sortedCardList.Count; i++)
		{
			// ジョーカーの場合、パワーは最大の10として扱う
			if (sortedCardList[i].number == -1)
			{
				power += 10;
			}
			else
			{
				power += sortedCardList[i].number;
			}
		}
		return (isFlush, power);
	}

	// ストレートかどうか
	public (bool isStraight, int power) IsStraight(List<NormalCard> sortedCardList)
	{
		bool containJoker = sortedCardList.Any(c => c.number == -1);
		bool useJoker = false;
		var cardListWithoutJoker = sortedCardList.Where(c => c.number != -1).ToList();
		var streatCardList = new List<NormalCard>();
		int power = 0;
		for (int i = 0; i < cardListWithoutJoker.Count; i++)
		{
			var lastCard = streatCardList.LastOrDefault();
			if (lastCard == null || 
				cardListWithoutJoker[i].number == (lastCard.number + 1))
			{
				streatCardList.Add(cardListWithoutJoker[i]);
				power += cardListWithoutJoker[i].number;
			}
			else if (containJoker && !useJoker && cardListWithoutJoker[i].number == (lastCard.number + 2))
			{
				streatCardList.Add(cardListWithoutJoker[i]);
				// このカードの1つ下の番号として、ジョーカーの分のパワーも加算
				power += cardListWithoutJoker[i].number + cardListWithoutJoker[i].number - 1;
				useJoker = true;
			}
			else
			{
				return (false, 0);
			}
		}
		// ジョーカー判定が余っていた場合、総パワーが最も高くなるようにジョーカー分のパワーを加算
		if (containJoker && !useJoker)
		{
			if (cardListWithoutJoker[cardListWithoutJoker.Count - 1].number == 10)
			{
				power += cardListWithoutJoker[0].number - 1;
			}
			else
			{
				power += cardListWithoutJoker[cardListWithoutJoker.Count - 1].number + 1;
			}
		}
		return (true, power);
	}

	// 理論上のエリアパワーを計算
	public int CalculateTheoreticallyAreaPower(List<NormalCard> actualCardList)
	{
		// カードが最大枚数あれば、実際のパワーを返す
		if (actualCardList.Count >= cardSlot)
		{
			return CalculateAreaPower(actualCardList);
		}

		// 計算用のリストに変換
		var cardList = new List<NormalCard>(actualCardList).OrderBy(c => c.number).ToList();

		// 霧カードが置かれているか、何の役も作れない場合は単純に数値の大きいカードを詰める
		if (flagCardList.Any(c => c.flagCardType == FlagCardType.Fog) ||
			(!CreateStraightFlush(ref cardList) &&
			!CreateThreeCard(ref cardList) &&
			!CreateFlush(ref cardList) &&
			!CreateStraight(ref cardList, color: Color.white)))
		{
			var deckAndHandCardList = GameManager.instance.battleMgr.GetDeckAndHandNormalCardList().OrderByDescending(c => c.number).ToList();
			cardList.AddRange(deckAndHandCardList.Take(cardSlot - actualCardList.Count));
		}

		// 理論上最強の役を作った状態でパワーを判定
		return CalculateAreaPower(cardList);
	}

	// 理論上で3カードを作成
	public bool CreateThreeCard(ref List<NormalCard> sortedCardList)
	{
		// カードが1枚もなければ必ず作れる
		if (sortedCardList.Count == 0)
		{
			return true;
		}

		// 現時点で3カードでなければ作れない
		if (!IsThreeCard(sortedCardList).isThreeCard)
		{
			return false;
		}

		int number = sortedCardList.FirstOrDefault().number;
		int cardCount = sortedCardList.Count;
		List<NormalCard> deckAndHandSameNumberCardList = GameManager.instance.battleMgr.GetDeckAndHandNormalCardList().Where(c => c.number == number).ToList();
		// 手札かデッキに、同じ数字のカードがスロット埋める分だけあれば作れる
		if (deckAndHandSameNumberCardList.Count() >= (cardSlot - cardCount))
		{
			sortedCardList.AddRange(deckAndHandSameNumberCardList.Take(cardSlot - cardCount));
			return true;
		}
		return false;
	}

	// 理論上でフラッシュを作成
	public bool CreateFlush(ref List<NormalCard> sortedCardList)
	{
		// カードが1枚もなければ必ず作れる
		if (sortedCardList.Count == 0)
		{
			return true;
		}

		// 現時点でフラッシュでなければ作れない
		if (!IsFlush(sortedCardList).isFlush)
		{
			return false;
		}

		Color color = sortedCardList.FirstOrDefault().cardColor;
		int cardCount = sortedCardList.Count;
		List<NormalCard> deckAndHandSameColorCardList = GameManager.instance.battleMgr.GetDeckAndHandNormalCardList().Where(c => c.cardColor == color).OrderByDescending(c => c.number).ToList();
		// 手札かデッキに、同じ色のカードがスロット埋める分だけあれば作れる
		if (deckAndHandSameColorCardList.Count() >= (cardSlot - cardCount))
		{
			sortedCardList.AddRange(deckAndHandSameColorCardList.Take(cardSlot - cardCount));
			return true;
		}
		return false;
	}

	// 理論上でストレートを作成
	public bool CreateStraight(ref List<NormalCard> sortedCardList, Color color)
	{
		// 大きい数字のストレートから順に判定
		for (int i = 10; i >= cardSlot; i--)
		{
			// 連続するスロット数分の数のリストを作成
			var straightNumberList = new List<int>();
			for (int j = 0; j < cardSlot; j++)
			{
				straightNumberList.Add(i - j);
			}

			// 上で作成したリストに、判定したいカードリストの番号が全て含まれていればストレートを作れる可能性がある
			bool judgeNext = false;
			bool haveJoker = false;
			for (int j = 0; j < sortedCardList.Count; j++)
			{
				if (sortedCardList[j].number == -1)
				{
					haveJoker = true;
				}
				else if (straightNumberList.Contains(sortedCardList[j].number))
				{
					straightNumberList.Remove(sortedCardList[j].number);
				}
				else
				{
					judgeNext = true;
					break;
				}
			}

			// 含まれていないカードがあった場合次を判定
			if (judgeNext)
			{
				continue;
			}
			// 残った番号全てがデッキか手札にあれば作れる
			var useCardList = new List<NormalCard>();
			for (int j = 0; j < straightNumberList.Count; j++)
			{
				// 色指定がwhite以外なら、色も同じである必要がある
				var appropriateCard = GameManager.instance.battleMgr.GetDeckAndHandNormalCardList().FirstOrDefault(c => c.number == straightNumberList[j] && (color == Color.white || c.cardColor == color));
				if (appropriateCard != null)
				{
					useCardList.Add(appropriateCard);
				}
				else if (haveJoker)
				{
					// なくても、ジョーカーが含まれている場合フラグを消費してチェックを続けられる
					haveJoker = false;
				}
				else
				{
					return false;
				}
			}
			sortedCardList.AddRange(useCardList.Take(cardSlot - sortedCardList.Count));
			return true;
		}
		return false;
	}

	// 理論上でストレートフラッシュを作成
	public bool CreateStraightFlush(ref List<NormalCard> sortedCardList)
	{
		// 現時点でフラッシュになっていなければアウト
		if (!IsFlush(sortedCardList).isFlush)
		{
			return false;
		}

		var card = sortedCardList.FirstOrDefault(c => c.cardColor != Color.white);
		var cardColor = card != null ? card.cardColor : Color.white;
		if (cardColor == Color.white)
		{
			// 特定の色の指定がない場合、最も数値の高いストレートフラッシュを作れる色で作成する
			bool canCreate = false;
			var strongestStraightList = new List<NormalCard>(sortedCardList);
			foreach (Color color in GameManager.instance.battleMgr.cardColorList)
			{
				var straightFlushList = new List<NormalCard>();
				canCreate |= CreateStraight(ref straightFlushList, color);
				// この色のストレートフラッシュが以前より強ければ、このリストで置き換える
				if (CardListUtility.GetTotalPower(straightFlushList) > CardListUtility.GetTotalPower(strongestStraightList))
				{
					strongestStraightList = straightFlushList;
				}
			}
			if (canCreate)
			{
				sortedCardList = strongestStraightList;
				return true;
			}
			return false;
		}
		else
		{
			return CreateStraight(ref sortedCardList, cardColor);
		}
	}
}
