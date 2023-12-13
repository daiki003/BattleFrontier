using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{
	[Header("CardArea")]
	public Transform banishArea;
	public Transform onplay;
	public GameObject resultPanel, restartButton, backButton, selectPanel, turnPanel, bossAlertPanel;
	public Transform waitArea, center, playArea, skillArea, invisibleArea;
	public Transform leaderDamageTransfotm;
	public Text resultText, turnText, remainingTurnText;


	[Header("ShowCard")]
	public GameObject show;
	public GameObject graveShow;
	public Transform showCard;
	public Transform graveShowCard;
	
	[Header("Select")]
	[SerializeField] GameObject select;
	[SerializeField] public Transform selectCard;
	[SerializeField] Text selectText;
	[SerializeField] Button decide;

	[Header("SkillPanel")]
	public GameObject skillPanel;
	public Transform battleSkillArea;
	public Transform editSkillArea;
	public Transform skillList;
	public Transform questList;
	public Transform skillAndQuest1;
	public Transform skillAndQuest2;
	public Transform skillAndQuest3;
	public Transform skillAndQuest4;
	public SkillAndQuest skillAndQuestPrefab;

	[Header("Tutorial")]
	[SerializeField] GameObject descriptionPanel;
	[SerializeField] GameObject tutorialPanel;
	[SerializeField] GameObject costPoint;
	[SerializeField] GameObject hpPoint;
	[SerializeField] GameObject shieldPoint;
	[SerializeField] GameObject skill1Point;
	[SerializeField] GameObject skill2_1Point;
	[SerializeField] Text descriptionText;
	[SerializeField] Button backDescription;
	[SerializeField] Button nextDescription;

	[Header("Information")]
	[SerializeField] GameObject information;
	[SerializeField] Text turn;
	[SerializeField] Text amulet;
	[SerializeField] Text enemyBuddy;
	[SerializeField] Text enemyBarrier;
	
	public CardCategory selectingCategory;
	public Button turnEndButton;

	public AudioSource audioSource;
	public AudioClip turnEndSound;
}
