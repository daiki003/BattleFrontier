using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
	[SerializeField] Text levelText;
	[SerializeField] GameObject selectPanel;
	public string levelId;
	public string displayName;
	private bool isCreateDeck;
	private EnvironmentMaster environment;
	private DeckEntity deck;

	public void init(string levelId, bool isCreateDeck)
	{
		this.levelId = levelId;
		this.isCreateDeck = isCreateDeck;
		if (isCreateDeck)
		{
			this.environment = GameManager.instance.masterManager.environmentList.FirstOrDefault(e => e.environmentId == levelId);
			levelText.text = environment.name;
		}
		else
		{
			this.deck = GameManager.instance.masterManager.deckEntityList.FirstOrDefault(e => e.deckId == levelId);
			levelText.text = deck.name;
		}
	}

	public void selectLevel()
	{
		if (isCreateDeck)
		{
			GameManager.instance.titleMgr.createDeckMenu.selectEnemy(environment);
		}
		else
		{
			GameManager.instance.titleMgr.battleMenu.selectEnemy(deck);
		}
	}

	public void setSelectPanel(bool active)
	{
		selectPanel.SetActive(active);
	}

	public string convertEnvironmentId(string environmentId)
	{
		switch (environmentId)
		{
			case "GrassLand":
				return "草原";
			case "Forest":
				return "森";
			default:
				return "";
		}
	}
}
