using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TrainingPhase : PanelPrefab
{
	public GameObject trainingPhaseObject;
	public GameObject exchangeObject, strongEnemyObject, selectStrongEnemyPanel, turnDisplayPanel;
	public Button levelUpButton, strengthenButton, exchangeButton;
	public Text soldCountText, turnText, levelUpCostText, strengthenCostText, exchangeCostText, exchangeIncreaseCostText;
	// 現在強化対象のカード
	public CardController strengthenTarget;

	public override void setup()
	{
		trainingPhaseObject.SetActive(false);
		transform.SetSiblingIndex(5);
	}
	public override void close()
	{
		trainingPhaseObject.SetActive(false);
	}
	public override void onClickExtra()
	{
		strengthenTarget = null;
	}

	public void setTrainingPhase(bool active)
	{
		trainingPhaseObject.SetActive(active);
	}

	public void draw()
	{
		PlayerController player = BattleManager.instance.player;
		int cost = player.model.cost;
		if (cost > 0)
		{
			player.fluctuateCost(-1);
			BattleManager.instance.mainAbilityProcessor.addPursuitComponent(AbilityTiming.WHEN_CONSUME_PP, activatePower: 1);
		}
	}

	public void exchange()
	{
		if (BattleManager.instance.isSelect)
		{
			BattleManager.instance.onClickExtra();
			return;
		}
		BattleManager.instance.stopPlayFlag = true;
		SelectComponent selectComponent = new SelectComponent(SelectType.CHANGE_HAND);
		ActionVfx actionVfx = new ActionVfx(() => BattleManager.instance.selectPanel.startSelectCommon(selectComponent));
		actionVfx.addToAllBlockList();
	}

	public void updateStrengthenButton()
	{
		bool canEvolveTarget = strengthenTarget != null && !strengthenTarget.model.applyInformation.eternalApplyValue.isEvolve;
		strengthenButton.interactable = canEvolveTarget && strengthenTarget.model.cost + 2 <= BattleManager.instance.player.model.cost;
		strengthenCostText.text = canEvolveTarget ? (strengthenTarget.model.cost + 2).ToString() : "0";
	}

	public void levelUp()
	{
		if (BattleManager.instance.isSelect)
		{
			BattleManager.instance.onClickExtra();
			return;
		}

		// レベルアップ後に再開用データを記録
		BattleManager.instance.recordRecoveryData();
	}
	public void updateLevelUpButton()
	{
		levelUpCostText.text = BattleManager.instance.player.model.levelUpCost.ToString();
		levelUpButton.interactable = BattleManager.instance.player.canLevelUp();
	}

	void Update()
	{
		updateLevelUpButton();
		updateStrengthenButton();
	}
}
