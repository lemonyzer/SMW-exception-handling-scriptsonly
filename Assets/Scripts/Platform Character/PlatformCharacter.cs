using UnityEngine;
using System.Collections;

public class PlatformCharacter : MonoBehaviour {

	/**
	 * Debugging GUI Element
	 **/
	public GUIText debugging;
	private string debugmsg="";

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
	public Animator anim;									// Animator State Machine
	public bool facingRight = true;							// keep DrawCalls low, Flip textures scale: texture can be used for both directions 
	public bool changedRunDirection = false;
	
	/** 
	 * Connection to GameController 
	 **/
	public GameObject gameController;
	public HashID hash;
	
	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
	}
	
	void Start() {
		anim = GetComponent<Animator>();
		isInJumpAbleSaveZone=false;
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
				if(this.gameObject.layer == 11)
				{
					isInRageModus = true;
					anim.SetBool(hash.rageModusBool,true);
					anim.SetTrigger(hash.rageTrigger);
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
		anim.SetBool(hash.rageModusBool,false);

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
		if(anim != null)
		{
			anim.SetBool(hash.groundedBool, grounded);
			anim.SetBool(hash.walledBool, walled);
			anim.SetFloat(hash.vSpeedFloat, rigidbody2D.velocity.y);
			anim.SetFloat(hash.hSpeedFloat, rigidbody2D.velocity.x);
		}
		else
			Debug.LogError("Animator not set");
		
	}


	void FixedMove()
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
		if(anim != null)
		{
			anim.SetFloat(hash.hSpeedFloat, inputVelocity);
		}
		else
			Debug.LogError("Animator not set");
		
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
			anim.SetBool(hash.groundedBool,false);
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,jumpForce.y);		//<--- besser für JumpAblePlatforms	
			//rigidbody2D.AddForce(new Vector2(0.0F, jumpForce.y));						//<--- klappt nicht 100% mit JumpAblePlatforms
			
		}
		else if(!grounded && walled && inputJump) {
			// Do WallJump
			AudioSource.PlayClipAtPoint(wallJumpSound,transform.position,1);			//WallJump
			rigidbody2D.velocity = new Vector2(0,0);									//alte Geschwindigkeit entfernen
			Flip();																		//Charakter drehen 
			anim.SetBool(hash.groundedBool,false);
			anim.SetBool(hash.walledBool,false);
			rigidbody2D.velocity = new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y);	//<--- besser für JumpAblePlatforms
		}
		
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
			anim.SetTrigger(hash.changeRunDirectionTrigger);	// Start Change Run Direction Animation
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
				//				Debug.LogWarning(gameObject.name + ": JumpAblePlatform Collision: Off!" + gameObject.layer);
				//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),gameObject.layer,true);
				Physics2D.IgnoreLayerCollision(18,gameObject.layer,true);
			}
			else if(rigidbody2D.velocity.y <0.1F)
			{
				//				Debug.LogWarning(gameObject.name + ": JumpAblePlatform Collision: On!" + gameObject.layer);
				//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),gameObject.layer,false);
				Physics2D.IgnoreLayerCollision(18,gameObject.layer,false);
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if(isInRageModus)
		{
			if(this.gameObject.layer != other.gameObject.layer)
			{
				bool enemyObject = false;
				if(other.gameObject.layer == LayerMask.NameToLayer("Player1"))
				{
					enemyObject = true;
				}
				else if(other.gameObject.layer == LayerMask.NameToLayer("Player2"))
				{
					enemyObject = true;
				}
				else if(other.gameObject.layer == LayerMask.NameToLayer("Player3"))
				{
					enemyObject = true;
				}
				else if(other.gameObject.layer == LayerMask.NameToLayer("Player4"))
				{
					enemyObject = true;
				}
//				else if(other.gameObject.layer == LayerMask.NameToLayer("PowerUpEnemy"))
//				{
//					enemyObject = true;
//				}

				if(enemyObject)
					other.gameObject.transform.Find(Tags.head).GetComponent<HealthController>().ApplyDamage(1.0f,true);
			}
		}
		
	}
}
