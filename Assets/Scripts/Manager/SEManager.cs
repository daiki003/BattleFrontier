using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
	public bool destroySeFlag;

	AudioSource audioSource;
	private Dictionary<string, AudioClip> seCache = new Dictionary<string, AudioClip>();

	public static SEManager instance;
	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		audioSource = GetComponent<AudioSource>();
	}

	public void destroy()
	{
		if (destroySeFlag)
		{
			return;
		}
		StartCoroutine(destroySeControl());
		audioSource.PlayOneShot(getSe("Destroy"));
	}

	public void playSe(string seName)
	{
		audioSource.PlayOneShot(getSe(seName));
	}

	public AudioClip getSe(string seName)
	{
		if (string.IsNullOrEmpty(seName))
		{
			seName = "Slash";
		}
		if (seCache.ContainsKey(seName))
		{
			return seCache[seName];
		}

		AudioClip se = Resources.Load<AudioClip>("Sound/SE/" + seName);
		seCache.Add(seName, se);
		return se;
	}

	// 再生管理
	private IEnumerator destroySeControl()
	{
		destroySeFlag = true;
		yield return new WaitForSeconds(0.1f);
		destroySeFlag = false;
	}
}
