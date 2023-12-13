using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffLogItem : MonoBehaviour
{
    public Image icon;
    public Text describeText, numberText;
    public CardController buffOwnerCard;
    public string buffOwnerCardId;
    public string buffOwnerIconPath;
    public string describe;
    public int number;
    public void init(string buffOwnerCardId, string describe)
    {
        this.buffOwnerCardId = buffOwnerCardId;
        this.buffOwnerIconPath = "Images/Card/" + GameManager.instance.masterManager.getCardCategoryById(buffOwnerCardId) + "/" + buffOwnerCardId.Replace("(instant)", "");
        this.describe = describe;
        number = 1;
        
        Sprite cardIcon = Resources.Load<Sprite>(this.buffOwnerIconPath);
        icon.sprite = cardIcon;
        describeText.text = describe;
        describeText.fontSize = SizeUtility.getBuffItemFontSize(describe);
        numberText.text = number.ToString();
    }

    public void init(string propertyName)
    {
        this.describeText.text = propertyName;
        describeText.fontSize = SizeUtility.getBuffItemFontSize(propertyName);
        number = 1;
        numberText.text = number.ToString();
    }

    public void increaseNumber(int number)
    {
        this.number += number;
        numberText.text = this.number.ToString();
    }
}
