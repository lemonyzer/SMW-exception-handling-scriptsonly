using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnScriptPlayerPrefs : MonoBehaviour {

	public bool startGameTrigger = false;
	public bool gameStarted = false;
	public bool startSpawning = false;
	

//	private int numberOfAllPlayer;
//	private int numberOfAIPlayer;
//	private int numberOfLocalUserPlayer;

	public Dictionary<string,GameObject> characterDictonary;
	public List<GameObject> characterAIPrefabList;
	public List<GameObject> characterUserPrefabList;

	private GameObject gameController;
    private HashID hash;
    private GameObject countDown;
	private Animator anim;

	private Stats statsScript;

	private GameObject[] characterArray;


	void Awake ()
	{

		characterArray = Resources.LoadAll<GameObject>(LobbyCharacterManager.resourcesPathLan);
		characterDictonary = new Dictionary<string,GameObject>();
		foreach(GameObject go in characterArray)
		{
			if(go != null)
			{
				if(go.name != null)
				{
					characterDictonary.Add(go.name,go);
					Debug.Log("characterDictionary: " + characterDictonary[go.name].name);
				}
			}
		}

		countDown = GameObject.FindGameObjectWithTag(Tags.countDown);
		anim = countDown.GetComponent<Animator>();
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();

		statsScript = GetComponent<Stats>();
		if(statsScript == null)
			Debug.LogError("GameController hat kein StatsScript");

		startGameTrigger = false;
        startSpawning = false;
//		numberOfAllPlayer = PlayerPrefs.GetInt("NumberOfAllPlayers");
//		numberOfAIPlayer = PlayerPrefs.GetInt("NumberOfAIPlayers");
//		numberOfLocalUserPlayer = PlayerPrefs.GetInt("NumberOfLocalUserPlayers");
//		Debug.Log("number of all player: " + numberOfAllPlayer);
//		Debug.Log("number of AI player: " + numberOfAIPlayer);
//		Debug.Log("number of User player: " + numberOfLocalUserPlayer);

//		PlayerPrefs.SetInt("AI0Character",0);
//		PlayerPrefs.SetInt("AI1Character",1);
//		PlayerPrefs.SetInt("AI2Character",2);
//
//		PlayerPrefs.SetInt("User0Character",0);

	}

	public string GetPlayerPrefsKey( string playerID)
	{
		string key = playerID + LobbyCharacterManager.suffixName;
		key = key.ToLower();
		return key;
	}

	public string GetPlayerCharacter(string playerId)
	{
		return PlayerPrefs.GetString(GetPlayerPrefsKey(playerId));
	}

	IEnumerator RequestClientsCharacterInstantiation()
	{

		Debug.Log("Before Waiting 2 seconds");
		yield return new WaitForSeconds(2);
		Debug.Log("After Waiting 2 Seconds");

//		string key = "0" + LobbyCharacterManager.suffixName;
//		key = key.ToLower();
//		string serverCharacterName = PlayerPrefs.GetString(key);
//
//		NetworkViewID viewID = Network.AllocateViewID();
//		networkView.RPC("SpawnBox", RPCMode.AllBuffered, viewID, getRandomPosition(), serverCharacterName);

		foreach(NetworkPlayer player in Network.connections)
		{
			string playerCharacterName = GetPlayerCharacter(player.ToString());
//			GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + playerCharacterName, typeof(GameObject));
			networkView.RPC( "net_DoSpawnGameScene", player, getRandomPosition(), playerCharacterName);
			Debug.LogWarning("Player " + player.ToString() + " Prefab Name: " + playerCharacterName);
		}
	}



	[RPC]
	void SpawnBox(NetworkViewID viewID, Vector3 position, string characterName) {
		Transform clone;
		clone = Instantiate(characterDictonary[characterName], position, Quaternion.identity) as Transform as Transform;
		NetworkView nView;
		nView = clone.GetComponent<NetworkView>();
		nView.viewID = viewID;
	}

	IEnumerator InstantiateServerCharacter()
	{
		Debug.Log("Before Waiting 2 seconds");
		yield return new WaitForSeconds(2);
		Debug.Log("After Waiting 2 Seconds");

		string key = "0" + LobbyCharacterManager.suffixName;
		key = key.ToLower();
		string serverCharacterName = PlayerPrefs.GetString(key);
		GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPathLan + serverCharacterName, typeof(GameObject));
		Network.Instantiate( myCharacter, getRandomPosition(), Quaternion.identity,0 );
		Debug.LogWarning("Server Player " + "0" + " Prefab Name: " + serverCharacterName);
	}

//	void PreparePrefabForPlayer(string player)
//	{
//		string prefabName = GetPlayerPrefabName(player);	// habe Sprite ID, brauche Prefab Namen
//
//		GameObject go = GameObject.Find(prefabName);
//		if(go != null)
//		{
//			// Prefab gefunden
//			go.GetComponent<PlatformCharacter>().enabled = true;
//			go.GetComponent<PlatformUserControlAnalogStickAndButton>().enabled = true;
//			go.GetComponent<PlatformUserControlKeyboard>().enabled = true;
//			go.GetComponent<PlatformCharacter>().enabled = true;
//		}
//	}

//	string GetPlayerPrefabName(string player)
//	{
//		string key = player + "_PrefabID";
//		int value = PlayerPrefs.GetInt(key);
//		if(value < 0 || value > characterArray.Length)
//			return "";									// Character nicht in liste gefunden
//		if(characterArray[value] != null)
//			return characterArray[value].name;
//		else
//			return "";									// Character nicht in liste gefunden
//	}

	Vector3 getRandomPosition()
	{
		return new Vector3(Random.Range(0.0f, 19.0f), Random.Range(2f, 15.0f), 0f);
	}

	// Use this for initialization
	void Start () {
//		anim.SetBool(hash.countDownEnabledBool,true);
//		anim.SetTrigger(hash.startCountDownTrigger);
//		StartCoroutine(StartCountDown());
		if( Network.isServer )
		{
			// server doesn’t trigger OnPlayerConnected, manually spawn
			StartCoroutine(InstantiateServerCharacter());
			StartCoroutine(RequestClientsCharacterInstantiation());
//			Network.Instantiate( (GameObject)(characterUserPrefabList.ToArray()[characterPrefabID]), getRandomPosition(), Quaternion.identity,0 );

		}

	}

	void BackButton()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			if(Network.isServer)
			{
				MasterServer.UnregisterHost();
				Debug.LogWarning("MasterServer.UnregisterHost();");
				
				foreach(NetworkPlayer player in Network.connections)
				{
					Network.CloseConnection(player,true);
					Debug.LogWarning("Network.CloseConnection("+player.ToString()+",true);");
				}
				// schlecht!
				//					for(int i=0;i<Network.connections.Length;i++)
				//					{
				//						Network.CloseConnection(Network.connections[i],true);
				//						Debug.LogWarning("Network.CloseConnection(Network.connections["+i+"],true);");
				//					}
			}
			Network.Disconnect();
			Debug.LogWarning("Network.Disconnect();");
			Application.LoadLevel("mp_Multiplayer");
			return;
		}
	}

	void Update() {

		BackButton();

		if(Network.connections.Length > 0)
		{
//			Debug.Log("min. 1 Client verbunden ");

			if(startGameTrigger)
			{
				startGameTrigger = false;
				anim.SetBool(hash.countDownEnabledBool,true);
				anim.SetTrigger(hash.startCountDownTrigger);
				StartCoroutine(StartCountDown());
			}
		}
	}
	
//	public void StartSpawn()
//	{
//		int characterID = -1;
//		GameObject currentCharacter;
//		for(int i=0; i<numberOfAIPlayer; i++)
//		{
//			characterID = PlayerPrefs.GetInt("AI"+i+"Character");
//			Debug.Log("AI " + i + " CharacterID: " + characterID);
//			if(characterID != null)
//			{
//				if(characterID >=0 && characterID < characterAIPrefabList.Count)
//				{
//					currentCharacter = (GameObject)Instantiate(characterAIPrefabList.ToArray()[i], getRandomPosition(), Quaternion.identity);
//					currentCharacter.GetComponent<Animator>().SetTrigger(hash.hitTrigger);
//					statsScript.AddPlayer(currentCharacter);
//                }
//            }
//        }
//
//		characterID = -1;
//		for(int i=0; i<numberOfLocalUserPlayer; i++)
//		{
//			characterID = PlayerPrefs.GetInt("User"+i+"Character");
//			Debug.Log("User " + i + " CharacterID: " + characterID);
//			if(characterID != null)
//			{
//				if(characterID >=0 && characterID < characterUserPrefabList.Count)
//				{
//					currentCharacter = (GameObject)Instantiate(characterUserPrefabList.ToArray()[i], getRandomPosition(), Quaternion.identity);
//					currentCharacter.GetComponent<Animator>().SetTrigger(hash.hitTrigger);
//					statsScript.AddPlayer(currentCharacter);
//                }
//            }
//        }
//		startGameTrigger = true;
//    }

	void OnPlayerConnected( NetworkPlayer player )
	{
		Debug.Log(player + " connected in GameScene!");
		// when a player joins, tell them to spawn
		networkView.RPC( "net_DoSpawn", player, getRandomPosition() );

		// change player 2’s score display from "waiting..." to "0"
		// Player2ScoreDisplay.text = "0";
	}

	void OnPlayerDisconnected( NetworkPlayer player )
	{
		// player 2 left, reset scores
//		p1Score = 0;
//		p2Score = 0;
		
		// display each player’s scores
		// display "Waiting..." for player 2
//		Player1ScoreDisplay.text = p1Score.ToString();
//		Player2ScoreDisplay.text = "Waiting...";
		Debug.Log(player.ipAddress + " disconnected!");
		Network.DestroyPlayerObjects(player);
	}

	void OnDisconnectedFromServer( NetworkDisconnection cause )
	{
		// go back to the main menu
		Application.LoadLevel( "mp_Multiplayer" );
	}

	[RPC]
	void net_DoSpawnGameScene( Vector3 position, string characterPrefabName )
	{
		// The object PikachuLanRigidBody2D must be a prefab in the project view.
		// spawn the player paddle
		
		// wäre Besser?! (alle GameObjects in scene, keine "manipulation") .... geht aber nicht, GameObject vorher clonen mit Instantiate(....)
		//		GameObject myCharacter = GameObject.Find (characterPrefabName);

//		GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + characterPrefabName, typeof(GameObject)); // in Resources Folder! \Assests\Resources\characterPrefabName
		GameObject myCharacter = characterDictonary[characterPrefabName];
		Debug.Log("RPC try to Spawn: " + myCharacter.name);
		//		PlatformCharacter myPlatformCharacter = myCharacter.GetComponent<PlatformCharacter>();
		//		AudioSource.PlayClipAtPoint(myPlatformCharacter.jumpSound,transform.position,1);
		if(myCharacter != null)
			Network.Instantiate( myCharacter, position, Quaternion.identity,0 );
	}

//	[RPC]
//	void net_DoSpawn( Vector3 position )
//	{
//		// spawn the player paddle
//		Network.Instantiate( (GameObject)(characterUserPrefabList.ToArray()[characterPrefabID]), position, Quaternion.identity,0 );
//	}


	IEnumerator StartCountDown()
	{
		yield return new WaitForSeconds(3.0F);
		anim.SetBool(hash.countDownEnabledBool,false);
        startSpawning = true;
//		StartSpawn();
	}
}
