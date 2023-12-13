using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTutorial : TutorialService
{
	public IEnumerator startSkillTutorial()
	{
		SoundManager.instance.battleStart();
		yield return new WaitForSeconds(2.0f);
		SoundManager.instance.startBattleBGM();

		List<string> deck = new List<string>(){"Attack", "Guard"};
	}

	public IEnumerator phase5()
	{
		string text = "スキルポイントが2になったので、「レベル2_1」を解放してみましょう。";
		panel.setDescriptionText(text);
		yield return null;

		// while(!battleManager.skillPanelController.model.level2[0].isActive) yield return null;
		phase6();
	}

	public void phase6()
	{
		string text = "「レベル2_1」を解放すると、ターン終了時にプレイヤーが敵にダメージを与えるようになります。\nここからは自由に戦って敵を倒してみましょう。";
		panel.setDescriptionText(text);
		// 次のターン用のデッキをセット
		battleManager.player.cardCollection.resetGrave();

		canTurnEnd = true;
	}
}
