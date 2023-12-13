using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnClick : MonoBehaviour, IPointerClickHandler
{
	public ClickType type;
	public float clickTime;
	public CardController card;

	public void OnPointerClick(PointerEventData eventData)
	{
		switch (type)
		{
			case ClickType.CARD:
				// クリック後ドラッグした場合は無効
				if (eventData.dragging) return;

				if (!GameManager.instance.isBattle || card.model.isEnemy)
				{
					SEManager.instance.playSe("Select");
					GameManager.instance.showDescribe(card);
					break;
				}
				else
				{
					if (card.isHandCard && !BattleManager.instance.player.model.focusHand)
					{
						BattleManager.instance.player.changeFocusHand(true);
					}
					else
					{
						if (card.isFieldCard)
						{
							BattleManager.instance.trainingPhase.strengthenTarget = card;
						}
						SEManager.instance.playSe("Select");
						GameManager.instance.showDescribe(card);
					}
				}
				break;
			case ClickType.SELECTED_CARD:
				SEManager.instance.playSe("Select");
				CardMovement movement = GetComponent<CardMovement>();
				GameManager.instance.showDescribe(movement.card);
				movement.card.switchSelected();
				break;
			case ClickType.BONUS:
				SEManager.instance.playSe("Select");
				BonusPanel bonusPanel = GetComponent<BonusPanel>();
				bonusPanel.changeSelected();
				break;
			case ClickType.EXTRA:
				if (GameManager.instance.isBattle)
				{
					if (BattleManager.instance.player != null && !BattleManager.instance.isDescribe && (!BattleManager.instance.isSelect || BattleManager.instance.isCantCancelSelect))
					{
						BattleManager.instance.player.changeFocusHand(false);
					}
					BattleManager.instance.onClickExtra();
				}
				else if (GameManager.instance.isTitle)
				{
					GameManager.instance.titleMgr.onClickExtra();
				}
				else if (GameManager.instance.isCardList)
				{
					GameManager.instance.cardListMgr.onClickExtra();
					GameManager.instance.cardListMgr.cancelPickUp();
				}
				GameManager.instance.debugMgr.setInputField(false);
				GameManager.instance.debugMgr.setAddCardField(false);
				break;
			case ClickType.DECK:
				break;
			case ClickType.FLAG:
				var flagArea = transform.parent.parent.GetComponent<FlagArea>();
				if (flagArea != null && flagArea.JudgeArea() == 1)
				{
					flagArea.MoveFlag(isSelf: true);
				}
				break;
			case ClickType.SHOW_CARD:
				GameManager.instance.deleteDescribe();
				break;
			case ClickType.SKILL:
				SEManager.instance.playSe("Select");
				SkillPanelController skill = GetComponent<SkillPanelController>();
				break;
			case ClickType.SKILL_AND_QUEST:
				SEManager.instance.playSe("Select");
				SkillAndQuest skillAndQuest = GetComponent<SkillAndQuest>();
				if (skillAndQuest.skill != null)
				{
					skillAndQuest.switchSelected();
					// skillAndQuest.deckEditSkill.showDescribe();
				}
				break;
			case ClickType.PLAYER:
				SEManager.instance.playSe("Select");
				break;
			case ClickType.DELETE_DESCRIBE:
				GameManager.instance.deleteDescribe();
				break;
			case ClickType.DEBUG:
				GameManager.instance.debugMgr.openDebugMenu();
				break;
			case ClickType.DEBUG_CARD_NAME:
				DebugCardName cardName = GetComponent<DebugCardName>();
				GameManager.instance.debugMgr.selectCardName(cardName);
				break;
			default:
				break;
		}
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			clickTime = Time.time;
		}
	}
}
