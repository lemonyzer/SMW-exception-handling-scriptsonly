using UnityEngine;
using System.Collections;

/**
 * xManager kommt an alle GameController
 **/

public class GameManager : MonoBehaviour {

	public static string gameSlotsCountPlayerPrefsString = "gameSlotsCount";

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
	static public GameObjectsPlayerDictionary playerDictonary;


	/**
	 * Initialisieren von ScriptableObjects, wie zB. letzte SinglePlayer Characterauswahl aus PlayerPrefs laden
	 **/
	private bool initValues = false;

	void Awake()
	{
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

		if(playerDictonary == null)
		{
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			playerDictonary = (GameObjectsPlayerDictionary) ScriptableObject.CreateInstance(typeof(GameObjectsPlayerDictionary));
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
		Debug.Log("GameSlots" + gamePrefs.GetGameSlots());
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
				DeleteInvalidCharacter(playerId);
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
	
}
