using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyCharacterManager : MonoBehaviour {

	public static string resourcesPath = "PlayerCharacter/UnityNetwork Lan RigidBody2D/";
	public static string suffixName = "_Prefab_Name";
	public static string noCharacter = "noCharacter";

	public GUIText player0GUIText;
	public GUIText player1GUIText;
	public GUIText player2GUIText;
	public GUIText player3GUIText;

//	public GUITexture player0GUITexture;
//	public GUITexture player1GUITexture;
//	public GUITexture player2GUITexture;
//	public GUITexture player3GUITexture;

	public SpriteRenderer player0SpriteRenderer;
	public SpriteRenderer player1SpriteRenderer;
	public SpriteRenderer player2SpriteRenderer;
	public SpriteRenderer player3SpriteRenderer;

	private string debugmsg="";

	private Dictionary<string,string> characterDictionary;

	//Texture2D[] characterArray;
//	Sprite[] characterArray;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		characterDictionary = new Dictionary<string, string>();
//		PlayerPrefs.DeleteAll();		// delete PlayerPrefs auf Server und Client

		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		debugmsg="";
//		characterArray = Resources.LoadAll<Sprite>("Skins");			// alle Sliced Sprites (Spritename_0) ...
		//characterArray = Resources.LoadAll<Texture2D>("Skins");		// nur ganze Bilder 
		
		//if(Network.isServer)
		if(networkView.isMine)
		{
			
//			for(int i=0; i < characterArray.Length; i=i+6)
//			{
//				debugmsg+=characterArray[i].name + "\n";
////				Debug.Log(characterArray[i].name);
//			}
		}
	}

	// ArgumentException: You can only call GUI functions from inside OnGUI.
	void OnGUI()
	{
		if(Network.isServer)
		{
			DebugListAllPlayer();
			if(EveryPlayerHasValidCharacter())
			{
				if(GUILayout.Button( "Start Game", GUILayout.Width( 100f ) ))
				{
					DebugListAllPlayer();
					networkView.RPC("StartGame", RPCMode.All, "mp_classic_selected_character");
				}
			}
		}
	}

	void DebugListAllPlayer()
	{
		string playerCharacterName = GetPlayerCharacter(GetPlayerPrefsKey("0"));
		Debug.LogWarning("Player " + GetPlayerPrefsKey("0") + " Character Prefab Name: " + playerCharacterName);

		foreach(NetworkPlayer player in Network.connections)
		{
			playerCharacterName = GetPlayerCharacter(GetPlayerPrefsKey(player.ToString()));
			Debug.LogWarning("Player " + GetPlayerPrefsKey(player.ToString()) + " Character Prefab Name: " + playerCharacterName);
		}
	}

	[RPC]
	void StartGame(string sceneName)
	{
		// starte das gewünschte Level auf allen Clients
		// Hash / static string scenename/levelname
		NetworkLevelLoader.Instance.LoadLevel(sceneName,0);
	}

	public string GetPlayerPrefsKey( string playerID)
	{
		string key = playerID + suffixName;
		key = key.ToLower();
		return key;
	}

	public bool PlayerPrefsHasKey( string playerID )
	{
		//return PlayerPrefs.HasKey(GetPlayerPrefsKey(playerID));
		return characterDictionary.ContainsKey(GetPlayerPrefsKey(playerID));
	}

	public void SetPlayerCharacter( string playerId, string characterPrefabName)
	{
		characterDictionary.Add(GetPlayerPrefsKey(playerId), characterPrefabName);
		//PlayerPrefs.SetString(GetPlayerPrefsKey(playerId), characterPrefabName);
		//Debug.Log("Key: " + GetPlayerPrefsKey(playerId) + " mit Value: " + GetPlayerCharacter(GetPlayerPrefsKey(playerId)) + " in PlayerPrefs eingetragen!");
		Debug.Log("Key: " + GetPlayerPrefsKey(playerId) + " mit Value: " + GetPlayerCharacter(GetPlayerPrefsKey(playerId)) + " in PlayerPrefs eingetragen!");
	}

	public void RemovePlayerCharacter( string playerId )
	{
		if(PlayerHasValidCharacter(playerId))
		{
			// PlayerPrefs.DeleteKey(GetPlayerPrefsKey(playerId));
			if(characterDictionary.ContainsKey(GetPlayerPrefsKey(playerId)))
			{
				characterDictionary.Remove(GetPlayerPrefsKey(playerId));
				Debug.Log("Player " + playerId + " hat Character " + GetPlayerCharacter(playerId) + " wieder freigegeben!");
			}
			else
				Debug.Log("Player " + playerId + " hate kein Character!");
		}

	}
	
	public bool PlayerHasValidCharacter( string playerId )
	{
		string key = GetPlayerPrefsKey(playerId);

		if(PlayerPrefsHasKey(playerId))
		{
			string value = GetPlayerCharacter(key);
			if(value == "")															// PrefabName = "" --> ERROR
			{
				Debug.LogError("PlayerPrefs " + key + " value is an empty string"); 
				return false;
			}
			if(GameObject.Find(value) != null)										// GameObject with PrefabName found? no --> ERROR
			{
				return true;
			}
			else
			{
				Debug.LogError("PlayerPrefs " + key + " with value " + value + " no GameObject in Scene found");
				return false;
			}
		}
		else
		{
			Debug.LogError("Player " + playerId + " has no character selected");
			return false;
		}
		
	}

	public bool ServerPlayerHasValidCharacter()
	{
		// Server Character
		string playerID = "0";
		if(PlayerHasValidCharacter(playerID))
		{
			return true;
		}
		return false;
	}
	
	public bool EveryPlayerHasValidCharacter()
	{
		// Server Character
		if(!ServerPlayerHasValidCharacter())
		{
			return false;
		}
		
		// Clients
		foreach(NetworkPlayer player in Network.connections)
		{
			if(!PlayerHasValidCharacter(player.ToString()))
				return false;
		}
		return true;
	}

	/**
	 * Check in PlayerPrefs
	 **/
	public bool CheckPrefabInUse(string characterPrefabName)
	{
		/*
		 * ACHTUNG!!!! Server hat ID 0, NetworkPlayer geht bei 1 los!
		 * //foreach(NetworkPlayer player in Network.connections)
		 * 
		 * wenn player 2 disconnected, wird slot nicht direkt freigegeben!
		 * Debug.LogWarning("Verbindungsanzahl: " + Network.connections.Length);
		 */
		
		// Charactere der Clients checken
		foreach(NetworkPlayer player in Network.connections)
		{
			if(PlayerHasValidCharacter(player.ToString()))
			{
				if(GetPlayerCharacter(player.ToString()) == characterPrefabName)
				{
					return true;
				}
			}
		}
		// Charactere des Master Clients (Server) checken
		if(ServerPlayerHasValidCharacter())
		{
			if(GetPlayerCharacter("0") == characterPrefabName)
			{
				return true;
			}
		}
		return false;
	}

	public string GetPlayerCharacter(string playerId)
	{
		//DontDestroyOnLoad 
		string temp = characterDictionary[GetPlayerPrefsKey(playerId)];
		//characterDictionary.TryGetValue(GetPlayerPrefsKey(playerId), out temp);
		return temp;
		//return 
		//return PlayerPrefs.GetString(GetPlayerPrefsKey(playerId));
	}

//	public GUITexture GetPlayerGUITexture(string playerID)
//	{
//		if(playerID == "0")
//		{
//			// Server
//			return player0GUITexture;
//		}
//		else if(playerID == "1")
//		{
//			return player1GUITexture;
//		}
//		else if(playerID == "2")
//		{
//			return player2GUITexture;
//		}
//		else if(playerID == "3")
//		{
//			return player3GUITexture;
//		}
//		else
//			return null;
//	}

	public void SetAllPlayerSprites(NetworkPlayer target)
	{
		// Server (Master Client)
		string playerID = "0"; 
		if(ServerPlayerHasValidCharacter())
			networkView.RPC("SetPlayerSprite", target, playerID, GetPlayerCharacter(playerID));
		else
			networkView.RPC("SetPlayerSprite", target, playerID, noCharacter);

		// Clients
		foreach(NetworkPlayer player in Network.connections)
		{
			playerID = player.ToString();
			if(PlayerHasValidCharacter(playerID))
			{
				networkView.RPC("SetPlayerSprite", target, playerID, GetPlayerCharacter(playerID));
			}
			else
				networkView.RPC("SetPlayerSprite", target, playerID, noCharacter);
		}
	}

	public void SetAllPlayerSprites(RPCMode target)
	{
		// Server (Master Client)
		string playerID = "0"; 
		if(ServerPlayerHasValidCharacter())
			networkView.RPC("SetPlayerSprite", target, playerID, GetPlayerCharacter(playerID));
		else
			networkView.RPC("SetPlayerSprite", target, playerID, noCharacter);
		// Clients
		foreach(NetworkPlayer player in Network.connections)
		{
			playerID = player.ToString();
			if(PlayerHasValidCharacter(playerID))
			{
				networkView.RPC("SetPlayerSprite", target, playerID, GetPlayerCharacter(playerID));
			}
			else
				networkView.RPC("SetPlayerSprite", target, playerID, noCharacter);
		}
	}

	[RPC]
	public void SetPlayerSprite(string playerID, string characterPrefabName)
	{
		GameObject character;
		Sprite characterSprite;
		if(characterPrefabName == noCharacter)
		{
			characterSprite = null;
		}
		else
		{
			character = GameObject.Find(characterPrefabName);
			if(character == null)
				return;
			characterSprite = character.GetComponent<SpriteRenderer>().sprite;
			if(characterSprite == null)
				return;
		}
		SpriteRenderer targetSpriteRenderer = GetPlayerSpriteRenderer(playerID);
		if(targetSpriteRenderer != null)
		{
			targetSpriteRenderer.sprite = characterSprite;
		}
		else
		{
			Debug.LogError("LobbyCharacterManager, Player " + playerID + " hat kein SpriteRenderer"); 
		}
	}

	public SpriteRenderer GetPlayerSpriteRenderer(string playerID)
	{
		if(playerID == "0")
		{
			// Server
			return player0SpriteRenderer;
		}
		else if(playerID == "1")
		{
			return player1SpriteRenderer;
		}
		else if(playerID == "2")
		{
			return player2SpriteRenderer;
		}
		else if(playerID == "3")
		{
			return player3SpriteRenderer;
		}
		else
			return null;
	}

	/**
	 * Client / Server Funktion
	 **/
	public string GetSelectedCharacterName(Vector3 clickedPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(clickedPosition);		
		Vector2 origin = ray.origin;										// startPoint
		Vector2 direction = ray.direction;									// direction
		float distance = 100f;
		RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance);
		if(hit.collider != null)
		{
			if(hit.collider.name != "Platform")
			{
				if(hit.collider.name == "Feet" ||
				   hit.collider.name == "Head")								// Layer Hash
				{
					// Kopf oder Füße getroffen -> Parent GameObject Name
					Debug.Log("LobbyCharacterManager, Head/Feet Selected Character:" + hit.collider.transform.parent.name );
					return hit.collider.transform.parent.name;
				}
				else
				{
					Debug.Log("LobbyCharacterManager, Selected Character: " + hit.collider.name);
					return hit.collider.name;
				}

			}
		}
		return null;
	}


//	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
//		string player0Text;
//		string player1Text;
//		string player2Text;
//		string player3Text;
//
//		string player0PrefabName;
//		string player1PrefabName;
//		string player2PrefabName;
//		string player3PrefabName;
//
//		if (stream.isWriting) {
//			player0Text = this.player0GUIText.text;
//			player1Text = this.player1GUIText.text;
//			player2Text = this.player2GUIText.text;
//			player3Text = this.player3GUIText.text;
//
//			stream.Serialize(ref player0Text);
//			stream.Serialize(ref player1Text);
//			stream.Serialize(ref player2Text);
//			stream.Serialize(ref player3Text);
//		} else {
//			player0Text = "";
//			player1Text = "";
//			player2Text = "";
//			player3Text = "";
//			stream.Serialize(ref player0Text);
//			stream.Serialize(ref player1Text);
//			stream.Serialize(ref player2Text);
//			stream.Serialize(ref player3Text);
//
//			this.player0GUIText.text = player0Text;
//			this.player1GUIText.text = player1Text;
//			this.player2GUIText.text = player2Text;
//			this.player3GUIText.text = player3Text;
//		}
//	}
}
