using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// für Dictionary

public class GameObjectDictionary : ScriptableObject {

	Dictionary<string, GameObject> gameObjects = new Dictionary<string , GameObject>();

	public void Add(string playerID, GameObject gameObject)
	{
		Debug.Log("GameObjectDictionary: " + gameObject.name + " added");
		GameObject oldValue = null;
		playerID = playerID.ToLower();
		
		if(gameObjects.TryGetValue(playerID, out oldValue))
		{
			// alten Wert überschreiben
			gameObjects[playerID] = gameObject;
			Debug.Log("GameObjectDictionary: Player " + playerID + " Character " + oldValue.name + " überschrieben mit " + gameObject.name);
		}
		else
		{
			// noch nicht vorhanden, item eintragen
			gameObjects.Add(playerID, gameObject);
			Debug.Log("GameObjectDictionary: Player " + playerID + " Character " + gameObject.name + " hinzugefügt.");
		}
	}

//	unnötig, eintrag wird immer überschrieben, keine addition von alt und neuen values!
//
//	public void Set(string playerID, GameObject gameObject)
//	{
//		GameObject oldValue = null;
//		playerID = playerID.ToLower();
//		
//		if(gameObjects.TryGetValue(playerID, out oldValue))
//		{
//			// alten Wert überschreiben
//			gameObjects[playerID] = gameObject;
//			Debug.Log("GameObjectDictionary: Player " + playerID + " Character " + oldValue.name + " überschrieben mit " + gameObject.name);
//		}
//		else
//		{
//			// noch nicht vorhanden, eintragen
//			gameObjects.Add(playerID, gameObject);
//			Debug.Log("GameObjectDictionary: Player " + playerID + " Character " + gameObject.name + " hinzugefügt.");
//		}
//	}

	public void Delete(string playerID)
	{
		GameObject removedValue = null;
		
		if(gameObjects.TryGetValue(playerID, out removedValue))
		{
			gameObjects.Remove(playerID);
			Debug.Log("GameObjectDictionary: Player " + playerID + " Character " + removedValue.name + " deleted");
		}
		else
        {
			Debug.Log("GameObjectDictionary: Player " + playerID + " hatte kein Character gewählt, nix zum Löschen");
        }

    }
    
	public GameObject Get(string playerID)
	{
		GameObject currentValue = null;
		GameObject result = null;
		playerID = playerID.ToLower();
		
		if(gameObjects.TryGetValue(playerID, out currentValue))
		{
			result = currentValue;
		}

		return result;
	}
}
