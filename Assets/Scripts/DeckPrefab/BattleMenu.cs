using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : PanelPrefab
{
	public GameObject battleMenu;
	public Transform levelTransform, enemyTransform;
	List<LevelController> enemyList = new List<LevelController>();
	private string selectingEnemy;
	private List<SaveData.CardComponent> playerDeck;
	private List<SaveData.CardComponent> enemyDeck;

	public override void setup()
	{
		close();
	}
	public override void close()
	{
		battleMenu.SetActive(false);
	}
	public override void onClickExtra()
	{

	}

	public void open(List<SaveData.CardComponent> playerDeck)
	{
		this.playerDeck = playerDeck;
		battleMenu.SetActive(true);
		goToSelectEnemy();
	}

	public void setEnemy(DeckEntity deckEntity)
	{
		
	}

	public void selectEnemy(DeckEntity deckEntity)
	{
		SEManager.instance.playSe("Button");
		this.selectingEnemy = deckEntity.deckId;
		foreach (LevelController levelController in enemyList)
		{
			levelController.setSelectPanel(levelController.levelId == deckEntity.deckId);
		}
		setEnemy(deckEntity);
	}

	public void goToSelectEnemy()
	{
		foreach (Transform levelPanel in levelTransform)
		{
			Destroy(levelPanel.gameObject);
		}

		enemyList = new List<LevelController>();
		// クリア状況次第で表示するレベルを変更
		foreach (DeckEntity deck in GameManager.instance.masterManager.deckEntityList)
		{
			enemyList.Add(PrefabManager.instance.createLevel(deck.deckId, isCreateDeck: false, levelTransform));
		}
		
		// ボタンが１つ以上あれば、最初のボタンを押して選択した状態にする
		if (enemyList.Count > 0)
		{
			enemyList[0].selectLevel();
		}
	}

	public void decide()
	{
		GameManager.instance.startBattleCoroutine(playerDeck, enemyDeck);
	}
}
