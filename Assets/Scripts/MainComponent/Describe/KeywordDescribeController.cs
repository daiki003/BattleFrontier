using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordDescribeController : DescribeController
{
	public override void init(object describeTarget, bool isSub = false, int magnification = 0)
	{
		base.init(describeTarget, isSub, magnification);
		setText((KeywordMaster)describeTarget);
	}
	public void setText(KeywordMaster keyword)
	{
		nameText.text = keyword.title;
		abilityText.text = keyword.describe;
	}
}
