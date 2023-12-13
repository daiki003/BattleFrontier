using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;

public class ScreenShot : MonoBehaviour
{
	Regex c_b = new Regex(@"-\d+?$");
	private const string SCREENSHOT_FILE_PATH = @"C:\Users\daiki\Documents\スクリーンショット\";

	private void Awake()
	{
		// ゲーム内に一つだけ保持
		if (FindObjectsOfType<ScreenShot>().Length > 1)
			Destroy(gameObject);
		else
			DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		// // スペースキーが押されたら
		if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.Space))
		{
			// StartCoroutine(CaptureCoroutine(1700, 950, -90));
			Debug.Log("Save CardCut");
		}
		else if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
		{
			// StartCoroutine(CaptureCoroutine(1400, 900, -90));
			Debug.Log("Save SkillCut");
		}
		// else if (Input.GetKeyDown(KeyCode.Space))
		// {
		//	 // スクリーンショットを保存
		//	 System.DateTime date = System.DateTime.Now;
		//	 string fileName = "Screenshot_" + date.ToString("yyyyMMddHHmmss") + ".png";
		//	 Debug.Log("Save to " + fileName);
		// }
	}

	protected virtual IEnumerator CaptureCoroutine(int width, int height, int startHight = 0)
	{
		// カメラのレンダリング待ち
		yield return new WaitForEndOfFrame();
		Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
		// 切り取る画像の左下位置を求める
		int x = (tex.width - width) / 2;
		int y = ((tex.height - height) / 2) + startHight;
		Color[] colors = tex.GetPixels(x, y, width, height);
		Texture2D saveTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
		saveTex.SetPixels(colors);

		string fileName = GameManager.instance.cardListMgr.currentPickupCard != null ? GameManager.instance.cardListMgr.currentPickupCard.model.cardId : "NotCard";
		File.WriteAllBytes("ScreenShot/" + fileName + ".png", saveTex.EncodeToPNG());
	}
}
