using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectPlace
{
	// 実際のカード群
	HAND = 0,
	FIELD = 1,
	ENEMY_FIELD = 11,
	BOTH_FIELD = 21,
	DECK = 2,
	GRAVE = 3,

	// 記録されているカード群
	PLAY_LIST = 51,
	DESTROY_LIST = 52,
	LEFT_LIST = 53,
	BOUNCE_LIST = 54,
	DISCARD_LIST = 55,
	DESTROY_THIS_TURN = 56,
	
	OTHER = 101
}
