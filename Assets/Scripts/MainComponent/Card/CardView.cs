using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class CardView : MonoBehaviour
{
	// 手札用
	[SerializeField] protected Text numberText;
	[SerializeField] protected Image iconImage;
	[SerializeField] protected GameObject mainPanel;
	[SerializeField] public GameObject activeCursor;

	// 表示に必要なカードの要素
	public class DisplayComponent
	{
		public string name = "";
		public int number;
		public Color color;

		public DisplayComponent(CardController card)
		{
			if (card is NormalCard normalCard)
			{
				this.number = normalCard.number;
				this.color = normalCard.cardColor;
			}
			else
			{
				this.color = Color.white;
			}
		}
	}

	public void initIcon(string iconPath)
	{
		Sprite cardIcon = Resources.Load<Sprite>(iconPath);
		iconImage.sprite = cardIcon;
	}
	public void resetIcon()
	{
		iconImage.sprite = null;
	}

	public virtual void Show(CardController card)
	{
		Show(new DisplayComponent(card));
	}

	public virtual void Show(DisplayComponent component, bool forceOther = false, bool isChangeSize = true)
	{
		if (component.number == -1)
		{
			numberText.text = "?";
		}
		else
		{
			numberText.text = component.number.ToString();
		}
		changeColor(component.color);
	}

	public void changeColor(Color color)
	{
		var mainImage = mainPanel.GetComponent<Image>();
		mainImage.color = color;
	}

	public void changeSize(Vector3 size)
	{
		RectTransform cardTransfomr = this.transform as RectTransform;
		cardTransfomr.localScale = size;
	}

	public void bigAndSmall()
	{
		Vector3 small = new Vector3(1.0f, 1.0f, 1);
		Vector3 midium = new Vector3(1.2f, 1.2f, 1);
		Vector3 big = new Vector3(1.4f, 1.4f, 1);

		Sequence sequence = DOTween.Sequence();
		GameObject target = this.gameObject;

		sequence.Append(target.transform.DOScale(big, 0.1f))
				.Append(target.transform.DOScale(midium, 0.1f))
				.Append(target.transform.DOScale(small, 0.1f))
				.Append(target.transform.DOScale(midium, 0.1f))
				.Append(target.transform.DOScale(big, 0.1f))
				.Append(target.transform.DOScale(midium, 0.1f));
	}

	public void setSorting(bool priority)
	{
		Canvas canvas = GetComponent<Canvas>();
		
		canvas.overrideSorting = priority;
		canvas.sortingOrder = priority ? 1 : 0;
	}
}
