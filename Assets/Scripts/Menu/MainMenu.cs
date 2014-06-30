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
	private int numberOfAllPlayer = 4;
	private int numberOfAIPlayer = 0;
	private int numberOfLocalUserPlayer = 1;


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
			numberOfAllPlayer = 4;
			numberOfAIPlayer = 3;
			numberOfLocalUserPlayer = 1;
			nextLevel = "sp_classic";
		}

		// links unten
		if(GUI.Button(new Rect(0,halfScreenHeight,halfScreenWidth,halfScreenHeight),"Online\n(Photon Network)"))
		{
			nextLevel = "pun_menu";
		}

		// rechts oben
		if(GUI.Button(new Rect(halfScreenWidth,0,halfScreenWidth,halfScreenHeight),"LAN/WLAN"))
		{
			numberOfAllPlayer = 4;
			numberOfAIPlayer = 0;
			numberOfLocalUserPlayer = 1;
			nextLevel = "mp_Multiplayer";

		}

		// rechts unten
		if(GUI.Button(new Rect(halfScreenWidth,halfScreenHeight,halfScreenWidth,halfScreenHeight),"Interpolation & Prediction"))
		{
			numberOfAllPlayer = 4;
			numberOfAIPlayer = 0;
			numberOfLocalUserPlayer = 1;
			nextLevel = "hostclientmenu";
		}

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
		PlayerPrefs.SetInt(StaticStrings.numberOfAllPlayers,numberOfAllPlayer);
		PlayerPrefs.SetInt(StaticStrings.numberOfAIPlayers,numberOfAIPlayer);
		PlayerPrefs.SetInt(StaticStrings.NumberOfLocalUserPlayers,numberOfLocalUserPlayer);
		Application.LoadLevel(nextLevel);
	}
}
