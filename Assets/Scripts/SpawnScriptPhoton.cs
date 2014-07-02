using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnScriptPhoton : MonoBehaviour {

	public bool startGameTrigger = false;
	public bool gameStarted = false;
	public bool startSpawning = false;

	private int characterPrefabID = 0;
	
	private int numberOfAllPlayer;
	private int numberOfAIPlayer;
	private int numberOfLocalUserPlayer;

//	public Character characterDictonary;
	public List<GameObject> characterAIPrefabList;
	public List<GameObject> characterUserPrefabList;

	private GameObject gameController;
    private HashID hash;
    private GameObject countDown;
	private Animator anim;

	private Stats statsScript;


	void Awake ()
	{

		countDown = GameObject.FindGameObjectWithTag(Tags.countDown);
		anim = countDown.GetComponent<Animator>();
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();

		statsScript = GetComponent<Stats>();
		if(statsScript == null)
			Debug.LogError("GameController hat kein StatsScript");

		startGameTrigger = false;
        startSpawning = false;
		numberOfAllPlayer = PlayerPrefs.GetInt("NumberOfAllPlayers");
		numberOfAIPlayer = PlayerPrefs.GetInt("NumberOfAIPlayers");
		numberOfLocalUserPlayer = PlayerPrefs.GetInt("NumberOfLocalUserPlayers");
		Debug.Log("number of all player: " + numberOfAllPlayer);
		Debug.Log("number of AI player: " + numberOfAIPlayer);
		Debug.Log("number of User player: " + numberOfLocalUserPlayer);

		PlayerPrefs.SetInt("AI0Character",0);
		PlayerPrefs.SetInt("AI1Character",1);
		PlayerPrefs.SetInt("AI2Character",2);

		PlayerPrefs.SetInt("User0Character",0);
	}

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
			characterPrefabID = 0;
			// server doesn’t trigger OnPlayerConnected, manually spawn
			Network.Instantiate( (GameObject)(characterUserPrefabList.ToArray()[characterPrefabID]), getRandomPosition(), Quaternion.identity,0 );
			Debug.Log("Server Character erzeugen");
			// nobody has joined yet, display "Waiting..." for player 2
//			Player2ScoreDisplay.text = "Waiting...";
		}
	}

	void Update() {
		if(Network.connections.Length > 0)
		{
			Debug.Log("min. 1 Client verbunden ");
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
		Application.LoadLevel( "DirectConnect" );
	}
	
	[RPC]
	void net_DoSpawn( Vector3 position )
	{
		// spawn the player paddle
		Network.Instantiate( (GameObject)(characterUserPrefabList.ToArray()[characterPrefabID]), position, Quaternion.identity,0 );
	}


	IEnumerator StartCountDown()
	{
		yield return new WaitForSeconds(3.0F);
		anim.SetBool(hash.countDownEnabledBool,false);
        startSpawning = true;
//		StartSpawn();
	}
}
