using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDescribeController : DescribeController
{
	[SerializeField] public Text currentPointText, targetPointText;
	public Quest describeQuest;

	// 初期表示
	public override void init(System.Object describeTarget, bool isSub = false, int magnification = 0)
	{
		describeQuest = (Quest)describeTarget;
		setText(this.describeQuest, magnification);
	}

	// テキスト設定
	public void setText(Quest quest, int magnification)
	{
		abilityText.text = quest.getQuestText(magnification);
		currentPointText.text = quest.questCount.ToString();
		targetPointText.text = quest.standardValue[magnification - 1].ToString();
	}
}
