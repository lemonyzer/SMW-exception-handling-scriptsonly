using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour {

	public bool gameRunning;
	private string gameWinner;

	private GameMode currentGameMode;
	private int gameModePointLimit = 10;

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

	private float damageValueHeadJump = 1;
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

	void Awake()
	{
		characterManager = GetComponent<LobbyCharacterManager>();
		slotsCount = characterManager.getNumberOfGameSlots();
		teamsCount = characterManager.getNumberOfTeams();

		points = new Dictionary<string, int>();
		initializePointsZero(slotsCount);
		teamPoints = new Dictionary<string, int>();
		initializeTeamPointsZero(teamsCount);
		nemesis = new Dictionary<string, int>();

		currentGameMode = GameMode.Classic;
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

	void AddPoint(string player, int pointValue)
	{
		int currentPoints = GetPoints(player);

		currentPoints += pointValue;
		points.Add(player,currentPoints);
		if(currentGameMode == GameMode.Classic)
		{
			if(currentPoints >= gameModePointLimit)
			{
				gameRunning = false;
				gameWinner = player;
			}
		}
	}

	int GetPoints(string player)
	{
		int currentPoints;// = points[player];
		if(points.TryGetValue(player, out currentPoints))
		{
		}
		else
		{
			currentPoints = 0;
		}
		return currentPoints;
	}

	void AddKill(string killer, string victim)
	{
		//if(!combo)
		AddPoint(killer,pointValueHeadJump);
		AddNemesis(killer, victim);
	}

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

	void AddNemesis(string killer, string victim)
	{
		int currentNemesis = GetNemesis(killer,victim);
		currentNemesis += 1;
		nemesis.Add(GetNemesisKey(killer,victim), currentNemesis);
	}

	public void HeadJump(Transform attacker, Transform victim)
	{
		if(gameRunning)
		{
			victim.GetComponent<HealthController>().ApplyDamage(damageValueHeadJump,true);
			AddKill(attacker.name, victim.name);
			AddNemesis(attacker.name, victim.name);
		}
	}
}
