using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : PanelPrefab
{
	[SerializeField] GameObject descriptionPanel, tutorialPanel;
	[SerializeField] Text descriptionText;
	[SerializeField] Button backDescription, nextDescription;

	public override void setup()
	{
		close();
	}
	public override void close()
	{
		tutorialPanel.SetActive(false);
	}
	public override void onClickExtra() {}

	public void setDescriptionPanel(bool show)
	{
		descriptionPanel.SetActive(show);
	}

	public void setDescriptionText(string text)
	{
		descriptionText.text = text;
	}

	public void setTutorialPanel(bool show)
	{
		tutorialPanel.SetActive(show);
		setDescriptionPanel(false);
	}
}
