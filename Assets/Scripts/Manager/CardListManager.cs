using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardListManager
{
	public CardListMainPanel mainPanel;
	
	private Vector3 cardSize = new Vector3(3.0f, 3.0f, 1f);

	public List<PanelPrefab> cardListPrefabs = new List<PanelPrefab>();
	public DescribeAreaController describeAreaController { get { return cardListPrefabs.FirstOrDefault(p => p is DescribeAreaController) as DescribeAreaController; } }

	private Dictionary<CardCategory, List<CardController>> allCardList = new Dictionary<CardCategory, List<CardController>>()
	{
		{CardCategory.Common, new List<CardController>()},
		{CardCategory.Ranger, new List<CardController>()},
		{CardCategory.Assassin, new List<CardController>()},
		{CardCategory.Blacksmith, new List<CardController>()},
		{CardCategory.Wizard, new List<CardController>()},
		{CardCategory.Dragoon, new List<CardController>()},
		{CardCategory.Necromancer, new List<CardController>()},
		{CardCategory.Hunter, new List<CardController>()},
		{CardCategory.Viking, new List<CardController>()},
		{CardCategory.Token, new List<CardController>()},
		{CardCategory.Enemy, new List<CardController>()}
	};

	private Dictionary<CardCategory, List<SkillPanelController>> allSkillList = new Dictionary<CardCategory, List<SkillPanelController>>()
	{
		{CardCategory.Common, new List<SkillPanelController>()},
		{CardCategory.Ranger, new List<SkillPanelController>()},
		{CardCategory.Assassin, new List<SkillPanelController>()},
		{CardCategory.Blacksmith, new List<SkillPanelController>()},
		{CardCategory.Wizard, new List<SkillPanelController>()},
		{CardCategory.Dragoon, new List<SkillPanelController>()},
		{CardCategory.Necromancer, new List<SkillPanelController>()},
		{CardCategory.Hunter, new List<SkillPanelController>()},
		{CardCategory.Viking, new List<SkillPanelController>()}
	};

	private CardCategory selectingClass;
	public CardController currentPickupCard;
	private SkillPanelController currentPickupSkill;
	private bool isSkill = false;

	public CardListManager()
	{
		mainPanel = PrefabManager.instance.createCardListMainPanel(GameManager.instance.mainCanvas);
		mainPanel.transform.SetSiblingIndex(GameManager.panelPrefabIndex);
	}

	public void startCardList()
	{
		Debug.Log("startCardList" + DateTime.Now + "." + DateTime.Now.Millisecond);
		// カードリストのカードを全て削除
		resetAllCardAndSkill();

		// 全てのカードをカードリストに作成
		isSkill = false;
		List<string> cardIdList = new List<string>();
		cardIdList.AddRange(GameManager.instance.masterManager.getCardIdListWithoutToken());

		// 最初はコモンだけを表示
		search(CardCategory.Common);
	}

	public void back()
	{
		resetAllCardAndSkill();
		GameManager.instance.GoToTitle();
	}

	// スキル関連--------------------------------------------------------------------------------------------------------------
	// スキルとカードの表示を変更
	public void changeCardAndSkill()
	{
		isSkill = !isSkill;

		// グリッドのサイズをカードやスキル用に変更
		GridLayoutGroup gridLayoutGroup = mainPanel.cardList.GetComponent<GridLayoutGroup>();
		gridLayoutGroup.cellSize = new Vector2(200, isSkill ? 200 : 240);

		Transform cardParent = !isSkill ? mainPanel.cardList : mainPanel.invisibleArea;
		Transform skillParent = isSkill ? mainPanel.cardList : mainPanel.invisibleArea;
		foreach (CardController card in allCardList[selectingClass])
		{
			card.gameObject.transform.SetParent(cardParent);
		}
		foreach (SkillPanelController skill in allSkillList[selectingClass])
		{
			skill.gameObject.transform.SetParent(skillParent);
		}

		mainPanel.changeSkillText.text = isSkill ? "カードへ" : "スキルへ";
	}

	// ピックアップ関連 -------------------------------------------------------------------------------------------------------------------------------------

	// ピックアップを終了
	public void cancelPickUp()
	{
		mainPanel.pickUpCard.resetCard();
		mainPanel.pickUpSkill.resetCard();
		mainPanel.pickUpCardPanel.SetActive(false);
		mainPanel.pickUpSkillPanel.SetActive(false);
	}

	// その他----------------------------------------------------------------------------------------------------------------------------------------------
	private void resetAllCardAndSkill()
	{
		mainPanel.cardList.resetCard();
		// skillList.resetCard();
		mainPanel.invisibleArea.resetCard();

		allCardList = new Dictionary<CardCategory, List<CardController>>()
		{
			{CardCategory.Common, new List<CardController>()},
			{CardCategory.Ranger, new List<CardController>()},
			{CardCategory.Assassin, new List<CardController>()},
			{CardCategory.Blacksmith, new List<CardController>()},
			{CardCategory.Wizard, new List<CardController>()},
			{CardCategory.Dragoon, new List<CardController>()},
			{CardCategory.Necromancer, new List<CardController>()},
			{CardCategory.Hunter, new List<CardController>()},
			{CardCategory.Viking, new List<CardController>()},
			{CardCategory.Token, new List<CardController>()},
			{CardCategory.Enemy, new List<CardController>()}
		};

		allSkillList = new Dictionary<CardCategory, List<SkillPanelController>>()
		{
			{CardCategory.Common, new List<SkillPanelController>()},
			{CardCategory.Ranger, new List<SkillPanelController>()},
			{CardCategory.Assassin, new List<SkillPanelController>()},
			{CardCategory.Blacksmith, new List<SkillPanelController>()},
			{CardCategory.Wizard, new List<SkillPanelController>()},
			{CardCategory.Dragoon, new List<SkillPanelController>()},
			{CardCategory.Necromancer, new List<SkillPanelController>()},
			{CardCategory.Hunter, new List<SkillPanelController>()},
			{CardCategory.Viking, new List<SkillPanelController>()}
		};
	}

	public void showChangeClassPanel(bool show)
	{
		if (show)
		{
			SEManager.instance.playSe("Button");
		}
		mainPanel.changeClassPanel.SetActive(show);
	}

	public void showSearchPanel(bool show)
	{
		if (show)
		{
			SEManager.instance.playSe("Button");
		}
		mainPanel.searchPanel.SetActive(show);
	}

	public void searchButton(int category)
	{
		SEManager.instance.playSe("Button");
		search((CardCategory)category);
	}

	// カテゴリを絞り込む
	public void search(CardCategory category)
	{
		mainPanel.classNameText.text = getClassNameText(category);
		selectingClass = category;

		if (!isSkill)
		{
			foreach (KeyValuePair<CardCategory, List<CardController>> pair in allCardList)
			{
				Transform parent = pair.Key == category ? mainPanel.cardList : mainPanel.invisibleArea;
				foreach (CardController card in pair.Value)
				{
					card.gameObject.transform.SetParent(parent);
				}
			}
		}
		else
		{
			foreach (KeyValuePair<CardCategory, List<SkillPanelController>> pair in allSkillList)
			{
				Transform parent = pair.Key == category ? mainPanel.cardList : mainPanel.invisibleArea;
				foreach (SkillPanelController skill in pair.Value)
				{
					skill.gameObject.transform.SetParent(parent);
				}
			}
		}

		showSearchPanel(false);
	}

	private string getClassNameText(CardCategory category)
	{
		switch (category)
		{
			case CardCategory.Ranger:
				return "レンジャー";
			case CardCategory.Assassin:
				return "アサシン";
			case CardCategory.Blacksmith:
				return "ブラックスミス";
			case CardCategory.Wizard:
				return "ウィザード";
			case CardCategory.Dragoon:
				return "ドラグーン";
			case CardCategory.Necromancer:
				return "ネクロマンサー";
			case CardCategory.Hunter:
				return "ハンター";
			case CardCategory.Viking:
				return "ヴァイキング";
			default:
				return "コモン";
		}
	}

	// 説明を表示する
	public void showDescribe(System.Object describeTarget, bool isSub = false, int magnification = 1)
	{
		describeAreaController.display(describeTarget, isSub, magnification);
	}
	public void deleteDescribe()
	{
		if (describeAreaController != null) describeAreaController.close();
	}

	public void onClickExtra()
	{
		foreach (PanelPrefab panel in cardListPrefabs)
		{
			panel.onClickExtra();
		}
	}
}
