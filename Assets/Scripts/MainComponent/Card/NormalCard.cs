using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 数字と色を持ったカードとして、カードエリアに置くカード
public class NormalCard : CardController
{
    public int number;
    public Color cardColor;
    public NormalCard(GameObject cardObject, int number, Color color, int index) : base(cardObject, index)
    {
        this.number = number;
        this.cardColor = color;
        spritePath = "Images/Card/BattleFrontier/" + number;
    }
}
