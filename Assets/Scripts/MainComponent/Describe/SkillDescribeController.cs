using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SkillDescribeController : DescribeController
{
	[SerializeField] public Text buttonText, questLevelText;
	[SerializeField] public Button activeButton;
	public SkillPanelController describeSkill;
	private bool isDeckEdit;
	private bool isSub;
	private const int USE_MODEL_LEVEL = -1;

	// 初期表示
	public override void init(System.Object describeTarget, bool isSub = false, int magnification = 0)
	{
		describeSkill = (SkillPanelController)describeTarget;
		this.isSub = isSub;
		questLevelText.text = describeSkill.model.questLevel.ToString();

		if (!GameManager.instance.isCardList) setButtonDisplay(describeSkill);
		setText(this.describeSkill);
	}

	// テキスト設定
	public void setText(SkillPanelController skill)
	{
		nameText.text = skill.model.skillName;
		abilityText.text = skill.model.text;
	}

	// 起動ボタン押下時の処理
	public void release()
	{
		string skillPanelId = this.describeSkill.model.skillPanelId;

		SEManager.instance.playSe("Button");

		describeSkill.editLevelUp();
		init(describeSkill, isSub: false);

	}

	// 起動ボタンの表示設定
	public void setButtonDisplay(SkillPanelController skill)
	{
		activeButton.interactable = skill.model.isActive;
		activeButton.gameObject.SetActive(GameManager.instance.isBattle && skill.model.abilityList.Any(a => a.abilityTrait.timing == "when_play"));
	}
}
