using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class ListUtility
{
	public static T dequeue<T>(this List<T> list)
	{
		var result = list[0];
		list.RemoveAt(0);
		return result;
	}

	public static T pullOut<T>(this List<T> list, int pullIndex)
	{
		var result = list[pullIndex];
		list.RemoveAt(pullIndex);
		return result;
	}

	public static void Replace<T>(this List<T> list, int firstIndex, int secondIndex)
	{
		var firstComponent = list[firstIndex];
		list[firstIndex] = list[secondIndex];
		list[secondIndex] = firstComponent;
	}

	public static void reset<T>(this List<T> list)
	{
		list = new List<T>();
	}

	public static void setNewList<T>(this List<T> list, List<T> newList)
	{
		if (newList == null) list = null;
		else list = new List<T>(newList);
		if (newList == null) Debug.Log(null);
		else Debug.Log(newList.Count);
	}
}
