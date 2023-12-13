using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class CoroutineProcessor : MonoBehaviour
{
	[System.NonSerialized] public List<VfxBase> allVfxList = new List<VfxBase>();

	public bool processing;

	public void startProcess()
	{
		StartCoroutine(processCoroutineBlock());
	}

	public IEnumerator processCoroutineBlock()
	{
		if (processing) yield break;
		processing = true;

		while (allVfxList.Count > 0)
		{
			VfxBase processingVfx = allVfxList.dequeue();
			StartCoroutine(process(processingVfx));
			while (!processingVfx.getIsEnd())
			{
				yield return null;
			}
		}

		processing = false;
	}

	public IEnumerator process(VfxBase vfx)
	{
		if (vfx is SeriesVfxBlock)
		{
			foreach (VfxBase playVfx in (vfx as SeriesVfxBlock).vfxList)
			{
				StartCoroutine(process(playVfx));
				while (!playVfx.getIsEnd())
				{
					yield return null;
				}
			}
		}
		else if (vfx is ParallelVfxBlock)
		{
			foreach (VfxBase playVfx in (vfx as ParallelVfxBlock).vfxList)
			{
				StartCoroutine(process(playVfx));
			}
			while ((vfx as ParallelVfxBlock).vfxList.Any(v => !v.getIsEnd()))
			{
				yield return null;
			}
		}
		else if (vfx is SingleVfx)
		{
			if (vfx.coroutine != null)
			{
				yield return StartCoroutine(vfx.coroutine);
			}
			vfx.isEnd = true;
		}
		else if (vfx is TweenVfx tweenVfx)
		{
			tweenVfx.tweenAction();
		}
		else if (vfx is ActionVfx)
		{
			foreach (Action action in (vfx as ActionVfx).actionList)
			{
				action();
			}
			vfx.isEnd = true;
		}
		else if (vfx is WaitVfx)
		{
			yield return new WaitForSeconds((vfx as WaitVfx).waitTime);
			vfx.isEnd = true;
		}
		else
		{
			yield return null;
		}
	}

	public IEnumerator HelperEnumerator(IEnumerator enumerator, Action callback)
	{
		yield return enumerator;
		callback();
	}

	public void clearCoroutineBlock()
	{
		allVfxList = new List<VfxBase>();
		processing = false;
	}

	public void stopAllCoroutine()
	{
		StopAllCoroutines();
		clearCoroutineBlock();
	}
}
