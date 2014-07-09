using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// für Dictionary

/**
 * ScriptableObject,
 * kann nicht DIREKT auf ein GameObject gezogen werden!
 * Im xManager wird eine static public instanz erzeugt, der den scenenübergreifenden Zugriff auf die generische Sammlung gewährleistet.
 * 
 **/


public class GameObjectsPlayerDictionary : ScriptableObject {

	// Key = instanz of PlayerCharacter GameObject
	// Value = Player

	Dictionary<GameObject, Player> gameObjectsPlayerDictionary = new Dictionary<GameObject, Player>();

	public void Add(GameObject gameObject, Player player)
	{
		Debug.Log("GameObjectsPlayerDictionary: " + gameObject.name + " " + player.getPlayerID() + " added");
		Player currentPlayer = null;
		
		if(gameObjectsPlayerDictionary.TryGetValue(gameObject, out currentPlayer))
		{
			// alten Wert überschreiben
			gameObjectsPlayerDictionary[gameObject] = player;
		}
		else
		{
			// noch nicht vorhanden, item eintragen
			gameObjectsPlayerDictionary.Add(gameObject, player);
		}
	}
	
	public void Set(GameObject gameObject, Player player)
	{
		Debug.Log("GameObjectsPlayerDictionary: " + gameObject.name + " " + player.getPlayerID() + " setted");
		Player currentPlayer = null;

		if(gameObjectsPlayerDictionary.TryGetValue(gameObject, out currentPlayer))
		{
			gameObjectsPlayerDictionary[gameObject] = player;
		}
		else
		{
			gameObjectsPlayerDictionary.Add(gameObject, player);
		}
	}
	
	public Player Get(GameObject gameObject)
	{
		Player currentValue = null;
		Player result = null;

		if(gameObjectsPlayerDictionary.TryGetValue(gameObject, out currentValue))
		{
			result = currentValue;
		}
		
		return result;
	}

	public void Delete(GameObject gameObject)
	{
		Player removedPlayer = null;
		
		if(gameObjectsPlayerDictionary.TryGetValue(gameObject, out removedPlayer))
		{
			gameObjectsPlayerDictionary.Remove(gameObject);
			Debug.Log("GameObjectsPlayerDictionary: Player " + removedPlayer.getPlayerID() + " Character " + gameObject.name + " deleted");
		}
		else
		{
			Debug.Log("GameObjectsPlayerDictionary: Player " + removedPlayer.getPlayerID() + " hatte kein Character gewählt, nix zum Löschen");
		}
		
	}

}
