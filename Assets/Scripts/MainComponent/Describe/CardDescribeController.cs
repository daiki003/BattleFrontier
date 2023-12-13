using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardDescribeController : DescribeController
{
	public Text costText, attackText, hpText;
	public Transform textBox;
	public TextUnit textUnitPrefab;
	public Transform assignmentsTransform;
	public CardController describeCard;
	public Button bootButton;
	public ScrollRect scroll;
	private const int NON_BOOT_HEIGHT = 450;

	public override void init(object describeTarget, bool isSub = false, int magnification = 0)
	{
		if (describeTarget is CardController card)
		{
			setText(card);
			RectTransform rectTransform = GetComponent<RectTransform>();
			float hight = NON_BOOT_HEIGHT;
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hight);
		}
		else if (describeTarget.GetType() == typeof(CardMaster))
		{
			setSubText((CardMaster)describeTarget);
		}
	}

	public override void update()
	{
		
	}

	// 初期表示時のテキスト設定
	public void setText(CardController card)
	{
		describeCard = card;
		nameText.text = this.describeCard.model.cardName;
		costText.text = this.describeCard.model.baseCost.ToString();
		if (card.model.cardType == CardType.UNIT)
		{
			attackText.text = this.describeCard.model.baseAttack.ToString();
			hpText.text = this.describeCard.model.baseHp.ToString();
		}
		attackText.gameObject.SetActive(card.model.cardType == CardType.UNIT);
		hpText.gameObject.SetActive(card.model.cardType == CardType.UNIT);

		setAbilityText(this.describeCard.model.displayText);
		scroll.verticalNormalizedPosition = 1.0f;

		// 付与情報の表示
		foreach (Transform buffLogItem in assignmentsTransform)
		{
			GameObject.Destroy(buffLogItem.gameObject);
		}
	}

	// 補足情報として表示する場合、CardEntityから情報を取得
	public void setSubText(CardMaster cardMaster)
	{
		nameText.text = cardMaster.name;
		costText.text = cardMaster.status.cost.ToString();
		if (cardMaster.cardType == CardType.UNIT)
		{
			attackText.text = cardMaster.status.attack.ToString();
			hpText.text = cardMaster.status.hp.ToString();
		}
		attackText.gameObject.SetActive(cardMaster.cardType == CardType.UNIT);
		hpText.gameObject.SetActive(cardMaster.cardType == CardType.UNIT);
		foreach (Transform child in textBox)
		{
			GameObject.Destroy(child.gameObject);
		}
		var textList = new List<string>();
		if (!string.IsNullOrEmpty(cardMaster.instantText))
		{
			textList.Add(cardMaster.instantText);
			textList.Add("---------------------------");
		}
		if (cardMaster.textList.Count > 0)
		{
			textList.AddRange(cardMaster.textList);
		}
		else
		{
			textList.Add(cardMaster.text);
		}
		setAbilityText(textList);
	}

	public void setAbilityText(List<string> textList)
	{
		// 既にテキストがある場合は削除
		foreach (Transform child in textBox)
		{
			GameObject.Destroy(child.gameObject);
		}

		var textUnitList = new List<TextUnit>();
		foreach (string text in textList)
		{
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			TextUnit textUnit = Instantiate(textUnitPrefab, textBox);
			textUnit.setText(text);
			textUnitList.Add(textUnit);
		}
		var layoutGroup = textBox.GetComponent<TextLayoutGroup>();
		layoutGroup.alignment(textUnitList);
	}
}
