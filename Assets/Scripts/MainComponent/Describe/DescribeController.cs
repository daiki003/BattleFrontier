using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hypertext;

public class DescribeController : MonoBehaviour
{
	[SerializeField] public Text nameText;
	[SerializeField] public RegexHypertext abilityText;

	// static化
	public static DescribeController instance;
	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public virtual void init(System.Object describeTarget, bool isSub = false, int magnification = 0)
	{

	}

	public virtual void update()
	{
		
	}

	public void deleteDescribe()
	{
		Destroy(this.gameObject);
	}
}
