using UnityEngine;
using System.Collections;

public class SpawnCharacterSelector : MonoBehaviour {

	public static string playerPrefs_PrefabIdString = "_PrefabID";

	public GameObject characterSelectorPrefab;

	private LobbyCharacterManager lobbyCharacterManager;

	void Awake()
	{
//		PlayerPrefs.DeleteAll();
	}

	void SpawnServerSpawnCharacterSelector()
	{
		// server doesn’t trigger OnPlayerConnected, manually spawn
		Network.Instantiate( characterSelectorPrefab, Vector3.zero, Quaternion.identity,0 );
		Debug.Log("Server CharacterSelector erzeugt");
	}

	void SetServerPlayerTextToScreen()
	{
		string text = "Player " + 0 + "\nconnected";									
		if(lobbyCharacterManager.player0GUIText != null)
			lobbyCharacterManager.player0GUIText.text = text ;
	}

	void Start()
	{
		lobbyCharacterManager = GetComponent<LobbyCharacterManager>();
		if( Network.isServer )
		{
			SpawnServerSpawnCharacterSelector();
		}
		else
		{
			if(Network.peerType == NetworkPeerType.Client)
			{

			}
		}
		SetServerPlayerTextToScreen();
	}

	void OnPlayerConnected( NetworkPlayer connectedPlayer )
	{
		// when a player joins, tell them to spawn
		networkView.RPC( "net_DoSpawn", connectedPlayer, Vector3.zero );

		lobbyCharacterManager.SetAllPlayerSprites(connectedPlayer);

		foreach(NetworkPlayer player in Network.connections)
		{
			if( connectedPlayer != player )
			{
				// tell connectedPlayer who is connected
				networkView.RPC( "WhoIsConnectedToLobby", connectedPlayer, player );
			}
		}

		// tell all players
		networkView.RPC( "PlayerConnectedToLobby", RPCMode.All, connectedPlayer );
//		SetPlayerTextToScreen(connectedPlayer, text);										// Local auf Server ausführen
	}

	[RPC]
	void WhoIsConnectedToLobby( NetworkPlayer player )
	{
		string text = "Player " + player.ToString() + "\nconnected";
		SetPlayerTextToScreen(player, text);
	}

	[RPC]
	void PlayerConnectedToLobby( NetworkPlayer connectedPlayer )
	{
		string text = "Player " + connectedPlayer.ToString() + "\nconnected";
		SetPlayerTextToScreen(connectedPlayer, text);
	}
	
	void OnPlayerDisconnected( NetworkPlayer disconnectedPlayer )
	{
		// Character in PlayerPrefs freigeben
		lobbyCharacterManager.RemovePlayerCharacter(disconnectedPlayer.ToString());

		lobbyCharacterManager.SetAllPlayerSprites(RPCMode.All);

		// tell all players
		networkView.RPC( "PlayerDisconnectedFromLobby", RPCMode.All, disconnectedPlayer );

        Debug.Log(disconnectedPlayer.ipAddress + " disconnected!");
//		SetPlayerTextToScreen(disconnectedPlayer, text);										// Local auf Server ausführen
		Debug.Log("Clean up after player " + disconnectedPlayer);
		Network.RemoveRPCs(disconnectedPlayer);
		Network.DestroyPlayerObjects(disconnectedPlayer);
	}

	[RPC]
	void PlayerDisconnectedFromLobby( NetworkPlayer disconnectedPlayer )
	{
		string text = "Player " + disconnectedPlayer.ToString() + "\ndisconnected";
		SetPlayerTextToScreen(disconnectedPlayer, text);
    }
    
	/**
	 * Messages to Screen
	 **/
	void SetPlayerTextToScreen( NetworkPlayer player, string text)
	{
		//		if(player.ToString() == "0")									// NetworkPlayer 0 ist Server, immer verbunden
		//		{
		//			if(lobbyCharacterManager.player0GUIText != null)
		//				lobbyCharacterManager.player0GUIText.text = text ;
		//		}
		if(player.ToString() == "1")
		{
			if(lobbyCharacterManager.player1GUIText != null)
				lobbyCharacterManager.player1GUIText.text = text;
		}
		else if(player.ToString() == "2")
		{
			if(lobbyCharacterManager.player2GUIText != null)
				lobbyCharacterManager.player2GUIText.text = text;
		}
		else if(player.ToString() == "3")
		{
			if(lobbyCharacterManager.player3GUIText != null)
                lobbyCharacterManager.player3GUIText.text = text;
        }
    }
    
    void OnDisconnectedFromServer( NetworkDisconnection cause )
	{
		// go back to the main menu
		Application.LoadLevel( "mp_Multiplayer" );
	}
	
	[RPC]
	void net_DoSpawn( Vector3 position )
	{
		// spawn the player paddle
		Network.Instantiate( characterSelectorPrefab, position, Quaternion.identity,0 );
	}
}
