using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RecoveryManager
{
    private string filePath = Application.persistentDataPath + "/" + "recovery.json";
    private string debugFilePath = Application.persistentDataPath + "/" + "debug_recovery.json";

    public string createFilePath(string fileName)
    {
        if (fileName == "")
        {
            fileName = "recovery";
        }
        return Application.persistentDataPath + "/" + fileName + ".json";
    }

    public void createRecoveryData(PlayerController player, string fileName)
    {
        SaveCardCollection cardCollection = new SaveCardCollection();
        cardCollection.handCardList = player.cardCollection.handCardList.Select(c => c.createSaveComponent()).ToList();
        cardCollection.fieldCardList = player.cardCollection.fieldCardList.Select(c => c.createSaveComponent()).ToList();
        cardCollection.deckCardList = player.cardCollection.deckCardList.Select(c => c.createSaveComponent()).ToList();
        cardCollection.graveCardList = player.cardCollection.graveCardList.Select(c => c.createSaveComponent()).ToList();

        StreamWriter streamWriter = new StreamWriter(createFilePath(fileName));
		streamWriter.Write(JsonUtility.ToJson(new RecoveryData(cardCollection)));
		streamWriter.Flush();
		streamWriter.Close();
    }

    public void checkRecovery(string fileName)
    {
        string targetFilePath = createFilePath(fileName);
        if (File.Exists(targetFilePath))
        {
            StreamReader streamReader;
			streamReader = new StreamReader(targetFilePath);
			string data = streamReader.ReadToEnd();
			streamReader.Close();
            Debug.Log(data);
			RecoveryData recoveryData = JsonUtility.FromJson<RecoveryData>(data);
            GameManager.instance.startRecoveryCoroutine(recoveryData);
        }
    }
}

[System.Serializable]
public class RecoveryData
{
    public SaveCardCollection cardCollection;
    public RecoveryData() {}
    public RecoveryData(SaveCardCollection saveCardCollection)
    {
        this.cardCollection = saveCardCollection;
    }
}

[System.Serializable]
public class SaveCardCollection
{
    [System.Serializable]
    public class TurnAndSaveComponent
    {
        public SaveData.CardComponent card;
        public int turn;
        public bool battlePhase;

        public TurnAndSaveComponent() {}
        public TurnAndSaveComponent(SaveData.CardComponent card, int turn, bool battlePhase)
        {
            this.card = card;
            this.turn = turn;
            this.battlePhase = battlePhase;
        }
    }

	public string test = "test";
    public List<CardComponent> masterDeck = new List<CardComponent>();

	// 実際のカードリスト ------------------------------------------------------------------------------------------------------------------------------
	public List<SaveData.CardComponent> handCardList = new List<SaveData.CardComponent>(); // 手札のカードリスト
	public List<SaveData.CardComponent> fieldCardList = new List<SaveData.CardComponent>(); // フィールドのカードリスト
	public List<SaveData.CardComponent> deckCardList = new List<SaveData.CardComponent>(); // デッキのカードリスト
	public List<SaveData.CardComponent> graveCardList = new List<SaveData.CardComponent>(); // 墓場のカードリスト

	// 記録されているカードリスト ------------------------------------------------------------------------------------------------------------------------
	public List<TurnAndSaveComponent> playCardList = new List<TurnAndSaveComponent>(); // バトル中にプレイしたカードリスト
	public List<TurnAndSaveComponent> summonedCardList = new List<TurnAndSaveComponent>(); // バトル中に場に出たカードリスト
	public List<TurnAndSaveComponent> destroyedCardList = new List<TurnAndSaveComponent>(); // バトル中に破壊されたカードリスト
	public List<TurnAndSaveComponent> leftCardList = new List<TurnAndSaveComponent>(); // バトル中に場を離れたカードリスト
	public List<TurnAndSaveComponent> returnCardList = new List<TurnAndSaveComponent>(); // バトル中に手札に戻ったカードリスト
	public List<TurnAndSaveComponent> discardedCardList = new List<TurnAndSaveComponent>(); // バトル中に捨てられたカードリスト
}