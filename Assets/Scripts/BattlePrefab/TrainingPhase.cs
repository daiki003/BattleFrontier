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

	public void exchange()
	{
		if (BattleManager.instance.isSelect)
		{
			BattleManager.instance.onClickExtra();
			return;
		}
		BattleManager.instance.stopPlayFlag = true;
		ActionVfx actionVfx = new ActionVfx(() => BattleManager.instance.selectPanel.StartSelectCommon(SelectType.CHANGE_HAND));
		actionVfx.addToAllBlockList();
	}

	public void updateStrengthenButton()
	{
		bool canEvolveTarget = strengthenTarget != null && !strengthenTarget.model.applyInformation.eternalApplyValue.isEvolve;
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

	void Update()
	{
		updateStrengthenButton();
	}
}
