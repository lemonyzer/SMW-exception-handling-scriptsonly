using UnityEngine;
using System.Collections;

/**
 * InventoryManager kommt in CharacterSelection Scene (dort ist inventory == null)
 * 
 * in der GameScene kommt InventoryManager auch vor, 
 * 
 **/

public class InventoryManager : MonoBehaviour {

	static public Inventory inventory;

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
		if(inventory == null)
		{
			// CharacterSelectionScene

			initValues = true;

			// instanz nur einmal erzeugen
			// instanz kann sceneübergrifend verwendet werden
			inventory = (Inventory) ScriptableObject.CreateInstance(typeof(Inventory));
		}
//		else
//		{
//			// GameScene
//
//			currentHealth = inventory.GetItems(healthText);
//			currentLifePoint = inventory.GetItems(maxHealthText);
//			currentMaxHealth = inventory.GetItems(lifePointsText);
//		}

		if(initValues)
		{
			// Werte initialisieren
			// zB. mit PlayerPrefs (sind auch nach beenden des Programms vorhanden!)
			for(int i=0; i<4; i++)
			{
				inventory.SetItems("player"+i+"slot0",0f);
			}

//			inventory.SetItems(healthText, initHealth);
//			inventory.SetItems(maxHealthText, initHealth);
//			inventory.SetItems(lifePointsText, initLifePoint);
		}
	}
	
	void Update()
	{
//		if(inventory != null) {
//			this.coin = inventory.GetItems("coin");
//		}
	}
}
