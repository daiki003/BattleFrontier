using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialService
{
	[SerializeField] protected BattleManager battleManager;
	protected TutorialPanel panel;
	public bool canTurnEnd;
	public bool canPlayCard;
	public bool canAttack;
	public bool canSkillCharge;
	protected int phase;

	public virtual List<string> createTutorialDeck()
	{
		return new List<string>();
	}

	public virtual string tutorialEnemy()
	{
		return "";
	}

	public virtual void setupTutorial()
	{
		panel = BattleManager.instance.tutorialPanel;
		panel.setTutorialPanel(true);
		restrictAllOperation();
	}

	public virtual void whenTurnEnd() {}

	public virtual void update() {}

	public IEnumerator detectCardPlay()
	{
		yield return null;
	}

	public void restrictAllOperation()
	{
		canAttack = false;
		canPlayCard = false;
		canTurnEnd = false;
		canSkillCharge = false;
	}

	public void onlyCanPlayCard(string cardId)
	{
		foreach (CardController card in battleManager.player.cardCollection.handCardList)
		{
			
		}
	}

	public virtual IEnumerator phase1() { return null; }
}
