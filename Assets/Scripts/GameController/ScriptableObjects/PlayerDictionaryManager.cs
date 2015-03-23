﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDictionaryManager : MonoBehaviour {


	public static PlayerDictionary _instance;
	public static bool serverHasPlayer = false;

	void Awake()
	{
		Debug.LogWarning(this.ToString() + ": Awake()");
		if(_instance == null)
		{
			// PlayerDictionary doesn't exist
			// this should haben in first time PhotonLobby!
			_instance = (PlayerDictionary)ScriptableObject.CreateInstance (typeof(PlayerDictionary));
			Debug.Log(this.ToString() +": _instance instantiert!");
		}
		else
		{
			// PlayerDictionary already exists
			// this should haben in PhotonRoom, GameScene and second time PhotonLobby!
			// in PhotonLobby clear PlayerDictionary!!! else they would be exist in next created/joined Room!!!!
			Debug.LogWarning(this.ToString() +": _instance is already instantiated!");

			List<Player> buffer = new List<Player> ( _instance.Values() );
			foreach(Player player in buffer)
			{
				Debug.Log(this.ToString() +": " + player.getName() + " in PlayerDictionary gefunden!");
            }
			if(Application.loadedLevelName == Scenes.photonLobby ||
			   Application.loadedLevelName == Scenes.mainmenu ||
			   Application.loadedLevelName == Scenes.unityNetworkConnectLobby ||
			   Application.loadedLevelName == Scenes.unityNetworkRace)
			{
				// wenn aktuelles Level PhotonLobby ist, lösche alle Einträge aus PlayerDictionary
				_instance.RemoveAll();
				Debug.LogWarning(this.ToString() +": _instance.RemoveAll() executed!!!");
			}
		}
	}

//	void OnLevelWasLoaded()
//	{
//		Debug.LogWarning(this.ToString() + ": OnLevelWasLoaded()");
//	}
//
//	void Start()
//	{
//		Debug.LogWarning(this.ToString() + ": Start()");
//	}

	/**
	 * Delete selection made previously ( all characters can be clicked )
	 **/
//	void Reset()
//	{
//		// _instance leeren, wenn Scene Photon Room gestartet wurde
//		// muss wieder gefüllt werden oder einfach nicht löschen!
//		
//		if(_instance != null)
//			_instance.RemoveAll();
//	}
	
	
//	/**
//	 * PhotonGameManager is also Manager of PlayerDictionary
//	 **/
//	void AwakeScriptableObjects()
//	{
//		if (gamePrefs == null) {
//			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
//			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
//			initValues = true;
//			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
//			gamePrefs = (GamePrefs)ScriptableObject.CreateInstance (typeof(GamePrefs));
//			Debug.Log ("ScriptableObject gamePrefs erzeugt");
//		}
//		if (initValues) {
//			// Werte initialisieren
//			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
//			int slots = PlayerPrefs.GetInt (gameSlotsCountPlayerPrefsString);
//			Debug.Log ("PlayerPrefs: " + slots + " (" + gameSlotsCountPlayerPrefsString + ")");
//			if (slots <= 0)
//				slots = 4;										// vertraue keinem Userinput!
//			setNumberOfGameSlots (slots);
//		}
//		
//		if (_instance == null) {
//			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
//			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
//			initValues = true;
//			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
//			_instance = (PlayerDictionary)ScriptableObject.CreateInstance (typeof(PlayerDictionary));
//			Debug.Log ("ScriptableObject GameObjectsPlayerDictionary erzeugt");
//        }
//        if (initValues) {
//            // Werte initialisieren
//            // zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
//            //			for(int i=0; i<serverslots; i++)
//            //			{
//            //				playerDictonary.SetGameObject(""+i,null);
//            //			}
//        }
//    }


	/**
	 * Delete selection made previously ( all characters can be clicked )
	 **/
	void Reset()
	{
		// _instance leeren, wenn Scene Photon Room gestartet wurde
		// muss wieder gefüllt werden oder einfach nicht löschen!
		if (_instance == null)
		{
			Debug.LogWarning("Dictionary is NULL!");
			//			_instance = (PlayerDictionary)ScriptableObject.CreateInstance (typeof(PlayerDictionary));
		}
		
		if(_instance != null)
		{
			_instance.RemoveAll();
            Debug.LogWarning("AutoReset clearing Dictionary!");
        }
    }

	/**
	 * sobald room joined empfängt client buffered aktionen ???
	 **/
	void AutoReset()
	{
		// _instance leeren, wenn Scene Photon Room gestartet wurde
		// muss wieder gefüllt werden oder einfach nicht löschen!
		if( Application.loadedLevelName != Scenes.photonRoomAuthorative ||
		   Application.loadedLevelName != Scenes.photonLevel1)
		{
			return;
		}
		
		if(_instance != null)
		{
			_instance.RemoveAll();
            Debug.LogWarning("AutoReset clearing Dictionary!");
        }
    }

}
