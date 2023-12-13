using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateDeckMenu : PanelPrefab
{
	public GameObject createDeckMenu;
	public Transform levelTransform, turnAndEnemyTransform;
	List<LevelController> environmentList = new List<LevelController>();
	private EnvironmentMaster selectingEnvironment;
	private CardCategory selectingCategory;

	public override void setup()
	{
		close();
	}
	public override void close()
	{
		createDeckMenu.SetActive(false);
	}
	public override void onClickExtra()
	{

	}

	public void open(CardCategory selectingCategory)
	{
		this.selectingCategory = selectingCategory;
		createDeckMenu.SetActive(true);
		goToSelectEnemy();
	}

	public void setEnemy(EnvironmentMaster environmentMaster)
	{
		turnAndEnemyTransform.resetCard();
		if (environmentMaster.enemy.rank1Enemy.Count > 0) PrefabManager.instance.createTurnAndEnemy(1, environmentMaster.enemy.rank1Enemy, turnAndEnemyTransform);
		if (environmentMaster.enemy.rank2Enemy.Count > 0) PrefabManager.instance.createTurnAndEnemy(2, environmentMaster.enemy.rank2Enemy, turnAndEnemyTransform);
		if (environmentMaster.enemy.rank3Enemy.Count > 0) PrefabManager.instance.createTurnAndEnemy(3, environmentMaster.enemy.rank3Enemy, turnAndEnemyTransform);
		if (environmentMaster.enemy.rank4Enemy.Count > 0) PrefabManager.instance.createTurnAndEnemy(4, environmentMaster.enemy.rank4Enemy, turnAndEnemyTransform);
		if (environmentMaster.enemy.rank5Enemy.Count > 0) PrefabManager.instance.createTurnAndEnemy(5, environmentMaster.enemy.rank5Enemy, turnAndEnemyTransform);
		if (environmentMaster.enemy.rank5Enemy.Count > 0) PrefabManager.instance.createTurnAndEnemy(6, environmentMaster.enemy.rank6Enemy, turnAndEnemyTransform);
		if (environmentMaster.enemy.rank6Enemy.Count > 0) PrefabManager.instance.createTurnAndEnemy(7, environmentMaster.enemy.rank7Enemy, turnAndEnemyTransform);
		if (environmentMaster.enemy.rank8Enemy.Count > 0) PrefabManager.instance.createTurnAndEnemy(8, environmentMaster.enemy.rank8Enemy, turnAndEnemyTransform);
		// if (environmentEntity.enemy.boss != null) PrefabManager.instance.createTurnAndEnemy(10, new List<string>() { environmentEntity.enemy.boss }, turnAndEnemyTransform);
	}

	public void selectEnemy(EnvironmentMaster environmentMaster)
	{
		SEManager.instance.playSe("Button");
		this.selectingEnvironment = environmentMaster;
		foreach (LevelController levelController in environmentList)
		{
			levelController.setSelectPanel(levelController.levelId == environmentMaster.environmentId);
		}
		setEnemy(environmentMaster);
	}

	public void goToSelectEnemy()
	{
		foreach (Transform levelPanel in levelTransform)
		{
			Destroy(levelPanel.gameObject);
		}

		environmentList = new List<LevelController>();
		// クリア状況次第で表示するレベルを変更
		foreach (CreateDeckEnvironment environment in Enum.GetValues(typeof(CreateDeckEnvironment)))
		{
			if (GameManager.instance.saveManager.getSaveData().getEnableEnvironment(environment))
			{
				environmentList.Add(PrefabManager.instance.createLevel(environment.ToString(), isCreateDeck: true, levelTransform));
			}
		}

		// ボタンが１つ以上あれば、最初のボタンを押して選択した状態にする
		if (environmentList.Count > 0)
		{
			environmentList[0].selectLevel();
		}
	}

	public void decide()
	{
		
	}
}
