using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

// フィールドにアタッチするクラス
public class DropPlace : MonoBehaviour, IDropHandler
{
	public PlayerController player { get { return BattleManager.instance.player; } }
	[SerializeField] Transform content;
	public DropType type;

	public void OnDrop(PointerEventData eventData) // ドロップされた時に行う処理
	{
		CardController card = eventData.pointerDrag.GetComponent<CardController>(); // ドラッグしてきた情報からCardMovementを取得
		SkillPanelController skill = eventData.pointerDrag.GetComponent<SkillPanelController>(); // ドラッグしてきた情報からSkillMovementを取得

		// ドロップしたオブジェクトがカードかスキルかを判定
		if (card != null) dropCard(card);
		else return;
	}

	public void dropCard(CardController card)
	{
		if (!card.movement.isDraggable) return;

		switch (type)
		{
			case DropType.HAND:
				return;
			case DropType.FIELD:
				break;
			case DropType.CARD:
				if (GameManager.instance.isBattle)
				{
					if (card.isFieldCard) return;
					if (!card.canPlay()) return;

					CardController dropedCard = GetComponent<CardController>();
					if (dropedCard.targetArea != null)
					{
						dropedCard.targetArea.SetCard(card);
					}
				}
				break;
		}
	}
}
