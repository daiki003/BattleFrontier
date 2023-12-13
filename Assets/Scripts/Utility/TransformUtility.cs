using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class TransformUtility
{
	// 子要素が存在するかどうかを取得
	public static bool hasChild(this Transform transform)
	{
		return 0 < transform.childCount;
	}

	// 子オブジェクト全て削除
	public static void DesteoyChildren(this Transform transform)
	{
		//自分の子供を全て調べる
        foreach (Transform child in transform)
        {
            //自分の子供をDestroyする
            GameObject.Destroy(child.gameObject);
        }
	}

	// 指定の場所のカードを全て削除する
	public static void resetCard(this Transform transform)
	{
		if (transform == null)
		{
			return;
		}
		foreach (Transform card in transform)
		{
			CardView cardView = card.GetComponent<CardView>();
			if (cardView != null)
			{
				cardView.resetIcon();
			}
			GameObject.Destroy(card.gameObject);
		}
	}
}
