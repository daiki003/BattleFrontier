using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEntity
{
	public int maxEnemyNumber;
	public int firstBonusMultiple;
	public int bonusIncreaseInterval;
	public double bonusIncreaseAmount;
	public List<string> rank1Enemy;
	public List<string> rank2Enemy;
	public List<string> rank3Enemy;
	public List<string> rank4Enemy;
	public List<string> rank5Enemy;
	public List<string> rank6Enemy;
	public List<string> rank7Enemy;
	public List<string> rank8Enemy;
	public List<Boss> boss;
	public List<EnemyBonus> bonus;
}
