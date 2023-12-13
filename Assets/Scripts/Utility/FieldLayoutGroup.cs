using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldLayoutGroup : MonoBehaviour
{
	public int spacing;

	void Start()
	{
		
	}

	void Update()
	{
		
	}

	public void alignment(int excludeIndex)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			var rectTransform = transform.GetChild(i) as RectTransform;
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			float cardPositionX = 0;
			if (i == excludeIndex)
			{
				continue;
			}
			if (transform.childCount % 2 == 0)
			{
				if (i < transform.childCount / 2)
				{
					cardPositionX = -1 * spacing / 2;
					cardPositionX += spacing * Math.Min(0, (i - transform.childCount / 2 + 1));
				}
				else
				{
					cardPositionX = spacing / 2;
					cardPositionX += spacing * Math.Max(0, (i - transform.childCount / 2));
				}
			}
			else
			{
				if (i == transform.childCount / 2)
				{
					cardPositionX = 0;
				}
				else
				{
					cardPositionX = spacing * (i - transform.childCount / 2);
				}
			}
			Vector3 cardPosition = new Vector3(cardPositionX, 0, 0);
			rectTransform.anchoredPosition = cardPosition;
		}
	}
}
