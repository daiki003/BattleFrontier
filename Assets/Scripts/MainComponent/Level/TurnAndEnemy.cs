using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnAndEnemy : MonoBehaviour
{
	[SerializeField] Text turnText;
	[SerializeField] Transform cardTransform;
	private Vector3 cardSize = new Vector3(0.8f, 0.8f, 1f);

	public void init(int turn, List<string> cardList)
	{
		
	}
}
