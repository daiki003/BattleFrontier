using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EffectObject : MonoBehaviour
{
	public float targetAlpha;
	public float periodTime;
	public Image effectImage;

	private Tween _tween;

	void OnEnable()
	{
		// 点滅するループトゥイーン作成
		_tween = effectImage.DOFade(targetAlpha, periodTime).SetLoops(-1, LoopType.Yoyo);
		
		// 他のトゥイーンと再生タイミング同期
		_tween.SyncWithPrimary();

		// 同期した位置から再生
		_tween.Play();
		
		// 同期用に登録 (Killで自動的に登録解除される)
		_tween.RegisterForSync();
	}

	void OnDisable()
	{
		_tween.Rewind();
		_tween.Kill();
		_tween = null;
	}
}
