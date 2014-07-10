using UnityEngine;
using System.Collections;

/**
 * xManager kommt an alle GameController
 **/

public class GameManager : MonoBehaviour {

	public static string resourcesPathLan = "PlayerCharacter/UnityNetwork Lan RigidBody2D/";
	public static string resourcesPathLocal = "PlayerCharacter/local/";

	public static string gameSlotsCountPlayerPrefsString = "gameSlotsCount";
	public static string noCharacter = "noCharacter";

	public SpriteRenderer player0SpriteRenderer;
	public SpriteRenderer player1SpriteRenderer;
	public SpriteRenderer player2SpriteRenderer;
	public SpriteRenderer player3SpriteRenderer;
	public GUIText player0GUIText;
	public GUIText player1GUIText;
	public GUIText player2GUIText;
	public GUIText player3GUIText;


//	static public GameObjectDictionary gameObjectDictionary;
//	static public GameObjectDictionary playerCharacterGameObjectDictionary;

	/**
	 * GameSlots, Scenename, GameMode, AISlots...
	 **/
	static public GamePrefs gamePrefs;

	/**
	 * Auswahl der Character in Lobby Scene
	 **/
	static public SelectedCharacterPrefabDictionary playerSelectedCharacterPrefabDictionary;

	/**
	 * Instanzierte CharacterPrefab GameObjects in  GameScene
	 **/
	static public GameObjectsPlayerDictionary playerDictionary;


	/**
	 * Initialisieren von ScriptableObjects, wie zB. letzte SinglePlayer Characterauswahl aus PlayerPrefs laden
	 **/
	private bool initValues = false;


	/**
	 * Einträge löschen
	 **/
	void Reset()
	{
		if(Application.loadedLevelName != "sp_CharacterSelection" &&
		   Application.loadedLevelName != "mp_CharacterSelection" &&
		   Application.loadedLevelName != "pun_CharacterSelection")
		{
			return;
		}
		if(playerSelectedCharacterPrefabDictionary != null)
			playerSelectedCharacterPrefabDictionary.RemoveAll();
		if(playerDictionary != null)
		playerDictionary.RemoveAll();
	}

	void Awake()
	{
		Reset();
		if(gamePrefs == null)
		{
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			gamePrefs = (GamePrefs) ScriptableObject.CreateInstance(typeof(GamePrefs));
			Debug.Log("ScriptableObject gamePrefs erzeugt");
		}
		if(initValues)
		{
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
			int slots = PlayerPrefs.GetInt(gameSlotsCountPlayerPrefsString);
			Debug.Log("PlayerPrefs: " + slots + " (" + gameSlotsCountPlayerPrefsString + ")");
			if(slots <= 0)
				slots = 4;										// vertraue keinem Userinput!
			setNumberOfGameSlots(slots);
		}

		if(playerSelectedCharacterPrefabDictionary == null)
		{
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			playerSelectedCharacterPrefabDictionary = (SelectedCharacterPrefabDictionary) ScriptableObject.CreateInstance(typeof(SelectedCharacterPrefabDictionary));
			Debug.Log("ScriptableObject playerSelectedCharacterPrefabDictionary erzeugt");
		}
		if(initValues)
		{
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
		}

		if(playerDictionary == null)
		{
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			playerDictionary = (GameObjectsPlayerDictionary) ScriptableObject.CreateInstance(typeof(GameObjectsPlayerDictionary));
			Debug.Log("ScriptableObject GameObjectsPlayerDictionary erzeugt");
		}
		if(initValues)
		{
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
//			for(int i=0; i<serverslots; i++)
//			{
//				playerDictonary.SetGameObject(""+i,null);
//			}
		}

		initLayout();
	}

	void initLayout()
	{
		if(Screen.dpi != 0)
		{
			minButtonHeight = 20f * Screen.height / Screen.dpi;
		}
		else
			minButtonHeight = 20f;

		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;


	}

	private float minButtonHeight;
	private bool levelloading = false;

	// ArgumentException: You can only call GUI functions from inside OnGUI.
	void OnGUI()
	{
		if(Application.loadedLevelName != "sp_CharacterSelection" &&
		   Application.loadedLevelName != "mp_CharacterSelection" &&
		   Application.loadedLevelName != "pun_CharacterSelection")
		{
			return;
		}
//		if(levelloading)
//			return;
		if(networkView == null)
		{
			// Local
			if(PlayerAndBotsHaveValidCharacter())
			{
				if(GUILayout.Button( "Start Game", GUILayout.Width( 100f ), GUILayout.Height (minButtonHeight) ))
				{
					levelloading = true;
					Application.LoadLevel("sp_classic_selected_character");
				}
			}
		}
		else
		{
			// Multiplayer
			if(Network.isServer)
			{
				if(EveryPlayerHasValidCharacter())
				{
					if(GUILayout.Button( "Start Game", GUILayout.Width( 100f ), GUILayout.Height (minButtonHeight) ))
					{
						DebugListAllPlayer();
						networkView.RPC("StartGame", RPCMode.All, "mp_classic_selected_character");
					}
				}
			}
		}
	}

	void DebugListAllPlayer()
	{
		string playerCharacterName = GetPlayerCharacter("0");
		Debug.LogWarning("Player " + "0" + " Character Prefab Name: " + playerCharacterName);
		
		foreach(NetworkPlayer player in Network.connections)
		{
			playerCharacterName = GetPlayerCharacter(player.ToString());
			Debug.LogWarning("Player " + player.ToString() + " Character Prefab Name: " + playerCharacterName);
		}
	}
	
	[RPC]
	void StartGame(string sceneName)
	{
		// starte das gewünschte Level auf allen Clients
		// Hash / static string scenename/levelname
		NetworkLevelLoader.Instance.LoadLevel(sceneName,0);
	}
	
	void Update()
	{
//		if(gameObjectDictionary != null) {
//			this.coin = gameObjectDictionary.GetItems("coin");
//		}
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
			if(hit.collider.tag == Tags.character)
			{
				Debug.Log("GameManager, Selected Character: " + hit.collider.transform.parent.name);

				// Name des getroffenen GameObject's zurückgeben
				return hit.collider.transform.parent.name;
			}
// Head und Feet gehen nicht, da auch die neu instantierten GameObject diese Eigenschaften haben!
//			else if(hit.collider.tag == Tags.feet ||
//			        hit.collider.tag == Tags.head)
//			{
//				// Kopf oder Füße getroffen -> Parent GameObject Name
//				Debug.Log("LobbyCharacterManager, Head/Feet Selected Character:" + hit.collider.transform.parent.name );
//				return hit.collider.transform.parent.name;
//			}
			else 
			{
				// nothing spawnable hitted
			}
		}
		return null;
	}


	/**
	 * Class GamePrefs
	 **/

	public void setNumberOfGameSlots(int slots)
	{
		gamePrefs.SetGameSlots(slots);
	}
	
	public int getNumberOfGameSlots()
	{
		Debug.Log("GameSlots " + gamePrefs.GetGameSlots());
		return gamePrefs.GetGameSlots();
	}
	
	public int getNumberOfTeams()
	{
		Debug.Log("gameTeamsCount " + gamePrefs.GetNumberOfTeams());
		return gamePrefs.GetNumberOfTeams();
	}

	/**
	 * Clas SelectedCharacterPrefabDictionary
	 * 
	 * SetCharacter(string, string)
	 * string GetCharacter(string)
	 * bool CharacterInUse(string)
	 **/

	public void SetPlayerCharacter(string playerId, string characterPrefabName)
	{
		playerSelectedCharacterPrefabDictionary.Add(playerId, characterPrefabName);
		Debug.Log("GameManager: Key: " + playerId + " mit Value: " + playerSelectedCharacterPrefabDictionary.Get(playerId) + " in Dictionary eingetragen!");
	}

	public string GetPlayerCharacter(string playerId)
	{
		return playerSelectedCharacterPrefabDictionary.Get(playerId);
	}

	public void RemovePlayerCharacter( string playerId )
	{
		playerSelectedCharacterPrefabDictionary.Remove(playerId);
	}

	public void RemoveAllPlayerCharacter()
	{
		playerSelectedCharacterPrefabDictionary.RemoveAll();
	}

	public bool PlayerHasValidCharacter( string playerId )
	{
		string playerPrefabName = GetPlayerCharacter(playerId);
		if(playerPrefabName != null)
		{
			// Dictionary hat einen Eintrag, Spieler hat Character gewählt.
			// Eintrag validieren 	(Filesystem nach CharacterPrefab durchsuchen
			//						oder Szene nach GameObject mit Dictionaryeigenschaften suchen
			if(GameObject.Find(playerPrefabName) != null)
			{
				// GameObject mit passendem Namen in aktueller Szene gefunden!!!
				return true;
			}
			else
			{
				// in aktueller Szene kein GameObject mit passendem Namen gefunden!!!
				Debug.LogError("Achtung! falscher PrefabName im Dictionary!!!!");
				Debug.LogError(playerId + " " + playerPrefabName);
//				DeleteInvalidCharacter(playerId);
				return false;
			}
		}
		else
		{
			// kein Eintrag im Dicionary, Spieler hat noch kein Character gewählt!!
			return false;
		}
	}
	private void DeleteInvalidCharacter(string playerId)
	{
		Debug.LogError("Automatische Löschung falscher Character Zuordnung");
		RemovePlayerCharacter(playerId);
	}

	/**
	 * MultiplayerMode == UnityNetwork
	 **/
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

	/**
	 * MulitplayerMode == OfflineBots
	 **/
	public bool PlayerAndBotsHaveValidCharacter()
	{
		for(int i=0; i < gamePrefs.GetGameSlots(); i++)
		{
			if(!PlayerHasValidCharacter(""+i))
			{
				return false;
			}
		}
		return true;
	}
	/**
	 * MulitplayerMode == OfflineBots
	 **/
	public bool CheckPrefabInUseSinglePlayer(string characterPrefabName)
	{
		for(int i=0; i < 4; i++)
		{
			if(PlayerHasValidCharacter(""+i))
			{
				if(GetPlayerCharacter(""+i) == characterPrefabName)
				{
					return true;
				}
			}
		}
		return false;
	}

	/**
	 * MultiplayerMode == UnityNetwork
	 **/
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
	 * MulitplayerMode == UnityNetwork
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
			Debug.LogError("GameManager, Player " + playerID + " hat kein SpriteRenderer"); 
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


	static public void SetupCharacterGameObjectLAN(GameObject currentCharacter)
	{
		currentCharacter.transform.Find("CharacterSelectionArea").gameObject.SetActive(false);
	}

}
