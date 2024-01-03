using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Cursor : MonoBehaviour
{
    public Image image;
	private float repeatTime = 0.8f;
    private float maxScale = 1.1f;
	private float maxAlpha = 0.1f;
	private Tween scaleTween;
	private Tween fadeTween;
	
	void OnEnable()
	{
		// 点滅するループトゥイーン作成
		scaleTween = this.transform.DOScale(maxScale, repeatTime).SetLoops(-1, LoopType.Restart);
		fadeTween = image.DOFade(maxAlpha, repeatTime).SetLoops(-1, LoopType.Restart);
		
		// 他のトゥイーンと再生タイミング同期
		fadeTween.SyncWithPrimary();
		scaleTween.SyncWithPrimary();

		// 同期した位置から再生
		fadeTween.Play();
		scaleTween.Play();
		
		// 同期用に登録 (Killで自動的に登録解除される)
		fadeTween.RegisterForSync();
		scaleTween.RegisterForSync();
	}

	void OnDisable()
	{
		StopTween(fadeTween);
		StopTween(scaleTween);
	}

	private void StopTween(Tween tween)
	{
		tween.Rewind();
		tween.Kill();
		tween = null;
	}
}
