using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnScript : MonoBehaviour {

	public bool startSpawning = false;
	
	private int numberOfAllPlayer;
	private int numberOfAIPlayer;
	private int numberOfUserPlayer;

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

        startSpawning = false;
		numberOfAllPlayer = PlayerPrefs.GetInt("NumberOfAllPlayers");
		numberOfAIPlayer = PlayerPrefs.GetInt("NumberOfAIPlayers");
		numberOfUserPlayer = PlayerPrefs.GetInt("NumberOfUserPlayers");
		Debug.Log("number of all player: " + numberOfAllPlayer);
		Debug.Log("number of AI player: " + numberOfAIPlayer);
		Debug.Log("number of User player: " + numberOfUserPlayer);

		PlayerPrefs.SetInt("AI0Character",0);
		PlayerPrefs.SetInt("AI1Character",1);
		PlayerPrefs.SetInt("AI2Character",2);

		PlayerPrefs.SetInt("User0Character",0);
	}

	// Use this for initialization
	void Start () {
		anim.SetBool(hash.countDownEnabledBool,true);
		anim.SetTrigger(hash.startCountDownTrigger);
		StartCoroutine(StartCountDown());
	}
	
	public void StartSpawn()
	{
		int characterID = -1;
		GameObject currentCharacter;
		for(int i=0; i<numberOfAIPlayer; i++)
		{
			characterID = PlayerPrefs.GetInt("AI"+i+"Character");
			Debug.Log("AI " + i + " CharacterID: " + characterID);
			if(characterID != null)
			{
				if(characterID >=0 && characterID < characterAIPrefabList.Count)
				{
					currentCharacter = (GameObject)Instantiate(characterAIPrefabList.ToArray()[i], new Vector2(Random.Range(0.0f, 19.0f), Random.Range(2f, 15.0f)), Quaternion.identity);
					currentCharacter.GetComponent<Animator>().SetTrigger(hash.hitTrigger);
					statsScript.AddPlayer(currentCharacter);
                }
            }
        }

		characterID = -1;
		for(int i=0; i<numberOfUserPlayer; i++)
		{
			characterID = PlayerPrefs.GetInt("User"+i+"Character");
			Debug.Log("User " + i + " CharacterID: " + characterID);
			if(characterID != null)
			{
				if(characterID >=0 && characterID < characterUserPrefabList.Count)
				{
					currentCharacter = (GameObject)Instantiate(characterUserPrefabList.ToArray()[i], new Vector2(Random.Range(0.0f, 19.0f), Random.Range(2f, 15.0f)), Quaternion.identity);
					currentCharacter.GetComponent<Animator>().SetTrigger(hash.hitTrigger);
					statsScript.AddPlayer(currentCharacter);
                }
            }
        }
    }

	IEnumerator StartCountDown()
	{
		yield return new WaitForSeconds(3.0F);
		anim.SetBool(hash.countDownEnabledBool,false);
        startSpawning = true;
		StartSpawn();
	}
}
