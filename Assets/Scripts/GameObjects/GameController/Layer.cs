﻿using UnityEngine;
using System.Collections;

public class Layer : MonoBehaviour {
	
	// Physic Layer
	
	public LayerMask allPlayer;
	public LayerMask whatIsStaticGround;
	public LayerMask whatIsJumpOnPlatform;
	public LayerMask whatIsAllGround;
	public LayerMask whatIsWall;

	public int player;

//	public int player1;
//	public int player2;
//	public int player3;
//	public int player4;
	
//	public int enemy;
	public int feet;
	public int head;
	public int body;
	public int item;

	public int ground;
//	public int tagAble;
//	public int floor;
	public int block;
	public int jumpAblePlatform;
//	public int jumpAblePlatformSaveZone;
	
	public int powerUp;
//	public int bullet;
	
	public int groundStopper;
	
//	public int fader;

	public const string playerLayerName = "Player";

//	public const string player1LayerName = "Player1";
//	public const string player2LayerName = "Player2";
//	public const string player3LayerName = "Player3";
//	public const string player4LayerName = "Player4";
//	
	public const string feetLayerName = "Feet";
	public const string headLayerName = "Head";
	public const string bodyLayerName = "Body";
	public const string itemLayerName = "Item";
	public const string powerUpLayerName= "PowerUp";

//	public const string enemyLayerName = "Enemy";

	public const string groundLayerName = "Ground";
//	public const string tagAbleLayerName = "TagAble";
//	public const string floorLayerName = "Floor";
	public const string blockLayerName = "Block";
	public const string jumpAblePlatformLayerName = "JumpOnPlatform";
//	public const string jumpAblePlatformSaveZoneLayerName = "JumpSaveZone";

//	public const string bulletLayerName = "Bullet";
	public const string groundStopperLayerName = "GroundStopper";
	
//	public const string faderLayerName = "Fader";
	
	void Awake()
	{
		Debug.LogWarning(this.ToString() + ": Awake() - init public layer integers, scripts layer instantiation have to be AFTER this initialisation, NOT IN AWAKE!!!" );
		player = LayerMask.NameToLayer(playerLayerName);
//		player1 = LayerMask.NameToLayer(player1LayerName);
//		player2 = LayerMask.NameToLayer(player2LayerName);
//		player3 = LayerMask.NameToLayer(player3LayerName);
//		player4 = LayerMask.NameToLayer(player4LayerName);

		allPlayer = 1 << player;

//		allPlayer = 1 << player1;
//		allPlayer |= 1 << player2;
//		allPlayer |= 1 << player3;
//		allPlayer |= 1 << player4;
		
		feet = LayerMask.NameToLayer(feetLayerName);
		head = LayerMask.NameToLayer(headLayerName);
		body = LayerMask.NameToLayer(bodyLayerName);
		item = LayerMask.NameToLayer(itemLayerName);
		groundStopper = LayerMask.NameToLayer(groundStopperLayerName);
		powerUp = LayerMask.NameToLayer(powerUpLayerName);

//		bullet = LayerMask.NameToLayer(bulletLayerName);

		
//		enemy = LayerMask.NameToLayer(enemyLayerName);

		ground = LayerMask.NameToLayer(groundLayerName);
//		tagAble = LayerMask.NameToLayer(tagAbleLayerName);
//		floor = LayerMask.NameToLayer(floorLayerName);
		block = LayerMask.NameToLayer(blockLayerName);
		jumpAblePlatform = LayerMask.NameToLayer(jumpAblePlatformLayerName);
//		jumpAblePlatformSaveZone = LayerMask.NameToLayer(jumpAblePlatformSaveZoneLayerName);

		whatIsStaticGround = 1 << ground;
//		whatIsGround |= 1 << tagAble;
//		whatIsGround = 1 << floor;
		whatIsStaticGround |= 1 << block;

		whatIsJumpOnPlatform = 1 << jumpAblePlatform;

		whatIsAllGround = whatIsStaticGround;
		whatIsAllGround |= 1 << jumpAblePlatform;

		whatIsWall = whatIsStaticGround;
		

//		fader = LayerMask.NameToLayer(faderLayerName);
	}

//	void OnLevelWasLoaded()
//	{
//		Debug.LogWarning(this.ToString() + ": OnLevelWasLoaded()" );
//	}
//
//	void Start()
//	{
//		Debug.LogWarning(this.ToString() + ": Start()" );
//	}
	
}
