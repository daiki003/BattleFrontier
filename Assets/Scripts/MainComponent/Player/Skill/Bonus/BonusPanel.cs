using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusPanel : MonoBehaviour
{
	public GameObject bonusPanel, selectedPanel;
	public Text bonusName, bonusText;
	public Image bonusIcon;
	[System.NonSerialized] public AbilityController ability;
	[System.NonSerialized] public bool selected;
	public void init(LevelBonusMaster bonusMaster)
	{
		bonusPanel.SetActive(true);
		selectedPanel.SetActive(false);
		bonusName.text = bonusMaster.bonusName;
		bonusText.text = bonusMaster.bonusText;
		bonusIcon.sprite = Resources.Load<Sprite>("Images/Bonus/" + bonusMaster.bonusId);

		ability = AbilityUtility.createAbilityController(bonusMaster.ability);
	}

	public void activate()
	{
		BattleManager.instance.mainAbilityProcessor.addComponentFromAbility(new List<AbilityController>() { ability }, ownerCard: BattleManager.instance.player.cardCollection.substanceCard);
	}

	public void changeSelected()
	{
		if (BattleManager.instance.selectPanel.judgeCanSelect())
		{
			selected = !selected;
			selectedPanel.SetActive(selected);
		}
	}

	public void setSelected(bool selected)
	{
		this.selected = selected;
		selectedPanel.SetActive(this.selected);
	}
}
