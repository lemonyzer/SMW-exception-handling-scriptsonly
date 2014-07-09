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
		else
		{
			// existiert nicht!
			result = null;
		}

		return result;
	}

	public void Remove(string playerID)
	{
		string removedCharacterPrefabName = null;
		
		if(selectedCharacterPrefabDictionary.TryGetValue(playerID, out removedCharacterPrefabName))
		{
			selectedCharacterPrefabDictionary.Remove(playerID);
			Debug.Log("SelectedCharacterPrefabDictionary: Player " + playerID + " Character " + removedCharacterPrefabName + " removed, (Character is free to use)");
		}
		else
		{
			Debug.Log("SelectedCharacterPrefabDictionary: Player " + playerID + " hatte kein Character gewählt, nix zum Löschen");
		}
	}

	public void RemoveAll()
	{
		string removedCharacterPrefabName = null;
//		for(IEnumerator e = selectedCharacterPrefabDictionary.GetEnumerator(); e.MoveNext();)
//		{
//			e.Current...
//		}
//		foreach(string key in selectedCharacterPrefabDictionary.Keys)
//		{
//			selectedCharacterPrefabDictionary.Remove(key);
//		}
//
//		foreach(KeyValuePair<string,string> cObj in selectedCharacterPrefabDictionary)
//		{
//
//		}
		selectedCharacterPrefabDictionary.Clear();
		Debug.Log("SelectedCharacterPrefabDictionary: Alle Charactere Zuordnungen freigegeben");
	}
}
