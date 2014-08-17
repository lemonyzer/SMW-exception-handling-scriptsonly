using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : Photon.MonoBehaviour {

	public bool gameRunning = false;
	public bool gameHasWinner = false;
	private GameObject gameWinner;

	private GameMode currentGameMode;
	public int gameModePointLimit = 10;
	
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
	
	private int slotsCount;
	private int teamsCount;

	private GameObject backGround;
	public GameObject winEffect;
	public GameObject winEffect2;
	public AudioClip winSound;

	private SortingLayer sortingLayer;

	PlayerDictionary playerDictionary = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary;

	void Awake()
	{
		sortingLayer = GetComponent<SortingLayer>();

		backGround = GameObject.FindGameObjectWithTag(Tags.background);
		if(backGround == null)
		{
			Debug.LogError("no Background found!");
		}

		currentGameMode = GameMode.Classic;

		gameHasWinner = false;
		gameRunning = true;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
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


	void AddKill(Player killer, Player victim)
	{
		//if(!combo)
		//AddPoint(killer, pointValueHeadJump);
		if(currentGameMode == GameMode.Classic)
		{
			killer.addPoints(pointValueHeadJump);
		}
		else if(currentGameMode == GameMode.ClassicReverse)
		{
			killer.addPoints(pointValueHeadJump);
			victim.addHealth(-damageValueHeadJump);
		}
	}

	public void HeadJump(GameObject attacker, GameObject victim)
	{
		if(photonView != null)
		{
			if(!PhotonNetwork.isMasterClient)
			{
				return;
			}
		}
		// wird nur von PhotonMasterClient ausgeführt....
		if(GameState.currentState == GameState.States.Running)
		{
			PhotonPlayer attackersRealOwner = attacker.GetComponent<RealOwner>().owner;
			PhotonPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;

			photonView.RPC("SyncHeadJump", PhotonTargets.AllBuffered, attackersRealOwner, victimsRealOwner);
		}
		else
		{
			Debug.LogWarning("current GameState = " + GameState.currentState.ToString() + " HeadJump zählt nicht!");
		}
	}

	[RPC]
	public void SyncHeadJump(PhotonPlayer attackersRealOwner, PhotonPlayer victimsRealOwner)
	{
		if(GameState.currentState == GameState.States.Running)
        {
			if(attackersRealOwner != null &&
			   victimsRealOwner != null)
			{
				Player playerAttacker = playerDictionary.GetPlayer(attackersRealOwner);
				Player playerVictim = playerDictionary.GetPlayer(victimsRealOwner);
			
				AnimatorController victimsAnimationController = playerVictim.getCharacter().getGameObject().GetComponent<AnimatorController>();
			
				victimsAnimationController.HeadJumpAnimation();

				AddKill(playerAttacker, playerVictim);
			}
			else
			{
				Debug.LogError("Character ohne RealOwner beteiligt! BOT?");
			}
		}
    }


	public void InvincibleAttack(GameObject attacker, GameObject victim)
	{
		if(photonView != null)
		{
			if(!PhotonNetwork.isMasterClient)
			{
				return;
			}
		}
		// wird nur von PhotonMasterClient ausgeführt....
		if(GameState.currentState == GameState.States.Running)
		{
			PhotonPlayer attackersRealOwner = attacker.GetComponent<RealOwner>().owner;
			PhotonPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;
			
			photonView.RPC("SyncInvincibleAttack", PhotonTargets.AllBuffered, attackersRealOwner, victimsRealOwner);
        }
	}


	[RPC]
	public void SyncInvincibleAttack(PhotonPlayer attackersRealOwner, PhotonPlayer victimsRealOwner)
	{
		if(GameState.currentState == GameState.States.Running)
		{
			Player playerAttacker = playerDictionary.GetPlayer(attackersRealOwner);
			Player playerVictim = playerDictionary.GetPlayer(victimsRealOwner);
			
			AnimatorController victimsAnimationController = playerVictim.getCharacter().getGameObject().GetComponent<AnimatorController>();
			
			victimsAnimationController.InvincibleAttackAnimation();
			
			AddKill(playerAttacker, playerVictim);
        }
    }
}
