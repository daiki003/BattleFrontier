using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepTutorial : TutorialService
{
	public IEnumerator startKeepTutorial()
	{
		SoundManager.instance.battleStart();
		yield return new WaitForSeconds(2.0f);
		SoundManager.instance.startBattleBGM();

		List<string> deck = new List<string>(){"SuperGuard", "Hit&Away", "Drain", "AutoGuard", "HeavyAttack", "Attack", 
												"SuperGuard", "Hit&Away", "Drain", "AutoGuard", "HeavyAttack", "Attack"};
	}

	public void phase5()
	{
		string text = "キープしたカードのみ、捨てられずに手札に残りました。\nここからは自由にカードを使って敵を倒してみましょう。";
		panel.setDescriptionText(text);
	}
}
