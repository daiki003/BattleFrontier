using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescribeAreaController : PanelPrefab
{
	public GameObject describeArea;
	public Transform mainArea, subArea;
	public CardDescribeController cardDescribePrefab;
	public SkillDescribeController skillDescribePrefab;
	public QuestDescribeController questDescribePrefab;
	public KeywordDescribeController keywordDescribePrefab;

	List<DescribeController> mainDescribes = new List<DescribeController>();
	List<DescribeController> subDescribes = new List<DescribeController>();
	private CardDescribeController mainCardDescribe;
	private SkillDescribeController mainSkillDescribe;
	private QuestDescribeController mainQuestDescribe;
	private CardDescribeController subCardDescribe;
	private SkillDescribeController subSkillDescribe;
	private KeywordDescribeController subKeywordDescribe;

	DescribeController displayingDescribe;

	public bool isDescribe { get { return describeArea.activeSelf; } }

	public override void setup()
	{
		mainCardDescribe = Instantiate(cardDescribePrefab, mainArea);
		mainDescribes.Add(mainCardDescribe);

		mainSkillDescribe = Instantiate(skillDescribePrefab, mainArea);
		mainDescribes.Add(mainSkillDescribe);
		
		mainQuestDescribe = Instantiate(questDescribePrefab, mainArea);
		mainDescribes.Add(mainQuestDescribe);
		
		subCardDescribe = Instantiate(cardDescribePrefab, subArea);
		subDescribes.Add(subCardDescribe);
		
		subSkillDescribe = Instantiate(skillDescribePrefab, subArea);
		subDescribes.Add(subSkillDescribe);
		
		subKeywordDescribe = Instantiate(keywordDescribePrefab, subArea);
		subDescribes.Add(subKeywordDescribe);
		
		close();
	}
	public override void close()
	{
		describeArea.SetActive(false);
		hideAllDescribe(false);
	}
	public override void onClickExtra()
	{
		close();
	}
	public void update()
	{
		if (displayingDescribe != null)
		{
			displayingDescribe.update();
		}
	}

	public void display(System.Object discribeTarget, bool isSub = false, int magnification = 0)
	{
		describeArea.SetActive(true);
		hideAllDescribe(isSub);

		displayingDescribe = null;
		if (discribeTarget.GetType() == typeof(CardController) || discribeTarget.GetType() == typeof(CardMaster))
		{
			displayingDescribe = isSub ? subCardDescribe : mainCardDescribe;
		}
		else if (discribeTarget.GetType() == typeof(SkillPanelController))
		{
			displayingDescribe = isSub ? subSkillDescribe : mainSkillDescribe;
		}
		else if (discribeTarget.GetType() == typeof(Quest))
		{
			displayingDescribe = mainQuestDescribe;
		}
		else if (discribeTarget.GetType() == typeof(KeywordMaster))
		{
			displayingDescribe = subKeywordDescribe;
		}

		if (displayingDescribe != null)
		{
			displayingDescribe.init(discribeTarget, isSub, magnification);
			displayingDescribe.gameObject.SetActive(true);
		}
	}

	public void hideAllDescribe(bool isSub)
	{
		List<DescribeController> hideDescribes = new List<DescribeController>();
		hideDescribes.AddRange(subDescribes);
		if (!isSub) hideDescribes.AddRange(mainDescribes);
		foreach (DescribeController describe in hideDescribes)
		{
			describe.gameObject.SetActive(false);
		}
	}
}
