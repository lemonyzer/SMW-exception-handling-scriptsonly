using UnityEngine;
using System.Collections;

public abstract class CPlayer : MonoBehaviour {

	/** 
	 * Position Check 
	 **/
	public bool grounded = false;
	public bool walled = false;
	public Vector2 groundCheckPosition = new Vector2(0, -0.5f);	// Position, where the the Ground will be checked
	public Vector2 wallCheckPosition = new Vector2(0.5f, 0); // Position, where the the Wall will be checked
	public float groundRadius = 0.2f;	// Size of the Circle @rround the Checkposition 
	public float wallRadius = 0.1f;	// Size of the Circle @rround the Checkposition
	public LayerMask whatIsGround;	// Floor, JumpAblePlatform, DestroyAblePlatform 
	public LayerMask whatIsWall;	// Floor, JumpAblePlatform, DestroyAblePlatform

	/** 
	 * Player Status 
	 **/
	public bool isDead = false;		// is Player currently dead?
	public bool JumpAllowed = true;	// denies/allows player&bots to jump
	public bool MoveAllowed = true;	// denies/allows player&bots to move horizontally
	public bool isInJumpAbleSaveZone = false;	// is Player currently in save Zone (prevent's colliding with Platform) 
	public bool isBouncing = false;	// move speed changed while bouncing with other player 

	/** 
	 * Player Invetory 
	 **/
	public int slot0 = 0;		// Power Up Slot 1
	public int slot1 = 0;		// Power Up Slot 2

	/** 
	 * Player Sounds 
	 **/
	public AudioClip jumpSound;					// Jump Sound
	public AudioClip changeRunDirectionSound;	// Skid Sound
	public AudioClip wallJumpSound;				// Wall Jump Sound

	/** 
	 * Player Movement 
	 **/
	public Vector2 moveDirection = Vector2.zero;			// stores Input Key horizontal Movement
	public float maxSpeed = 10.0f;							// max horizontal Speed
	public Vector2 jumpForce = new Vector2(10.0F, 14.0F);	// jump Force : wall jump, jump
	public float velocity = 0;
	public bool changedRunDirection = false;				
	public bool inputPCJump = false;							// stores Input Key 
	public bool inputPCMove = false;							// stores Input Key 

	/** 
	 * Player Animation 
	 **/
	public bool facingRight = true;							// keep DrawCalls low, Flip textures scale: texture can be used for both directions 
	public Animator anim;									// Animator State Machine

	/** 
	 * Connection with GameController 
	 **/
	public GameObject gameController;
	public HashID hash;

	public CPlayer() {
	
	}
}
