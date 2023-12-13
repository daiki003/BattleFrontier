using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelectMenu : PanelPrefab
{
	public GameObject classSelectMenu;
	public ClassSelectButton classSelectButtonPrefab;
	public Transform classSelectTransform;
	// クラス選択時の表示用
	[SerializeField] Text className;
	[SerializeField] Text classDescribe;
	// 選択中のクラス
	private CardCategory selectingCategory;
	private List<ClassSelectButton> buttonList = new List<ClassSelectButton>();
	private const int defaultClassNumber = 2;
	private const int maxClassNumber = 9;

	// デバッグ用
	public Toggle doubleClass;

	public override void setup()
	{
		close();
	}
	public override void close()
	{
		classSelectMenu.SetActive(false);
	}
	public override void onClickExtra()
	{

	}

	public void open()
	{
		classSelectMenu.SetActive(true);
		
		// 表示中のクラスボタンは一度削除する
		foreach (ClassSelectButton button in buttonList)
		{
			GameObject.Destroy(button.gameObject);
		}
		buttonList.Clear();

		for (int i = defaultClassNumber; i <= maxClassNumber; i++)
		{
			ClassSelectButton classButton = Instantiate(classSelectButtonPrefab, classSelectTransform);
			var category = (CardCategory)Enum.ToObject(typeof(CardCategory), i);
			classButton.init(category, () => setClass(category));
			buttonList.Add(classButton);
		}
		
		var firstButton = buttonList.FirstOrDefault();
		if (firstButton != null)
		{
			setClass((CardCategory)Enum.ToObject(typeof(CardCategory), firstButton.category));
			firstButton.setSelectingPanel(true);
		}
	}

	public void setClass(CardCategory category)
	{
		this.selectingCategory = category;
		this.className.text = GameManager.instance.masterManager.classTextList.FirstOrDefault(c => c.classId == selectingCategory.ToString()).className;
		this.classDescribe.text = GameManager.instance.masterManager.classTextList.FirstOrDefault(c => c.classId == selectingCategory.ToString()).describe;
		SEManager.instance.playSe("Button");

		foreach(ClassSelectButton button in buttonList)
		{
			button.setSelectingPanel(false);
		}
	}

	public void decide()
	{
		GameManager.instance.titleMgr.openCreateDeckMenu(selectingCategory);
	}
}
