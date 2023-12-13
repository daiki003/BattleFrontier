using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTutorial : TutorialService
{
	public IEnumerator startEntityTutorial()
	{
		SoundManager.instance.battleStart();
		yield return new WaitForSeconds(2.0f);
		SoundManager.instance.startBattleBGM();

		List<string> deck = new List<string>(){"AutoGuard"};
	}

	public void phase5()
	{
		string text = "ここからは自由にカードを使って敵を倒してみましょう。";
		panel.setDescriptionText(text);

		canTurnEnd = true;

		// 次のターン用のデッキをセット
		battleManager.player.cardCollection.resetGrave();
	}
}
