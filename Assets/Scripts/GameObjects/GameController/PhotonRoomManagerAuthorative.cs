using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Photon GUIDE Tutorial 3: SpawnScript

public class PhotonRoomManagerAuthorative : Photon.MonoBehaviour {

	public static string resourcesPath = "PlayerCharacter/Photon/";
	public static string gameSlotsCountPlayerPrefsString = "gameSlotsCount";
	public static string noCharacter = "noCharacter";

//	Dictionary<GameObject, Player> localPlayerDictionary;
	
	Vector3 spawnPosition = new Vector3(0,0,0);
	
	// Prefab des CharacterSelectors (benötigt PhotonView)
	public GameObject photonCharacterSelectorPrefab;

// Brain

	bool masterClientSwitched = false;

	/**
	 * GameSlots, Scenename, GameMode, AISlots...
	 **/
	static public GamePrefs gamePrefs;
	
	/**
	 *  CharacterPrefab GameObjects in  GameScene
	 **/
	PlayerDictionary syncedLocalPersistentPlayerDictionary = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary;


	public GameState.States currentGameSate = GameState.currentState;
	
	/**
	 * Initialisieren von ScriptableObjects, wie zB. letzte SinglePlayer Characterauswahl aus PlayerPrefs laden
	 **/
	private bool initValues = false;

	/**
	 * PhotonGameManager is also Manager of PlayerDictionary
	 **/
	void AwakeScriptableObjects()
	{
		if (gamePrefs == null) {
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			gamePrefs = (GamePrefs)ScriptableObject.CreateInstance (typeof(GamePrefs));
			Debug.Log ("ScriptableObject gamePrefs erzeugt");
		}
		if (initValues) {
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
			int slots = PlayerPrefs.GetInt (gameSlotsCountPlayerPrefsString);
			Debug.Log ("PlayerPrefs: " + slots + " (" + gameSlotsCountPlayerPrefsString + ")");
			if (slots <= 0)
				slots = 4;										// vertraue keinem Userinput!
			setNumberOfGameSlots (slots);
		}
		
		if (syncedLocalPersistentPlayerDictionary == null) {
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			syncedLocalPersistentPlayerDictionary = (PlayerDictionary)ScriptableObject.CreateInstance (typeof(PlayerDictionary));
			Debug.Log ("ScriptableObject GameObjectsPlayerDictionary erzeugt");
		}
		if (initValues) {
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
			//			for(int i=0; i<serverslots; i++)
			//			{
			//				playerDictonary.SetGameObject(""+i,null);
			//			}
		}
	}

	/**
	 * Menu zuürck, akteull in auf PhotonCharacterSelector und ConnectToPhoton
	 **/
	void BackButton()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(networkView == null && photonView == null)
			{
				Debug.Log("SinglePlayer");
				Application.LoadLevel( Scenes.mainmenu );
				return;
			}
			
			if(networkView != null)
			{
				// Unity Network disconnect
				
				// if(connected)
				
				if(Network.isServer)
				{
					// Server disconnect
				}
				if(Network.isClient)
				{
					// Client disconnect
				}
			}
			
			if(photonView != null)
			{
				// Photon Network disconnect
				if(PhotonNetwork.inRoom)
				{
					if(PhotonNetwork.isMasterClient)
					{
						// sync some late infos?
						// set new MasterClient
						if(PhotonNetwork.room.playerCount == 1)
						{
							// last player in room, delete everything
							PhotonNetwork.DestroyAll();
						}
						PhotonNetwork.LeaveRoom();
						return;
					}
					else
					{
						// RPC leave?
						PhotonNetwork.LeaveRoom();				
						return;
					}
				}
			}
			
			Application.LoadLevel( Scenes.mainmenu );
			return;
		}
	}

	// Load Lobby Scene
	void OnLeftRoom()
	{
		Application.LoadLevel(Scenes.photonLobby);
	}

	void Update()
	{
		BackButton();
	}

	void Awake()
	{
		InitGUIStyle ();
//		PhotonNetwork.automaticallySyncScene = false;
//		localPlayerDictionary = new Dictionary<GameObject, Player> ();
		Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
//		AwakeScriptableObjects ();
//		playerList = new List<Player> ();


		if(PhotonNetwork.isMessageQueueRunning)
		{
			Debug.LogError("MessageQueue is running, NOT ALLOWED AT THIS MOMENT!!!");
		}
		else
		{
			Debug.Log("MessageQueue is queuing!!!");
		}

		// wichtig, vor kommunications beginn alten MasterClient = aktueller Master Client setzen.
		oldMasterClient = PhotonNetwork.masterClient;
		Debug.Log("Awake: oldMasterClient = " + oldMasterClient.name);

		// Kommunikationsbeginn/fortzsetzung (Buffered RPC's werden abgearbeitet)
		PhotonNetwork.isMessageQueueRunning = true;
	}


//	void Start()
//	{
//		PhotonNetwork.autoCleanUpPlayerObjects = true;
//		// removes 
//	}

	/**
	 * Class GamePrefs
	 **/
	
	public void setNumberOfGameSlots(int slots)
	{
//		gamePrefs.SetGameSlots(slots);
	}
	
	public int getNumberOfGameSlots()
	{
		return 4;
//		Debug.Log("GameSlots " + gamePrefs.GetGameSlots());
//		return gamePrefs.GetGameSlots();
	}
	
	public int getNumberOfTeams()
	{
		return 4;
//		Debug.Log("gameTeamsCount " + gamePrefs.GetNumberOfTeams());
//		return gamePrefs.GetNumberOfTeams();
	}

	void OnJoinedLobby()
	{
		Debug.LogWarning("LobbyScreen: OnJoinedLobby()");
	}

	void OnConnectedToMaster ()
	{
		Debug.LogWarning("LobbyScreen: OnConnectedToMaster()");
	}

	// Room wird nur von MasterClient erstellt
	void OnCreatedRoom()
	{
		Debug.LogWarning("PhotonGameManagerAuthorative: OnCreatedRoom()");
		//Spawn a player for the server itself
//		SpawnAuthorativeCharacterSelector(PhotonNetwork.player);			// CharacterSelector
	}

	/**
	 * OnJoinedRoom() wird nur auf aktuellem Client ausgeführt (müsste heißen OnMeJoinedRoom)
	 * 
	 * wenn dieser Client den Raume erstellt hat und...
	 * 
	 * a)	dabei in OnRoomCreated PhotonNetwork.LoadLevel startet ( isMessageQueueRunning = false )
	 * 
	 * oder...
	 * 
	 * b)	eine vergleichbare ALTERNATIVE Funktion (keine CoRoutine!!!) in OnRoomCreated()
	 * 		ausführt (isMessageQueueRunning = false muss darin gesetzt werden)!!
	 * 		und isMessageQueueRunning = true darf erst in der Scene aktiviert werden in der dieses Script als Componente eines
	 * 		aktiven GameObject arbeitet.
	 * 
	 * ODER... dieser Client den Raum durch PhotonNetwork.automaticallySyncScene = true connected hat
	 * 		   bei false, wird OnJoined() noch in der LobbyScene ausgeführt
	 * ------------------------------------------------------------------------------------------------------------------------------------------------
	 * ACHTUNG!!!:
	 * Wenn Client in Lobby (ohne autoSyncScene) einen Raumauswählt, wird er auch in der
	 * LobbyScene noch gejoined! OnJoinedRoom() findet dann in LobbyScene statt.
	 * Entweder... ist die ROOMScene immer konstant photonRoom dann kann Client LoadNextLevel(photonRoom)
	 * oder... MasterClient sendet in OnPhotonPlayerConnected(PhotonPlayer newPlayer) RPC mit dem zu ladenden Level <--- dynamischer == BESSER ?! ;)
	 * ------------------------------------------------------------------------------------------------------------------------------------------------
	 * ALTERNATIVE Funktion:
	 *  
	 *  void OnRoomCreated()
	 *  {
	 * 		// ggf. Scenenauswahl (Votes) auswerten
	 * 		LoadNextLevel(mostVotedLevel);
	 * 		// code wird nicht ausgeführt!
	 *  }
	 * 
	 * 	void LoadNextLevel(string scenename)
	 * 	{
	 * 		PhotonNetwork.isMessageQueueRunning = false;
	 * 		Application.LoadLevel(scenename);
	 * 	}
	 * 
	 *  [ScriptComponente in folgendem Level (geladene Scene) (aktives GameObject)]
	 *  void Awake()
	 *  {
	 * 		// ...
	 *  	// wichtiger Code bevor empfangene RPC's abgearbeitet werden
	 * 		// ...
	 * 		
	 * 		// zB.: alle GameObject referenzen aus Dictionary löschen, da diese den
	 * 		// Scenenwechsel (kein DontDestroyOnLoad) nicht überlebt haben und nun nicht mehr existieren
	 * 		syncedLocalPersistentPlayerDictionary.RemoveAllPlayerCharacterGameObjects();
	 * 
	 * 		PhotonNetwork.isMessageQueueRunning = true;
	 *  }
	 **/
	void OnJoinedRoom()
	{
		//aktueller Ablauf: nur MasterClient führt diese Methode im PhotonRoomManagerAuthorative aus
//		if (PhotonNetwork.isMasterClient)
//		{
//			SpawnAuthorativeCharacterSelector(PhotonNetwork.player);			// Authorative CharacterSelector des MasterClients
//		}
	}
	

	void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Debug.LogWarning("PhotonGameManagerAuthorative: OnPhotonPlayerConnected()");

		//wird für MasterClient nicht ausgeführt!


		// OnPhotonPlayerConnected event is triggered on every Client!
		if (PhotonNetwork.isMasterClient)
		{
			//A player connected to me(the MasterClient), spawn a player for it:
			// Authorative
			// Called on the MasterClient only, MasterClient instantiated GameObject and tells ControlsOwner to all Clients
//			SpawnAuthorativeCharacterSelector(newPlayer);

			// Check if PhotonPlayer who connected is other than me (MasterClient)
			// newPlayer != localPlayer (MasterClient)
			if(newPlayer != PhotonNetwork.player)
			{
				// sync Dictionary
//				SendCompletePlayerDictionary(newPlayer);			// sync with BUFFERED, need to check if PhotonPlayer still in room?
			}
		}
	}

	/**
	 * sync Dictionary
	 * new Client in room gets Complete Dictionary from MasterClient
	 **/
	void SendCompletePlayerDictionary(PhotonPlayer target)
	{
		string characterPrefabFilename = null;
		GameObject characterGameObject = null;
		PhotonView characterGameObjectPhotonView = null;
		int viewID = 0;

		foreach(PhotonPlayer currentPhotonPlayer in PhotonNetwork.playerList)
		{
			viewID = 0;
			characterPrefabFilename = syncedLocalPersistentPlayerDictionary.TryGetCharacterPrefabFilename(currentPhotonPlayer);
			if(string.IsNullOrEmpty(characterPrefabFilename))
				return;

			// only sync, if PhotonPlayer has Character

			characterGameObject = syncedLocalPersistentPlayerDictionary.TryGetCharacterGameObject(currentPhotonPlayer);
			characterGameObjectPhotonView = characterGameObject.GetComponent<PhotonView>();
			if(characterGameObjectPhotonView != null)
			{	
				viewID = characterGameObjectPhotonView.viewID;
				if(viewID != 0)
				{
					photonView.RPC("SyncCurrentPlayer", target, currentPhotonPlayer, characterPrefabFilename, viewID);
				}
				else
				{
					Debug.LogError("PhotonView.ViewID von " + characterGameObject.name + " == 0!!!");
				}
			}
			else
			{
				Debug.LogError("CharacterGameObject " + characterGameObject.name + " besitzt kein PhotonView!!!");
			}
		}
	}

	/**
	 * sync Dictionary
	 * new Client in room gets Complete Dictionary from MasterClient
	 * 
	 * viewID ist entweder von ChraracterSelector oder von CharacerGameObject!
	 * nachtrag, viewID kann nur von ChracterGameObject sein, MasterClient übertragt nur wenn Client character besitzt!!!
	 **/
	[RPC]
	void SyncCurrentPlayer(PhotonPlayer syncedPhotonPlayer, string prefabFilename, int viewID)
	{
		Debug.LogWarning("RPC SyncCurrentPlayer" + syncedPhotonPlayer.name + "'s GameObject: " + prefabFilename + " viewID: " + viewID);
		if (PhotonNetwork.isMasterClient)		// MasterClient nicht syncen!
			return;
		PhotonView view = PhotonView.Find (viewID); 
		GameObject characterGameObject = null;
		if(view != null)
			characterGameObject = view.gameObject;
		else
		{
			characterGameObject = null;
			Debug.Log ("Sync Problem: kein GameObject mit ViewID " + viewID + " gefunden!");
		}
		Character syncedCharacter;
		Player syncedPlayer;

		if(characterGameObject == null)
		{
			Debug.LogError("syncCurrentPlayer failed, no characterGameObject with correct ViewID " + viewID + " found!");
		}

		if(string.IsNullOrEmpty(prefabFilename))
		{
			Debug.LogError("syncCurrentPlayer failed, no character prefab filename received!");
		}
		syncedCharacter = new Character (prefabFilename, characterGameObject, false);
		syncedPlayer = new Player (syncedPhotonPlayer, syncedCharacter);
		syncedLocalPersistentPlayerDictionary.AddPlayer (syncedPhotonPlayer, syncedPlayer);
	}

	public void SpawnAuthorativeCharacterSelector(PhotonPlayer realOwner)
	{
		// Called on the MasterClient only! (Authorative Mode)

		if(photonCharacterSelectorPrefab == null)
		{
			Debug.LogError("no CharacterSelector prefab setted!!! fix it in editor!!!");
			return;
		}
		// Instantiate a new object for this player, remember; the server is therefore the owner.
		GameObject playerCharacterSelectorGameObject = PhotonNetwork.Instantiate(photonCharacterSelectorPrefab.name, transform.position, transform.rotation, 0) as GameObject;

		// Get the networkview of this new transform
		PhotonView newObjectsview = playerCharacterSelectorGameObject.GetComponent<PhotonView>();

		// Tell all Clients new Client joined Room
		photonView.RPC ("AddCharacterSelectorToPlayerDictionary", PhotonTargets.AllBuffered, realOwner, newObjectsview.viewID);

/*interessant->*/		
		// Call an RPC on this new PhotonView, set the PhotonPlayer who controls this new player
		newObjectsview.RPC("SetCharacterSelectorOwner", PhotonTargets.AllBuffered, realOwner); // Tell every Client who is the real Owner of this CharacterSelector
/*<-interessant*/
	}

	[RPC]
	void AddCharacterSelectorToPlayerDictionary(PhotonPlayer realOwner, int characterSelectorGameObjectViewID)
	{
		if(realOwner == null)
		{
			Debug.LogWarning("tried syncing diconnected player from buffer (masterclient/server), STOP!");
			return;
		}
		Debug.LogWarning("RPC AddCharacterSelectorToPlayerDictionary" + realOwner.name + "'s CharacterSelector: " +characterSelectorGameObjectViewID);

		bool stillConnected = false;
		foreach(PhotonPlayer player in PhotonNetwork.playerList)
		{
			if(realOwner == player)
			{
				Debug.Log(realOwner.name + " is still connected!");
				stillConnected = true;
			}
		}
		if(stillConnected)
		{
			GameObject characterSelectorGameObject = PhotonView.Find (characterSelectorGameObjectViewID).gameObject;
			if(characterSelectorGameObject != null)
			{
				Player syncedPlayer = new Player (realOwner, characterSelectorGameObject);			// add characterSelectorGameObject to newPlayer
				syncedLocalPersistentPlayerDictionary.AddPlayer(realOwner,syncedPlayer);			// add newPlayer to Dictionary
			}
		}
		else
		{
			Debug.LogWarning(realOwner.name + " is not connected anymore!\n Dont apply buffered Action");
		}
	}



	/**
	 * PhotonPlayer clicked unUsed Character, MasterClient instantiats authorativ characterGameObject with Input Controls by realOwner (PhotonPlayer)
	 * after instantiation, tell every Client who is the realOwner (Controller)
	 **/
	public void SpawnAuthorativePlayerCharacter(PhotonPlayer realOwner, string characterPrefabName, Vector3 spawnPosition)
	{
		// Called on the MasterClient only! (Authorative Mode)

		if(!string.IsNullOrEmpty(characterPrefabName))
		{
			// Instantiate a new object for this player, remember; the server is therefore the owner.
			GameObject playerCharacterGameObject = (GameObject)PhotonNetwork.Instantiate( characterPrefabName, spawnPosition, Quaternion.identity,0 );

//			// Bot AI
//			bool isAI = false;
//			Character newCharacter = new Character(characterPrefabName, playerCharacterGameObject, isAI);

//			// Photon Network
//			// update Dictionary with new Characterinformation
//			Player player = syncedLocalPersistentPlayerDictionary.GetPlayer(realOwner);
//			if(player == null)
//			{
//				// Player was not in Dictionary
//				player = new Player(realOwner, newCharacter);
//				syncedLocalPersistentPlayerDictionary.AddPlayer(realOwner,player);
//				Debug.LogWarning(realOwner.name + " was not in Dictionary, therefore has no CharacterSelector.\n Is added to Dictionary with a Character now! WTF?!");
//			}
//			else
//			{
//				// Player is in Dictionary
//				player.setCharacter(newCharacter);
//				Debug.Log(realOwner.name + " added new Character");
//			}

			// sync Dictionary with MasterClient
			photonView.RPC("UpdateCurrentPlayerCharacter", PhotonTargets.AllBuffered, realOwner, characterPrefabName, playerCharacterGameObject.GetPhotonView().viewID);	// ??? OthersBuffered

			// Get the networkview of this new transform
			PhotonView newObjectsview = playerCharacterGameObject.GetComponent<PhotonView>();

			// Keep track of this new player so we can properly destroy it when required.
			RealOwner playerControlScript = playerCharacterGameObject.GetComponent<RealOwner>();
			playerControlScript.owner = realOwner;

			// Call an RPC on this new PhotonView, set the PhotonPlayer who controls this new player
			newObjectsview.RPC("SetCharacterControlsOwner", PhotonTargets.AllBuffered, realOwner);
			newObjectsview.RPC("DeactivateKinematic", PhotonTargets.AllBuffered);
		}
		else
			Debug.LogError("Empty prefabFilename string: " + characterPrefabName);
	}

	/**
	 * sync Dictionary if PhotonPlayer selects first/new Character
	 **/
	[RPC]
	void UpdateCurrentPlayerCharacter(PhotonPlayer currentPhotonPlayer, string prefabFilename, int characterGameObjectViewID)
	{
//		if(PhotonNetwork.isMasterClient)
//		{
//			Debug.LogWarning("UpdateCurrentPlayerCharacter already done!"); // ??? maybe bad, should be also doin it in this RPC
//			return;
//		}
		Debug.LogWarning("RPC UpdateCurrentPlayerCharacter " + currentPhotonPlayer + ", " + prefabFilename + ", " + characterGameObjectViewID); 
		GameObject characterGameObject = null;
		try {
			characterGameObject = PhotonView.Find (characterGameObjectViewID).gameObject;
		} catch (System.Exception e)
		{
			characterGameObject = null;
			Debug.LogError("PhotonView.Find( " + characterGameObjectViewID + " ) returns NULL!");
			Debug.LogException(e);
		}
		Character character = null;
		if(characterGameObject == null)
		{
			Debug.LogError("Kein GameObject mit passender ViewID gefunden: " + characterGameObjectViewID);
		}
		else
		{
			// found CharacterGameObject with correct ViewID
			if(!string.IsNullOrEmpty(prefabFilename))
				character = new Character(prefabFilename, characterGameObject, false);
			else
				Debug.LogError("prefabFilename is empty -> character = null !!!");
		}

		if(characterGameObject == null || character == null)
		{
			return;				// ?????????????????????????????????????????? konsistenz ?!! only add if all infos found!
		}

		Player player = syncedLocalPersistentPlayerDictionary.GetPlayer(currentPhotonPlayer);
		if(player == null)
		{
			// player was not in Dictionary
			player = new Player(currentPhotonPlayer, character);
			syncedLocalPersistentPlayerDictionary.AddPlayer(currentPhotonPlayer, player);
		}
		else
		{
			// player found in dictionary
			player.setCharacter(character);
		}
	}


//	void Spawnplayer(PhotonPlayer newPlayer)
//	{
//		// Authorative
//		// Called on the MasterClient only
//
//		// Instantiate a new object for this player, remember; the server is therefore the owner.
//		// Transform myNewTransPhotonNetwork.Instantiate...
//
//
//		// Naive
//		// Called on the Client itself in OnJoinedRoom event
//
//		//Instantiate a new object for this player, remember; the server is therefore the owner.
//		//Transform myNewTrans = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, transform.rotation, 0).transform;
//	}

	PhotonPlayer oldMasterClient;

	/**
	 * sync Dictionary and MasterClient Destroys current GameObjects "owned" by disconnecting PhotonPlayer
	 **/
	void OnPhotonPlayerDisconnected(PhotonPlayer disconnectedPhotonPlayer)
	{
		Debug.Log(disconnectedPhotonPlayer.name + " (disconnected)");
		if(masterClientSwitched)
		{
			masterClientSwitched = false;

			Debug.Log(oldMasterClient.name + " (old MasterClient)");
			Debug.Log(PhotonNetwork.masterClient  + " (current/new MasterClient)");
			oldMasterClient = PhotonNetwork.masterClient;		// oldMasterClient muss nach View ID Allocation gesetzt werden,
																// da bei Allocation alter MasterClients ausgelassen werden
		}


		// wenn MasterClient disconnected kann er keinem mehr sagen das er gegangen ist!
		// neuer MasterClient muss alten verabschieden, sync Dictionary!!!

		if(PhotonNetwork.isMasterClient)
		{
			// Current Player is MasterClient

			// remove Player Objects
			RemovePlayer(disconnectedPhotonPlayer);
			
			// remove Player from Dictionary
			RemoveFromPlayerDictionary(disconnectedPhotonPlayer);
		}
		else
		{
			// Current Player is normal Client

			// remove disconnected Player from local Dictionary
			RemoveFromPlayerDictionary(disconnectedPhotonPlayer);
		}
	}
	
	/**
	 * Client disconnects, sync Dictionary
	 * MasterClient Destroys GameObject controlled by disconnecting Client
	 **/

	void RemoveFromPlayerDictionary(PhotonPlayer photonPlayer)
	{
		Debug.Log("LOCAL RemoveFromPlayerDictionary " + photonPlayer); 
		if(photonPlayer == null)
		{
			Debug.LogError("photonPlayer == null!");
			Debug.LogError("cant remove from Dictionary!!");
			return;
		}
		syncedLocalPersistentPlayerDictionary.RemovePlayer(photonPlayer);
	}


	void RemovePlayer(PhotonPlayer networkPlayer)
	{
		Debug.LogWarning("RemovePlayer " + networkPlayer.name);
		if (PhotonNetwork.isMasterClient)
		{
			GameObject photonPlayerCharacter = syncedLocalPersistentPlayerDictionary.TryGetCharacterGameObject(networkPlayer);
			GameObject characterSelector = syncedLocalPersistentPlayerDictionary.TryGetCharacterSelectorGameObject(networkPlayer);

			if(photonPlayerCharacter != null)
			{
				PhotonNetwork.Destroy(photonPlayerCharacter);
				Debug.Log("MasterClient: " + photonPlayerCharacter.name + " destroyed!");
			}
			else
			{
				Debug.LogWarning(networkPlayer.name + " had no CharacterGameObject!");
			}
			if(characterSelector != null)
			{
				PhotonNetwork.Destroy(characterSelector);
				Debug.Log("MasterClient: " + characterSelector.name + " destroyed!");
			}
			else
			{
				Debug.LogWarning(networkPlayer.name + " had no CharacterSelectorGameObject!");
			}
		}
	}

	/**
	 * hopfully next MasterClient has synced playerDictionary
	 * foreach GameObject with PhotonView Allocate new viewID
	 * 
	 * Life Cycle:
	 * OnMasterClientSwitched();	bool masterclientswitched = true
	 * [RPC] AllocateNewViewIDs
	 * OnPhotonPlayerDisconnected()	bool masterclientswitched = false
	 * RemoveFromPlayerDictionary();
	 **/
	void OnMasterClientSwitched(PhotonPlayer newMaster)
	{
		Debug.Log("OnMasterClientSwitched: Old MasterClient: " + oldMasterClient.name + " ID: " + oldMasterClient.ID) ;
		Debug.Log("OnMasterClientSwitched: New MasterClient: " + newMaster.name + " ID: " + newMaster.ID) ;

		masterClientSwitched = true;

		// PhotonNetwork.MasterClient is already newMaster!

		if(PhotonNetwork.player == newMaster)
		{
			// this Client is new Master
//			photonView.RPC ("RemoveFromPlayerDictionary", PhotonTargets.AllBuffered, oldMasterClient); // complete Buffered Actions doesn't work, PhotonPlayer oldMasterClient == null!!

			// Achtung: in PhotonNetwork.playerList ist alter masterclient nicht mehr enthalten!!!

			// alle PlayerCharacter/Selector GameObjects bekommen neue ViewIDs
			// alle außer alter MasterClient (ist disconnected)
			foreach(PhotonPlayer currentPhotonPlayer in PhotonNetwork.playerList)
			{
				if(currentPhotonPlayer == oldMasterClient)
				{
					// dont update oldMasterClient GameObjects
					Debug.Log("dont update ViewIDs of oldMasterClient GameObjects");		// works! BUT new MasterClient needs to be owner of the PhotonView to destroy iT!

					//return; <--- IDIOT
				}
				else
				{
					// other Clients GameObjects are getting updated
					GameObject go;
					int currentViewID = 0;
					int newViewID = 0;

					// CharacterSelector GO
					go = syncedLocalPersistentPlayerDictionary.TryGetCharacterSelectorGameObject(currentPhotonPlayer);
					currentViewID = 0;
					newViewID = 0;
					if(go != null)
					{
						try {
							currentViewID = go.GetComponent<PhotonView>().viewID;
						}catch(System.Exception e)
						{
							Debug.LogException(e);
							currentViewID = 0;
						}
						if(currentViewID != 0)
						{
							newViewID = PhotonNetwork.AllocateViewID();
							photonView.RPC("ApplyNewAllocatedViewID", PhotonTargets.AllBuffered, currentViewID, newViewID);	// Buffered Via Server ???
																																		// MasterClient disconnects, neuer MasterClient
																																		// calls RPC ApplyNewAllocatedViewID
						}
					}
					else
					{
						// no CharacterSelector GameObject found in Dictionary!
						Debug.Log(currentPhotonPlayer.name + " has no CharacterSelector in Dictionary to change viewID");
					}

					// Character GO
					go = syncedLocalPersistentPlayerDictionary.TryGetCharacterGameObject(currentPhotonPlayer);
					currentViewID = 0;
					newViewID = 0;
					if(go != null)
					{
						try {
							currentViewID = go.GetComponent<PhotonView>().viewID;
						}catch(System.Exception e)
						{
							Debug.LogException(e);
							currentViewID = 0;
						}
						if(currentViewID != 0)
						{
							newViewID = PhotonNetwork.AllocateViewID();
							photonView.RPC("ApplyNewAllocatedViewID", PhotonTargets.AllBuffered, currentViewID, newViewID);	// Buffered Via Server ???
						}
					}
					else
					{
						// no Character GameObject found in Dictionary!
						Debug.Log(currentPhotonPlayer.name + " has no CharacterGameObject in Dictionary to change viewID");
					}
				}
			}
		}

		/* We have a design problem in this tutorial: ONLY the masterclient maintains the list of which 
         * PlayerScripts belongs to which PhotonPlayer, thus if the MasterClient leaves we cannot recover this.
         * This problem would be easy enough to solve by having everyone maintain this list. However I wanted to
         * keep this tutorial clear and to the point.
         */
		
		//Abort, abort...
		//PhotonNetwork.Disconnect();
		
		//After disconnection the scene will reload via OnDisconnectedFromPhoton.
	}


	/***
	 * 
	 * viewID design problems!!!!
	 * 
	 * delete current (old) PhotonView
	 * create new PhotonView, set newAllocatedViewID
	 * 
	 ***/
	[RPC]
	void ApplyNewAllocatedViewID(int currentViewID, int newViewID)
	{
		Debug.Log("RPC AllocateNewViewID currentViewID: " + currentViewID + ", newViewID: " + newViewID) ; 
		PhotonView currentPhotonView = null;
		GameObject go = null;
		try {
			currentPhotonView = PhotonView.Find(currentViewID);
			go = currentPhotonView.gameObject;
		} catch(System.Exception e) { 
			currentPhotonView = null;
			go = null;
			Debug.LogError("AllocateNewViewID didn't find GameObject with correct ViewID, GO cleaned UP?");
			Debug.LogException(e);
		}
		if(currentPhotonView != null &&
		   go != null)
		{
			// go with old (current) ViewID found
			// set new allocated ViewID, (runs on all Clients)
			//go.GetComponent<PhotonView>().viewID. = newViewID;		//not allowed, reference is missingin Views-Dictionary
			ViewSynchronization observeoption = currentPhotonView.synchronization;
			Component observedComponent = currentPhotonView.observed;
			if(currentPhotonView != null)
			{
				Debug.LogWarning("locally Destroying PhotonView");
				Destroy(currentPhotonView);
			}
			else
			{
				Debug.LogError("go has no PV, how could it be found, MUST NOT BE HERE???!!!");
			}
			PhotonView newPhotonView = go.AddComponent<PhotonView>();
			Debug.Log("newPV untouched ViewID: " + newPhotonView.viewID);
			newPhotonView.viewID = newViewID;

//			PhotonNetworkRigidbody pnRigidbody = go.GetComponent<PhotonNetworkRigidbody>();

			if(observedComponent != null)
			{
				if(PhotonNetwork.isMasterClient)
				{
					newPhotonView.observed = observedComponent;											// activate observation on new photonView (?? backup and load)
					newPhotonView.synchronization = observeoption;			// activate observation option on new photonView (?? backup and load)
					Debug.LogWarning(go.name + " " + observedComponent.name + " wird " + observeoption.ToString() + " observed!");
				}
				else
				{	
					newPhotonView.observed = observedComponent;
					newPhotonView.synchronization = observeoption;
					Debug.LogWarning(go.name + " " + observedComponent.name + " wird " + observeoption.ToString() + " observed!");
				}
			}
			else
			{
				Debug.LogWarning(go.name + " hatte keine Observed Component! - should be CharacterSelector ?!! ");
			}

			Debug.Log("MasterClientAllocated ViewID: " + newViewID);
			Debug.Log("newPV ViewID: " + newPhotonView.viewID);

			//go.GetComponent<PhotonView>().owner = 
			Debug.Log(go.name + " current ViewId: " +currentViewID + " new ViewID: " + newViewID);
		}
		else
		{
			Debug.LogError("GameObject mit ViewID " + currentViewID + " existiert nicht mehr");
			Debug.LogError("go has no PV, how could it be found, MUST NOT BE HERE???!!!");
		}
	}

	void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton -> MainMenu");
		Application.LoadLevel(Scenes.mainmenu);
	}

	/**
	 * RPC des Servers, (Client fordert Server auf diese Funktion zu starten)
	 * Server checks if Character at clicked Position is already in Use
	 * if not in use rigister character in PlayerPrefs and send 
	 * RPC to Animate selection on all Clients
	 * if in use RPC to requested Client and play characterInUseSound
	 **/
	[RPC]
	void CharacterClicked(string characterPrefabName, PhotonPlayer player)
	{
		// if Funktion started on normal Client (why are we here?!) break!
		if(!PhotonNetwork.isMasterClient)
			return;
		
		string playerClickedID = player.ToString ();
		
		GameObject prefab = GameObject.Find(characterPrefabName);
		if( prefab != null)
		{
			// Prefab (GameObject) in Scene gefunden
			Debug.Log("Server: Prefab " + characterPrefabName + " in aktueller Scene gefunden.");
			
			if(!CheckPrefabInUse(characterPrefabName))
			{
				// kein Spieler hat diesen Character gewählt, Client Character zuteilen und freigabe mitteilen.

				// information in Dictionary / Liste / Array speichern
//				SetPlayerCharacter(playerClickedID, characterPrefabName);	// Register CharacterPrefab with Player in PlayerPref

				// Check if player aready have a Character
				GameObject currentCharacter = syncedLocalPersistentPlayerDictionary.TryGetCharacterGameObject(player);
				if(currentCharacter != null)
				{
					RemoveCurrentCharacterGameObject(currentCharacter);
					photonView.RPC( "RemoveCurrentCharacterFromDictionary", PhotonTargets.All, player );
				}

				// Authorative: PlayerCharacter is instantiated by MasterClient,
				// and CharacterControlsScript is enabled on Owner Client
				SpawnAuthorativePlayerCharacter(player, characterPrefabName, new Vector3(Random.Range(0,10),1,0));

				// Zuteilung allen Clients mitteilen
				photonView.RPC( "AllowSelectedCharacter", PhotonTargets.AllBufferedViaServer, playerClickedID, characterPrefabName );	// RPC geht von Server an alle
			}
			else
			{
				// Character schon in Verwendung
				// anfragendem Client mitteilen (spielt sound ab)
				photonView.RPC( "SelectedCharacterInUse", player );									// RPC geht von Server an requested Client
			}
		}
		else
		{
			// keine Prefab (GameObject) mit passendem Name in Scene gefunden
			Debug.LogError("Server: Prefab " + characterPrefabName + " NICHT in aktueller Scene gefunden.");
			return;																			// RPC abbrechen!
		}
	}

	void RemoveCurrentCharacterGameObject(GameObject go)
	{
		PhotonNetwork.Destroy(go);
	}

	[RPC]
	void RemoveCurrentCharacterFromDictionary(PhotonPlayer photonPlayer)
	{
		Debug.Log("Removing Current Character From Dictionary: " + photonPlayer.name);
		Player player = syncedLocalPersistentPlayerDictionary.GetPlayer(photonPlayer);
		if(player != null)
		{
			player.setCharacter(null);
		}
		else {
			Debug.Log("Player didnt exist in Dictionary: " + photonPlayer.name);
		}
	}

	/**
	 * RPC des Clients, (Server fordert Clients auf diese Funktion zu starten)
	 * Server teilt Clients mit welcher Player einen neuen Character gewählt hat
	 **/
	[RPC]
	void AllowSelectedCharacter(string networkPlayerID, string characterPrefabName, PhotonMessageInfo info)
	{
		Debug.LogWarning("RPC AllowSelectedCharacter");
		// animation
	}

	[RPC]
	void SelectedCharacterInUse()
	{
		Debug.LogWarning("RPC SelectedCharacterInUse");
		// play Sound
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
		foreach(PhotonPlayer photonPlayer in PhotonNetwork.playerList)
		{
			if( syncedLocalPersistentPlayerDictionary.TryGetCharacterPrefabFilename(photonPlayer) == characterPrefabName)
			{
				return true;
			}
		}
		return false;
	}

	public bool PlayerHasValidCharacter( PhotonPlayer photonPlayer )
	{
		string playerPrefabName = syncedLocalPersistentPlayerDictionary.TryGetCharacterPrefabFilename(photonPlayer);
		if(!string.IsNullOrEmpty(playerPrefabName))
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
				Debug.LogError(photonPlayer.name + " " + playerPrefabName);
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
	
//	public void SetPlayerCharacter(PhotonPlayer photonPlayer, string characterPrefabName)
//	{
//		syncedLocalPersistentPlayerDictionary.Add(photonPlayer, characterPrefabName);
//		Debug.Log("GameManager: Key: " + playerId + " mit Value: " + playerSelectedCharacterPrefabDictionary.Get(playerId) + " in Dictionary eingetragen!");
//	}

	float minButtonHeight;
	GUIStyle guiStyle;

	GUIStyle masterStyle;
	GUIStyle masterSmallStyle;

	GUIStyle clientStyle;
	GUIStyle clientSmallStyle;

	GUIStyle myStyle;

	GUIStyle avatarStyle;
	
	void InitGUIStyle()
	{

		if(Screen.dpi != 0)
		{
			minButtonHeight = 20f * Screen.height / Screen.dpi;
		}
		else
		{
			minButtonHeight = 20f;
		}

		myStyle = new GUIStyle ();
		myStyle.fontStyle = FontStyle.Bold;

		guiStyle = new GUIStyle ();
		guiStyle.normal.textColor = Color.black;
		guiStyle.fontSize = 16;

		masterStyle = new GUIStyle();
		masterStyle.normal.textColor = Color.green;
		masterStyle.fontSize = 16;

		masterSmallStyle = new GUIStyle();
		masterSmallStyle.normal.textColor = masterStyle.normal.textColor;

		clientStyle = new GUIStyle();
		clientStyle.normal.textColor = Color.red;
		clientStyle.fontSize = 16;

		clientSmallStyle = new GUIStyle();
		clientSmallStyle.normal.textColor = masterStyle.normal.textColor;

		avatarStyle = new GUIStyle ();
		avatarStyle.fixedWidth = 64f;
		avatarStyle.fixedHeight = 64f;
	}

	bool EveryPlayerHasCharacter()
	{
		foreach(PhotonPlayer player in PhotonNetwork.playerList)
		{
			if(string.IsNullOrEmpty(syncedLocalPersistentPlayerDictionary.TryGetCharacterPrefabFilename(player)) )
			{
				return false;
			}
		}
		return true;
	}

	public bool debugEnabled = false;

	private Rect windowPlayerDictionaryRect = new Rect(20, 25, Screen.width-40, 200);
	private Rect windowPlayerInfoGUIRect = new Rect(10,Screen.height-100f,Screen.width-20,90);

	void OnGUI()
	{
		if(syncedLocalPersistentPlayerDictionary == null)
		{
			return;
		}
		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		if (!PhotonNetwork.inRoom)
			return;
		if (buffer == null)
			return;
		// FullScreen 0,0 = Left,Top

		if(PhotonNetwork.isMasterClient)
		{
			// GameScene kann nur von Master Client gestartet werden

			if(EveryPlayerHasCharacter())
			{
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace();
				if(GUILayout.Button("Start!", GUILayout.MinHeight(minButtonHeight)))
				{
					InitGameScene();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal ();
			}
		}
		if(debugEnabled)
			windowPlayerDictionaryRect = GUI.Window(0, windowPlayerDictionaryRect, WindowPlayerDictionary, "Player Dictionary");
		windowPlayerInfoGUIRect = GUI.Window(1, windowPlayerInfoGUIRect, WindowPlayerInfoGUI, "Players");

	}

	void WindowPlayerInfoGUI(int windowID)
	{
		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		/**
		 * Player Info
		 **/
//		GUILayout.BeginArea (new Rect(0,Screen.height-100f,PhotonNetwork.room.playerCount*200,100));
		GUILayout.BeginHorizontal ();
		// Schleife über Spielerliste
		foreach(Player player in buffer)
		{
			GUILayout.BeginHorizontal (GUILayout.Width(200));
			if(player.getPhotonPlayer() == PhotonNetwork.player)
			{
				// Eigener Spieler
				masterStyle.fontStyle = FontStyle.Bold;
				clientStyle.fontStyle = FontStyle.Bold;
				clientSmallStyle.fontStyle = FontStyle.Bold;
				masterSmallStyle.fontStyle = FontStyle.Bold;
			}
			else
			{
				// anderer Spieler
				masterStyle.fontStyle = FontStyle.Normal;
				clientStyle.fontStyle = FontStyle.Normal;
				clientSmallStyle.fontStyle = FontStyle.Normal;
				masterSmallStyle.fontStyle = FontStyle.Normal;
			}

			// Character Bild
			if(player.getCharacter() != null)
			{
				if(player.getCharacter().getAvatarTexture() != null)
				{
					GUILayout.Box(player.getCharacter().getAvatarTexture(), avatarStyle);	// kann Image enthalten
				}
			}
			GUILayout.BeginVertical ();
			// Player Name
			if(player.getPhotonPlayer().isMasterClient)
			{
				GUILayout.Label(player.getName(), masterSmallStyle);	// um Text anzuzeigen
				GUILayout.Label(player.getPoints().ToString());
			}
			else
			{
				GUILayout.Label(player.getName());	// um Text anzuzeigen
				GUILayout.Label(player.getPoints().ToString());
			}
			// Character Name
			if(player.getCharacter() != null)
			{
				if(player.getPhotonPlayer().isMasterClient)
				{
					GUILayout.Label(player.getCharacter().getName(), masterSmallStyle);	// um Text anzuzeigen
				}
				else
				{
					GUILayout.Label(player.getCharacter().getName());	// um Text anzuzeigen
				}
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
		}
		GUILayout.EndHorizontal ();
//		GUILayout.EndArea ();
	}


	void WindowPlayerDictionary(int windowID)
	{
		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		/**
		 * Connection Info
		 **/
		GUILayout.BeginVertical ();
		GUILayout.Label ("Connected clients: " + PhotonNetwork.room.playerCount + " / " + PhotonNetwork.room.maxPlayers, guiStyle);
		GUILayout.Label ("Selected characters: " + buffer.Count + " / " + PhotonNetwork.room.playerCount, guiStyle);
		//		if(PhotonNetwork.isMasterClient)
		//			GUILayout.Label ("isMasterClient! "+ PhotonNetwork.player.name, guiStyle);
		//		else
		//			GUILayout.Label ("isClient! "+ PhotonNetwork.player.name, guiStyle);
		
		foreach(Player player in buffer)
		{
			GUILayout.BeginHorizontal();
			//			GUIStyle textStyle = clientStyle;
			//			GUILayout.Label (player.getPhotonPlayer().name +
			//			                 (player.getPhotonPlayer().isMasterClient ? " MasterClient":" Client"), guiStyle);
			if(player.getPhotonPlayer() == PhotonNetwork.player)
			{
				// Eigener Spieler
				masterStyle.fontStyle = FontStyle.Bold;
				clientStyle.fontStyle = FontStyle.Bold;
				clientSmallStyle.fontStyle = FontStyle.Bold;
				masterSmallStyle.fontStyle = FontStyle.Bold;
			}
			else
			{
				// anderer Spieler
				masterStyle.fontStyle = FontStyle.Normal;
				clientStyle.fontStyle = FontStyle.Normal;
				clientSmallStyle.fontStyle = FontStyle.Normal;
				masterSmallStyle.fontStyle = FontStyle.Normal;
			}
			if(player.getPhotonPlayer().isMasterClient)
			{
				//				textStyle = masterStyle;
				GUILayout.Label (player.getPhotonPlayer().name + " MasterClient", masterStyle);
			}
			else
			{
				GUILayout.Label (player.getPhotonPlayer().name + " Client", clientStyle);
			}
			GUILayout.Space(20);
			if(player.getCharacterSelector() != null)
				GUILayout.Label( "charSelector: Yes", masterStyle);
			else
				GUILayout.Label( "charSelector: NO", clientStyle);
			
			GUILayout.Space(20);
			if(player.getCharacter() != null)
			{
				GUILayout.Label( "Character: " + player.getCharacter().getPrefabFilename(), masterStyle);
				GUILayout.Space(20);
				if(player.getCharacter().getGameObject() != null)
					GUILayout.Label( "GO: Yes", masterStyle);
				else
					GUILayout.Label( "GO: NO", clientStyle);
			}
			else
				GUILayout.Label( "Character: NO", clientStyle);
			
			
			
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical ();

		
		GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}


	// ROOM Scene (CharacterSelection, and Testing Character)
	// MasterClient clicks Start:
	//	- checks wich Scene should be loaded (Voted/Selected)
	// 	- Send RPC to all Clients StartScene(int scenenumber)
	// All Clients:
	//  - in StartScene() all Clients set isMessageQueueRunning = false // to stop executing already received or next incomming RPC's
	// 	- in StartScene() Application.LoadLevel(scenenumber)
	
	// LoadedScene (GameScene, PLAYING!!!)
	// [Order]	[Who]			[What]
	// 1		All Clients		
	// 							PlayerDictionaryManager() - Awake() tries to find instantiated ScriptableObject PlayerDictionary				// Order wain?
	// 							GameManger: Awake() - set Referenz to public static PlayerDictionary from PlayerDictionaryManager()				// Order wain?
	// 2						GameManager: Start() - referenz.RemoveAllPlayerCharacterGameObjects();											// ORDER!!!! should be START
	// 3						GameManager: LateStart() isMessageQueueRunning: TRUE
	// 4		MasterClient	GameManager: SpawnAllPlayerCharacter() need BUFFERED, need update and add new CharacterGameObjects to PlayerDictionary (RPC UpdateCurrentPlayerCharacter)
	
	void InitGameScene( )
	{
		//	PhotonNetwork.LoadLevel(Scenes.photonTagging); <-- does not work 100% with RemoveAllPlayerCharacterGameObjects before incomming RPC are executed!!! 
		photonView.RPC("LoadGameScene", PhotonTargets.AllBuffered, Scenes.photonTagging, 1);
	}
	
	[RPC]
	void LoadGameScene( string scenename, int gameMode)
	{
		// gameMode auswerten int -> GameMode gm = new GameMode(gameMode);
		PhotonNetwork.isMessageQueueRunning = false;	// deaktivieren
		Application.LoadLevel(scenename);
	}

}
