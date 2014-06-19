using UnityEngine;
using System.Collections;

public class PlatformUserControlTapButton : MonoBehaviour {

	private PlatformCharacter character;
	
	/**
	 * Debugging GUI Element
	 **/
	public GUIText debugging;
	private string debugmsg="";
	
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

	// Use this for initialization
	void Start() {
		character = GetComponent<PlatformCharacter>();
	}
	
	// Update is called once per frame
	void Update () {
		TapButton();
	}

	void TapButton() {
		
		debugmsg = "";
		buttonIsPressed = false;
		buttonIsTapped = false;

		debugmsg = "Loop starting\n";
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
		}
		
		
		if(!buttonIsPressed)
		{
			debugmsg += "kein Button gefunden\n";
			//kein Button in der Schleife oben gefunden, zurücksetzen
			buttonTouchID = -1;
			buttonTapCount = 0;
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
