using UnityEngine;
using System.Collections;

public class PlayerUserControlAnalogStick : MonoBehaviour {

//	private PlatformCharacter character;

	public float MoveSpeed = 5f;

	/**
	 * Debugging GUI Element
	 **/
	public GUIText debugging;
	private string debugmsg="";

	/**
	 * Mobile: Android / iOs
	 **/
	
		/**
		 * Input Flags (Analog Stick)
		 **/
	Touch analogStick;
	int analogStickTouchID=-1;
	bool analogStickTouchBegan = false;
	bool analogStickIsStillPressed = false;
	
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

	// Use this for initialization
	void Start() {
//		character = GetComponent<PlatformCharacter>();

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
	}
	
	// Update is called once per frame
	void Update() {

		if( networkView == null || networkView.isMine )
		{
			AnalogStick();
			transform.Translate( new Vector3( deltaX, deltaY, 0f ) * MoveSpeed * Time.deltaTime );
		}
	}

	void AnalogStick() {
		debugmsg = "";
		analogStickIsStillPressed = false;
		debugmsg = "Loop starting\n";
		foreach (Touch touch in Input.touches)
		{
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
					//float rightEnd = (Screen.width * 0.5f);		// nur linke Bildschirmhälfte
					float rightEnd = (Screen.width);				// kompletter Bildschirm
					if(touch.position.x < rightEnd)
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
						if((touch.position.x > textureSizeWithSaveZoneX) && (touch.position.x < (rightEnd-textureSizeWithSaveZoneX)))
						{
							// X position korrekt (ohne SaveZone)
							touchBeganPositionX = touch.position.x;
						}
						else if(touch.position.x > (rightEnd-textureSizeWithSaveZoneX))
						{
							// zu weit rechts am Rand
							// X position muss korrigiert werden (ohne SaveZone)
							touchBeganPositionX = (rightEnd-textureSizeWithSaveZoneX);
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
						stickTexture.pixelInset = new Rect(stickPosX - stickTextureWidth*0.5f,	// left
						                                   stickPosY - stickTextureHeight*0.5f,	// top
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

		if(!analogStickIsStillPressed)
		{
			debugmsg += "kein AnalogStick gefunden\n";
			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
			deltaX = 0f;
			deltaY = 0f;
		}
		
		if(debugging != null)
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
