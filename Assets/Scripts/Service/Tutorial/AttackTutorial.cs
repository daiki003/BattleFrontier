using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTutorial : TutorialService
{
	public override List<string> createTutorialDeck()
	{
		List<string> tutorialDeck = new List<string>()
		{
			"SkeltonFighter",
			"RedGuard",
			"Behemoth",
			"SkeltonFighter",
			"RedGuard",
			"Behemoth",
			"SkeltonFighter",
			"RedGuard",
			"Behemoth"
		};
		return tutorialDeck;
	}

	public override string tutorialEnemy()
	{
		return "AttackTutorial";
	}

	public override void setupTutorial()
	{
		base.setupTutorial();
		canTurnEnd = false;
		phase = 0;
	}

	public override void whenTurnEnd()
	{
		panel.setDescriptionPanel(false);
	}

	private void nextPhase()
	{
		this.phase++;
		switch (phase)
		{
			case 1:
				panel.setDescriptionPanel(true);
				panel.setDescriptionText("PPを消費することで、カードをプレイすることができます。\nコストが1である「スケルトンファイター」をプレイしてみましょう。");
				canPlayCard = true;
				break;
			case 2:
				panel.setDescriptionText("「スケルトンファイター」を場に出すことができました。\nこのターンは敵もいないので、このままターンを終了しましょう。");
				canTurnEnd = true;
				break;
			case 3:
				panel.setDescriptionPanel(true);
				panel.setDescriptionText("PPの最大値が増え、「レッドガード」をプレイ可能になりました。\n「レッドガード」をプレイしてみましょう。");
				canTurnEnd = false;
				break;
			case 4:
				panel.setDescriptionText("前のターンに召喚した「スケルトンファイター」は攻撃することができます。\n「スライム」に攻撃してみましょう。");
				canAttack = true;
				break;
			case 5:
				panel.setDescriptionText("攻撃すると敵のユニットも反撃してきます。\n今回はどちらも体力が0以下になったので、両方のユニットが破壊されました。\nこのターンはもうできることがないのでターンを終了しましょう。");
				canTurnEnd = true;
				break;
			case 6:
				panel.setDescriptionPanel(true);
				panel.setDescriptionText("特定のターンに出てくるボスが登場した後、全ての敵を倒せばゲームクリアです。\n今回はこのターンに出てくるので、自由に行動してボスを倒してください。");
				break;
			default:
				break;
		}
	}

	public override void update()
	{
		
	}
}
