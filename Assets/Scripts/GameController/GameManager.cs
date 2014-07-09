using UnityEngine;
using System.Collections;

/**
 * xManager kommt an alle GameController
 **/

public class GameManager : MonoBehaviour {

//	static public GameObjectDictionary gameObjectDictionary;
//	static public GameObjectDictionary playerCharacterGameObjectDictionary;

	/**
	 * GameSlots, Scenename, GameMode, AISlots...
	 **/
	static public GamePrefs gamePrefs;

	/**
	 * Auswahl der Character in Lobby Scene
	 **/
	static public SelectedCharacterPrefabDictionary playerSelectedCharacterPrefabDictionary;

	/**
	 * Instanzierte CharacterPrefab GameObjects in  GameScene
	 **/
	static public GameObjectsPlayerDictionary playerDictonary;


	/**
	 * Initialisieren von ScriptableObjects, wie zB. letzte SinglePlayer Characterauswahl aus PlayerPrefs laden
	 **/
	private bool initValues = false;

	void Awake()
	{
		if(gamePrefs == null)
		{
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			gamePrefs = (GamePrefs) ScriptableObject.CreateInstance(typeof(GamePrefs));
			Debug.Log("ScriptableObject gamePrefs erzeugt");
		}
		if(initValues)
		{
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
		}

		if(playerSelectedCharacterPrefabDictionary == null)
		{
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			playerSelectedCharacterPrefabDictionary = (SelectedCharacterPrefabDictionary) ScriptableObject.CreateInstance(typeof(SelectedCharacterPrefabDictionary));
			Debug.Log("ScriptableObject playerSelectedCharacterPrefabDictionary erzeugt");
		}
		if(initValues)
		{
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
		}

		if(playerDictonary == null)
		{
			// ScriptableObject wurde seit Appstart noch nicht erzeugt.
			// Spätestens in CharacterSelectionScene erfolgt die erste Instanzierung!!!
			initValues = true;
			// instanz kann sceneübergrifend verwendet werden (wenn dieses Script in Scene eingebaut ist (am GameController zB.))
			playerDictonary = (GameObjectsPlayerDictionary) ScriptableObject.CreateInstance(typeof(GameObjectsPlayerDictionary));
			Debug.Log("ScriptableObject GameObjectsPlayerDictionary erzeugt");
		}
		if(initValues)
		{
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
//			for(int i=0; i<serverslots; i++)
//			{
//				playerDictonary.SetGameObject(""+i,null);
//			}
		}
	}
	
	void Update()
	{
//		if(gameObjectDictionary != null) {
//			this.coin = gameObjectDictionary.GetItems("coin");
//		}
	}
}
