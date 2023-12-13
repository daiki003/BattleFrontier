using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelectButton : MonoBehaviour
{
	public Image classIcon;
	public Button button;
    public GameObject selectingPanel;
    public CardCategory category;
	public void init(CardCategory category, UnityEngine.Events.UnityAction action)
	{
		this.category = category;
        Sprite sprite = Resources.Load<Sprite>("Images/Character/" + category.ToString());
		this.classIcon.sprite = sprite;
		button.onClick.AddListener(action);
        button.onClick.AddListener(() => setSelectingPanel(true));
	}
    public void setSelectingPanel(bool isActive)
    {
        selectingPanel.SetActive(isActive);
    }
}