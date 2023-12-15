using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum NetworkOperationType
{
	PLAY_CARD,
	DRAW,
	TURN_END,
	MOVE_FLAG,
}

public enum RoomPhase
{
	NONE = 0,
	WAIT_ENEMY = 1,
	PREPARE = 2,
	BATTLE = 3,
	RESULT = 4,
	INIT = 5
}

public class NetworkParameter
{
	public int cardIndex;
	public int[] indexList;
	public bool isSpecial;
	public bool isSelf;
	public int fieldIndex;
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public string firstHandKey = "first_hand";
	public string roomPhaseKey = "room_phase";

	public void Connect()
	{
		// PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
			var localPlayer = PhotonNetwork.LocalPlayer;
			localPlayer.NickName = "localPlayer";
			Debug.Log("StartToConnect");
		}
	}

	// ランダムマッチング開始
	public void RandomMatch()
	{
		if (PhotonNetwork.IsConnectedAndReady)
		{
			PhotonNetwork.JoinRandomRoom();
			GameManager.instance.titleMgr.DisplayLoading();
		}
	}
	
	// ルーム作成
	public void CreateRoom()
	{
		if (PhotonNetwork.IsConnectedAndReady)
		{
			// 作成するルームのルーム設定を行う
			var roomOptions = new RoomOptions();
			roomOptions.MaxPlayers = 2;
			PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
			GameManager.instance.titleMgr.DisplayLoading();
		}
	}
	
	// プライベートルーム作成
	public void CreateRoom(string roomName, bool isVisible)
	{
		if (PhotonNetwork.IsConnectedAndReady)
		{
			// 作成するルームのルーム設定を行う
			var roomOptions = new RoomOptions();
			roomOptions.MaxPlayers = 2;
			roomOptions.IsVisible = isVisible;
			PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
			GameManager.instance.titleMgr.DisplayLoading(roomName);
		}
	}

	// ルーム退出
	public void LeaveRoom()
	{
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	// コールバック ------------------------------------------------------------------------------------------------
	
	// 切断時
	public override void OnDisconnected(DisconnectCause cause)
	{
		
	}

	// マスターサーバーへの接続が成功した時
	public override void OnConnectedToMaster()
	{
		Debug.Log("ConnectToMaster");
	}
	
	// ゲームサーバー作成時
	public override void OnCreatedRoom()
	{
		Debug.Log("CreateRoom");
	}

	// ゲームサーバーへの接続が成功した時
	public override void OnJoinedRoom()
	{
		Debug.Log("JoinRoom");
		UpdatePlayerPhase(RoomPhase.WAIT_ENEMY, isSelf: true);
	}

	// ランダムルーム参加失敗時
	public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }
	
	// ゲームサーバーへの接続失敗時
	public override void OnJoinRoomFailed(short returnCode, string message)
    {
        GameManager.instance.titleMgr.selectDialog.Init(mainText: "ルームが満室です", okButtonText: "戻る");
    }
	
	// ゲームサーバー退出時
	public override void OnLeftRoom()
	{
		Debug.Log("LeftRoom");
	}

	// 他のプレイヤーがルームに入った時
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		
	}

	// 他プレイヤー退出時
	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		if (GameManager.instance.battleMgr != null)
		{
			GameManager.instance.battleMgr.EnemyRetire();
		}
	}

	// ルームプロパティ変更時
	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
        
    }

	// プレイヤープロパティ変更時
	public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProp)
	{
		// ルームに2人以上いて、マスタークライアントの場合のみ以降の処理を行う
		if (!PhotonNetwork.IsMasterClient || PhotonNetwork.PlayerList.Length < 2)
		{
			return;
		}

		if (CurrentRoomPhase() == RoomPhase.WAIT_ENEMY)
		{
			BattlePrepare();
		}
		else if (CurrentRoomPhase() == RoomPhase.PREPARE)
		{
			photonView.RPC(nameof(BattleStart), RpcTarget.All);
		}
    }

	// バトル開始までの導線 ------------------------------------------------------------------------------------------------
	// 両者の初期手札決定（マスタークライアントが両者分更新）
	public void BattlePrepare()
	{
		var selfHashtable = new ExitGames.Client.Photon.Hashtable();
		var enemyHashtable = new ExitGames.Client.Photon.Hashtable();
		List<int> playerFirstDrawIndex = new List<int>();
		List<int> enemyFirstDrawIndex = new List<int>();
		var indexList = RandomUtility.random(60, 14);
		for (int i = 0; i < indexList.Count; i++)
		{
			if (i < 7)
			{
				playerFirstDrawIndex.Add(indexList[i]);
			}
			else
			{
				enemyFirstDrawIndex.Add(indexList[i]);
			}
		}
		selfHashtable[firstHandKey] = playerFirstDrawIndex.ToArray();
		enemyHashtable[firstHandKey] = enemyFirstDrawIndex.ToArray();
		selfHashtable[roomPhaseKey] = enemyHashtable[roomPhaseKey] = RoomPhase.PREPARE;
		PhotonNetwork.LocalPlayer.SetCustomProperties(selfHashtable);
		GetEnemyPlayer().SetCustomProperties(enemyHashtable);
	}
	
	// Action ------------------------------------------------------------------------------------------------
	public void SendAction(NetworkOperationType operationType, NetworkParameter parameter)
	{
		switch (operationType)
		{
			case NetworkOperationType.PLAY_CARD:
				photonView.RPC(nameof(EnemyCardPlay), RpcTarget.OthersBuffered, parameter.cardIndex, parameter.fieldIndex);
				break;
			case NetworkOperationType.DRAW:
				photonView.RPC(nameof(Draw), RpcTarget.OthersBuffered, parameter.indexList, parameter.isSpecial, parameter.isSelf);
				break;
			case NetworkOperationType.TURN_END:
				photonView.RPC(nameof(TurnEnd), RpcTarget.OthersBuffered);
				break;
			case NetworkOperationType.MOVE_FLAG:
				photonView.RPC(nameof(MoveFlag), RpcTarget.OthersBuffered, parameter.fieldIndex);
				break;
		}
	}

	[PunRPC]
	public void BattleStart()
	{
		UpdatePlayerPhase(RoomPhase.BATTLE, isSelf: true);
		bool isMasterClient = PhotonNetwork.IsMasterClient;
		StartCoroutine(GameManager.instance.StartBattle(isMasterClient));
	}

	[PunRPC]
	public void EnemyCardPlay(int cardIndex, int fieldIndex)
	{
		BattleManager.instance.enemyActionController.SetCard(cardIndex, fieldIndex);
	}

	[PunRPC]
	public void Draw(int[] indexArray, bool isSpecial, bool isSelf)
	{
		var indexList = indexArray.ToList();
		BattleManager.instance.enemyActionController.Draw(indexList, isSelf, isSpecial);
	}

	[PunRPC]
	public void TurnEnd()
	{
		BattleManager.instance.changeTurn();
	}

	[PunRPC]
	public void MoveFlag(int fieldIndex)
	{
		BattleManager.instance.enemyActionController.GetFlag(fieldIndex);
	}

	// 各種取得用 ------------------------------------------------------------------------------------------------
	public Player GetEnemyPlayer()
	{
		if (PhotonNetwork.PlayerListOthers.Length == 0)
		{
			return null;
		}
		return PhotonNetwork.PlayerListOthers[0];
	}
	public List<int> GetPlayerFirstHand()
	{
		return ((int[])PhotonNetwork.LocalPlayer.CustomProperties[firstHandKey]).ToList();
	}

	public List<int> GetEnemyFirstHand()
	{
		return ((int[])GetEnemyPlayer().CustomProperties[firstHandKey]).ToList();
	}

	// フェーズ管理 ------------------------------------------------------------------------------------------------------------------------
	// プレイヤーのルームフェーズを更新
	public void UpdatePlayerPhase(RoomPhase phase, bool isSelf)
	{
		var player = isSelf ? PhotonNetwork.LocalPlayer : GetEnemyPlayer();
		var hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[roomPhaseKey] = phase;
		player.SetCustomProperties(hashtable);
	}
	
	// 現在のルームフェーズを取得
	public RoomPhase CurrentRoomPhase()
	{
		var currentPhase = RoomPhase.INIT;
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			// 全員が同じフェーズの場合のみ、そのフェーズを返す
			if (currentPhase == RoomPhase.INIT)
			{
				currentPhase = (RoomPhase)player.CustomProperties[roomPhaseKey];
			}
			else if ((RoomPhase)player.CustomProperties[roomPhaseKey] != currentPhase)
			{
				return RoomPhase.NONE;
			}
		}
		return currentPhase;
	}
}