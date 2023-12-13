using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanel : PanelPrefab
{
	public GameObject turnPanel;
	public Text turnText;

	public override void setup()
	{
		close();
	}
	public override void close()
	{
		turnPanel.SetActive(false);
	}
	public override void onClickExtra() {}

	public void setTurnPanel(bool active, bool isSelfTurn)
	{
		if (active)
		{
			turnText.text = (isSelfTurn ? "自分" : "相手") + "のターン";
		}
		turnPanel.SetActive(active);
	}
}
