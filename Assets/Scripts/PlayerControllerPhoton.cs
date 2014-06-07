using UnityEngine;
using System.Collections;

public class PlayerControllerPhoton : Photon.MonoBehaviour {
	
	//	public bool debug=true;
	//	GUIText guitext;
	//	GUIText guitext2;
	
	public bool isDead = false;
	public bool JumpAllowed=true;
	public bool MoveAllowed=true;
	
	public AudioClip jumpSound;
	public AudioClip changeRunDirectionSound;
	public AudioClip wallJumpSound;
	
	public Vector2 moveDirection = Vector2.zero;
	//CharacterController characterController;
	//public float gravity=10;
	public float maxSpeed = 10.0f;
	public Vector2 jumpForce = new Vector2(10.0F, 14.0F);
	float velocity = 0;
	public bool inputJump = false;
	public bool inputMove = false;
	bool facingRight = true;
	
	Animator anim;
	
	public bool grounded = false;
	public bool walled = false;
	public Vector2 groundCheckPosition = new Vector2(0, -0.5f);
	public Vector2 wallCheckPosition = new Vector2(0.5f, 0);
	public Transform groundCheck;
	public Transform wallCheck;
	float groundRadius = 0.2f;
	float wallRadius = 0.1f;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;
	
	
//	/* Android */
//	public Touch myTouch;
//	float deltaX=0;
//	/* Input */
//	bool inputTouchJump;
//	public float rangeX = Screen.width*0.125f;	//Pixelbreite  Bildschirmauflösung/2 /2
//	float touchBeganPositionX=0;
//	float touchBeganPositionY=0;
//	bool buttonIsPressed = false;
//	bool stickIsPressed = false;
//	/* / Input */
//	//float deltaY=0;
//	public Button buttonA;
//	public Button buttonB;
//	public AnalogStick leftStick;
//	/* / Android */

	/* Android */
	/* Input */
	bool inputTouchJump;
	bool buttonIsPressed = false;
	int buttonTouchID=-1;
	int buttonTapCount=0;
	bool stickIsPressed = false;
	/* / Input */
	
	//	public Button buttonA;
	//	public Button buttonB;
	//	public AnalogStick leftStick;
	
	Touch analogStick;
	int analogStickTouchID=-1;
	bool analogStickTouchBegan=false;
	
	float touchBeganPositionX;
	float touchBeganPositionY;
	float deltaX=0;
	float deltaY=0;
	
	public GUITexture analogStickTexture;
	public GUITexture stickTexture;
	public float analogStickTextureWidth=0;
	public float analogStickTextureHeight=0;
	
	float textureSizeWithSaveZoneX;
	float textureSizeWithSaveZoneY;
	/* / Android */
	
	void Awake()
	{
		if (!photonView.isMine)
		{
			//We aren't the photonView owner, disable this script
			//RPC's and OnPhotonSerializeView will STILL get trough but we prevent Update from running
			enabled = false;
		}
	}
	
	// Use this for initialization
	void Start () {
		//characterController = GetComponent<CharacterController>();	//aktuelle Componente des Gameobjects zuweisen 
		anim = GetComponent<Animator>();
	}
	
	void Update() {
		if (photonView.isMine)
		{
			if(!isDead)
			{
				InputCheck ();
				InputTouchCheck ();	
				JumpAblePlatform();
			}
		}
	}
	void InputCheck()
	{
		/* Run */
		//Pfeil nach links = -1 
		//Pfeil nach rechts = +1
		//Links, Rechts
		moveDirection.x = Input.GetAxis ("Horizontal");
		if(moveDirection.x != 0)
		{
			inputMove = true;
		}
		else
			inputMove = false;
		
		/* Jump Keyboard */
		//moveDirection.y =  Input.GetAxis("Vertical");
		if (Input.GetKeyDown (KeyCode.Space))
			inputJump = true;
		else
			inputJump = false;
		
		/* Jump Mouse (and Touch) */
		//Achtung: gilt auf für Touch!
		//		if (Input.GetMouseButtonDown (0)) {
		//			Debug.Log ("Achtung bei Touch!!!");
		//			inputJump = true;
		//		}
	}
	void InputTouchCheck() 
	{
		buttonIsPressed=false;
		stickIsPressed=false;

		AnalogStickAndButton();
		stickIsPressed = analogStickTouchBegan;

		if(!stickIsPressed) {
			deltaX = 0.0F;
		}
		inputTouchJump=buttonIsPressed;
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!isDead)
		{
			FixCheckPosition();
			FixSetAnim();
			FixMove();							//Jump, Wall-Jump, rechts, links Bewegung					
			JumpAblePlatform();
		}
	}
	void FixCheckPosition()
	{
		Vector2 playerPos = new Vector2(rigidbody2D.transform.position.x,rigidbody2D.transform.position.y);
		grounded = Physics2D.OverlapCircle (playerPos+groundCheckPosition, groundRadius, whatIsGround);
		walled = Physics2D.OverlapCircle (playerPos+wallCheckPosition, wallRadius, whatIsWall);
		/*
		if(groundCheck != null)
		{
			//Boden unter den Füßen
			grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		}
		else
			Debug.LogError("groundCheck not set");
		if(wallCheck != null)
		{
			//Gesicht an Wand (nur Gesicht, kein Rücken!)
			walled = Physics2D.OverlapCircle (wallCheck.position, wallRadius, whatIsWall);
		}
		else
			Debug.LogError("wallCheck not set");
		*/
	}
	void FixSetAnim() 
	{
		if(anim != null)
		{
			anim.SetBool ("Ground", grounded);
			anim.SetBool ("Wall", walled);
			anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
		}
		else
			Debug.LogError("Animator not set");
		
	}
	void FixMove()
	{
		//rigidbody2D.velocity = new Vector2 (moveDirection.x * maxSpeed, rigidbody2D.velocity.y);
		//Alte Kraft in X Richtung wird ignoriert!
		//velocity enthält alte Kraft -/+
		velocity = (moveDirection.x + deltaX) * maxSpeed;
		//velocity = rigidbody2D.velocity.x + (moveDirection.x + deltaX) * maxSpeed;				//schwammig!!!! bei Flip() Kraftrichtung auch wechseln
		if(anim != null)
		{
			anim.SetFloat("Speed", Mathf.Abs (velocity));
		}
		else
			Debug.LogError("Animator not set");
		//abs für beide Richtungen!!! richtung behalten!!
		if(Mathf.Abs(velocity) < maxSpeed)
		{
			//rigidbody2D.AddForce( new Vector2 (velocity,0));
			rigidbody2D.velocity = new Vector2 (velocity, rigidbody2D.velocity.y);
		}
		else 
		{
			//rigidbody2D.AddForce( new Vector2 ((moveDirection.x + deltaX)*maxSpeed,0));
			rigidbody2D.velocity = new Vector2 ((moveDirection.x + deltaX)*maxSpeed, rigidbody2D.velocity.y);
		}
		if (velocity > 0 && !facingRight)
		{
			Flip ();
		}
		else if (velocity < 0 && facingRight)
		{
			Flip ();
		}
		
		/* mit CharacterController
		//		moveDirection.y -= gravity; // nicht nötig, Physic2D!
		//		if(characterController.isGrounded) 
		//		{
		//			if(inputJump)
		//			{
		//				characterController.Move(moveDirection * Time.deltaTime);
		//			}
		//		}
		*/
		
		if (grounded && (inputJump || inputTouchJump)) {
			//Springen
			AudioSource.PlayClipAtPoint(jumpSound,transform.position,1);				//Jump
			anim.SetBool("Ground",false);
			rigidbody2D.velocity = new Vector2(0.0F,jumpForce.y);								//<--- besser für JumpAblePlatforms
			//rigidbody2D.fixedAngle = false;
			//rigidbody2D.AddTorque(10);
			//rigidbody2D.AddForce(new Vector2(0.0F, jumpForce.y));
			//ForceJumpAblePlatform();
		}
		else if (!grounded && walled && (inputJump || inputTouchJump)) {
			//von Wand wegspringen
			AudioSource.PlayClipAtPoint(wallJumpSound,transform.position,1);				//WallJump
			rigidbody2D.velocity = new Vector2(0,0);		//alte Geschwindigkeit entfernen
			Flip ();										//Charakter drehen 
			anim.SetBool("Wall",false);
			//rigidbody2D.velocity = jumpForce;
			//			rigidbody2D.AddForce(new Vector2(300, 300));
			//			rigidbody2D.AddForce(jumpForce);
			//rigidbody2D.AddForce(new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y)); //Kraft in Richtung localScale.x anwenden
			rigidbody2D.velocity = new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y);								//<--- besser für JumpAblePlatforms
		}
	}
	
	void StartJump() {
		if(JumpAllowed)
			inputJump = true;
	}
	
	void StopJump() {
		inputJump = false;
	}
	
	
	void Flip() {
		// Drift sound abspielen
		if(grounded)
			AudioSource.PlayClipAtPoint(changeRunDirectionSound,transform.position,1);				//ChangeDirection
		
		// Richtungvariable anpassen
		facingRight = !facingRight;
		
		// WallCheck anpassen
		wallCheckPosition *= -1;
		
		// Transform spiegeln
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		
	}
	
	/* Collider2D
	void OnTriggerEnter2D(Collider2D other) {
//		Debug.LogError(other.gameObject.tag);
		if(other.gameObject.tag == "Enemy")
		{
			other.gameObject.SetActive(false);
		}
		if(other.gameObject.tag == "Dead")
		{

		}
		if(other.gameObject.tag == "PlatformTrigger")
		{
			Debug.LogError("Collision Off");
			//other.gameObject.collider2D.enabled = false;
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
			                               LayerMask.NameToLayer("JumpAblePlatform"),
			                               rigidbody2D.velocity.y > 0);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.tag == "PlatformTrigger")
		{
			Debug.LogError("Collision On");
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
			                               LayerMask.NameToLayer("JumpAblePlatform"),
			                               false);
		}
	}
*/
	void JumpAblePlatform()
	{
		if(rigidbody2D.velocity.y >0.0F)
		{
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),true);
			//Physics2D.IgnoreCollision(platform.collider2D, collider2D,true);
		}
		else
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),false);
		//Physics2D.IgnoreCollision(platform.collider2D, collider2D,false);
	}
	
	void ForceJumpAblePlatform()
	{
		Debug.Log("Force Jump-Able-Platform");
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),true);
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//Executed on the owner of this PhotonView; 
			//The server sends it's position over the network
			
			stream.SendNext(transform.position);//"Encode" it, and send it
			
		}
		else
		{
			//Executed on the others; 
			//receive a position and set the object to it
			
			transform.position = (Vector3)stream.ReceiveNext();
			
		}
	}

	void AnalogStickAndButton () {
		buttonIsPressed=false;
		foreach (Touch touch in Input.touches)
		{
			if(!buttonIsPressed)
			{
				if(touch.position.x > (Screen.width * 0.5f))
				{
					buttonTouchID = touch.fingerId;
					if(buttonTapCount < touch.tapCount) {
						Debug.Log("Button: " + buttonTapCount + " new TapCount: " + touch.tapCount); 
						buttonTapCount = touch.tapCount;
						buttonIsPressed=true;
					}
				}
			}
			/*
			 * Touch nach Touchphase auswerten:
			 * 	1. Began
			 *  2. Moved
			 *  3. Stationary
			 *  4. Ended
			 * */
			switch (touch.phase) {
				/* 1. */
			case TouchPhase.Began:
				//				Steuerung reagiert schlecht!
				//
				//				if(touch.position.x > (Screen.width * 0.5f))
				//				{
				//					buttonIsPressed=true;
				//				}
				if(touch.position.x < (Screen.width * 0.5f))
				{
					// Analog Stick gefunden
					analogStick = touch;
					analogStickTouchID = touch.fingerId;
					analogStickTouchBegan=true;
					
					//Finger befindet sich in linker bildschirmhälfte?
					
					//Screen.width/(2*2*2) = Screen.width*0.125
					float texturesizeX = analogStickTextureWidth * 0.5f;
					float texturesizeY = analogStickTextureHeight * 0.5f;
					float savezoneX = texturesizeX*0.5f;
					float savezoneY = texturesizeY*0.5f;
					textureSizeWithSaveZoneX = texturesizeX + savezoneX;
					textureSizeWithSaveZoneY = texturesizeY + savezoneY;
					
					/* X Position checken
						 * 
						 * 
						 * */
					if((touch.position.x > textureSizeWithSaveZoneX) && (touch.position.x < ((Screen.width*0.5)-textureSizeWithSaveZoneX)))
					{
						// X position korrekt (ohne SaveZone)
						touchBeganPositionX = touch.position.x;
					}
					else if(touch.position.x > ((Screen.width*0.5f)-textureSizeWithSaveZoneX))
					{
						// zu weit rechts am Rand
						// X position muss korrigiert werden (ohne SaveZone)
						touchBeganPositionX = ((Screen.width*0.5f)-textureSizeWithSaveZoneX);
					}
					else if(touch.position.x < textureSizeWithSaveZoneX)
					{
						// zu weit links am Rand
						// X position muss korrigiert werden (ohne SaveZone)
						touchBeganPositionX = textureSizeWithSaveZoneX;
					}
					
					/* Y Position checken
						 * 
						 * 
						 * */
					if((touch.position.y > textureSizeWithSaveZoneY) && (touch.position.y < (Screen.height)-textureSizeWithSaveZoneY))
					{
						// alles perfekt
						touchBeganPositionY = touch.position.y;
					}
					else if(touch.position.y > ((Screen.height)-textureSizeWithSaveZoneY))
					{
						// Problem: zu nah am oberen Rand, OFFSET (ohne SaveZone)!
						touchBeganPositionY = Screen.height-textureSizeWithSaveZoneY;
					}
					else if(touch.position.y < textureSizeWithSaveZoneY)
					{
						// Problem: zu nah am unteren Rand, OFFSET (ohne SaveZone)!
						touchBeganPositionY = textureSizeWithSaveZoneY;
					}
					
					
					//Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
					analogStickTexture.pixelInset = new Rect(touchBeganPositionX-analogStickTextureWidth*0.5f,touchBeganPositionY-analogStickTextureHeight*0.5f,analogStickTextureWidth,analogStickTextureHeight);
					Rect pixelInset = new Rect(touch.position.x-stickTexture.pixelInset.width*0.5f,touch.position.y-stickTexture.pixelInset.height*0.5f,stickTexture.pixelInset.width,stickTexture.pixelInset.height);
					stickTexture.pixelInset = pixelInset;
				}
				break;
				/* 2. */
			case TouchPhase.Moved:
				if(touch.fingerId == analogStickTouchID && analogStickTouchBegan) 			/// needed??
				{
					float stickPosX=0;
					float stickPosY=0;
					//Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
					if(touch.position.x > touchBeganPositionX + analogStickTextureWidth*0.5f)
						stickPosX=touchBeganPositionX + analogStickTextureWidth*0.5f;
					
					else if(touch.position.x < touchBeganPositionX - analogStickTextureWidth*0.5f)
						stickPosX=touchBeganPositionX - analogStickTextureWidth*0.5f;
					
					else
						stickPosX = touch.position.x;
					
					if(touch.position.y > touchBeganPositionY + analogStickTextureHeight*0.5f)
						stickPosY=touchBeganPositionY + analogStickTextureHeight*0.5f;
					
					else if(touch.position.y < touchBeganPositionY - analogStickTextureHeight*0.5f)
						stickPosY=touchBeganPositionY - analogStickTextureHeight*0.5f;
					
					else
						stickPosY = touch.position.y;
					
					Rect pixelInset = new Rect(stickPosX-stickTexture.pixelInset.width*0.5f,stickPosY-stickTexture.pixelInset.height*0.5f,stickTexture.pixelInset.width,stickTexture.pixelInset.height);
					stickTexture.pixelInset = pixelInset;
					
					deltaX = (touch.position.x - touchBeganPositionX)/(analogStickTextureWidth*0.5f);
					if(deltaX > 1.0f)
						deltaX = 1.0f;
					else if(deltaX < -1.0f)
						deltaX = -1.0f;
					
					deltaY = (touch.position.y - touchBeganPositionY)/(analogStickTextureHeight*0.5f);
					if(deltaY > 1.0f)
						deltaY = 1.0f;
					else if(deltaY < -1.0f)
						deltaY = -1.0f;
					
				}
				break;
				
				/* 3. */
			case TouchPhase.Stationary:
				break;
				
				/* 4. */
			case TouchPhase.Ended:
				if(touch.fingerId == analogStickTouchID) 
				{
					// Analog Stick ausblenden (aus sichtfeld verschieben)
					analogStickTexture.pixelInset = new Rect(-100,-100,analogStickTexture.pixelInset.width,analogStickTexture.pixelInset.height);
					stickTexture.pixelInset = new Rect(-100,-100,stickTexture.pixelInset.width,stickTexture.pixelInset.height);
					
					// Analog Stick als nicht aktiv setzen
					analogStickTouchBegan = false;
					analogStickTouchID = -1;
				}
				break;
			}
			
			//	BUGGY!
			// sollte verhindern das zweiter Finger auf linker hälfte als neuer AnalogStick arbeitet!
			// zweiter finger sollte ignoriert werden
			//
			//			if(analogStickTouchID != -1)
			//			{
			//				break;
			//			}
		}
		//kein Button gedrueck, zurücksetzen
		if(!buttonIsPressed)
		{
			buttonTouchID = -1;
			buttonTapCount = 0;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				// Insert Code Here (I.E. Load Scene, Etc)
				// OR Application.Quit();
				Application.LoadLevel("MainMenuOld");
				return;
			}
		}
	}
	
}
