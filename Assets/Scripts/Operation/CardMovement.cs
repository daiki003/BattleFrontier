using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public Transform beforeDragParent;
	public Transform draggingParent;
	public Vector3 firstPosition;
	public bool isDraggable;
	public CardController card;
	private LayoutGroup layoutGroup;
	private bool isScroll;
	private BattleManager battleMgr { get { return GameManager.instance.battleMgr; } }

	// ドラッグを始めるときに行う処理
	public void OnBeginDrag(PointerEventData eventData)
	{
		isDraggable = false;

		isDraggable = GameManager.instance.canOperationCard && !card.model.isShowCard && !card.model.isSelectTarget && !card.isFieldCard;
		ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
		isScroll = !isDraggable && scrollRect != null;
		if (isScroll)
		{
			scrollRect.OnBeginDrag(eventData);
		}
		else if (isDraggable)
		{
			battleMgr.movingCard = card;

			beforeDragParent = transform.parent;
			firstPosition = transform.position;

			GetComponent<CanvasGroup>().blocksRaycasts = false; // blocksRaycastsをオフにする

			draggingParent = transform.parent;
			card.view.setSorting(true);
		}
	}

	// ドラッグした時に起こす処理
	public void OnDrag(PointerEventData eventData)
	{
		if (isScroll)
		{
			ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
			scrollRect.OnDrag(eventData);
		}
		else if (isDraggable)
		{
			transform.position = eventData.position;
			List<CardController> fieldCardList = BattleManager.instance.player.cardCollection.fieldCardList;
			int index = fieldCardList.IndexOf(card);

			if (battleMgr.selectPanel.currentPhase == SelectPhase.RETURN_DECK)
			{
				battleMgr.mainPanel.ChangeGraveColor(IsOnTargetObject(battleMgr.mainPanel.spellArea.gameObject));
			}
			else if (card is SpellCard)
			{
				battleMgr.mainPanel.ChangeGraveColor(IsOnTargetObject(battleMgr.mainPanel.spellArea.gameObject));
			}
			else
			{
				var shineArea = GetRaycastResults().Select(r => r.gameObject.GetComponent<FlagArea>()).FirstOrDefault(s => s != null);
				foreach (FlagArea flagArea in BattleManager.instance.flagAreaList)
				{
					flagArea.ChangeAreaColor(card, flagArea == shineArea);
				}
			}
		}
	}

	// カードを離したときに行う処理
	public void OnEndDrag(PointerEventData eventData)
	{
		if (isScroll)
		{
			ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
			scrollRect.OnEndDrag(eventData);
			isScroll = false;
		}
		else if (isDraggable)
		{
			BattleManager.instance.movingCard = null;
			card.view.setSorting(false);

			// フィールドの色を元に戻す
			foreach (FlagArea flagArea in BattleManager.instance.flagAreaList)
			{
				flagArea.ChangeAreaColor(card, false);
			}
			battleMgr.mainPanel.ChangeGraveColor(isShine: false);

			if (battleMgr.selectPanel.currentPhase == SelectPhase.RETURN_DECK)
			{
				card.ReturnToDeck();
				battleMgr.selectPanel.AdvancePhase();
			}
			else if (card is SpellCard spellCard && IsOnTargetObject(GameManager.instance.battleMgr.mainPanel.spellArea.gameObject))
			{
				spellCard.OnWhenPlaySkill();
			}

			// 離したカードを元の位置に戻す処理
			if (transform.parent == draggingParent &&
				!card.isDead)
			{
				transform.SetParent(beforeDragParent, false);
				transform.position = this.firstPosition;
			}

			layoutGroup = beforeDragParent.gameObject.GetComponent<LayoutGroup>();
			if (layoutGroup != null)
			{
				layoutGroup.CalculateLayoutInputHorizontal();
				layoutGroup.SetLayoutHorizontal();
			}

			// blocksRaycastsをオンにする
			GetComponent<CanvasGroup>().blocksRaycasts = true;
		}
	}

	private List<RaycastResult> GetRaycastResults()
	{
		PointerEventData pointer = new PointerEventData(EventSystem.current);
		pointer.position = Input.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointer, results);
		return results;
	}

	public bool IsOnTargetObject(GameObject targetObject)
	{
		return GetRaycastResults().Any(r => r.gameObject == targetObject);
	}
}
