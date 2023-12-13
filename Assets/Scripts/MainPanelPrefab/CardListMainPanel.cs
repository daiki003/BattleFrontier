using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardListMainPanel : MonoBehaviour
{
    public Transform cardListTransform;
	public Transform cardList, skillList, invisibleArea;
	public GameObject changeClassPanel, searchPanel, pickUpCardPanel, pickUpSkillPanel;
	public Transform pickUpCard, pickUpDescribe;
	public Transform pickUpSkill, pickUpDescribeLevel1, pickUpDescribeLevel2, pickUpDescribeLevel3;
	public Text classNameText, changeSkillText;

    public void destroy()
    {
        Destroy(this.gameObject);
    }

    public void back()
    {
        GameManager.instance.cardListMgr.back();
    }

    public void showChangeClassPanel(bool show)
    {
        GameManager.instance.cardListMgr.showChangeClassPanel(show);
    }

    public void showSearchPanel(bool show)
    {
        GameManager.instance.cardListMgr.showSearchPanel(show);
    }

    public void searchButton(int category)
	{
		GameManager.instance.cardListMgr.searchButton(category);
	}
}
