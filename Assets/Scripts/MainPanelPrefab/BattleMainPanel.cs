using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMainPanel : MonoBehaviour
{
    public Transform battleTransform;
	public Transform field, deck;
	public Transform banishArea, onplay, onsold;
	public Transform waitArea, center, playArea, skillArea, invisibleArea;
    public HorizontalLayoutGroup centerLayoutGroup;
    public Transform playerHand;
    public HorizontalLayoutGroup holizontalLayoutGroup;
    public Text enemyHandText;

	public Button turnEndButton, normalDrawButton, specialDrawButton;
    private BattleManager battleMgr = BattleManager.instance;

	private const int FLAG_AREA_NUMBER = 9;

    public void destroy()
    {
        Destroy(this.gameObject);
    }

    public void onClickTurnEnd()
    {
        battleMgr.onClickTurnEnd();
    }

    public void goToTitle()
    {
        battleMgr.selectDialog.Init(mainText: "リタイアしますか？", okButtonText: "リタイア", okCallback: GameManager.instance.GoToTitle);
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

    public void NormalDraw(bool isSpecial)
    {
        if (battleMgr.player.cardCollection.handCardList.Count < 7)
        {
            battleMgr.player.drawCard(1, isSpecial);
        }
    }

    public void update()
    {
        bool isActive = battleMgr.player.cardCollection.handCardList.Count < 7;
        normalDrawButton.interactable = isActive;
        specialDrawButton.interactable = isActive;
        turnEndButton.interactable = battleMgr.canOperationCard && battleMgr.player.cardCollection.handCardList.Count == 7;
        enemyHandText.text = battleMgr.enemy.cardCollection.handCardList.Count.ToString();
    }
}
