using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour {

	static public Inventory inventory;

//	public bool create = false;
	public float coin = 0;

	public float initHealth = 5;
	public float initLifePoint = 3;
	private string healthText = "Health";
	private string lifePointsText = "LifePoint";
	private string maxHealthText = "MaxHealth";

	void Awake()
	{
		if(inventory == null)
		{
			// instanz nur einmal erzeugen
			// instanz kann sceneübergrifend verwendet werden
			inventory = (Inventory) ScriptableObject.CreateInstance(typeof(Inventory));

			// Werte initialisieren
			inventory.SetItems(healthText, initHealth);
			inventory.SetItems(maxHealthText, initHealth);
			inventory.SetItems(lifePointsText, initLifePoint);
		}
//		zum Testen
//		else
//		{
//			inventory.AddItems("money", 2);
//			this.money = inventory.GetItems("money");
//		}
	}


	void Update()
	{
		if(inventory != null) {
			this.coin = inventory.GetItems("coin");
		}
	}
}
