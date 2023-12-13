using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SupplementTextType
{
	NONE = 0,

	// ターン中のカウント
	PLAY_COUNT = 1,
	DISCARD_COUNT = 2,


	// バトル中のカウント
	DESTROY_UNIT_COUNT = 21,
	DESTROY_ANULET_COUNT = 22,

	// カードごとのカウント
	CUSTOM_POWER = 51,
	SPIRAL_CHARGE = 52,
	
	OTHER = 101
}
