using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

[Serializable]
public class Boss
{
	public int turn;
	public List<string> bossList;
}

public class EnemyController
{
	public List<List<string>> enemyList = new List<List<string>>();
	public List<Boss> bossList;
	public BattleCardCollection cardCollection = new BattleCardCollection();

	// 各種パラメータ系
	private int maxEnemyNumber = 7;
	private int firstBonusMultiple = 1;
	private int bonusIncreaseInterval = 3;
	private double bonusIncreaseAmount = 1;

	private AbilityProcessor abilityProcessor { get { return BattleManager.instance.mainAbilityProcessor; } }

	public EnemyController()
	{
		
	}
}
