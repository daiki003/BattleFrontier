using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InputArea : MonoBehaviour
{
    public Text describe;
    public Button decideButton;
    public InputField inputField;
    public void Init(string text, UnityAction<string, bool> callback)
    {
        this.describe.text = text;
        decideButton.onClick.RemoveAllListeners();
        UnityAction action = () => callback.Invoke(inputField.text, false);
        action += () => Destroy();
		decideButton.onClick.AddListener(action);
    }

    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
