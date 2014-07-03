using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour {

	public bool gameRunning = false;
	public bool gameHasWinner = false;
	private GameObject gameWinner;

	private GameMode currentGameMode;
	private int gameModePointLimit = 1;
	
	enum GameMode
	{
		Classic,
		ClassicReverse,
		CatchMe,
		CTF,
		Tagging,
		RaceCheckPoints,
		RaceRaskulls,
		FeedYoshi,
		Survival,
		FlappyBird
	}

	private int damageValueHeadJump = 1;
	private int pointValueHeadJump = 1;

	private Dictionary<string,int> points;
	private Dictionary<string,int> teamPoints;
	private Dictionary<string,int> nemesis;

	private LobbyCharacterManager characterManager;
	private int slotsCount;
	private int teamsCount;

	public GUIText player0GUIText;
	public GUIText player1GUIText;
	public GUIText player2GUIText;
	public GUIText player3GUIText;

	private GameObject backGround;
	public GameObject winEffect;
	public GameObject winEffect2;
	public AudioClip winSound;

	private SpawnScript spawnScript;
	private SortingLayer sortingLayer;

	void Awake()
	{
		sortingLayer = GetComponent<SortingLayer>();

		backGround = GameObject.FindGameObjectWithTag(Tags.background);
		if(backGround == null)
		{
			Debug.LogError("no Background found!");
		}

		spawnScript = GetComponent<SpawnScript>();

		characterManager = GetComponent<LobbyCharacterManager>();
		slotsCount = characterManager.getNumberOfGameSlots();
		teamsCount = characterManager.getNumberOfTeams();

		points = new Dictionary<string, int>();
		initializePointsZero(slotsCount);
		teamPoints = new Dictionary<string, int>();
		initializeTeamPointsZero(teamsCount);
		nemesis = new Dictionary<string, int>();

		currentGameMode = GameMode.Classic;

		gameHasWinner = false;
		gameRunning = true;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GUIPoints();
	}

	void GUIPoints()
	{
		if(player0GUIText != null)
		{
			player0GUIText.text = "Player 0\n" + GetPoints(""+0);
		}
		if(player1GUIText != null)
		{
			player1GUIText.text = "Player 1\n" + GetPoints(""+1);
		}
		if(player2GUIText != null)
		{
			player2GUIText.text = "Player 2\n" + GetPoints(""+2);
		}
		if(player3GUIText != null)
		{
			player3GUIText.text = "Player 3\n" + GetPoints(""+3);
		}
	}

	void initializePointsZero(int playerCount)
	{
		for(int i=0; i<playerCount; i++)
		{
			points.Add(""+i,0);
		}
	}

	void initializeTeamPointsZero(int teamsCount)
	{
		for(int i=0; i<teamsCount; i++)
		{
			teamPoints.Add(""+i,0);
		}
	}

//	void AddPoint(string player, int pointValue)
//	{
//		int currentPoints;// = points[player];
//		if(points.TryGetValue(player, out currentPoints))
//		{
//			// Key exists
//			currentPoints += pointValue;
//			points[player] = currentPoints;
//			Debug.Log("Player " + player + " hat " + points[player] + " Punkte");
//			Debug.Log("Player " + player + " hat " + currentPoints + " Punkte");
//		}
//		else
//		{
//			// Key exists not
//			currentPoints = pointValue;
//			points.Add(player,currentPoints);
//		}
//		if(PointLimitReached(currentPoints))
//		{
//			GameHasAWinner(player);
//		}
//	}

	void AddPoint(GameObject player, int pointValue)
	{
		int currentPoints;// = points[player];
		string playerID = spawnScript.playerDictonary[player].getPlayerID().ToString();
		string playerCharacter = spawnScript.playerDictonary[player].getCharacter().getName();
		if(points.TryGetValue(playerID, out currentPoints))
		{
			// Key exists
			currentPoints += pointValue;
			points[playerID] = currentPoints;
			Debug.Log("Player " + playerID + " ("+playerCharacter+") " + "hat " + points[playerID] + " Punkte");
//			Debug.Log("Player " + playerID + " ("+playerCharacter+") " + " hat " + currentPoints + " Punkte");
		}
		else
		{
			// Key exists not
			currentPoints = pointValue;
			points.Add(playerID,currentPoints);
		}
		if(PointLimitReached(currentPoints))
		{
			// genug Punkte, gewonnen!
			GameHasAWinner(player);
		}
		else
		{
			// nicht genug Punkte, weiter...
			//Debug.Log("Player " + playerID + ", Points: " + currentPoints);
		}
	}

	bool PointLimitReached(int currentPoints)
	{
//		if(currentGameMode == GameMode.Classic)
//		{
			if(currentPoints >= gameModePointLimit)
			{
				return true;
			}
//		}
		return false;
	}

	void GameHasAWinner(GameObject player)
	{
		gameRunning = false;
		gameHasWinner = true;
		gameWinner = player;
		WinnerEffect();
		//gameWinner = spawnScript.playerDictonary[player].getCharacter().getPrefab();
	}

	void WinnerEffect()
	{
//		Debug.Log("WinnerEffect started");
//		backGround.renderer.enabled = false;
		if(winEffect != null)
		{
			winEffect.SetActive(true);
			winEffect.transform.position = gameWinner.transform.position;
			winEffect.transform.parent = gameWinner.transform;
			winEffect.renderer.sortingLayerID = sortingLayer.guiSortingLayer;
		}
		if(winEffect2 != null)
		{
			winEffect2.SetActive(true);
			winEffect2.transform.position = gameWinner.transform.position;
			winEffect2.transform.parent = gameWinner.transform;
			foreach(Transform child in winEffect2.transform)
			{
				child.renderer.sortingLayerID = sortingLayer.guiSortingLayer;
			}
		}
		if(winSound != null) {
			transform.GetComponent<AudioSource>().Stop();
			AudioSource.PlayClipAtPoint(winSound,transform.position,1);
		}
	}

	int GetPoints(string player)
	{
		int currentPoints;// = points[player];
		if(points.TryGetValue(player, out currentPoints))
		{
			// Key exists
		}
		else
		{
			// Key exists not
			currentPoints = 0;
		}
		return currentPoints;
	}

	void AddKill(GameObject killer, GameObject victim)
	{
		//if(!combo)
		AddPoint(killer, pointValueHeadJump);
		AddNemesis(killer, victim);
	}

//	void AddKill(string killer, string victim)
//	{
//		//if(!combo)
//		AddPoint(killer,pointValueHeadJump);
//		AddNemesis(killer, victim);
//	}

	string GetNemesisKey(string killer, string victim)
	{
		return killer + " " + victim;
	}

	int GetNemesis(string killer, string victim)
	{
		int currentNemesis;// = points[player];
		if(nemesis.TryGetValue(GetNemesisKey(killer,victim), out currentNemesis))
		{
		}
		else
		{
			currentNemesis = 0;
		}
		return currentNemesis;
	}

	void AddNemesis(GameObject killer, GameObject victim)
	{
		int currentNemesis;// = points[player];
		string killerID = spawnScript.playerDictonary[killer].getPlayerID().ToString();
		string victimID = spawnScript.playerDictonary[victim].getPlayerID().ToString();
		if(nemesis.TryGetValue(GetNemesisKey(killerID,victimID), out currentNemesis))
		{
			// Key Exists
			currentNemesis++;
			nemesis[GetNemesisKey(killerID,victimID)] = currentNemesis;
		}
		else
		{
			// Key is new
			currentNemesis = 1;
			nemesis.Add(GetNemesisKey(killerID,victimID), currentNemesis);
		}
		
	}

//	void AddNemesis(string killer, string victim)
//	{
//		int currentNemesis;// = points[player];
//		if(nemesis.TryGetValue(GetNemesisKey(killer,victim), out currentNemesis))
//		{
//			// Key Exists
//			currentNemesis++;
//			nemesis[GetNemesisKey(killer,victim)] = currentNemesis;
//		}
//		else
//		{
//			// Key is new
//			currentNemesis = 1;
//			nemesis.Add(GetNemesisKey(killer,victim), currentNemesis);
//		}
//
//	}

	public void HeadJump(GameObject attacker, GameObject victim)
	{
		if(gameRunning)
		{
//			victim.GetComponent<HealthController>().ApplyDamage(damageValueHeadJump,true);

			Player playerVictim = spawnScript.playerDictonary[victim];
			Character characterVictim = spawnScript.playerDictonary[victim].getCharacter();
			HealthController victimHealthController = spawnScript.playerDictonary[victim].getCharacter().getHealthController();

			victimHealthController.ApplyDamage(attacker, damageValueHeadJump, true);

			//AddKill(spawnScript.playerDictonary[attacker].getPlayerID()+"", spawnScript.playerDictonary[victim].getPlayerID()+"");
			//AddNemesis(spawnScript.playerDictonary[attacker].getPlayerID()+"", spawnScript.playerDictonary[victim].getPlayerID()+"");
		}
	}

	public void HeadJumpConfirm(GameObject attacker, GameObject victim)
	{
		if(gameRunning)
		{
			AddKill(attacker,victim);
			AddNemesis(attacker,victim);
		}
	}

	public void InvincibleAttack(GameObject attacker, GameObject victim)
	{
		if(gameRunning)
		{
			Debug.Log("Sternstunde!!!");
		}
	}
}
