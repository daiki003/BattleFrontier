using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillPanelController : MonoBehaviour
{
	public SkillPanelView view; // 見た目の処理
	public SkillPanelModel model; // データを処理
	private PlayerController player;

	private AbilityProcessor abilityProcessor { get { return BattleManager.instance.mainAbilityProcessor; } }

	private void Awake()
	{
		view = GetComponent<SkillPanelView>();
		if (GameManager.instance.isBattle)
		{
			player = BattleManager.instance.player;
		}
	}

	// 生成した時に呼ばれる関数
	public void Init(string skillId)
	{
		view = GetComponent<SkillPanelView>();
		if (GameManager.instance.isBattle)
		{
			player = BattleManager.instance.player;
		}
		// SkillPanelEntity skillEntity = SkillName.instance.getSkillEntity(skillId);
		// model = new SkillPanelModel(skillEntity);
		// view.setIcon(model);
		// setAbility(skillEntity);
	}

	public void setQuest(Quest quest)
	{
		this.model.quest = quest;
	}

	public void setAbility(SkillPanelEntity skillEntity)
	{
		for (int i = 0; i < skillEntity.abilityList.Count; i++)
		{
			model.abilityList.Add(AbilityUtility.createAbilityController(skillEntity.abilityList[i], ownerSkill: this));
		}
	}

	public void battleLevelUp()
	{
		if (model.level < 3)
		{
			model.level += 1;
			view.levelUp();
		}
	}

	public void editLevelUp()
	{
		if (model.level < 2) model.level += 1;
		else resetLevel();
	}

	public void changeActive(bool active)
	{
		model.isActive = active;
		view.changeActive(active);
	}

	public void resetLevel()
	{
		model.level = 0;
	}

	public void showDescribe()
	{
		bool canActivate = model.isActive;
		if (GameManager.instance.isBattle && !model.isActive)
		{
			BattleManager.instance.showDescribe(model.quest, magnification: model.questLevel);
			BattleManager.instance.showDescribe(this, isSub: true);
		}
		else BattleManager.instance.showDescribe(this);
	}

	// クエストカウントを更新し、基準値を超えていれば解放する
	public void updateQuestCount()
	{
		int questCount = model.quest.questCount;
		view.setQuestCount(questCount);
		changeActive(questCount >= model.quest.standardValue[model.questLevel - 1]);
	}
}
