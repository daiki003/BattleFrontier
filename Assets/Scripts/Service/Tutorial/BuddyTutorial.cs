using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddyTutorial : TutorialService
{
	public IEnumerator startBuddyTutorial()
	{
		// 効果音とともに暗転
		SoundManager.instance.battleStart();
		yield return new WaitForSeconds(2.0f);
		SoundManager.instance.startBattleBGM();

		List<string> deck = new List<string>(){"Guard", "Draw", "RunePower", "HeavyAttack", "Attack", 
												"SuperGuard", "Hit&Away", "Drain", "AutoGuard", "SkillCharge"};
	}

	public override IEnumerator phase1()
	{
		panel.setDescriptionPanel(true);
		string text = "敵をタッチすることで、敵を選択状態にすることができます。\n敵をタッチしてみましょう。";
		panel.setDescriptionText(text);
		// battleManager.boss.buddyAction("skelton");

		while(battleManager.player.cardCollection.handCardList.Count < 5) yield return null;
		onlyCanPlayCard(null);
		phase2();
	}

	public void phase2()
	{
		string text = "赤い枠で囲われているのが選択されている敵です。\n攻撃カードを使うと、選択されている敵に攻撃します。\nここからは自由に戦って敵を倒してみましょう。";
		panel.setDescriptionText(text);

		this.canTurnEnd = true;
	}
}
