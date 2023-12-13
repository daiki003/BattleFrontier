using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioClip battleStartBGM;
	public AudioClip battleBGM;
	public AudioClip bossBGM;
	public AudioClip titleBGM;
	public AudioClip winBGM;
	public AudioClip loseBGM;

	AudioSource audioSource;

	public static SoundManager instance;
	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		audioSource = GetComponent<AudioSource>();
	}

	public void battleStart()
	{
		audioSource.clip = battleStartBGM;
		audioSource.Play();
	}

	public void startBattleBGM()
	{
		audioSource.clip = battleBGM;
		audioSource.Play();
	}

	public void startBossBGM()
	{
		audioSource.clip = bossBGM;
		audioSource.Play();
	}

	public void startTitleBGM()
	{
		audioSource.clip = titleBGM;
		audioSource.Play();
	}

	public void startWinBGM()
	{
		audioSource.clip = winBGM;
		audioSource.Play();
	}

	public void startLoseBGM()
	{
		audioSource.clip = loseBGM;
		audioSource.Play();
	}

	// 音量の設定 ------------------------------------------------------------------------------------------------------------------------------------------------
	
	// フェードアウト
	public IEnumerator fadeOut()
	{
		while (audioSource.volume > 0)
		{
			audioSource.volume -= 0.05f;
			yield return new WaitForSeconds(0.08f);
		}
	}

	public void resetVolme()
	{
		audioSource.volume = 0.71f;
	}
}
