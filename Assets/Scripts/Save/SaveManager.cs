using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class SaveManager
{
	string filePath;
	SaveData saveData;
	public string playFabId { get; private set; }

	public SaveManager()
	{
		filePath = Application.persistentDataPath + "/" + ".savedata.json";
		saveData = new SaveData();
		Load();
		// 空で入ってしまったデータは取り除く
		if (saveData.decks != null)
		{
			saveData.decks.RemoveAll(d => d.cards == null);
		}
	}

	public void Save()
	{
		string json = JsonUtility.ToJson(saveData);
		StreamWriter streamWriter = new StreamWriter(filePath);
		streamWriter.Write(json);
		streamWriter.Flush();
		streamWriter.Close();
	}

	public void Load()
	{
		if (File.Exists(filePath))
		{
			StreamReader streamReader;
			streamReader = new StreamReader(filePath);
			string data = streamReader.ReadToEnd();
			streamReader.Close();
			saveData = JsonUtility.FromJson<SaveData>(data);
		}
		PlayFabController.login();
	}

	public void setPlayFabId(string id)
	{
		this.playFabId = id;
	}

	// 全てのデータを新しい値で上書きする
	public void overWriteSaveData(string playerName, List<SaveData.DeckData> deckList)
	{
		saveData.playerName = playerName ?? "";
		saveData.decks = deckList ?? new List<SaveData.DeckData>();

		GameManager.instance.titleMgr.updateDisplayName(saveData.playerName);
	}

	public SaveData getSaveData()
	{
		return saveData;
	}

	public void savePlayerId(string id)
	{
		saveData.playerId = id;
		Save();
	}

	public void addDeckData(int deckNumber, string deckName, List<SaveData.CardComponent> cards)
	{
		if (saveData.getDeckData(deckNumber) != null)
		{
			saveData.replaceOneData(deckNumber, deckName, cards);
		}
		else
		{
			saveData.decks.Add(new SaveData.DeckData(deckNumber, deckName, cards));
		}
		Save();
		PlayFabController.updateAllDecksData(saveData.decks);
	}

	public void changeDeckName(int deckNumber, string deckName)
	{
		SaveData.DeckData deckData = saveData.getDeckData(deckNumber);
		if (deckData != null)
		{
			saveData.replaceOneData(deckNumber, deckName, deckData.cards);
		}
		Save();
		PlayFabController.updateAllDecksData(saveData.decks);
	}

	public void deleteAllData()
	{
		saveData = new SaveData();
		Save();
		PlayFabController.updateAllDecksData(saveData.decks);
	}
}
