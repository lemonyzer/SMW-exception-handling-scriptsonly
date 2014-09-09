﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour {

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

	NetworkView myNetworkView;

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

		myNetworkView = GetComponent<NetworkView>();
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
		if(myNetworkView != null)
		{
			if(!Network.isServer)
			{
				return;
			}
		}
		// wird nur von PhotonMasterClient ausgeführt....

		if(GameState.currentState == GameState.States.Running)
		{
			NetworkPlayer attackersRealOwner = attacker.GetComponent<RealOwner>().owner;
			NetworkPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;

			myNetworkView.RPC("SyncHeadJump", RPCMode.AllBuffered, attackersRealOwner, victimsRealOwner);
		}
		else
		{
			Debug.LogWarning("current GameState = " + GameState.currentState.ToString() + " HeadJump zählt nicht!");
		}
	}

	[RPC]
	public void SyncHeadJump(NetworkPlayer attackersRealOwner, NetworkPlayer victimsRealOwner)
	{
		if(GameState.currentState == GameState.States.Running)
        {
			if(attackersRealOwner != null &&
			   victimsRealOwner != null)
			{
				Player playerAttacker = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(attackersRealOwner);
				Player playerVictim = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(victimsRealOwner);
			
				//AnimatorController victimsAnimationController = playerVictim.getCharacter().getGameObject().GetComponent<AnimatorController>();
				//victimsAnimationController.HeadJumpAnimation();
				PlatformCharacter victimCharacterScript = playerVictim.getCharacter().getGameObject().GetComponent<PlatformCharacter>();
				victimCharacterScript.HeadJumpVictim();

				AddKill(playerAttacker, playerVictim);
			}
			else
			{
				Debug.LogError("Character ohne RealOwner beteiligt! BOT?");
			}
		}
    }

	bool isAuthoritativeHost()
	{
		if(offline ())
			return true;
		
		if(server ())
			return true;
		
		return false;
		
	}
	
	bool offline()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return true;
		
		return false;
	}
	
	bool server()
	{
		if(Network.isServer)
			return true;
		
		return false;
	}

	public void InvincibleAttack(GameObject attacker, GameObject victim)
	{
		if(myNetworkView != null)
		{
			if(!Network.isServer && Network.peerType != NetworkPeerType.Disconnected)
			{
				return;
			}
		}
		// wird nur von PhotonMasterClient ausgeführt....
		if(GameState.currentState == GameState.States.Running)
		{
			NetworkPlayer attackersRealOwner = attacker.GetComponent<RealOwner>().owner;
			NetworkPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;

			if(server())
				myNetworkView.RPC("SyncInvincibleAttack", RPCMode.AllBuffered, attackersRealOwner, victimsRealOwner);

			if(offline())
			{
//				AnimatorController victimsAnimationController = victim.GetComponent<AnimatorController>();
//				victimsAnimationController.InvincibleAttackAnimation();
//				AddKill(attacker,victim);
			}
        }
		else
		{
			Debug.LogWarning("current GameState = " + GameState.currentState.ToString() + " InvincibleAttack zählt nicht!");
		}
	}


	[RPC]
	public void SyncInvincibleAttack(NetworkPlayer attackersRealOwner, NetworkPlayer victimsRealOwner)
	{
		if(GameState.currentState == GameState.States.Running)
		{
			Player playerAttacker = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(attackersRealOwner);
			Player playerVictim = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(victimsRealOwner);
			
			//AnimatorController victimsAnimationController = playerVictim.getCharacter().getGameObject().GetComponent<AnimatorController>();
			//victimsAnimationController.InvincibleAttackAnimation();

			PlatformCharacter victimCharacterScript = playerVictim.getCharacter().getGameObject().GetComponent<PlatformCharacter>();
			victimCharacterScript.InvincibleAttackVictim();
			
			AddKill(playerAttacker, playerVictim);
        }
    }


	
	public void BulletHit(GameObject attacker, GameObject victim)
	{
		if(myNetworkView != null || Network.peerType != NetworkPeerType.Disconnected)
		{
			if(!Network.isServer)
			{
				return;
			}
		}
		// wird nur von PhotonMasterClient ausgeführt....
		
		if(GameState.currentState == GameState.States.Running)
		{
			NetworkPlayer attackersRealOwner = attacker.GetComponent<RealOwner>().owner;
			NetworkPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;
			
			myNetworkView.RPC("SyncBulletHitAnimation", RPCMode.All, attackersRealOwner, victimsRealOwner);
			myNetworkView.RPC("SyncBulletHit", RPCMode.AllBuffered, attackersRealOwner, victimsRealOwner);
		}
		else
		{
			Debug.LogWarning("current GameState = " + GameState.currentState.ToString() + " BulletHit zählt nicht!");
		}
	}

	[RPC]
	public void SyncBulletHitAnimation(NetworkPlayer attackersRealOwner, NetworkPlayer victimsRealOwner)
	{
		if(GameState.currentState == GameState.States.Running)
		{
			Player playerAttacker = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(attackersRealOwner);
			Player playerVictim = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(victimsRealOwner);
			
			//AnimatorController victimsAnimationController = playerVictim.getCharacter().getGameObject().GetComponent<AnimatorController>();
			//victimsAnimationController.InvincibleAttackAnimation();

			PlatformCharacter victimCharacterScript = playerVictim.getCharacter().getGameObject().GetComponent<PlatformCharacter>();
			victimCharacterScript.InvincibleAttackVictim();
			
			//AddKill(playerAttacker, playerVictim);
		}
	}

	[RPC]
	public void SyncBulletHit(NetworkPlayer attackersRealOwner, NetworkPlayer victimsRealOwner)
	{
		Player playerAttacker = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(attackersRealOwner);
		Player playerVictim = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(victimsRealOwner);
		AddKill(playerAttacker, playerVictim);
	}
}
