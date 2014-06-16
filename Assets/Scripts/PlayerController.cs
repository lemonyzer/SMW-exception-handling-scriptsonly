using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public GUIText debugging;
	
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
	public float velocity = 0f;
	public bool changedRunDirection = false;				
	public bool inputPCJump = false;							// stores Input Key 
	public bool inputPCMove = false;							// stores Input Key
	public float pushForce;
	public float pushTime = 0f;
	
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
	
	/**
	 * Mobile: Android / iOs
	 **/
	
	/**
		 * Input Flags (Jump Button)
		 **/
	int buttonTouchID=-1;			// ID of current jump touch (right screen)
	int buttonTapCount=0;			// tap count current jump touch (right screen)
	bool inputTouchJump = false;	// flag if player presses jump 		
	bool buttonIsPressed = false;	// flag if player presses jump 		
	bool buttonIsTapped = false;	// flag if player presses jump again		
	
	/**
		 * Input Flags (Analog Stick)
		 **/
	Touch analogStick;
	int analogStickTouchID=-1;
	bool analogStickTouchBegan = false;
	bool analogStickIsStillPressed = false;
	bool inputTouchStick = false;
	
	float touchBeganPositionX;
	float touchBeganPositionY;
	float deltaX=0;
	float deltaY=0;
	
	public GUITexture analogStickTexture;
	public GUITexture stickTexture;
	float analogStickTextureWidth;
	float analogStickTextureHeight;
	float stickTextureWidth;
	float stickTextureHeight;
	
	float textureSizeWithSaveZoneX;
	float textureSizeWithSaveZoneY;
	
	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
	}
	
	void Start() {
		anim = GetComponent<Animator>();
		
		analogStickTexture = (GUITexture) Instantiate(analogStickTexture);		// needed? pre-instantiete in hierachie?!
		stickTexture = (GUITexture) Instantiate(stickTexture);					// needed? pre-instantiete in hierachie?!
		analogStickTextureWidth = analogStickTexture.pixelInset.width;
		analogStickTextureHeight = analogStickTexture.pixelInset.height;
		stickTextureWidth = stickTexture.pixelInset.width;
		stickTextureHeight = stickTexture.pixelInset.height;
		// Analog Stick ausblenden (aus sichtfeld verschieben)
		analogStickTexture.pixelInset = new Rect(0,
		                                         0,
		                                         0,
		                                         0);
		stickTexture.pixelInset = new Rect(0,
		                                   0,
		                                   0,
		                                   0);
		isInJumpAbleSaveZone=false;
	}
	
	void Update() {
		
		if (Application.platform == RuntimePlatform.Android)
		{
			InputTouchCheck();
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			InputPCKeyboardCheck();
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			InputTouchCheck();
			InputPCKeyboardCheck();
		}

		JumpAblePlatform();
	}
	void InputPCKeyboardCheck()
	{
		/* Run */
		//Pfeil nach links = -1 
		//Pfeil nach rechts = +1
		//Links, Rechts
		moveDirection.x = Input.GetAxis("Horizontal");
		if(moveDirection.x != 0)
		{
			inputPCMove = true;
		}
		else
			inputPCMove = false;
		
		/* Jump Keyboard */
		inputPCJump = Input.GetButton("Jump");
	}
	void InputPCMouseCheck()
	{
		/* Jump Mouse (and Touch) */
		//Achtung: gilt auch für Touch!
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log ("Achtung bei Touch!!!");
			inputPCJump = true;
		}
	}
	void InputTouchCheck() 
	{
		AnalogStickAndButton();
		inputTouchJump = buttonIsTapped;				//
		inputTouchStick = analogStickIsStillPressed;
	}
	
	// FixedUpdate is called once per frame
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
		Vector2 playerPos = new Vector2(rigidbody2D.transform.position.x, rigidbody2D.transform.position.y);
		
		grounded = Physics2D.OverlapCircle(playerPos+groundCheckPosition, groundRadius, whatIsGround);
		Debug.DrawLine(playerPos,playerPos+groundCheckPosition,Color.green);
		
		//		bool areaTest = Physics2D.OverlapArea(playerPos+groundCheckPosition, playerPos+groundCheckPosition+ new Vector2(groundRadius,0f), whatIsGround);
		//		Debug.Log("areaTest = " + areaTest);
		
		walled = Physics2D.OverlapCircle(playerPos+wallCheckPosition, wallRadius, whatIsWall);
		Debug.DrawLine(playerPos,playerPos+wallCheckPosition,Color.green);
	}
	void FixSetAnim() 
	{
		if(anim != null)
		{
			anim.SetBool(hash.groundedBool, grounded);
			anim.SetBool(hash.walledBool, walled);
			anim.SetFloat(hash.vSpeedFloat, rigidbody2D.velocity.y);
		}
		else
			Debug.LogError("Animator not set");
		
	}
	void FixMove()
	{
		// Platformen vereinen
		velocity = (moveDirection.x + deltaX);

		if(isBouncing)
		{
			//Alte Kraft in X Richtung wird nicht komplett überschrieben!

			if(pushForce > 0f)
			{
				if(velocity > 0f)
				{
					velocity *= maxSpeed;		// wenn Spieler in die gleiche Richtung wie pushForce sich bewegt,
												// volle Geschwindigkeit nehmen  
				}
				else
					velocity *= maxSpeed * 0.2f;
			}
			else if(pushForce < 0f)
			{
				if(velocity < 0f)
				{
					velocity *= maxSpeed;
				}
				else
					velocity *= maxSpeed * 0.2f;
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
				velocity += pushSpeed;
//				Debug.LogError(this.gameObject.transform.name+ " velocity = " + velocity);
			}

		}
		else // if(!isBouncing)
		{
			velocity *= maxSpeed;
		}
		
		/**
		 * maxSpeed check
		 **/
		if(Mathf.Abs(velocity) > maxSpeed)
		{
			// neue velocity überschreitet maxSpeed!!!
			if(velocity < (-1.0f*maxSpeed))
			{
				velocity = -1.0f*maxSpeed;
				//rigidbody2D.velocity = new Vector2((-1.0f)*maxSpeed, rigidbody2D.velocity.y);
			}
			else if(velocity > maxSpeed)
			{
				velocity = maxSpeed;
				//rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
			}
		}
		
		// gedrosselte velocity übernehmen
		rigidbody2D.velocity = new Vector2(velocity, rigidbody2D.velocity.y);
		
		/**
		 * Animator status Update
		 **/
		if(anim != null)
		{
			anim.SetFloat(hash.hSpeedFloat, velocity);
		}
		else
			Debug.LogError("Animator not set");
		
		/**
		 * Check Direction Change
		 **/
		if(velocity > 0f && !facingRight)
		{
			Flip();
		}
		else if(velocity < 0f && facingRight)
		{
			Flip();
		}
		else
		{
			changedRunDirection = false;
		}
		
		if(grounded && (inputPCJump || inputTouchJump)) {
			// Do Jump
			AudioSource.PlayClipAtPoint(jumpSound,transform.position,1);				//JumpSound
			anim.SetBool(hash.groundedBool,false);
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,jumpForce.y);		//<--- besser für JumpAblePlatforms	
			//rigidbody2D.AddForce(new Vector2(0.0F, jumpForce.y));						//<--- klappt nicht 100% mit JumpAblePlatforms
			
		}
		else if(!grounded && walled && (inputPCJump || inputTouchJump)) {
			// Do WallJump
			AudioSource.PlayClipAtPoint(wallJumpSound,transform.position,1);			//WallJump
			rigidbody2D.velocity = new Vector2(0,0);									//alte Geschwindigkeit entfernen
			Flip();																		//Charakter drehen 
			anim.SetBool(hash.groundedBool,false);
			anim.SetBool(hash.walledBool,false);
			rigidbody2D.velocity = new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y);	//<--- besser für JumpAblePlatforms
		}
		
	}
	
	void StartJump() {
		if(JumpAllowed)
			inputPCJump = true;
	}
	
	void StopJump() {
		inputPCJump = false;
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
	 * Wird extra abgefragt, da Spieler auch ohne selbst zu Springen eine positive vertikale Geschwindigkeit bekommen kann
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
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// Executed on the owner of this PhotonView; 
			// The server sends it's position over the network
			
			stream.SendNext(transform.position);//"Encode" it, and send it
			
		}
		else
		{
			// Executed on the others; 
			// receive a position and set the object to it
			
			transform.position = (Vector3)stream.ReceiveNext();
			
		}
	}
	
	void AnalogStickAndButton () {

		string debugmsg="";
		buttonIsPressed = false;
		buttonIsTapped = false;
		analogStickIsStillPressed = false;
		debugmsg += "Loop starting\n";
		foreach (Touch touch in Input.touches)
		{
			if(!buttonIsTapped)	// Button (rechte Seite) muss nur einmal gefunden werden
			{
				if(touch.position.x > (Screen.width * 0.5f))
				{
					debugmsg += "Jump found\n";
					buttonTouchID = touch.fingerId;			// ID des Touches speichern um beim nächsten durchlauf TapCount des Touches kontrollieren zu können
					if(buttonTapCount < touch.tapCount) {	// Spieler muss Taste immer wieder erneut drücken, um Aktion auszulösen
						buttonTapCount = touch.tapCount;	
						buttonIsTapped = true;				
						buttonIsPressed = true;
					}
					else
					{
						buttonIsTapped = false;
						buttonIsPressed = true;
					}
				}
			}
			
			if(!analogStickIsStillPressed)
			{
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
						debugmsg += "AnalogStick began()\n";
						// Analog Stick gefunden (Touch auf linker Bildschirmhälfte)
						analogStick = touch;
						analogStickTouchID = touch.fingerId;
						analogStickTouchBegan = true;
						analogStickIsStillPressed = true;
						
						// Screen.width/(2*2*2) = Screen.width*0.125
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
						
						
						// Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
						analogStickTexture.pixelInset = new Rect(touchBeganPositionX - analogStickTextureWidth*0.5f,	// left
						                                         touchBeganPositionY - analogStickTextureHeight*0.5f,	// top
						                                         analogStickTextureWidth,								// width
						                                         analogStickTextureHeight);								// height
						
						// Stick um Touch Position zeichnen
						stickTexture.pixelInset = new Rect(touch.position.x - stickTexture.pixelInset.width*0.5f,		// left
						                                   touch.position.y - stickTexture.pixelInset.height*0.5f,		// top
						                                   stickTextureWidth,								// width
						                                   stickTextureHeight);								// height
					}
					break;
					/* 2. */
				case TouchPhase.Moved:
					if(touch.fingerId == analogStickTouchID) 			/// needed??, for now yes! switch case geht über ganzen bildschirm
					{
						debugmsg += "AnalogStick moved()\n";
						analogStickIsStillPressed = true;
						float stickPosX=0;
						float stickPosY=0;
						
						// Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
						if(touch.position.x > touchBeganPositionX + analogStickTextureWidth*0.5f)
							stickPosX=touchBeganPositionX + analogStickTextureWidth*0.5f;				// touch x-pos außerhalb des analogsticks (rechts)
						
						else if(touch.position.x < touchBeganPositionX - analogStickTextureWidth*0.5f)
							stickPosX=touchBeganPositionX - analogStickTextureWidth*0.5f;				// touch x-pos außerhalb des analogsticks (links)
						
						else
							stickPosX = touch.position.x;												// touch x-pos innerhalb des analogsticks
						
						if(touch.position.y > touchBeganPositionY + analogStickTextureHeight*0.5f)
							stickPosY=touchBeganPositionY + analogStickTextureHeight*0.5f;				// touch y-pos außerhalb des analogsticks (oben)
						
						else if(touch.position.y < touchBeganPositionY - analogStickTextureHeight*0.5f)
							stickPosY=touchBeganPositionY - analogStickTextureHeight*0.5f;				// touch y-pos außerhalb des analogsticks (unten)
						
						else
							stickPosY = touch.position.y;												// touch y-pos innerhalb des analogsticks
						
						// Stick um Touch Position zeichnen
						stickTexture.pixelInset = new Rect(stickPosX - stickTexture.pixelInset.width*0.5f,	// left
						                                   stickPosY - stickTexture.pixelInset.height*0.5f,	// top
						                                   stickTextureWidth,								// width
						                                   stickTextureHeight);								// height
						
						// Entfernung zum Analogstick Mittelpunkt berechnen (x-Ache)
						deltaX = (touch.position.x - touchBeganPositionX)/(analogStickTextureWidth*0.5f);
						if(deltaX > 1.0f)
							deltaX = 1.0f;
						else if(deltaX < -1.0f)
							deltaX = -1.0f;
						
						// Entfernung zum Analogstick Mittelpunkt berechnen (y-Ache)
						deltaY = (touch.position.y - touchBeganPositionY)/(analogStickTextureHeight*0.5f);
						if(deltaY > 1.0f)
							deltaY = 1.0f;
						else if(deltaY < -1.0f)
							deltaY = -1.0f;
						
					}
					break;
					
					/* 3. */
				case TouchPhase.Stationary:
					if(touch.fingerId == analogStickTouchID) 
					{
						debugmsg += "AnalogStick stationary()\n";
						analogStickIsStillPressed = true;
					}
					break;
					
					/* 4. */
				case TouchPhase.Ended:
					if(touch.fingerId == analogStickTouchID) 
					{
						debugmsg += "AnalogStick ended()\n";
						// Analog Stick ausblenden (aus sichtfeld verschieben)
						analogStickTexture.pixelInset = new Rect(0,
						                                         0,
						                                         0,
						                                         0);
						stickTexture.pixelInset = new Rect(0,
						                                   0,
						                                   0,
						                                   0);
						
						// Analog Stick als nicht aktiv setzen
						analogStickTouchBegan = false;
						analogStickIsStillPressed = false;
						analogStickTouchID = -1;
					}
					break;
				}
			}
		}
		
		
		if(!buttonIsPressed)
		{
			debugmsg += "kein Button gefunden\n";
			//kein Button in der Schleife oben gefunden, zurücksetzen
			buttonTouchID = -1;
			buttonTapCount = 0;
		}
		if(!analogStickIsStillPressed)
		{
			debugmsg += "kein AnalogStick gefunden\n";
			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
			deltaX = 0f;
			deltaY = 0f;
		}

		debugging.text = debugmsg;

		/**
		 * Android Softbutton: Back
		 **/
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
