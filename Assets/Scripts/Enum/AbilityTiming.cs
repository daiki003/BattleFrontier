using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTiming
{
	WHEN_PLAY, // プレイ時
	WHEN_DESTROY, // 破壊時
	WHEN_ATTACK, // 攻撃時
	WHEN_ATTACK_AFTER, // 攻撃した後
	WHEN_ATTACKED, // 攻撃される時
	WHEN_SELECTED, // 対象選択される時
	WHEN_FIGHT, // 交戦時
	WHEN_FIGHT_AFTER, // 交戦した後
	TURN_START, // ターン開始時
	ENEMY_TURN_START, // 相手のターン開始時
	TURN_END, // ターン終了時
	ENEMY_TURN_END, // 相手のターン終了時
	WHEN_SUMMON, // 召喚時
	WHEN_PLAY_OTHER, // 他のカードをプレイした時
	WHEN_RETURN, // 手札に戻った時
	BATTLE_START, // 戦闘開始時
	WHEN_SUMMON_OTHER, // 他のカードが場に出た時
	WHEN_DESTROY_OTHER, // 他のカードが破壊された時
	WHEN_RETURN_OTHER, // 他のカードが手札に戻った時
	WHEN_GET_SHIELD, // シールドを獲得した時
	WHEN_ATTACK_OTHER, // 他のカードが攻撃する時
	WHEN_LEAVE_OTHER, // 他のカードが場を離れる時
	WHEN_RETURN_ACTIVATE, // カードを手札に戻す能力が働いた時
	WHEN_DRAW_OTHER, // カードをデッキから手札に加えた時
	WHEN_SOLD_OTHER, // カードを場から売却したとき
	WHEN_CREATE_OTHER, // カードをデッキ以外から手札に加えた時
	WHEN_BUFF, // 攻撃力か体力がバフされた時
	WHEN_ATTACK_BUFF, // 攻撃力がバフされた時
	WHEN_HP_BUFF, // 体力がバフされた時
	WHEN_CONSUME_PP, // PPを消費した時
	WHEN_SELECT, // 対象選択をした時
	WHEN_DISCARD, // 手札を捨てた時
	RALLY, // 連携
	SPIRAL, // スパイラルチャージされた時
	WHEN_RALLY_OTHER, // 他のカードが連携能力を発動した時
	WHEN_LEADER_DAMAGE, // リーダーがダメージを受けた時
	DISCARDED, // 捨てられた時
	SOLDED, // 売却された時
	BOOT, // 起動時
	WHEN_FIREBALL, // 味方のファイアボールが発動した時
	RELEASE, // リリース
}
