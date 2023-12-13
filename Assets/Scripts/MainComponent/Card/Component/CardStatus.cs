using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardStatus
{
	public int cost;
	public int attack;
	public int hp;

	public CardStatus() {}
	public CardStatus(int cost, int attack, int hp)
	{
		this.cost = cost;
		this.attack = attack;
		this.hp = hp;
	}
}
