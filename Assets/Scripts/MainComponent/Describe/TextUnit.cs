using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Hypertext;

public class TextUnit : MonoBehaviour
{
	public RectTransform textArea;
	public RegexHypertext abilityText;
	[System.NonSerialized] public string rawText;
	[System.NonSerialized] public float hight;
	private List<char> noFirstLineChar = new List<char>()
	{
		'。',
		'、',
		'」',
		'】',
		'』',
		'》',
		'）',
	};
	private List<char> noLastLineChar = new List<char>()
	{
		'「',
		'【',
		'『',
		'《',
		'（',
		'+',
	};

	public void setText(string text)
	{
		abilityText.text = text;
		rawText = text;
		setHight();
		applyLineRule();
	}

	public void setHight()
	{
		float hight = abilityText.preferredHeight;
		textArea.SetHeight(hight);
		this.hight = hight;
	}

	public void setPosition(Vector3 position)
	{
		textArea.anchoredPosition = position;
	}

	public void applyLineRule()
	{
		if (abilityText.text == "")
		{
			return;
		}
		// 改行位置等を計算
		var generator = new TextGenerator();
		var size = new Vector2(textArea.rect.width, textArea.rect.height);
		generator.Populate(abilityText.text, abilityText.GetGenerationSettings(size));

		// 改行位置を取得
		foreach(var l in generator.lines)
		{
			int index = l.startCharIdx;
			Debug.Log(index);
			Debug.Log(abilityText.text[index]);
			if (index == 0 || abilityText.text.Length <= index)
			{
				continue;
			}
			// 行頭禁則、行末禁則に引っ掛かる場合、行末の1文字前に改行を入れる
			if (noFirstLineChar.Contains(abilityText.text[index]) || noLastLineChar.Contains(abilityText.text[index - 1]))
			{
				abilityText.text = abilityText.text.Insert(index - 1, "\n");
			}
		}
	}
}
