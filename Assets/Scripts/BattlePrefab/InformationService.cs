using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationService : PanelPrefab
{
	[SerializeField] GameObject information;
	[SerializeField] Text turn, amulet, enemyBarrier;

	public override void setup()
	{
		close();
	}
	public override void close()
	{
		this.information.SetActive(false);
	}
	public override void onClickExtra()
	{
		close();
	}

	public void start()
	{
		this.information.SetActive(false);
	}
}
