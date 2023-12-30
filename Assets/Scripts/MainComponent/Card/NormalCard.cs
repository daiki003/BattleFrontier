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

public class JokerCard : NormalCard
{
    public JokerCard(GameObject cardObject, int index) : base(cardObject, -1, Color.white, index)
	{
		spritePath = "Images/Card/BattleFrontier/Joker";
	}
}

public class WildSevenCard : NormalCard
{
    public WildSevenCard(GameObject cardObject, int index) : base(cardObject, 7, Color.white, index)
	{
		spritePath = "Images/Card/BattleFrontier/WildSeven";
	}
}