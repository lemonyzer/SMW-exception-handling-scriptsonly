using UnityEngine;
using System.Collections;

/**
 * gameObjectManager kommt in CharacterSelection Scene (dort ist gameObjectDictionary == null)
 * 
 * in der GameScene kommt gameObjectManager auch vor, 
 * 
 **/

public class GameObjectManager : MonoBehaviour {

	static public GameObjectDictionary gameObjectDictionary;

//	public bool create = false;
//	public float coin = 0;
//
//	public float initHealth = 5;
//	public float initLifePoint = 3;

	private bool initValues = false;

	public string player0slot0 = "player0slot0";
	public string player0slot1 = "player0slot1";
	
	public string player1slot0 = "player1slot0";
	public string player1slot1 = "player1slot1";

	public string player2slot0 = "player2slot0";
	public string player2slot1 = "player2slot1";
	
	public string player3slot0 = "player3slot0";
	public string player3slot1 = "player3slot1";

	public string lifePointsText = "LifePoint";
	public string maxHealthText = "MaxHealth";

//	private float currentHealth;
//	private float currentLifePoint;
//	private float currentMaxHealth;

	void Awake()
	{
		if(gameObjectDictionary == null)
		{
			// CharacterSelectionScene

			initValues = true;

			// instanz nur einmal erzeugen
			// instanz kann sceneübergrifend verwendet werden
			gameObjectDictionary = (GameObjectDictionary) ScriptableObject.CreateInstance(typeof(GameObjectDictionary));
		}
//		else
//		{
//			// GameScene
//
//			currentHealth = gameObjectDictionary.GetItems(healthText);
//			currentLifePoint = gameObjectDictionary.GetItems(maxHealthText);
//			currentMaxHealth = gameObjectDictionary.GetItems(lifePointsText);
//		}

		if(initValues)
		{
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
			for(int i=0; i<4; i++)
			{
				gameObjectDictionary.SetItems("player"+i+"slot0",0f);
			}

//			gameObjectDictionary.SetItems(healthText, initHealth);
//			gameObjectDictionary.SetItems(maxHealthText, initHealth);
//			gameObjectDictionary.SetItems(lifePointsText, initLifePoint);
		}
	}
	
	void Update()
	{
//		if(gameObjectDictionary != null) {
//			this.coin = gameObjectDictionary.GetItems("coin");
//		}
	}
}
