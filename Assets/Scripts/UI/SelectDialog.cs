using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelectDialog : PanelPrefab
{
	public GameObject selectDialogPanel;
	public Button okButton, cancelButton;
    public Text mainText, okButtonText;
    public override void setup()
    {
        close();
    }
	public override void close()
    {
        this.selectDialogPanel.SetActive(false);
    }
	public override void onClickExtra()
    {

    }

    public void Init(string mainText, string okButtonText, UnityAction okCallback = null)
    {
        this.selectDialogPanel.SetActive(true);
		this.mainText.text = mainText;
		this.okButtonText.text = okButtonText;
		if (okCallback == null)
		{
			// コールバックが設定されていなければダイアログを閉じる
			okButton.onClick.AddListener(close);
			// キャンセルボタンは役割が被るので非表示
			cancelButton.gameObject.SetActive(false);
		}
		else
		{
			okButton.onClick.AddListener(okCallback);
			cancelButton.onClick.AddListener(close);
		}
    }
}
