using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

// フィールドにアタッチするクラス
public class DropPlace : MonoBehaviour, IDropHandler
{
	public PlayerController player { get { return BattleManager.instance.player; } }
	public DropType type;

	public void OnDrop(PointerEventData eventData) // ドロップされた時に行う処理
	{
		CardMovement movement = eventData.pointerDrag.GetComponent<CardMovement>(); // ドラッグしてきた情報からCardMovementを取得
		SkillPanelController skill = eventData.pointerDrag.GetComponent<SkillPanelController>(); // ドラッグしてきた情報からSkillMovementを取得

		// ドロップしたオブジェクトがカードかスキルかを判定
		if (movement != null) dropCard(movement.card);
		else return;
	}

	public void dropCard(CardController card)
	{
		if (!GameManager.instance.canOperationCard) return;

		switch (type)
		{
			case DropType.FLAG:
				if (GameManager.instance.isBattle)
				{
					FlagArea flagArea = GetComponent<FlagArea>();
					flagArea.DropCard(card);
				}
				break;
			case DropType.CARD:
				if (GameManager.instance.isBattle)
				{
					if (card.isFieldCard) return;
					if (!card.canPlay()) return;

					CardMovement movement = GetComponent<CardMovement>();
					if (!movement.isDraggable)
					{
						return;
					}
					if (movement.card.targetArea != null)
					{
						movement.card.targetArea.SetCard(card, GameManager.instance.battleMgr.isSelfTurn);
					}
				}
				break;
		}
	}
}
