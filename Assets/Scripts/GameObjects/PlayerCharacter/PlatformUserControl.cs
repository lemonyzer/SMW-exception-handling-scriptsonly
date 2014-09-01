﻿using UnityEngine;
using System.Collections;

/**
 *  Ein GameObject (Tag: UserControls) mit diesem UserControlScript in JEDER Scene
 *  
 *  Vorteil:
 *  Mehrere GameObjects benötigen nur ein AnalogStick  
 *  BackButton funktioniert überall und muss nur einmal geschrieben werden
 *  GUI elemente müssen nicht destroyed werden, nur deaktiviert wenn Character wechsel/entfernt  
 *   
 *  Nachteil:
 *  FindGameObjectWithTag aufruf in Awake()    
 *  Brotgrümel erzeugen
 *  oder Scenenamen abhängige funktion 
 *
 **/    

public class PlatformUserControl : MonoBehaviour {
	
	// check who is the owner of the current character
	// allow input if local photonnetwork.player == realOwner.owner
	private RealOwner realOwner;
	
	/** 
	 * Combined Input
	 **/
	
	//	[System.NonSerialized]
	public float inputHorizontal = 0f;
	
	//	[System.NonSerialized]
	public float inputVertical = 0f;
	
	//	[System.NonSerialized]
	public bool inputJump = false;

	public bool keyPressed = true;
	
	/**
	* Input Touch
	**/    
	
	private float inputTouchHorizontal = 0f;
	private float inputTouchVertical = 0f;
	private bool inputTouchJump = false;
	
	/**
	 * Input Flags (Jump Button)
	 **/
	//	int buttonTouchID=-1;			// ID of current jump touch (right screen)
	int buttonTapCount=0;			// tap count current jump touch (right screen)
	bool buttonIsPressed = false;	// flag if player presses jump 		
	bool buttonIsTapped = false;	// flag if player presses jump again		
	
	/**
	 * Input Flags (Analog Stick)
	 **/
	//	Touch analogStick;
	int analogStickTouchID=-1;
	bool analogStickTouchBegan = false;
	bool analogStickIsStillPressed = false;
	
	float touchBeganPositionX;
	float touchBeganPositionY;
	
	
	public GUITexture prefabAnalogStickTexture;
	public GUITexture prefabStickTexture;
	GUITexture analogStickTexture;
	GUITexture stickTexture;
	float analogStickTextureWidth;
	float analogStickTextureHeight;
	float stickTextureWidth;
	float stickTextureHeight;
	
	float textureSizeWithSaveZoneX;
	float textureSizeWithSaveZoneY;
	
	/**
	* Input Keyboard
	**/
	private float inputKeyboardHorizontal = 0f;
	private float inputKeyboardVertical = 0f;
	private bool inputKeyboardJump = false;                    
	
	void Awake()
	{
		realOwner = GetComponent<RealOwner>();
	}
	
	// Use this for initialization
	void Start() {
		
		analogStickTexture = (GUITexture) Instantiate(prefabAnalogStickTexture);		// needed? pre-instantiete in hierachie?!
		stickTexture = (GUITexture) Instantiate(prefabStickTexture);					// needed? pre-instantiete in hierachie?!
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
	}
	
	void ApplicationPlatformInputCheck()
	{
		/**
		 * not on Mobile Devices (Android / IOs)
		 **/
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			Keyboard();
			Touch();
		}
		else if (Application.platform == RuntimePlatform.OSXEditor)
		{
			Keyboard();
			Touch();
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			Keyboard();
			Touch();			
		}
		else if (Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			Keyboard();	
			Touch();		
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			//Keyboard();   // nur bei angeschlossesner USB/Bluetooth Tastatur
			Touch();			
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Touch();			
		}
		else
		{
			Keyboard();
			Touch();
			Debug.LogWarning(this.name + ": disabled!!!");
		}
	}

	public bool simulate = false;

	// Update is called once per frame
	void Update() {
		// Wenn jeder Character ein UserControl script hat muss abgefragt werden ob der Character dem lokalen Spieler gehört
		if(Network.peerType == NetworkPeerType.Disconnected || Network.player == realOwner.owner)
		{
			if(simulate)
				return;
//			Debug.LogWarning(this.ToString() +": is reading local controls input!!!");
			ApplicationPlatformInputCheck();		//
			CombineInput(); 						// kombiniert alle abgefragten Eingabemöglichkeiten (Keyboard, Touchpad, Mouse...)
			// dannach stehen die Eingabedaten in inputHorizontal und inputJump
		}
		else
		{
			// Update darf input Variablen nicht überschreiben, da NetworkedPlayer diese ebenfals verwendet
			this.enabled = false;
		}
	}
	
	void CombineInput()
	{
		if(buttonIsPressed || inputKeyboardJump)
		{
			inputJump = true;
		}
		else
		{
			inputJump = false;
		}
		
		// combine the horizontal input
		inputHorizontal = inputTouchHorizontal + inputKeyboardHorizontal;

		// 3 stateMovement	(prediction will be more precis)
		if(inputHorizontal < 0f)
			inputHorizontal = -1f;
		else if(inputHorizontal > 0f)
			inputHorizontal = 1f;
		else
			inputHorizontal = 0f;

		// limit combination to [-1,1]
		Mathf.Clamp(inputHorizontal, -1, +1);    // kein cheaten möglich mit touch+keyboard steuerung
	}
	
	void Keyboard() {
		inputKeyboardHorizontal = Input.GetAxis ("Horizontal");
		inputKeyboardVertical = Input.GetAxis ("Vertical");
		if(keyPressed)
		{
			inputKeyboardJump = Input.GetKey (KeyCode.Space);
		}
		else
		{
			inputKeyboardJump = Input.GetKeyDown (KeyCode.Space);
		}
	}
	
	void Touch()
	{
		AnalogStickAndButton();
	}
	
	void AnalogStickAndButton() {
		// muss auf false gesetzt werden, da schleife beendet wird wenn touch gefunden
		buttonIsPressed = false;
		buttonIsTapped = false;
		analogStickIsStillPressed = false;
		foreach (Touch touch in Input.touches)
		{
			if(!buttonIsTapped)	// Button (rechte Seite) muss nur einmal gefunden werden
			{
				if(touch.position.x > (Screen.width * 0.5f))
				{
					//debugmsg += "Jump found\n";
					//					buttonTouchID = touch.fingerId;			// ID des Touches speichern um beim nächsten durchlauf TapCount des Touches kontrollieren zu können
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
						//debugmsg += "AnalogStick began()\n";
						// Analog Stick gefunden (Touch auf linker Bildschirmhälfte)
						//						analogStick = touch;
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
						stickTexture.pixelInset = new Rect(touch.position.x - stickTextureWidth*0.5f,		// left
						                                   touch.position.y - stickTextureHeight*0.5f,		// top
						                                   stickTextureWidth,								// width
						                                   stickTextureHeight);								// height
					}
					break;
					/* 2. */
				case TouchPhase.Moved:
					if(touch.fingerId == analogStickTouchID) 			/// needed??, for now yes! switch case geht über ganzen bildschirm
					{
						//debugmsg += "AnalogStick moved()\n";
						analogStickIsStillPressed = true;
						float stickPosX=0;
						float stickPosY=0;
						
						// Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
						if(touch.position.x > touchBeganPositionX + analogStickTextureWidth*0.5f)
							stickPosX = touchBeganPositionX + analogStickTextureWidth*0.5f;				// touch x-pos außerhalb des analogsticks (rechts)
						
						else if(touch.position.x < touchBeganPositionX - analogStickTextureWidth*0.5f)
							stickPosX = touchBeganPositionX - analogStickTextureWidth*0.5f;				// touch x-pos außerhalb des analogsticks (links)
						
						else
							stickPosX = touch.position.x;												// touch x-pos innerhalb des analogsticks
						
						if(touch.position.y > touchBeganPositionY + analogStickTextureHeight*0.5f)
							stickPosY = touchBeganPositionY + analogStickTextureHeight*0.5f;				// touch y-pos außerhalb des analogsticks (oben)
						
						else if(touch.position.y < touchBeganPositionY - analogStickTextureHeight*0.5f)
							stickPosY = touchBeganPositionY - analogStickTextureHeight*0.5f;				// touch y-pos außerhalb des analogsticks (unten)
						
						else
							stickPosY = touch.position.y;												// touch y-pos innerhalb des analogsticks
						
						// Stick um Touch Position zeichnen
						stickTexture.pixelInset = new Rect(stickPosX - stickTextureWidth*0.5f,	// left
						                                   stickPosY - stickTextureHeight*0.5f,	// top
						                                   stickTextureWidth,								// width
						                                   stickTextureHeight);								// height
						
						// Entfernung zum Analogstick Mittelpunkt berechnen (x-Ache)
						inputTouchHorizontal = (touch.position.x - touchBeganPositionX)/(analogStickTextureWidth*0.5f);
						if(inputTouchHorizontal > 1.0f)
							inputTouchHorizontal = 1.0f;
						else if(inputTouchHorizontal < -1.0f)
							inputTouchHorizontal = -1.0f;
						
						// Entfernung zum Analogstick Mittelpunkt berechnen (y-Ache)
						inputTouchVertical = (touch.position.y - touchBeganPositionY)/(analogStickTextureHeight*0.5f);
						if(inputTouchVertical > 1.0f)
							inputTouchVertical = 1.0f;
						else if(inputTouchVertical < -1.0f)
							inputTouchVertical = -1.0f;
						
					}
					break;
					
					/* 3. */
				case TouchPhase.Stationary:
					if(touch.fingerId == analogStickTouchID) 
					{
						//debugmsg += "AnalogStick stationary()\n";
						analogStickIsStillPressed = true;
					}
					break;
					
					/* 4. */
				case TouchPhase.Ended:
					if(touch.fingerId == analogStickTouchID) 
					{
						//debugmsg += "AnalogStick ended()\n";
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
			//debugmsg += "kein Button gefunden\n";
			//kein Button in der Schleife oben gefunden, zurücksetzen
			//			buttonTouchID = -1;
			buttonTapCount = 0;
		}
		
		if(!analogStickTouchBegan)
		{
			//debugmsg += "kein AnalogStick gefunden (analogStickTouchBegan)\n";
			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
			inputTouchHorizontal = 0f;
			inputTouchVertical = 0f;
		}
		
		if(!analogStickIsStillPressed)
		{
			//debugmsg += "kein AnalogStick gefunden (analogStickIsStillPressed)\n";
			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
			inputTouchHorizontal = 0f;
			inputTouchVertical = 0f;
		}
		
		//		if(debugging != null)
		//			debugging.text = debugmsg;
		
	}
	
	// Wenn jeder Character ein UserControlScript hat müssen die benutzten AnalogSticks dieses Script 
	// beim entfernen mitzerstört werden
	// GUITexture sind nicht dem CharacterGameObject untergeordnet (Child), da sich dessen Position ändert und 
	// im child die position immer auf Vector3.zero gesetzt werden müsste um die GUITexture korrekt auf dem Display
	// anzeigen zu lassen.
	void OnDestroy()
	{
		if(analogStickTexture != null)
			Destroy(analogStickTexture);
		
		if(stickTexture != null)
			Destroy(stickTexture);
	}
}
