using UnityEngine;
using System.Collections;

public class Layer : MonoBehaviour {

	// Physic Layer

	public LayerMask allPlayer;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;

	public int player1;
	public int player2;
	public int player3;
	public int player4;

	public int enemy;
	public int feet;
	public int head;

	public int floor;
	public int destroyAbleBlock;
	public int jumpAblePlatform;
	public int jumpAblePlatformSaveZone;
	
	public int powerUp;

	public int groundStopper;

	public int fader;

	public const string player1LayerName = "Player1";
	public const string player2LayerName = "Player2";
	public const string player3LayerName = "Player3";
	public const string player4LayerName = "Player4";

	public const string feetLayerName = "Feet";
	public const string headLayerName = "Head";

	public const string enemyLayerName = "Enemy";
	
	public const string floorLayerName = "Floor";
	public const string destroyAbleBlockLayerName = "DestroyAbleBlock";
	public const string jumpAblePlatformLayerName = "JumpAblePlatform";
	public const string jumpAblePlatformSaveZoneLayerName = "JumpAblePlatformSaveZone";

	public const string powerUpLayerName = "PowerUp";
	public const string groundStopperLayerName = "GroundStopper";

	public const string faderLayerName = "Fader";

	void Awake()
	{
		player1 = LayerMask.NameToLayer(player1LayerName);
		player2 = LayerMask.NameToLayer(player2LayerName);
		player3 = LayerMask.NameToLayer(player3LayerName);
		player4 = LayerMask.NameToLayer(player4LayerName);

		allPlayer = 1 << player1;
		allPlayer |= 1 << player2;
		allPlayer |= 1 << player3;
		allPlayer |= 1 << player4;

		feet = LayerMask.NameToLayer(feetLayerName);
		head = LayerMask.NameToLayer(headLayerName);

		enemy = LayerMask.NameToLayer(enemyLayerName);

		floor = LayerMask.NameToLayer(floorLayerName);
		destroyAbleBlock = LayerMask.NameToLayer(destroyAbleBlockLayerName);
		jumpAblePlatform = LayerMask.NameToLayer(jumpAblePlatformLayerName);
		jumpAblePlatformSaveZone = LayerMask.NameToLayer(jumpAblePlatformSaveZoneLayerName);

		whatIsGround = 1 << floor;
		whatIsGround |= 1 << jumpAblePlatform;
		whatIsGround |= 1 << destroyAbleBlock;
		
		whatIsWall = whatIsGround;

		powerUp = LayerMask.NameToLayer(powerUpLayerName);
		groundStopper = LayerMask.NameToLayer(groundStopperLayerName);
		fader = LayerMask.NameToLayer(faderLayerName);
	}


}
