using UnityEngine;
using System.Collections;

public class CHuman : CPlayer {

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
		inputTouchJump = buttonIsPressed;
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
		if(isBouncing)
		{
			//Alte Kraft in X Richtung wird nicht komplett überschrieben!
			// Platformen vereinen
			velocity = (moveDirection.x + deltaX);

			velocity *= maxSpeed * 0.5f;
			velocity += rigidbody2D.velocity.x;		//summand gegen null laufen lassen
		}
		else // if(!isBouncing)
		{
			// Platformen vereinen
			velocity = (moveDirection.x + deltaX);
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
			rigidbody2D.velocity = new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y);								//<--- besser für JumpAblePlatforms
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

		buttonIsPressed = false;
		analogStickIsStillPressed = false;

		foreach (Touch touch in Input.touches)
		{
			if(!buttonIsPressed)	// Button (rechte Seite) muss nur einmal gefunden werden
			{
				if(touch.position.x > (Screen.width * 0.5f))
				{
					buttonTouchID = touch.fingerId;			// ID des Touches speichern um beim nächsten durchlauf TapCount des Touches kontrollieren zu können
					if(buttonTapCount < touch.tapCount) {	// Spieler muss Taste immer wieder erneut drücken, um Aktion auszulösen
						buttonTapCount = touch.tapCount;	
						buttonIsPressed = true;				// Button (rechte Seite) nicht weiter suchen
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
					if(touch.fingerId == analogStickTouchID && analogStickTouchBegan) 			/// needed??
					{
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
					analogStickIsStillPressed = true;
					break;
					
					/* 4. */
				case TouchPhase.Ended:
					if(touch.fingerId == analogStickTouchID) 
					{
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
			//kein Button in der Schleife oben gefunden, zurücksetzen
			buttonTouchID = -1;
			buttonTapCount = 0;
		}
		if(!analogStickIsStillPressed)
		{
			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
			deltaX = 0f;
			deltaY = 0f;
		}

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
