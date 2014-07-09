using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// für Dictionary

/**
 * ScriptableObject,
 * kann nicht DIREKT auf ein GameObject gezogen werden!
 * Im xManager wird eine static public instanz erzeugt, der den scenenübergreifenden Zugriff auf die generische Sammlung gewährleistet.
 * 
 **/


public class SelectedCharacterPrefabDictionary : ScriptableObject {

	// Key = string of PlayerCharacterPrefab name
	// Value = string PlayerID

	Dictionary<string, string> selectedCharacterPrefabDictionary = new Dictionary<string , string>();

	public void Add(string playerID, string characterPrefabName)
	{
		string oldValue = null;
		playerID = playerID.ToLower();
		
		if(selectedCharacterPrefabDictionary.TryGetValue(playerID, out oldValue))
		{
			// alten Wert überschreiben
			selectedCharacterPrefabDictionary[playerID] = characterPrefabName;
			Debug.Log("SelectedCharacterPrefabDictionary: Player " + playerID + " Character " + oldValue + " überschrieben mit " + characterPrefabName);
		}
		else
		{
			// noch nicht vorhanden, item eintragen
			selectedCharacterPrefabDictionary.Add(playerID, characterPrefabName);
			Debug.Log("SelectedCharacterPrefabDictionary: Player " + playerID + " Character " + characterPrefabName + " hinzugefügt.");
		}
	}

//	unnötig, Eintrag wird immer überschrieben, keine Addition von alt und neuen Values!
//
	public void Set(string playerID, string characterPrefabName)
	{
		Add(playerID, characterPrefabName);
	}

	public string Get(string playerID)
	{
		string currentValue = null;
		string result = null;
		playerID = playerID.ToLower();
		
		if(selectedCharacterPrefabDictionary.TryGetValue(playerID, out currentValue))
		{
			result = currentValue;
		}

		return result;
	}

	public void Delete(string playerID)
	{
		string removedCharacterPrefabName = null;
		
		if(selectedCharacterPrefabDictionary.TryGetValue(playerID, out removedCharacterPrefabName))
		{
			selectedCharacterPrefabDictionary.Remove(playerID);
			Debug.Log("SelectedCharacterPrefabDictionary: Player " + playerID + " Character " + removedCharacterPrefabName + " deleted");
		}
		else
		{
			Debug.Log("SelectedCharacterPrefabDictionary: Player " + playerID + " hatte kein Character gewählt, nix zum Löschen");
		}
		
	}
}
