using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Damage : MonoBehaviour
{
	[SerializeField]
	public float deleteTime;
	[SerializeField]
	private float moveRange;

	private float timeCount;
	private Text damageText;

	void Start()
	{
		timeCount = 0.0f;
		Destroy(this.gameObject, deleteTime);
		damageText = this.gameObject.GetComponent<Text>();
	}

	void Update()
	{
		timeCount += Time.deltaTime;
		this.gameObject.transform.localPosition += new Vector3(0, 
		moveRange/deleteTime*Time.deltaTime, 0);
	}
}