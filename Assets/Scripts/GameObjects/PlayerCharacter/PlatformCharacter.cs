using UnityEngine;
using System.Collections;

public class PlatformCharacter : MonoBehaviour {

	// the position read from the network
	// used for interpolation
	private Vector3 readNetworkPos;
	// whether this paddle can accept player input
//	public bool AcceptsInput = true;
	public RealOwner realOwner;
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
//	public float groundRadius = 0.2f;	// Size of the Circle @rround the Checkposition 
	public float wallRadius = 0.1f;	// Size of the Circle @rround the Checkposition
//	public LayerMask whatIsGround;	// Floor, JumpAblePlatform, DestroyAblePlatform 
//	public LayerMask whatIsWall;	// Floor, JumpAblePlatform, DestroyAblePlatform
	
	/** 
	 * Character Status 
	 **/
	public bool isHit = false;
	public bool controlsEnabled = true;
	public bool isDead = false;					// is Player currently dead?
	public bool jumpAllowed = true;				// denies/allows player&bots to jump
	public bool moveAllowed = true;				// denies/allows player&bots to move horizontally
//	public bool isInJumpAbleSaveZone = false;	// is Player currently in save Zone (prevent's colliding with Platform) 
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
	private Vector2 jumpForce = new Vector2(10.0F, 10.0F);	// jump Force : wall jump, jump

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
//	private GameSceneManager gameSceneManager;

	/**
	 * Connection to other Body parts
	 **/
	private BoxCollider2D bodyCollider2D;
	private BoxCollider2D headCollider2D;
	private BoxCollider2D feetCollider2D;
	private SpriteRenderer spriteRenderer;

	public void setMaxSpeed(float newMaxSpeed)
	{
		maxSpeed = newMaxSpeed;
	}

	public float getMaxSpeed()
	{
		return maxSpeed;
	}

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		bodyCollider2D = GetComponent<BoxCollider2D>();
		headCollider2D = transform.FindChild(Tags.head).GetComponent<BoxCollider2D>();
		feetCollider2D = transform.FindChild(Tags.feet).GetComponent<BoxCollider2D>();

		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		layer = gameController.GetComponent<Layer>();
//		gameSceneManager = gameController.GetComponent<GameSceneManager>();

		//LayerMasks();	// <-- wichtig in Start... Awake ist zu früh
	}

//	void LayerMasks()
//	{
//		whatIsGround = 1 << layer.floor;
//		whatIsGround |= 1 << layer.jumpAblePlatform;
//		whatIsGround |= 1 << layer.destroyAbleBlock;
//	}

	void Start() {
		anim = GetComponent<Animator>();
		realOwner = GetComponent<RealOwner>();
//		LayerMasks();

		// if this is our paddle, it accepts input
		// otherwise, if it is someone else’s paddle, it does not
//		if(Network.peerType == NetworkPeerType.Disconnected)
//			AcceptsInput = true;
//		else
//			AcceptsInput = networkView.isMine;
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
	}


	// FixedUpdate is called once per frame
	void FixedUpdate () {
		
		//Actually move the player using his/her input
		if(isDead)
		{
			rigidbody2D.velocity = new Vector2(0f,rigidbody2D.velocity.y);
			inputJump = false;
			inputVelocity = 0f;
			isBouncing = false;
		}
		else
		{
			CheckPosition();
			SetAnim();
			FixedMove();							//Jump, Wall-Jump, rechts, links Bewegung
		}
	}

	void CheckPosition()
	{
		//playerPos spriterenderer boundaries
		Vector2 playerPos = new Vector2(rigidbody2D.transform.position.x, rigidbody2D.transform.position.y);

		Vector2 groundedOffset = new Vector2(0f,0.5f);

		Vector2 playerColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider2D.size.x*0.5f + bodyCollider2D.center.x,
		                                               transform.position.y);	// Collider Top Left
		
		Vector2 playerColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider2D.size.x*0.5f + bodyCollider2D.center.x,
		                                                   transform.position.y - spriteRenderer.bounds.extents.y*1.2f);	// Collider Bottom Right

		Vector2 playerColliderTopRightPos = new Vector2(transform.position.x + bodyCollider2D.size.x*0.5f + bodyCollider2D.center.x,
		                                               transform.position.y);	// Collider Top Right
		
		Vector2 playerColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider2D.size.x*0.5f + bodyCollider2D.center.x,
		                                                   transform.position.y - spriteRenderer.bounds.extents.y*1.2f);	// Collider Bottom Left

		Debug.DrawLine(playerColliderTopLeftPos, playerColliderBottomRightPos, Color.yellow);
		Debug.DrawLine(playerColliderBottomLeftPos, playerColliderTopRightPos, Color.yellow);
		Debug.DrawLine(playerColliderTopLeftPos, playerColliderTopRightPos, Color.yellow);
		Debug.DrawLine(playerColliderBottomLeftPos, playerColliderBottomRightPos, Color.yellow);

		grounded = Physics2D.OverlapArea(playerColliderTopLeftPos, playerColliderBottomRightPos, layer.whatIsGround);
//		if(gameObject.layer == layer.player1)
//			Debug.Log(gameObject.name + " grounded: " + grounded);
		//grounded = Physics2D.OverlapCircle(playerPos+groundCheckPosition, groundRadius, layer.whatIsGround);
//		Debug.DrawLine(playerPos, playerPos + groundCheckPosition + new Vector2(0,-groundRadius), Color.green);

		walled = Physics2D.OverlapCircle(playerPos+wallCheckPosition, wallRadius, layer.whatIsWall);
		Debug.DrawLine(playerPos, playerPos+wallCheckPosition + 1*transform.localScale.x * new Vector2(wallRadius,0), Color.green);
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
//			if(gameObject.name.StartsWith("Kirby"))
//		   	{	
//				Debug.Log(gameObject.name + ": " + rigidbody2D.velocity);
//			}
		}
	}


	void FixedMove()
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
//		rigidbody2D.AddForce( new Vector2(inputVelocity,0));
		rigidbody2D.velocity = new Vector2(inputVelocity, rigidbody2D.velocity.y);
		
//		/**
//		 * Animator status Update
//		 **/
//		if(anim == null)
//		{
//			Debug.LogError("Animator not set");
//		}
//		else
//		{
//			anim.SetFloat(hash.hSpeedFloat, inputVelocity);					// BUG! in Mulitplayer ist input nicht von jedem Spieler gesetzt!!!!
//		}
			
		/**
		 * Check Direction Change
		 **/
		if(rigidbody2D.velocity.x > 0f && !facingRight)						// BUG Input!!!
		{
			Flip();
		}
		else if(rigidbody2D.velocity.x < 0f && facingRight)					// BUG Input!!!
		{
			Flip();
		}
		else
		{
			changedRunDirection = false;
		}
		
		if(grounded && inputJump) {
			// Do Jump
			if(jumpSound != null)
				AudioSource.PlayClipAtPoint(jumpSound,transform.position,1);				//JumpSound
			else
				Debug.LogError("jumpSound nicht gesetzt!");
			if(anim == null)
			{
				Debug.LogError("Animator not set");
			}
			else
			{
				anim.SetBool(hash.groundedBool,false);
			}
			// JumpOnAblePlatforms neues Design erlaubt Force Ray nach oben deaktiviert alle getroffenen Platformen
			// Ray -> wird zu Box höher und breiter als Character, -> JumpingSaveZone wird autom. gewährleistet! einfacher Levelbau
			// JumpingSaveZone verhindert das Anstoßen an Kanten der Platform
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
			if(changeRunDirectionSound != null)
				AudioSource.PlayClipAtPoint(changeRunDirectionSound,transform.position,1);				//ChangeDirection
			else
				Debug.LogError("change run direction sound fehlt!");
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

	[RPC]
	void DeactivateKinematic()
	{
		gameObject.rigidbody2D.isKinematic = false;
		gameObject.rigidbody2D.WakeUp();
	}
	
	[RPC]
	void ActivateKinematic()
	{
		gameObject.rigidbody2D.isKinematic = true;
		gameObject.rigidbody2D.WakeUp();
	}
}
