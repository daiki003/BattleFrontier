using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextLayoutGroup : MonoBehaviour
{
    public int spacing;

    public void alignment(List<TextUnit> textUnitList)
	{
		float textPositionY = 0;
        for (int i = 0; i < textUnitList.Count; i++)
		{
			Vector3 textPosition = new Vector3(0, textPositionY, 0);
			textUnitList[i].setPosition(textPosition);
            textPositionY -= (textUnitList[i].hight + spacing);
		}
        var rectTransform = transform as RectTransform;
        rectTransform.SetHeight(-1 * textPositionY);
	}
}
