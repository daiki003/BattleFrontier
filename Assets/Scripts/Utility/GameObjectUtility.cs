using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static partial class GameObjectUtility
{
	public static bool hasChild(this GameObject gameObject)
	{
		return 0 < gameObject.transform.childCount;
	}

	public static void shine(this GameObject target, Color shineColor)
	{
		Image image = target.GetComponent<Image>();
		Color defaultColor = image.color;
		Sequence sequence = DOTween.Sequence();

		sequence.Append(image.DOColor(shineColor, 0.2f))
				.Append(image.DOColor(defaultColor, 0.2f));
	}
}
