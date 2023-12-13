using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtility
{
	public class CoroutineWithCardList
	{
		public List<IEnumerator> coroutineList;
		public List<CardController> cardList;
		public CoroutineWithCardList(List<IEnumerator> coroutineList, List<CardController> cardList)
		{
			this.coroutineList = coroutineList;
			this.cardList = cardList;
		}
	}

	public class CoroutineWithGameObject
	{
		public List<IEnumerator> coroutineList;
		public GameObject gameObject;
		public CoroutineWithGameObject(List<IEnumerator> coroutineList, GameObject gameObject)
		{
			this.coroutineList = coroutineList;
			this.gameObject = gameObject;
		}
	}

	public static IEnumerator waitCoroutine(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}

	public static void createAndAddWaitVfx(float seconds)
	{
		WaitVfx waitVfx = new WaitVfx(seconds);
		waitVfx.addToAllBlockList();
	}
}
