using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RandomUtility
{
	// 乱数を整数で生成する
	public static int random(int max)
	{
		int number = UnityEngine.Random.Range(0, max);
		return number;
	}

	// 乱数を小数で生成する
	public static float random(float max)
	{
		float number = UnityEngine.Random.Range(0.0f, max);
		return number;
	}

	// 重複のない複数の乱数を整数で生成する
	public static List<int> random(int max, int number)
	{
		List<int> randomList = new List<int>();
		List<int> resultList = new List<int>();
		for(int i = 0; i < max; i++)
		{
			randomList.Add(i);
		}
		for(int i = 0; i < number; i++)
		{
			int index = UnityEngine.Random.Range(0, randomList.Count);
			resultList.Add(randomList.pullOut(index));
		}
		return resultList;
	}

	public static int randomWithoutCenter()
	{
		return 0;
	}
}
