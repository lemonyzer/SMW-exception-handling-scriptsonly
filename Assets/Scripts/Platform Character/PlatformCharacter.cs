﻿using UnityEngine;
using System.Collections;

public class PlatformCharacter : MonoBehaviour {

	// the position read from the network
	// used for interpolation
	private Vector3 readNetworkPos;
	// whether this paddle can accept player input
	public bool AcceptsInput = true;
//	public float gravity=10;

	/**
	 * Debugging GUI Element
	 **/
	public GUIText debugging;
//	private string debugmsg="";

	/** 
	 * Character Position Check 
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
	 * Character Status 
	 **/
	public bool controlsEnabled = true;
	public bool isDead = false;					// is Player currently dead?
	public bool jumpAllowed = true;				// denies/allows player&bots to jump
	public bool moveAllowed = true;				// denies/allows player&bots to move horizontally
	public bool isInJumpAbleSaveZone = false;	// is Player currently in save Zone (prevent's colliding with Platform) 
	public bool isBouncing = false;				// move speed changed while bouncing with other player
	public bool isInRageModus = false;
	
	/** 
	 * Character Inventory 
	 **/
	public int inventorySlot0 = 0;		// Power Up Slot 1
	public int inventorySlot1 = 0;		// Power Up Slot 2
	
	/** 
	 * Character Sounds 
	 **/
	public AudioClip jumpSound;					// Jump Sound
	public AudioClip changeRunDirectionSound;	// Skid Sound
	public AudioClip wallJumpSound;				// Wall Jump Sound
	
	/** 
	 * Character Movement 
	 **/
	private float maxSpeed = 8.0f;							// max horizontal Speed
	private Vector2 jumpForce = new Vector2(10.0F, 14.0F);	// jump Force : wall jump, jump

	/// <summary>
	/// The user input.
	/// </summary>
	/// 
	private float inputVelocity = 0f;
	private bool inputJump = false;	
	private float inputPCVelocity = 0f;
	private bool inputPCJump = false;							// stores Input Key 
	private float inputTouchVelocity = 0f;
	private bool inputTouchJump = false;							// stores Input Key 

	/// <summary>
	/// The game movement physics.
	/// </summary>
	public float pushForce;
	public float pushTime = 0f;

	/** 
	 * Character Animation 
	 **/
//	private SpriteController spriteController;
	public Animator anim;									// Animator State Machine
	public bool facingRight = true;							// keep DrawCalls low, Flip textures scale: texture can be used for both directions 
	public bool changedRunDirection = false;
	
	/** 
	 * Connection to GameController 
	 **/
	private GameObject gameController;
	private HashID hash;
	private Layer layer;
	private StatsManager statsManager;
	
	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		layer = gameController.GetComponent<Layer>();
		statsManager = gameController.GetComponent<StatsManager>();
	}
	
	void Start() {
		anim = GetComponent<Animator>();
		isInJumpAbleSaveZone=false;

		// if this is our paddle, it accepts input
		// otherwise, if it is someone else’s paddle, it does not
		if(Network.peerType == NetworkPeerType.Disconnected)
			AcceptsInput = true;
		else
			AcceptsInput = networkView.isMine;
	}
	
	// The Move function is designed to be called from a separate component
	// based on User input, or an AI control script
	public void MoveKeyboard(float moveHorizontal, bool jump)
	{
		this.inputPCVelocity = moveHorizontal;
		this.inputPCJump = jump;
	}

	// The Move function is designed to be called from a separate component
	// based on User input, or an AI control script
	public void MoveTouch(float moveHorizontal, bool jump)
	{
		this.inputTouchVelocity = moveHorizontal;
		this.inputTouchJump = jump;
	}

	void Update()
	{
		if(InventoryManager.inventory != null)
		{
			if(InventoryManager.inventory.GetItems("Star(Clone)") > 0f)
			{
				if(this.gameObject.layer == layer.player1)
				{
					isInRageModus = true;
					if(anim == null)
					{
					}
					else
					{
						anim.SetBool(hash.rageModusBool,true);
						anim.SetTrigger(hash.rageTrigger);
					}
					Debug.LogError("isInRageModus: On");
					InventoryManager.inventory.SetItems("Star(Clone)",0f);
					StartCoroutine(RageTime());

				}
			}
		}
	}

	IEnumerator RageTime()
	{
		yield return new WaitForSeconds(8.0f);
		isInRageModus = false;
		Debug.LogError("isInRageModus: Off");
		if(anim == null)
		{
		}
		else
		{
			anim.SetBool(hash.rageModusBool,false);
		}

		//anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		//AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
	}

	// FixedUpdate is called once per frame
	void FixedUpdate () {
		
		//Actually move the player using his/her input
		if(!isDead)
		{
			CheckPosition();
			SetAnim();
			FixedMove();							//Jump, Wall-Jump, rechts, links Bewegung					
			JumpAblePlatform();
		}
	}

	void CheckPosition()
	{
		Vector2 playerPos = new Vector2(rigidbody2D.transform.position.x, rigidbody2D.transform.position.y);
		
		grounded = Physics2D.OverlapCircle(playerPos+groundCheckPosition, groundRadius, whatIsGround);
		Debug.DrawLine(playerPos,playerPos+groundCheckPosition,Color.green);

		walled = Physics2D.OverlapCircle(playerPos+wallCheckPosition, wallRadius, whatIsWall);
		Debug.DrawLine(playerPos,playerPos+wallCheckPosition,Color.green);
	}

	void SetAnim() 
	{
		if(anim == null)
		{
			Debug.LogError("Animator not set");
		}
		else
		{
			anim.SetBool(hash.groundedBool, grounded);
			anim.SetBool(hash.walledBool, walled);
			anim.SetFloat(hash.vSpeedFloat, rigidbody2D.velocity.y);
			anim.SetFloat(hash.hSpeedFloat, rigidbody2D.velocity.x);
		}
	}


	void FixedMove()
	{
//		 does not accept input, interpolate network pos
//		 jetzt über NetworkRigidBody2D
		if( !AcceptsInput )
		{
			//transform.position = Vector3.Lerp( transform.position, readNetworkPos, 100f * Time.deltaTime );
//			transform.position = readNetworkPos;
			// don’t use player input
			inputJump = false;
			inputVelocity = 0f;
		}
		else
		{
			if(controlsEnabled)
			{
				inputJump = inputPCJump || inputTouchJump;
				if(!jumpAllowed)
					inputJump = false;
				
				inputVelocity = inputPCVelocity + inputTouchVelocity;
				if(!moveAllowed)
					inputVelocity = 0f;
				
				if(inputVelocity > 1f)
					inputVelocity = 1f;
				else if(inputVelocity < -1f)
					inputVelocity = -1f;
			}
			else
			{
				inputJump = false;
				inputVelocity = 0f;
			}
		}




		if(isBouncing)
		{
			//Alte Kraft in X Richtung wird nicht komplett überschrieben!
			
			if(pushForce > 0f)
			{
				if(inputVelocity > 0f)
				{
					inputVelocity *= maxSpeed;		// wenn Spieler in die gleiche Richtung wie pushForce sich bewegt,
					// volle Geschwindigkeit nehmen  
				}
				else
					inputVelocity *= maxSpeed * 0.2f;
			}
			else if(pushForce < 0f)
			{
				if(inputVelocity < 0f)
				{
					inputVelocity *= maxSpeed;
				}
				else
					inputVelocity *= maxSpeed * 0.2f;
			}
			
			pushTime += Time.deltaTime;
			float pushSpeed;
			pushForce = pushForce - (pushForce * 4f * Time.deltaTime);
			pushSpeed = pushForce;
			//			Debug.LogError(this.gameObject.transform.name+ " pushSpeed = " + pushSpeed);
			if(Mathf.Abs(pushSpeed) < 1)
			{
				//				Debug.LogError(this.gameObject.transform.name+ " pushSpeed = 0");
				isBouncing = false;
				pushTime = 0f;
			}
			else
			{
				inputVelocity += pushSpeed;
				//				Debug.LogError(this.gameObject.transform.name+ " inputVelocity = " + inputVelocity);
			}
			
		}
		else // if(!isBouncing)
		{
			inputVelocity *= maxSpeed;
		}
		
		/**
		 * maxSpeed check
		 **/
		if(Mathf.Abs(inputVelocity) > maxSpeed)
		{
			// neue inputVelocity überschreitet maxSpeed!!!
			if(inputVelocity < (-1.0f*maxSpeed))
			{
				inputVelocity = -1.0f*maxSpeed;
				//rigidbody2D.velocity = new Vector2((-1.0f)*maxSpeed, rigidbody2D.velocity.y);
			}
			else if(inputVelocity > maxSpeed)
			{
				inputVelocity = maxSpeed;
				//rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
			}
		}
		
		// gedrosselte velocity übernehmen
		rigidbody2D.velocity = new Vector2(inputVelocity, rigidbody2D.velocity.y);
		
		/**
		 * Animator status Update
		 **/
		if(anim == null)
		{
			Debug.LogError("Animator not set");
		}
		else
		{
			anim.SetFloat(hash.hSpeedFloat, inputVelocity);
		}
			
		/**
		 * Check Direction Change
		 **/
		if(inputVelocity > 0f && !facingRight)
		{
			Flip();
		}
		else if(inputVelocity < 0f && facingRight)
		{
			Flip();
		}
		else
		{
			changedRunDirection = false;
		}
		
		if(grounded && inputJump) {
			// Do Jump
			AudioSource.PlayClipAtPoint(jumpSound,transform.position,1);				//JumpSound
			if(anim == null)
			{
				Debug.LogError("Animator not set");
			}
			else
			{
				anim.SetBool(hash.groundedBool,false);
			}
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,jumpForce.y);		//<--- besser für JumpAblePlatforms	
			//rigidbody2D.AddForce(new Vector2(0.0F, jumpForce.y));						//<--- klappt nicht 100% mit JumpAblePlatforms
			
		}
		else if(!grounded && walled && inputJump) {
			// Do WallJump
			AudioSource.PlayClipAtPoint(wallJumpSound,transform.position,1);			//WallJump
			rigidbody2D.velocity = new Vector2(0,0);									//alte Geschwindigkeit entfernen
			Flip();																		//Charakter drehen
			if(anim == null)
			{
				Debug.LogError("Animator not set");
			}
			else
			{
				anim.SetBool(hash.groundedBool,false);
				anim.SetBool(hash.walledBool,false);
			}
			rigidbody2D.velocity = new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y);	//<--- besser für JumpAblePlatforms
		}
//		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,rigidbody2D.velocity.y-gravity);		//<--- besser für JumpAblePlatforms	
		
	}
	
	public void StartJump() {
		if(jumpAllowed)
			inputJump = true;
	}
	
	public void StopJump() {
		inputJump = false;
	}
	
	
	void Flip() {
		
		// Drift sound abspielen
		if(grounded)
		{
			changedRunDirection = true;
			if(anim == null)
			{
				Debug.LogError("Animator not set");
			}
			else
			{
				anim.SetTrigger(hash.changeRunDirectionTrigger);	// Start Change Run Direction Animation
			}
			AudioSource.PlayClipAtPoint(changeRunDirectionSound,transform.position,1);				//ChangeDirection
		}
		
		// Richtungvariable anpassen
		facingRight = !facingRight;
		
		// WallCheck anpassen
		wallCheckPosition *= -1;
		
		// Transform spiegeln
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		
	}
	
	/**
	 * 
	 * Wird extra abgefragt, da Spieler auch ohne selbst zu Springen eine positive vertikale Geschwindigkeit bekommen können
	 * zB.: steht auf Platform, Gegenspieler springt von unten an die Füße => Spieler macht automatischen Sprung
	 * 
	 **/
	void JumpAblePlatform()
	{
		if(!isInJumpAbleSaveZone)
		{
			//			Debug.LogWarning(gameObject.name + ": velocity.y=" + rigidbody2D.velocity.y);
			if(rigidbody2D.velocity.y >0.1F)
			{
				Physics2D.IgnoreLayerCollision(layer.jumpAblePlatform,gameObject.layer,true);		// Kollisionsdetection ausschalten
			}
			else if(rigidbody2D.velocity.y <0.1F)
			{
				Physics2D.IgnoreLayerCollision(layer.jumpAblePlatform,gameObject.layer,false);		// Kollisionsdetection einschalten
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if(Network.isServer)
		{

			if(isInRageModus)
			{
				if(this.gameObject.layer != other.gameObject.layer)										// Spieler aus eigenem Team(layer) nicht zerstören
				{
					bool enemyObject = false;
					if(other.gameObject.layer == layer.player1)
					{
						enemyObject = true;
					}
					else if(other.gameObject.layer == layer.player2)
					{
						enemyObject = true;
					}
					else if(other.gameObject.layer == layer.player3)
					{
						enemyObject = true;
					}
					else if(other.gameObject.layer == layer.player4)
					{
						enemyObject = true;
					}
	//				else if(other.gameObject.layer == layer.powerUp)
	//				{
	//					enemyObject = true;
	//				}

					if(enemyObject)
					{
						//networkView.RPC
						//other.gameObject.GetComponent<NetworkView>().RPC(
						statsManager.InvincibleAttack(this.gameObject, other.gameObject);			// Layerfilter -> wir sind auf PlatformCharacter ebene (nicht im child feet/head)
						//other.gameObject.GetComponent<HealthController>().ApplyDamage(this.gameObject, 1 ,true);
					}
				}
			}
		}
		
	}
}
