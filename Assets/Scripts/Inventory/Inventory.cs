using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// für Dictionary

public class Inventory : ScriptableObject {

//	public float money = 0;

	Dictionary<string, float> items = new Dictionary<string , float>();

	public void AddItems(string itemName, float quantity)
	{
		Debug.LogError(itemName + " added");
		float oldValue = 0;
		itemName = itemName.ToLower();
		
		if(items.TryGetValue(itemName, out oldValue))
		{
			// zum alten Wert addieren
			quantity += oldValue;
			items[itemName] = quantity;
		}
		else
		{
			// noch nicht vorhanden, item eintragen
			items.Add(itemName, quantity);
		}
	}

	public void SetItems(string itemName, float quantity)
	{
		Debug.LogError(itemName + " setted");
		float oldValue = 0;
		itemName = itemName.ToLower();
		
		if(items.TryGetValue(itemName, out oldValue))
		{
			items[itemName] = quantity;
		}
		else
		{
			items.Add(itemName, quantity);
		}
	}

	public float GetItems(string itemName)
	{
		float currentValue = 0;
		float result = 0;
		itemName = itemName.ToLower();
		
		if(items.TryGetValue(itemName, out currentValue))
		{
			result = currentValue;
		}

		return result;
	}
}
