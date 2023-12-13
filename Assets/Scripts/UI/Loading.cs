using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Timers;

public class Loading : PanelPrefab
{
    public GameObject loadingPanel, roomNamePanel;
    public Text roomName, loadingText;
    private string defaultText = "対戦相手を待っています";
    private int timer;
    private int num;
    public override void setup()
    {
        close();
    }
	public override void close()
    {
        this.loadingPanel.SetActive(false);
    }
	public override void onClickExtra()
    {

    }

    public void Init(string roomName, string waitingText)
    {
        this.loadingPanel.SetActive(true);
        if (string.IsNullOrEmpty(roomName))
        {
            roomNamePanel.SetActive(false);
        }
        else
        {
            roomNamePanel.SetActive(true);
            this.roomName.text = roomName;
        }
        defaultText = waitingText;
        loadingText.text = defaultText;
        num = 0;
        timer = 0;
    }

    public void UpdateLoadingText()
    {
        if (num < 3)
        {
            loadingText.text = loadingText.text + ".";
            Debug.Log("タイマー+");
            num++;
        }
        else
        {
            loadingText.text = defaultText;
            Debug.Log("タイマーリセット");
            num = 0;
        }
    }

    public void CancelLoading()
    {
        GameManager.instance.networkManager.LeaveRoom();
        close();
    }
    
    void Update()
    {
        timer++;
        if (timer >= 60)
        {
            if (num < 3)
            {
                loadingText.text = loadingText.text + ".";
                num++;
            }
            else
            {
                loadingText.text = defaultText;
                num = 0;
            }
            timer = 0;
        }
    }
}