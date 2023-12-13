using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelView : MonoBehaviour
{
	[SerializeField] Image skillIcon;
	[SerializeField] Text level, questCount;

	public void setIcon(SkillPanelModel model)
	{
		skillIcon.sprite = model.icon;
		questCount.gameObject.SetActive(false);
	}

	public void resetLevel()
	{
		level.text = "";
	}

	public void levelUp()
	{
		level.text += "★";
	}

	public void setQuestCount(int count)
	{
		questCount.gameObject.SetActive(true);
		questCount.text = count.ToString();
	}

	public void changeActive(bool active)
	{
		changeColor(active ? Color.white : Color.gray);
		questCount.gameObject.SetActive(!active);
	}

	public void changeColor(Color color)
	{
		Image image = skillIcon.GetComponent<Image>();
		image.color = color;
	}

	public void shine(Color color)
	{
		this.skillIcon.gameObject.shine(color);
	}
}
