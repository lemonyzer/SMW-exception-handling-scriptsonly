using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnScript : MonoBehaviour {

	public bool startGameTrigger = false;
	public bool gameStarted = false;
	public bool startSpawning = false;
	
	private int numberOfAllPlayer;
	private int numberOfAIPlayer;
	private int numberOfLocalUserPlayer;

//	public Dictionary<GameObject, Player> playerDictonary;

	private GameObject gameController;
    private HashID hash;
    private GameObject countDown;
	private Animator anim;
	
	private StatsManager statsManager;
//	private LobbyCharacterManager characterManager;
	private GameManager gameManager;

	GameObject[] localCharacterArray;



	void Awake ()
	{
//		playerDictonary = new Dictionary<GameObject, Player>();

		statsManager = GetComponent<StatsManager>();
		gameManager = GetComponent<GameManager>();

		localCharacterArray = Resources.LoadAll<GameObject>("PlayerCharacter/local");

		foreach(GameObject currCharacter in localCharacterArray)
		{
			Debug.Log(currCharacter.name);
		}

		countDown = GameObject.FindGameObjectWithTag(Tags.countDown);
		anim = countDown.GetComponent<Animator>();
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();

		statsManager = GetComponent<StatsManager>();
		if(statsManager == null)
			Debug.LogError("GameController hat kein StatsManager");

		startGameTrigger = false;
        startSpawning = false;

//		numberOfAllPlayer = PlayerPrefs.GetInt("NumberOfAllPlayers");
//		numberOfAIPlayer = PlayerPrefs.GetInt("NumberOfAIPlayers");
//		numberOfLocalUserPlayer = PlayerPrefs.GetInt("NumberOfLocalUserPlayers");
//		Debug.Log("number of all player: " + numberOfAllPlayer);
//		Debug.Log("number of AI player: " + numberOfAIPlayer);
//		Debug.Log("number of User player: " + numberOfLocalUserPlayer);

	}

	Vector3 getRandomPosition()
	{
		return new Vector3(Random.Range(0.0f, 19.0f), Random.Range(2f, 15.0f), 0f);
	}

	// Use this for initialization
	void Start () {

		StartCoroutine(StartCountDown());
		if(networkView == null)
		{
			// Offline
			StartCoroutine(InstantiateCharacters());
		}
		else
		{
			if(Network.isServer)
			{
				// Server fordert Clients auf Character zu instantieren

				// server doesn’t trigger OnPlayerConnected, manually spawn
				StartCoroutine(InstantiateServerCharacter());
				StartCoroutine(RequestClientsCharacterInstantiation());
//				Network.Instantiate( (GameObject)(characterUserPrefabList.ToArray()[characterPrefabID]), getRandomPosition(), Quaternion.identity,0 );

			}
		}
	}
	
	/**
	 * Mulitplayer (Server)
	 **/
	IEnumerator InstantiateServerCharacter()
	{
		Debug.Log("Before Waiting 3 seconds");
		yield return new WaitForSeconds(3);
		Debug.Log("After Waiting 3 Seconds");

		string serverCharacterName =  GameManager.playerSelectedCharacterPrefabDictionary.Get("0");

		GameObject myCharacterPrefab = (GameObject) Resources.Load(GameManager.resourcesPathLan + serverCharacterName, typeof(GameObject));
		Network.Instantiate( myCharacterPrefab, getRandomPosition(), Quaternion.identity,0 );
		Debug.LogWarning("Server Player " + "0" + " Prefab Name: " + serverCharacterName);
	}

	/**
	 * Mulitplayer (Clients)
	 **/
	IEnumerator RequestClientsCharacterInstantiation()
	{
		
		Debug.Log("Before Waiting 3 seconds");
		yield return new WaitForSeconds(3);
		Debug.Log("After Waiting 3 Seconds");
		
		//		string key = "0" + LobbyCharacterManager.suffixName;
		//		key = key.ToLower();
		//		string serverCharacterName = PlayerPrefs.GetString(key);
		//
		//		NetworkViewID viewID = Network.AllocateViewID();
		//		networkView.RPC("SpawnBox", RPCMode.AllBuffered, viewID, getRandomPosition(), serverCharacterName);
		
		foreach(NetworkPlayer player in Network.connections)
		{
			string playerCharacterName = GameManager.playerSelectedCharacterPrefabDictionary.Get(player.ToString());
			//			GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + playerCharacterName, typeof(GameObject));
			networkView.RPC( "net_DoSpawnGameScene", player, getRandomPosition(), playerCharacterName);
			Debug.LogWarning("Player " + player.ToString() + " Prefab Name: " + playerCharacterName);
		}
	}


	/**
	 * SinglePlayer with Bots
	 **/
	IEnumerator InstantiateCharacters()
	{
		
		Debug.Log("Before Waiting 3 seconds");
		yield return new WaitForSeconds(3);
		Debug.Log("After Waiting 3 Seconds");
		
		//		string key = "0" + LobbyCharacterManager.suffixName;
		//		key = key.ToLower();
		//		string serverCharacterName = PlayerPrefs.GetString(key);
		//
		//		NetworkViewID viewID = Network.AllocateViewID();
		//		networkView.RPC("SpawnBox", RPCMode.AllBuffered, viewID, getRandomPosition(), serverCharacterName);
		
		for(int i=0; i < gameManager.getNumberOfGameSlots(); i++)
		{
			string playerCharacterName = GameManager.playerSelectedCharacterPrefabDictionary.Get(""+i);
			//			GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + playerCharacterName, typeof(GameObject));
			GameObject currentCharacterPrefab = (GameObject) Resources.Load(GameManager.resourcesPathLocal + playerCharacterName, typeof(GameObject));
			if(currentCharacterPrefab != null)
			{
				GameObject currentCharacterGameObject = (GameObject)Instantiate(currentCharacterPrefab, getRandomPosition(), Quaternion.identity);
				currentCharacterGameObject.layer += i; // jeden Spieler in eigene Layer (JumpAblePlatform)

				Character currentCharacter;
				/**
				 * Erster Character ist User, rest ist AI
				 **/
				if(i==0)
				{
					currentCharacterGameObject.tag = Tags.player;
					currentCharacter = new Character(currentCharacterGameObject,false);			// User Controller
				}
				else
				{
					currentCharacterGameObject.tag = Tags.ai;
					currentCharacter = new Character(currentCharacterGameObject,true);			// AI Controller
				}
				currentCharacterGameObject.transform.Find("CharacterSelectionArea").gameObject.SetActive(false);

				Player currentPlayer = new Player(i, "Player", currentCharacter);

				currentPlayer.getCharacter().GetAnimator().SetBool(hash.headJumpedBool,false);
				currentPlayer.getCharacter().GetAnimator().SetBool(hash.gameOverBool,false);
				currentPlayer.getCharacter().GetAnimator().SetBool(hash.deadBool,true);
				currentPlayer.getCharacter().GetAnimator().SetBool(hash.spawnBool,true);
				currentPlayer.getCharacter().GetAnimator().SetTrigger(hash.hitTrigger);

				//currentCharacterGameObject.Awake();
				GameManager.playerDictionary.Add(currentCharacterGameObject,currentPlayer);
			}

			Debug.LogWarning("Player " + i + " Prefab Name: " + playerCharacterName);
		}
	}

	void Update() {

	}

	IEnumerator StartCountDown()
	{
		anim.SetBool(hash.countDownEnabledBool,true);
		anim.SetTrigger(hash.startCountDownTrigger);
		yield return new WaitForSeconds(3.0F);
		anim.SetBool(hash.countDownEnabledBool,false);
//      startSpawning = true;
//		StartSpawn();
	}


	[RPC]
	void net_DoSpawnGameScene( Vector3 position, string characterPrefabName )
	{
		// The object PikachuLanRigidBody2D must be a prefab in the project view.
		// spawn the player paddle
		
		// wäre Besser?! (alle GameObjects in scene, keine "manipulation") .... geht aber nicht, GameObject vorher clonen mit Instantiate(....)
		//		GameObject myCharacter = GameObject.Find (characterPrefabName);
		
		//		GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + characterPrefabName, typeof(GameObject)); // in Resources Folder! \Assests\Resources\characterPrefabName
		GameObject myCharacterPrefab = (GameObject) Resources.Load(GameManager.resourcesPathLan + characterPrefabName, typeof(GameObject));
		Debug.Log("RPC try to Spawn: " + myCharacterPrefab.name);
		//		PlatformCharacter myPlatformCharacter = myCharacter.GetComponent<PlatformCharacter>();
		//		AudioSource.PlayClipAtPoint(myPlatformCharacter.jumpSound,transform.position,1);
		if(myCharacterPrefab != null)
		{
			GameObject myCharacter = (GameObject) Network.Instantiate( myCharacterPrefab, position, Quaternion.identity,0 );
			GameManager.SetupCharacterGameObjectLAN(myCharacter);
			networkView.RPC ("net_RegisterCharacterGameObject", RPCMode.Server, myCharacter);
		}
	}

	[RPC]
	void net_RegisterCharacter(GameObject playerCharacter, NetworkMessageInfo info)
	{
		Debug.Log("Server: " + playerCharacter.name + " empfangen!");

		Character character = new Character(playerCharacter,false);
		Player requestedClient = new Player( int.Parse(info.sender.ToString()), info.sender.ToString()+" lanPlayer", character );
		GameManager.playerDictionary.Add(playerCharacter, requestedClient);
	}
}
