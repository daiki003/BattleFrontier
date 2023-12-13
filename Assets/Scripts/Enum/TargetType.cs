using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
	// TargetPlaceが必要ないもの
	NONE = 0,
	SELECT = 1,
	SELF = 2,
	SPECIFIC = 3,
	ATTACKED = 5,
	CHOICE = 6,
	ACTIVATE_CARD = 7,
	LAST_TARGET = 8,
	SKILL_DREW_CARD = 9,
	SKILL_SUMMON_CARD = 11,

	// 旧Place系
	HAND = 21,
	FIELD = 22,
	ENEMY_FIELD = 23,
	BOTH_FIELD = 24,
	GRAVE = 25,
	DECK = 26,
	PLAY_LIST = 31,
	DESTROY_LIST = 32,
	LEFT_LIST = 33,
	DISCARDED_LIST = 34,
	BOUNCE_LIST = 35,
	SUMMON_LIST = 36,

	ENEMY_TARGET = 100,
	
	// 検索用
	ANY = 100,
	// 補足表示用
	SPECIAL = 1000
}
