
public enum DetailFilterType
{
	NONE = 0,
	CARD_ID = 1,

	// カードタイプ
	UNIT = 2,
	SPELL = 3,
	AMULET = 4,

	// ステータス
	COST = 11,
	ATTACK = 12,
	HP = 13,

	// 場所
	HAND = 21,
	FIELD = 22,

	// プロパティ

	// アビリティ
	ABILITY_TIMING = 41,

	// 種族
	TRIBE = 50,
	BEAST = 51,
	SPIRAL = 52,

	// その他
	IS_SELF = 71,
	THIS_TURN = 72,
}
