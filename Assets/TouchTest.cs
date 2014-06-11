using UnityEngine;
using System.Collections;

public class TouchTest: MonoBehaviour {

	float deltaX=0;
	float deltaY=0;

	Touch analogStick;
	int analogStickTouchID=-1;
	bool analogStickTouchBegan=false;
	float touchBeganPositionX;
	float touchBeganPositionY;

	public Transform background;
	public Transform spawnobject;
	public GUIText guitext;
	public GUIText guitext2;
	public GUIText guitextstick;
	public GUITexture analogStickTexture;
	public GUITexture stickTexture;
	float analogStickTextureWidth;
	float analogStickTextureHeight;
	float stickTextureWidth;
	float stickTextureHeight;
	float textureSizeWithSaveZoneX;
	float textureSizeWithSaveZoneY;

	bool buttonIsPressed;

	void Start() {
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

	void Update () {
		buttonIsPressed=false;
		foreach (Touch touch in Input.touches)
		{
			string message = "";
			message += "ID: " + touch.fingerId + "\n";
			message += "Phase: " + touch.phase.ToString () + "\n";
			message += "TapCount: " + touch.tapCount + "\n";
			message += "Pos X: " + touch.position.x + "\n";
			message += "Pos Y: " + touch.position.y + "\n";
			message += "Delta X: " + touch.deltaPosition.x + "\n";
			message += "Delta Y: " + touch.deltaPosition.y + "\n";
			message += "Delta X ges: " + deltaX + "\n";
			message += "Delta Y ges: " + deltaY + "\n";
			
//			int num = touch.fingerId;
//			if(num == 0)
//				guitext.text = message;
//
//			if(num<6)
//				GUI.Label (new Rect (0 + 130 * num, 0, 120, 145), message);
//			else
//				GUI.Label (new Rect (0 + 130 * (num-6), 160, 120, 145), message);


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

					//Startposition ausgeben
					guitext.text = message;


					//Prefab an berührter Position erzeugen
					if(spawnobject != null)
					{
						// Pixel in Spielweltkoordinate umwandeln
						float gamepositionx = touch.position.x / Screen.width * background.GetComponent<BoxCollider2D>().size.x;
						float gamepositiony = touch.position.y / Screen.height * background.GetComponent<BoxCollider2D>().size.y;

						// Mittelpunkt des Prefabs (Sprite) finden
						float spriteoffsetX = spawnobject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f;
						float spriteoffsetY = spawnobject.GetComponent<SpriteRenderer>().bounds.size.y*0.5f;

						//Prefab an berührter Position erzeugen
						Instantiate(spawnobject, new Vector3(gamepositionx-spriteoffsetX,gamepositiony-spriteoffsetY,0), Quaternion.identity);
					}

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
						guitext2.text =  "TouchPosX: " + touch.position.x + "\n";
						guitext2.text += "TouchBeganPosX: " + touchBeganPositionX + "\n";
						guitext2.text += "TouchPosY: " + touch.position.y + "\n";
						guitext2.text += "TouchBeganPosY: " + touchBeganPositionY + "\n";
						guitext2.text += "Screen Size X:" + Screen.width + "\n";
						guitext2.text += "Screen Size Y:" + Screen.height + "\n";
						guitext2.text += "Texture X " + texturesizeX + "\n";
						guitext2.text += "Texture Y " + texturesizeY + "\n";


						//Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
						analogStickTexture.pixelInset = new Rect(touchBeganPositionX-analogStickTextureWidth*0.5f,
					                                         touchBeganPositionY-analogStickTextureHeight*0.5f,
					                                         analogStickTextureWidth,
					                                         analogStickTextureHeight);
						stickTexture.pixelInset = new Rect(touch.position.x-stickTexture.pixelInset.width*0.5f,
					                           touch.position.y-stickTexture.pixelInset.height*0.5f,
					                           stickTextureWidth,
					                           stickTextureHeight);
					}
					else
					{
						buttonIsPressed=true;
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

					stickTexture.pixelInset = new Rect(stickPosX-stickTexture.pixelInset.width*0.5f,
					                           stickPosY-stickTexture.pixelInset.height*0.5f,
					                           stickTextureWidth,
					                           stickTextureHeight);

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

						guitextstick.text = "";
						guitextstick.text += "ID: " + touch.fingerId + "\n";
						guitextstick.text += "Phase: " + touch.phase.ToString () + "\n";
						guitextstick.text += "TapCount: " + touch.tapCount + "\n";
						guitextstick.text += "Pos X: " + touch.position.x + "\n";
						guitextstick.text += "Pos Y: " + touch.position.y + "\n";
						guitextstick.text += "Delta X: " + touch.deltaPosition.x + "\n";
						guitextstick.text += "Delta Y: " + touch.deltaPosition.y + "\n";
						guitextstick.text += "Delta X ges: " + deltaX + "\n";
						guitextstick.text += "Delta Y ges: " + deltaY + "\n";
					}
				break;

				/* 3. */
				case TouchPhase.Stationary:
									/*
									 * 
									 * WICHTIG analogStickStillTouched=true !
									 * 
									 **/
				break;

				/* 4. */
				case TouchPhase.Ended:
					if(touch.fingerId == analogStickTouchID) 
					{
						// Analog Stick ausblenden (aus sichtfeld verschieben)
						analogStickTexture.pixelInset = new Rect(0,0,0,0);
						stickTexture.pixelInset = new Rect(0,0,0,0);
						
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

	}
}
