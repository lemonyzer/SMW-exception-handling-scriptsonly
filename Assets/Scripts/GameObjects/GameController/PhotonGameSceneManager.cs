using UnityEngine;
using System.Collections;

public class PhotonGameSceneManager : Photon.MonoBehaviour {
	
	// was ist wenn in diesem Moment PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary == null ist ?!
	// wird die Referenz automatisch erneuert?
	PlayerDictionary playerDictionary = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary;

	// was ist wenn in diesem Moment GameState.currentState == null ist ?!
	// wird die Referenz automatisch erneuert?
	GameState.States currentGameState = GameState.currentState;

	// Spawnpoints tagged Spawn
	GameObject[] spawns;

	// wichtig! MasterClientSwitch feature
	PhotonPlayer oldMasterClient;
	bool masterClientSwitched = false;
    
	// all Players Ready ? compare with PhotonNetwork.playerList.Length
	int playerReadyCount = 0;

	/**
	 * GUI
	 **/

	float minButtonHeight;
	GUIStyle guiStyle;
	
	GUIStyle masterStyle;
	GUIStyle masterSmallStyle;
	
	GUIStyle clientStyle;
	GUIStyle clientSmallStyle;
	
	GUIStyle myStyle;
	
	GUIStyle avatarStyle;
	
	public bool debugEnabled = false;
	
	private Rect windowPlayerDictionaryRect = new Rect(20, 25, Screen.width-40, 200);
	private Rect windowPlayerInfoGUIRect = new Rect(10,Screen.height-100f,Screen.width-20,90);

	void InitSpawnPoints()
	{
		spawns = GameObject.FindGameObjectsWithTag("Spawn");
		Debug.Log(spawns.Length + " gefundene SpawnPoints");
	}

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

	void Awake()
	{
		InitSpawnPoints();
		InitGUIStyle();
	}
	
	// Use this for initialization
	void Start () {																			// ???? 
		Debug.LogWarning("PhotonGameSceneManager: Start");
		if(playerDictionary != null)
		{
			// auf allen Clients ausführen
			
			// PhotonNetwork.isMessageQueueRunning DARF NICHT TRUE SEIN!!!
			if(PhotonNetwork.isMessageQueueRunning)
			{
				Debug.LogError("MessageQueue is running, NOT ALLOWED AT THIS MOMENT!!!");
			}
			else
			{
				Debug.Log("MessageQueue is queuing!!!");
			}
			
			//PROBLEM	// was passiert wenn Client lange Sceneläd und RPC UpdateCurrentPlayerCharacter vor diesem aufruf ausführt?!!!!!
			// ???? dieser client dürfte darf kein MasterClient werden da er keine Referenz zu dem neuen GameObject des players hat!!!
			playerDictionary.RemoveAllPlayerCharacerGameObjects();
			oldMasterClient = PhotonNetwork.masterClient;
			//SOLUTION	// erst jetzt Communication fortsetzen! Buffered RPC's kommen jetzt erst rein
			PhotonNetwork.isMessageQueueRunning = true;
			// wird nur auf MasterClient ausgeführt
			SpawnAllPlayerCharacter();
			photonView.RPC("ClientReady", PhotonTargets.AllBuffered);
			//photonView.RPC("StartGame", PhotonTargets.AllBufferedViaServer);			// start Game IS NOT SYNC! need to wait for all players ClientReady RPC
        }
        else
        {
            Debug.LogError("BIG ERROR, PlayerDictionary doesn't exist, cant Spawn Players!!!");
            Application.LoadLevel(Scenes.mainmenu);
		}
		
	}

	void SpawnAllPlayerCharacter()
	{
		if(PhotonNetwork.isMasterClient)
		{
			int i=0;
			foreach(PhotonPlayer currentPhotonPlayer in PhotonNetwork.playerList)
			{
				Debug.Log("SpawnAllPlayerCharacter working On: " + currentPhotonPlayer.name);
				string currPrefabFilename = null;
				Player currPlayer = null;
				currPlayer = playerDictionary.GetPlayer(currentPhotonPlayer);
				if(currPlayer != null)
				{
					currPrefabFilename = currPlayer.getCharacter().getPrefabFilename();
					if( !string.IsNullOrEmpty(currPrefabFilename) )
					{
						Debug.Log("Spawning: " + currentPhotonPlayer.name + " Character: " + currPrefabFilename);
						
						if(spawns.Length >0)
						{
							SpawnAuthorativePlayerCharacter(currentPhotonPlayer, currPrefabFilename, spawns[i].transform.position);
							i++;
							if(i >= spawns.Length -1)
							{
								// alle spawm spots voll, wieder von 0 anfangen
								i=0;
							}
						}
						//SpawnAuthorativePlayerCharacter(currentPhotonPlayer, currPrefabFilename, new Vector3(Random.Range(-10,10),5,Random.Range(-10,10)));
						//photonView.RPC("SpawnAuthorativePlayerCharacter", PhotonTargets.AllBuffered, currentPhotonPlayer, currPrefabFilename);
					}
				}
			}
		}
	}

	/**
	 * PhotonPlayer clicked unUsed Character, MasterClient instantiats authorativ characterGameObject with Input Controls by realOwner (PhotonPlayer)
	 * after instantiation, tell every Client who is the realOwner (Controller)
	 **/
	void SpawnAuthorativePlayerCharacter(PhotonPlayer realOwner, string characterPrefabName, Vector3 spawnPosition)
	{
		// Called on the MasterClient only! (Authorative Mode)
		
		if(!string.IsNullOrEmpty(characterPrefabName))
		{
			// Instantiate a new object for this player, remember; the server is therefore the owner.
			GameObject playerCharacterGameObject = (GameObject)PhotonNetwork.Instantiate( characterPrefabName, spawnPosition, Quaternion.identity,0 );
			
			// update Dictionary of all clients with
			photonView.RPC("UpdateCurrentPlayerCharacter", PhotonTargets.AllBuffered, realOwner, characterPrefabName, playerCharacterGameObject.GetPhotonView().viewID);
			
			// Get the networkview of this new transform
			PhotonView newObjectsview = playerCharacterGameObject.GetComponent<PhotonView>();
			
			// Keep track of this new player so we can properly destroy it when required.
			RealOwner realOwnerScript = playerCharacterGameObject.GetComponent<RealOwner>();
			realOwnerScript.owner = realOwner;
			
			// Call an RPC on this new PhotonView, set the PhotonPlayer who controls this new player
			newObjectsview.RPC("SetCharacterControlsOwner", PhotonTargets.AllBuffered, realOwner);
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
		
		Player player = playerDictionary.GetPlayer(currentPhotonPlayer);
		if(player == null)
        {
            // player was not in Dictionary
            player = new Player(currentPhotonPlayer, character);
			playerDictionary.AddPlayer(currentPhotonPlayer, player);
        }
        else
        {
            // player found in dictionary
            player.setCharacter(character);
        }
    }

	[RPC]
	void ClientReady(PhotonMessageInfo info)
	{
		playerReadyCount++;
		Debug.Log( playerReadyCount + "/" + PhotonNetwork.room.playerCount + " PhotonPlayer sind ready!" );
		if(PhotonNetwork.isMasterClient)
		{
			if(playerReadyCount >= PhotonNetwork.room.playerCount)
			{
				photonView.RPC("StartGame", PhotonTargets.All);			// dont need to be buffered (no player can join PhotonROOM after masterclient closes roomscene
				// RPC's need to be reliable. player Dictionary need to be filled before game starts to support OnMasterClientSwitched feature
			}
		}
	}
	
	// alle Spieler sind ready, starte Spiel...
	[RPC]
    void StartGame()
    {
		currentGameState = GameState.States.Starting;
//        partyCam.StartCam();
//        photonTaggingStats.StartTagging();
    }
    
    // Update is called once per frame
	void Update () {
	
	}


	
	void OnGUI()
	{
		if(playerDictionary == null)
		{
			return;
		}

		if (!PhotonNetwork.inRoom)
			return;

		if (playerDictionary.Values() == null)
			return;

		// FullScreen 0,0 = Left,Top
		
		if(debugEnabled)
			windowPlayerDictionaryRect = GUI.Window(0, windowPlayerDictionaryRect, WindowPlayerDictionary, "Player Dictionary");
		windowPlayerInfoGUIRect = GUI.Window(1, windowPlayerInfoGUIRect, WindowPlayerInfoGUI, "Players");
		
	}
	
	void WindowPlayerInfoGUI(int windowID)
	{
//		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		/**
		 * Player Info
		 **/
		//		GUILayout.BeginArea (new Rect(0,Screen.height-100f,PhotonNetwork.room.playerCount*200,100));
		GUILayout.BeginHorizontal ();
		// Schleife über Spielerliste
		foreach(Player player in playerDictionary.Values())
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
			}
			else
			{
				GUILayout.Label(player.getName());	// um Text anzuzeigen
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
//		List<Player> buffer = new List<Player> ( syncedLocalPersistentPlayerDictionary.Values() );
		/**
		 * Connection Info
		 **/
		GUILayout.BeginVertical ();
		GUILayout.Label ("Connected clients: " + PhotonNetwork.room.playerCount + " / " + PhotonNetwork.room.maxPlayers, guiStyle);
		GUILayout.Label ("Selected characters: " + playerDictionary.Values().Count + " / " + PhotonNetwork.room.playerCount, guiStyle);
		//		if(PhotonNetwork.isMasterClient)
		//			GUILayout.Label ("isMasterClient! "+ PhotonNetwork.player.name, guiStyle);
		//		else
		//			GUILayout.Label ("isClient! "+ PhotonNetwork.player.name, guiStyle);
		
		foreach(Player player in playerDictionary.Values())
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

	void OnPhotonPlayerConnected (PhotonPlayer connectedPhotonPlayer)
	{
		Debug.LogWarning("GameSceneManager: OnPhotonPlayerConnected(" + connectedPhotonPlayer.name + ")");
	}
	
	void OnJoinedRoom  ()
	{
		// sollte nicht ausgeführt werden, OnJoinedRoom wird in PhotonRoomScene ausgeführt
		Debug.LogWarning("GameSceneManager: OnJoinedRoom ()");
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
					go = playerDictionary.TryGetCharacterSelectorGameObject(currentPhotonPlayer);
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
					go = playerDictionary.TryGetCharacterGameObject(currentPhotonPlayer);
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
		playerDictionary.RemovePlayer(photonPlayer);
	}
	
	void RemovePlayer(PhotonPlayer networkPlayer)
	{
		Debug.LogWarning("RemovePlayer " + networkPlayer.name);
		if (PhotonNetwork.isMasterClient)
		{
			GameObject photonPlayerCharacter = playerDictionary.TryGetCharacterGameObject(networkPlayer);
			GameObject characterSelector = playerDictionary.TryGetCharacterSelectorGameObject(networkPlayer);
			
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
}
