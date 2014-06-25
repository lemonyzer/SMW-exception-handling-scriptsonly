using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float MoveSpeed = 5f;
	
	[System.NonSerialized]
	public float horizAxis = 0f;
	[System.NonSerialized]
	public float vertAxis = 0f;
//	[System.NonSerialized]
//	public bool touchEnabled = false;
//	[System.NonSerialized]
//	public bool keyboardEnabled = false;
//
//	/**
//	 * Debugging GUI Element
//	 **/
//	public GUIText debugging;
//	private string debugmsg="";
//	
///**
// * Mobile: Android / iOs
// **/
//	
//	/**
//	 * Input Flags (Jump Button)
//	 **/
//	int buttonTouchID=-1;			// ID of current jump touch (right screen)
//	int buttonTapCount=0;			// tap count current jump touch (right screen)
//	bool buttonIsPressed = false;	// flag if player presses jump 		
//	bool buttonIsTapped = false;	// flag if player presses jump again		
//	
//	/**
//	 * Input Flags (Analog Stick)
//	 **/
//	Touch analogStick;
//	int analogStickTouchID=-1;
//	bool analogStickTouchBegan = false;
//	bool analogStickIsStillPressed = false;
//	
//	float touchBeganPositionX;
//	float touchBeganPositionY;
//	float deltaX=0;
//	float deltaY=0;
//	
//	public GUITexture analogStickTexture;
//	public GUITexture stickTexture;
//	float analogStickTextureWidth;
//	float analogStickTextureHeight;
//	float stickTextureWidth;
//	float stickTextureHeight;
//	
//	float textureSizeWithSaveZoneX;
//	float textureSizeWithSaveZoneY;

//	void Awake()
//	{
////		ApplicationPlatformCheck();		// Touchfunction only on mobile devices
//	}
//
//	void ApplicationPlatformCheck()
//	{
//		/**
//		 * Android
//		 **/
//		if (Application.platform == RuntimePlatform.Android)
//		{
//			touchEnabled = true;
//			keyboardEnabled = false;
//			Debug.LogError("Platform: Android: Touch enabled & Keyboard disabled!");
//		}
//		else if (Application.platform == RuntimePlatform.WindowsEditor)
//		{
//			touchEnabled = true;
//			keyboardEnabled = true;
//			Debug.LogError("Platform: Windows Editor: Touch & Keyboard enabled!");
//		}
//		else if (Application.platform == RuntimePlatform.OSXEditor)
//		{
//			touchEnabled = true;
//			keyboardEnabled = true;
//			Debug.LogError("Platform: OSX Editor: Touch & Keyboard enabled!");
//		}
//		else
//		{
//			Debug.LogError("andere Platform!");
//		}
//	}

	void Update()
	{
		if( networkView.isMine )
		{
			horizAxis = 0f;
			vertAxis = 0f;
			foreach (Touch touch in Input.touches)
			{
				if(touch.position.x > (Screen.width * 0.75f))
					horizAxis += 1f;
				else if(touch.position.x < (Screen.width * 0.25f))
					horizAxis += -1f;
				else
				{
					if(touch.position.y > Screen.height * 0.5f)
						vertAxis += 1f;
					else
						vertAxis += -1f;
				}

			}
//			if(touchEnabled)
//			{
//				AnalogStickAndButton();
//			}
//			else
//			{
//				deltaX = 0f;
//				deltaY = 0f;
//			}
			horizAxis += Input.GetAxis( "Horizontal" );
//			horizAxis += deltaX;

			vertAxis += Input.GetAxis( "Vertical" );
//			vertAxis += deltaY;
		}
	}
	
	public void Simulate()
	{
		transform.Translate( new Vector3( horizAxis, vertAxis, 0f) * MoveSpeed * Time.fixedDeltaTime );
	}

//	void AnalogStickAndButton() {
//		debugmsg = "";
//		buttonIsPressed = false;
//		buttonIsTapped = false;
//		analogStickIsStillPressed = false;
//		debugmsg = "Loop starting\n";
//		foreach (Touch touch in Input.touches)
//		{
//			if(!buttonIsTapped)	// Button (rechte Seite) muss nur einmal gefunden werden
//			{
//				if(touch.position.x > (Screen.width * 0.5f))
//				{
//					debugmsg += "Jump found\n";
//					buttonTouchID = touch.fingerId;			// ID des Touches speichern um beim nächsten durchlauf TapCount des Touches kontrollieren zu können
//					if(buttonTapCount < touch.tapCount) {	// Spieler muss Taste immer wieder erneut drücken, um Aktion auszulösen
//						buttonTapCount = touch.tapCount;	
//						buttonIsTapped = true;				
//						buttonIsPressed = true;
//					}
//					else
//					{
//						buttonIsTapped = false;
//						buttonIsPressed = true;
//					}
//				}
//			}
//			
//			if(!analogStickIsStillPressed)
//			{
//				/*
//			 * Touch nach Touchphase auswerten:
//			 * 	1. Began
//			 *  2. Moved
//			 *  3. Stationary
//			 *  4. Ended
//			 * */
//				switch (touch.phase) {
//					/* 1. */
//				case TouchPhase.Began:
//					//				Steuerung reagiert schlecht!
//					//
//					//				if(touch.position.x > (Screen.width * 0.5f))
//					//				{
//					//					buttonIsPressed=true;
//					//				}
//					if(touch.position.x < (Screen.width * 0.5f))
//					{
//						debugmsg += "AnalogStick began()\n";
//						// Analog Stick gefunden (Touch auf linker Bildschirmhälfte)
//						analogStick = touch;
//						analogStickTouchID = touch.fingerId;
//						analogStickTouchBegan = true;
//						analogStickIsStillPressed = true;
//						
//						// Screen.width/(2*2*2) = Screen.width*0.125
//						float texturesizeX = analogStickTextureWidth * 0.5f;
//						float texturesizeY = analogStickTextureHeight * 0.5f;
//						float savezoneX = texturesizeX*0.5f;
//						float savezoneY = texturesizeY*0.5f;
//						textureSizeWithSaveZoneX = texturesizeX + savezoneX;
//						textureSizeWithSaveZoneY = texturesizeY + savezoneY;
//						
//						/* X Position checken
//						 * 
//						 * 
//						 * */
//						if((touch.position.x > textureSizeWithSaveZoneX) && (touch.position.x < ((Screen.width*0.5)-textureSizeWithSaveZoneX)))
//						{
//							// X position korrekt (ohne SaveZone)
//							touchBeganPositionX = touch.position.x;
//						}
//						else if(touch.position.x > ((Screen.width*0.5f)-textureSizeWithSaveZoneX))
//						{
//							// zu weit rechts am Rand
//							// X position muss korrigiert werden (ohne SaveZone)
//							touchBeganPositionX = ((Screen.width*0.5f)-textureSizeWithSaveZoneX);
//						}
//						else if(touch.position.x < textureSizeWithSaveZoneX)
//						{
//							// zu weit links am Rand
//							// X position muss korrigiert werden (ohne SaveZone)
//							touchBeganPositionX = textureSizeWithSaveZoneX;
//						}
//						
//						/* Y Position checken
//						 * 
//						 * 
//						 * */
//						if((touch.position.y > textureSizeWithSaveZoneY) && (touch.position.y < (Screen.height)-textureSizeWithSaveZoneY))
//						{
//							// alles perfekt
//							touchBeganPositionY = touch.position.y;
//						}
//						else if(touch.position.y > ((Screen.height)-textureSizeWithSaveZoneY))
//						{
//							// Problem: zu nah am oberen Rand, OFFSET (ohne SaveZone)!
//							touchBeganPositionY = Screen.height-textureSizeWithSaveZoneY;
//						}
//						else if(touch.position.y < textureSizeWithSaveZoneY)
//						{
//							// Problem: zu nah am unteren Rand, OFFSET (ohne SaveZone)!
//							touchBeganPositionY = textureSizeWithSaveZoneY;
//						}
//						
//						
//						// Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
//						analogStickTexture.pixelInset = new Rect(touchBeganPositionX - analogStickTextureWidth*0.5f,	// left
//						                                         touchBeganPositionY - analogStickTextureHeight*0.5f,	// top
//						                                         analogStickTextureWidth,								// width
//						                                         analogStickTextureHeight);								// height
//						
//						// Stick um Touch Position zeichnen
//						stickTexture.pixelInset = new Rect(touch.position.x - stickTextureWidth*0.5f,		// left
//						                                   touch.position.y - stickTextureHeight*0.5f,		// top
//						                                   stickTextureWidth,								// width
//						                                   stickTextureHeight);								// height
//					}
//					break;
//					/* 2. */
//				case TouchPhase.Moved:
//					if(touch.fingerId == analogStickTouchID) 			/// needed??, for now yes! switch case geht über ganzen bildschirm
//					{
//						debugmsg += "AnalogStick moved()\n";
//						analogStickIsStillPressed = true;
//						float stickPosX=0;
//						float stickPosY=0;
//						
//						// Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
//						if(touch.position.x > touchBeganPositionX + analogStickTextureWidth*0.5f)
//							stickPosX=touchBeganPositionX + analogStickTextureWidth*0.5f;				// touch x-pos außerhalb des analogsticks (rechts)
//						
//						else if(touch.position.x < touchBeganPositionX - analogStickTextureWidth*0.5f)
//							stickPosX=touchBeganPositionX - analogStickTextureWidth*0.5f;				// touch x-pos außerhalb des analogsticks (links)
//						
//						else
//							stickPosX = touch.position.x;												// touch x-pos innerhalb des analogsticks
//						
//						if(touch.position.y > touchBeganPositionY + analogStickTextureHeight*0.5f)
//							stickPosY=touchBeganPositionY + analogStickTextureHeight*0.5f;				// touch y-pos außerhalb des analogsticks (oben)
//						
//						else if(touch.position.y < touchBeganPositionY - analogStickTextureHeight*0.5f)
//							stickPosY=touchBeganPositionY - analogStickTextureHeight*0.5f;				// touch y-pos außerhalb des analogsticks (unten)
//						
//						else
//							stickPosY = touch.position.y;												// touch y-pos innerhalb des analogsticks
//						
//						// Stick um Touch Position zeichnen
//						stickTexture.pixelInset = new Rect(stickPosX - stickTextureWidth*0.5f,	// left
//						                                   stickPosY - stickTextureHeight*0.5f,	// top
//						                                   stickTextureWidth,								// width
//						                                   stickTextureHeight);								// height
//						
//						// Entfernung zum Analogstick Mittelpunkt berechnen (x-Ache)
//						deltaX = (touch.position.x - touchBeganPositionX)/(analogStickTextureWidth*0.5f);
//						if(deltaX > 1.0f)
//							deltaX = 1.0f;
//						else if(deltaX < -1.0f)
//							deltaX = -1.0f;
//						
//						// Entfernung zum Analogstick Mittelpunkt berechnen (y-Ache)
//						deltaY = (touch.position.y - touchBeganPositionY)/(analogStickTextureHeight*0.5f);
//						if(deltaY > 1.0f)
//							deltaY = 1.0f;
//						else if(deltaY < -1.0f)
//							deltaY = -1.0f;
//						
//					}
//					break;
//					
//					/* 3. */
//				case TouchPhase.Stationary:
//					if(touch.fingerId == analogStickTouchID) 
//					{
//						debugmsg += "AnalogStick stationary()\n";
//						analogStickIsStillPressed = true;
//					}
//					break;
//					
//					/* 4. */
//				case TouchPhase.Ended:
//					if(touch.fingerId == analogStickTouchID) 
//					{
//						debugmsg += "AnalogStick ended()\n";
//						// Analog Stick ausblenden (aus sichtfeld verschieben)
//						analogStickTexture.pixelInset = new Rect(0,
//						                                         0,
//						                                         0,
//						                                         0);
//						stickTexture.pixelInset = new Rect(0,
//						                                   0,
//						                                   0,
//						                                   0);
//						
//						// Analog Stick als nicht aktiv setzen
//						analogStickTouchBegan = false;
//						analogStickIsStillPressed = false;
//						analogStickTouchID = -1;
//					}
//					break;
//				}
//			}
//		}
//		
//		if(!buttonIsPressed)
//		{
//			debugmsg += "kein Button gefunden\n";
//			//kein Button in der Schleife oben gefunden, zurücksetzen
//			buttonTouchID = -1;
//			buttonTapCount = 0;
//		}
//		
//		if(!analogStickTouchBegan)
//		{
//			debugmsg += "kein AnalogStick gefunden (analogStickTouchBegan)\n";
//			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
//			deltaX = 0f;
//			deltaY = 0f;
//		}
//		
//		if(!analogStickIsStillPressed)
//		{
//			debugmsg += "kein AnalogStick gefunden (analogStickIsStillPressed)\n";
//			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
//			deltaX = 0f;
//			deltaY = 0f;
//		}
//		
//		if(debugging != null)
//			debugging.text = debugmsg;
//		
//		/**
//		 * Android Softbutton: Back
//		 **/
//		if (Application.platform == RuntimePlatform.Android)
//		{
//			if (Input.GetKey(KeyCode.Escape))
//			{
//				// Insert Code Here (I.E. Load Scene, Etc)
//				// OR Application.Quit();
//				Network.Disconnect();
//				Application.LoadLevel(Application.loadedLevel-1);
//				return;
//			}
//		}
//	}
}