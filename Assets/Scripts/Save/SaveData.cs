using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
	[Serializable]
	public class DeckData
	{
		public int deckNumber;
		public string deckName;
		public List<CardComponent> cards;
		public DeckData()
		{
			this.cards = new List<CardComponent>();
		}
		public DeckData(int deckNumber, string deckName, List<CardComponent> cards)
		{
			this.deckNumber = deckNumber;
			this.deckName = deckName;
			this.cards = new List<CardComponent>(cards);
		}
	}

	[Serializable]
	public class CardComponent
	{
		public string cardId;
		public ApplyInformation.SaveComponent applyInformation;
		public CardComponent()
		{

		}
		public CardComponent(string cardId, ApplyInformation applyInformation = null)
		{
			this.cardId = cardId;
			this.applyInformation = applyInformation != null ? applyInformation.createSaveComponent() : new ApplyInformation.SaveComponent();
		}
	}

	public string playerId;
	public string playerName;
	public List<DeckData> decks;
	public Dictionary<CreateDeckEnvironment, bool> enableEnvironmentList;

	public SaveData()
	{
		this.playerId = null;
		this.playerName = null;
		decks = new List<DeckData>();
		enableEnvironmentList = new Dictionary<CreateDeckEnvironment, bool>()
		{
			{CreateDeckEnvironment.GrassLand, true},
			{CreateDeckEnvironment.Forest, false},
			{CreateDeckEnvironment.GrassLand1, true}
		};
	}

	// DeckDataの指定カテゴリ、指定番号箇所を上書き保存
	public void replaceOneData(int deckNumber, string deckName, List<SaveData.CardComponent> cards)
	{
		int index = decks.FindIndex(d => d.deckNumber == deckNumber);
		decks[index] = new DeckData(deckNumber, deckName, cards);
	}

	// 環境解放状況を保存
	public void saveEnableEnvironment(CreateDeckEnvironment environment, bool enable)
	{
		if (enableEnvironmentList.ContainsKey(environment))
		{
			enableEnvironmentList[environment] = enable;
		}
		else
		{
			enableEnvironmentList.Add(environment, enable);
		}
	}

	// データを取得 --------------------------------------------------------------------------------------------------------------
	// 指定番号、指定カテゴリのDeckDataを取得
	public DeckData getDeckData(int deckNumber)
	{
		return decks.FirstOrDefault(d => d.deckNumber == deckNumber);
	}

	// 指定番号のDeckDataからデッキを取得
	public List<SaveData.CardComponent> getOneDeck(int deckNumber)
	{
		foreach (DeckData deck in decks)
		{
			if (deck.deckNumber == deckNumber)
			{
				return deck.cards;
			}
		}
		return new List<SaveData.CardComponent>();
	}

	// 指定カテゴリのクリア状況を取得
	public bool getEnableEnvironment(CreateDeckEnvironment environment)
	{
		if (enableEnvironmentList.ContainsKey(environment))
		{
			return enableEnvironmentList[environment];
		}
		else
		{
			return false;
		}
	}

	// データリセット --------------------------------------------------------------------------------------------------------------
	public void resetDeckData()
	{
		decks = new List<DeckData>();
	}
}
