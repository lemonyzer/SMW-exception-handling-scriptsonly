using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

	Dictionary<int, GameObject> items = new Dictionary<int , GameObject>();
	
	public void AddItems(int id, GameObject go)
	{
		GameObject oldGO;
		if(items.TryGetValue(id, out oldGO))
		{
			// Prefab existiert bereits
		}
		else
		{
			// noch nicht vorhanden, Prefab eintragen
			items.Add(id, go);
		}
	}
	
	public void SetItems(int id, GameObject go)
	{
		GameObject oldGO;
		if(items.TryGetValue(id, out oldGO))
		{
			// Prefab existiert bereits
			// Prefab aktualisieren
			items.Add(id, go);
		}
		else
		{
			// noch nicht vorhanden, Prefab eintragen
			items.Add(id, go);
		}
	}
	
	public GameObject GetItems(int id)
	{
		GameObject currentGO;
		GameObject result = null;
		
		if(items.TryGetValue(id, out currentGO))
		{
			result = currentGO;
		}
		
		return result;
	}
}
