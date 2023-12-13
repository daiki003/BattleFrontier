using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : PanelPrefab
{
	[SerializeField] GameObject resultPanel;
	[SerializeField] Transform lastCardTramsform;
	[SerializeField] Text resultText;

	public override void setup()
	{
		close();
	}
	public override void close()
	{
		resultPanel.SetActive(false);
	}
	public override void onClickExtra() {}

	public void finishBattle(bool isSelfWin)
	{
		resultPanel.SetActive(true);
		resultText.text = isSelfWin ? "勝利" : "敗北";
	}

	public void Restart()
	{
		GameManager.instance.networkManager.UpdatePlayerPhase(RoomPhase.WAIT_ENEMY, isSelf: true);
		GameManager.instance.battleMgr.loadingPanel.Init(roomName: null, waitingText: "対戦相手を探しています");
	}

	public void GoToTitle()
	{
		GameManager.instance.GoToTitle();
	}
}
