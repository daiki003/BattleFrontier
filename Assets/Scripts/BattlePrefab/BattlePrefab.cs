using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelPrefab : MonoBehaviour
{
	public abstract void setup();
	public abstract void close();
	public abstract void onClickExtra();
	public void destroy()
	{
		Destroy(this.gameObject);
	}
}
