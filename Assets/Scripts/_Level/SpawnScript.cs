using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnScript : MonoBehaviour {

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

	GameObject[] characterArray;

	void Awake ()
	{

		characterArray = Resources.LoadAll<GameObject>("PlayerCharacter/lokal");

		foreach(GameObject currCharacter in characterArray)
		{
			Debug.Log(currCharacter.name);
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

		StartCoroutine(StartCountDown());
		StartCoroutine(InstantiateCharacters());
	}

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
		
		for(int i=0; i<4; i++)
		{
			string playerCharacterName = GetPlayerCharacter(""+i);
			//			GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + playerCharacterName, typeof(GameObject));
			GameObject currentCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + playerCharacterName, typeof(GameObject));
			if(currentCharacter != null)
				currentCharacter = (GameObject)Instantiate(currentCharacter, getRandomPosition(), Quaternion.identity);

			Debug.LogWarning("Player " + i + " Prefab Name: " + playerCharacterName);
		}
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

	void Update() {
//		if(Network.connections.Length > 0)
//		{
//			Debug.Log("min. 1 Client verbunden ");
//			if(startGameTrigger)
//			{
//				startGameTrigger = false;
//				anim.SetBool(hash.countDownEnabledBool,true);
//				anim.SetTrigger(hash.startCountDownTrigger);
//				StartCoroutine(StartCountDown());
//			}
//		}
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


	IEnumerator StartCountDown()
	{
		anim.SetBool(hash.countDownEnabledBool,true);
		anim.SetTrigger(hash.startCountDownTrigger);
		yield return new WaitForSeconds(3.0F);
		anim.SetBool(hash.countDownEnabledBool,false);
//      startSpawning = true;
//		StartSpawn();
	}
}
