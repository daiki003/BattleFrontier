using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMainPanel : MonoBehaviour
{
    public Transform battleTransform;
	public Transform field, deck, grave;
	public Transform banishArea, onplay, onsold;
	public Transform center, playArea, spellArea, invisibleArea;
    public HorizontalLayoutGroup centerLayoutGroup;
    public Transform playerHand;
    public HorizontalLayoutGroup holizontalLayoutGroup;
    public Text enemyHandText;

	public Button turnEndButton, normalDrawButton, specialDrawButton;
    [System.NonSerialized] public BattleManager battleMgr;
	[System.NonSerialized] public bool focusHand;

	private const int FLAG_AREA_NUMBER = 9;

    public void StartBattle()
	{
		playArea.resetCard();
		center.resetCard();
		invisibleArea.resetCard();
		banishArea.resetCard();
		
		ChangeFocusHand(false, force: true);
		SetUpField();
	}
	
	public void destroy()
    {
        Destroy(this.gameObject);
    }

	// ボタン系 ------------------------------------------------------------------------------------------------------------
    public void onClickTurnEnd()
    {
        battleMgr.onClickTurnEnd();
    }

    public void goToTitle()
    {
        battleMgr.selectDialog.Init(mainText: "リタイアしますか？", okButtonText: "リタイア", okCallback: GameManager.instance.GoToTitle);
    }

    public void NormalDraw(bool isSpecial)
    {
        battleMgr.DrawCard(isSpecial);
    }
	
	public void changeCenterSpacing(float spacing)
    {
        centerLayoutGroup.spacing = spacing;
    }

    public void SetUpField()
    {
        field.resetCard();
        battleMgr.flagAreaList = new List<FlagArea>();
        for (int i = 0; i < BattleManager.FLAG_AREA_NUMBER; i++)
        {
            var area = PrefabManager.instance.createArea(field, i);
            battleMgr.flagAreaList.Add(area);
        }
    }

    public void ChangeGraveColor(bool isShine)
    {
        grave.gameObject.GetComponent<Image>().color = isShine ? new Color32(255, 200, 0, 100) : new Color32(50, 50, 50, 100);
    }

	// 手札の拡大、縮小
	public void ChangeFocusHand(bool focus, bool force = false)
	{
		if (focusHand == focus && !force) return;

		focusHand = focus;
		Vector2 focusSize = new Vector2(2000, 370);
		Vector2 unFocusSize = new Vector2(1250, 300);
		Vector3 focusPosition = new Vector3(110, 150, 0);
		Vector3 unFocusPosition = new Vector3(500, 100, 0);

		RectTransform handTransform = (RectTransform)playerHand;
		handTransform.sizeDelta = focus ? focusSize : unFocusSize;
		handTransform.anchoredPosition = focus ? focusPosition : unFocusPosition;

		var handCardList = GameManager.instance.battleMgr.player.cardCollection.handCardList;
		foreach (CardController card in  handCardList)
		{
			card.view.changeSize(focus ? card.focusHandSize : card.unFocusHandSize);
		}
		refreshCardSpace(handCardList.Count);
		if (!force) SEManager.instance.playSe("Draw");
	}

	// 手札の枚数に応じてカードのスペースを変更する
	public void refreshCardSpace(int count)
	{
		BattleManager.instance.mainPanel.holizontalLayoutGroup.spacing = getHandSpacing(count);
	}

	public int getHandSpacing(int cardNumber)
	{
		if (cardNumber < 8)
		{
			return focusHand ? 50 : -50;
		}
		else if (cardNumber == 8)
		{
			return focusHand ? 10 : -70;
		}
		else if (cardNumber == 9)
		{
			return focusHand ? -10 : -80;
		}
		else
		{
			return focusHand ? -25 : -100;
		}
	}

    public void update(bool canDraw, bool canTurnEnd, string enemyHandText)
    {
        normalDrawButton.interactable = canDraw;
        specialDrawButton.interactable = canDraw;
        turnEndButton.interactable = canTurnEnd;
        this.enemyHandText.text = enemyHandText;
		refreshCardSpace(playerHand.childCount);
    }
}
