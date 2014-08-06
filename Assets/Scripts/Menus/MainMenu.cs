using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// GUI
	private float halfScreenWidth;
	private float halfScreenHeight;
	private float btnCount;
	private float btnSpacing;
	private float btnHeight;

	// Settings
	private string nextLevel;


	void Awake()
	{
		halfScreenHeight = Screen.height*0.5f;
		halfScreenWidth = Screen.width*0.5f;
		nextLevel = "";
	}

	void OnGUI() {

		// links oben

		if(GUI.Button(new Rect(0,0,halfScreenWidth,halfScreenHeight),"Single Player"))
		{
			nextLevel = Scenes.SingleplayerMenu;
		}


		// links unten
		if(GUI.Button(new Rect(0,halfScreenHeight,halfScreenWidth,halfScreenHeight),"Online\n(Photon Network)"))
		{
			nextLevel = Scenes.photonLobby;
		}

		// rechts oben
		GUI.enabled = false;
		if(GUI.Button(new Rect(halfScreenWidth,0,halfScreenWidth,halfScreenHeight),"LAN/WLAN"))
		{
			nextLevel = "mp_Multiplayer";

		}
		GUI.enabled = true;

		// rechts unten
		GUI.enabled = false;
		if(GUI.Button(new Rect(halfScreenWidth,halfScreenHeight,halfScreenWidth,halfScreenHeight),"Interpolation & Prediction"))
		{
			nextLevel = "hostclientmenu";
		}
		GUI.enabled = true;

		if(GUI.changed)
		{
			if(nextLevel != "")
			{
				OnGUIChanged();
			}
		}
		
	}

	void OnGUIChanged()
	{
		Application.LoadLevel(nextLevel);
	}
}
