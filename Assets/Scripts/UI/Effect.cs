using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Effect : MonoBehaviour
{
	[SerializeField] private float deleteTime;
	[SerializeField] Transform targetTransform;
	[SerializeField] Transform playerTransform;
	[SerializeField] Transform effectTransform;
	[SerializeField] private float offset;
	private const float WAIT_TIME_OFFSET = 0.1f;
	private const float AFTER_LANDING_TIME = 0.3f;

	private List<string> linearMovementEffectName = new List<string>()
	{
		"Fireball"
	};

	private Dictionary<string, GameObject> effectCache = new Dictionary<string, GameObject>();

	public static Effect instance;
	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void changeTarget(CardController card)
	{
		if (card != null)
		{
			this.targetTransform = card.cardObject.transform;
		}
		else
		{
			this.targetTransform = playerTransform;
		}
	}

	public GameObject wholeAttack(string effectName)
	{
		GameObject instantiateEffect = Instantiate(getEffect(effectName), effectTransform);
		instantiateEffect.transform.position = BattleManager.instance.mainPanel.field.transform.position;
		return instantiateEffect;
	}

	public GameObject energy(CardController card)
	{
		GameObject instantiateEffect = Instantiate(getEffect("Energy"), effectTransform);
		instantiateEffect.transform.position = card.cardObject.gameObject.transform.position;
		return instantiateEffect;
	}

	public GameObject unionEnergy(Vector3 energyPosition)
	{
		GameObject instantiateEffect = Instantiate(getEffect("UnionEnergy"), effectTransform);
		instantiateEffect.transform.position = energyPosition;
		return instantiateEffect;
	}

	public float generateEffect(string effectName = "", Transform targetTransform = null, Transform firstTransform = null)
	{
		if (linearMovementEffectName.Contains(effectName))
		{
			return linearMovementEffect(effectName, firstTransform, targetTransform);
		}
		else
		{
			return fixedEffect(targetTransform, effectName);
		}
	}

	// 特定の場所にエフェクトを出現させる
	public float fixedEffect(Transform targetTransform = null, string effectName = "")
	{
		GameObject instantiateEffect = Instantiate(getEffect(effectName), targetTransform);
		return instantiateEffect.GetComponent<Damage>().deleteTime - WAIT_TIME_OFFSET;
	}

	// プレイヤーの場所にエフェクトを出現させる
	public void playerFixedEffect(string effectName)
	{
		GameObject instantiateEffect = Instantiate(getEffect(effectName), BattleManager.instance.mainPanel.invisibleArea);
	}

	// 直線移動するエフェクト
	public float linearMovementEffect(string effectName = "", Transform firstTransform = null, Transform moveTargetTransform = null)
	{
		var sequence = DOTween.Sequence();
		GameObject instantiateEffect = Instantiate(getEffect(effectName), effectTransform);
		instantiateEffect.transform.position = firstTransform.position;
		float moveTime = instantiateEffect.GetComponent<Damage>().deleteTime - AFTER_LANDING_TIME;
		sequence.Append(instantiateEffect.transform.DOMove(moveTargetTransform.position, moveTime).SetEase(Ease.InQuad));
		// 着弾時に対応するSEを流す
		sequence.AppendCallback(() => SEManager.instance.playSe(effectName));
		return moveTime;
	}

	public GameObject getEffect(string effectName)
	{
		if (string.IsNullOrEmpty(effectName))
		{
			effectName = "Slash";
		}
		if (effectCache.ContainsKey(effectName))
		{
			return effectCache[effectName];
		}

		GameObject effect = Resources.Load<GameObject>("Animation/Prefab/" + effectName);
		effectCache.Add(effectName, effect);
		return effect;
	}
}
