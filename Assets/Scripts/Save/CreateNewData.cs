using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNewData : MonoBehaviour
{
	private SaveData saveData;
	[SerializeField]
	private GameObject obj;

	public SaveData getSaveData() {
		return saveData;
	}
}
