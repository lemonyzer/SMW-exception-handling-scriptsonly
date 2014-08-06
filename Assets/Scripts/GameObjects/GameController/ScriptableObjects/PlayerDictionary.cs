using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// für Dictionary

/**
 * ScriptableObject,
 * kann nicht DIREKT auf ein GameObject gezogen werden!
 * Im xManager wird eine static public instanz erzeugt, der den scenenübergreifenden Zugriff auf die generische Sammlung gewährleistet.
 * 
 **/


public class PlayerDictionary : ScriptableObject {

	// Key = instanz of PlayerCharacter GameObject
	// Value = Player

	Dictionary<PhotonPlayer, Player> playerDictionary = new Dictionary<PhotonPlayer, Player>();

//	public void SetCharacterSelector(GameObject characterSelector, Player player)
//	{
//		List<Player> buffer = new List<Player>(gameObjectsPlayerDictionary.Values);
//		foreach( Player currPlayer in buffer )
//		{
//			if( currPlayer == player )
//			{
//
//			}
//		}
//	}

	public void RemoveAllPlayerCharacerGameObjects()
	{
		List<PhotonPlayer> buffer = new List<PhotonPlayer>(playerDictionary.Keys);
		foreach(PhotonPlayer photonPlayer in buffer)								// iterator will result in generic method ???
		{
			Player currPlayer = null;
			if(playerDictionary.TryGetValue(photonPlayer, out currPlayer))
		 	{
				// Key PhotonPlayer has Value in Dictionary (Player exists in Dictionary)
				currPlayer.getCharacter().RemoveCharacterGameObject();
				Debug.Log("removed GameObject referenz (" + currPlayer.getCharacter().getPrefabFilename() + ") from Player: " + photonPlayer.name );
			}
			else
			{
				Debug.Log("Player: " + photonPlayer.name + " is not in Dictionary! WTF, how is that possible?!!");
			}
		}
	}

	public string TryGetCharacterPrefabFilename ( PhotonPlayer photonPlayer )
	{
		Player player = null;
		
		if( playerDictionary.TryGetValue(photonPlayer, out player) )
		{
			// Key PhotonPlayer has Value in Dictionary
			
			if(player.getCharacter() != null)
			{
				// Player has Character, should have GameObject, too!
				
				if( !string.IsNullOrEmpty( player.getCharacter().getPrefabFilename() ) )
				{
					// GameObject found
					return player.getCharacter().getPrefabFilename();
				}
				else
				{
					// Character but no GameObject
					Debug.LogError(photonPlayer.name + " has Player and Character but no PrefabFilename!!!");
				}
			}
			else
			{
				// no Character
				Debug.LogError(photonPlayer.name + " has Player but no Character set in Dictionary!!!");
			}
		}
		else
		{
			Debug.LogError(photonPlayer.name + " has no Player set in Dictionary!!!");
		}
		return null;
	}

	public GameObject TryGetCharacterGameObject( PhotonPlayer photonPlayer )
	{
		Player player = null;
		
		if( playerDictionary.TryGetValue(photonPlayer, out player) )
		{
			// Key PhotonPlayer has Value in Dictionary
			
			if(player.getCharacter() != null)
			{
				// Player has Character, should have GameObject, too!
				
				if(player.getCharacter().getGameObject() != null )
				{
					// GameObject found
					return player.getCharacter().getGameObject();
				}
				else
				{
					// Character but no GameObject
					Debug.LogError(photonPlayer.name + " has Player and Character but no GameObject!!!");
				}
			}
			else
			{
				// no Character
				Debug.LogError(photonPlayer.name + " has Player but no Character set in Dictionary!!!");
			}
		}
		{
			Debug.LogError(photonPlayer.name + " has no Player set in Dictionary!!!");
		}
		return null;
	}

	public GameObject TryGetCharacterSelectorGameObject( PhotonPlayer photonPlayer )
	{
		Player player = null;
		
		if( playerDictionary.TryGetValue(photonPlayer, out player) )
		{
			// Key PhotonPlayer has Value in Dictionary
			
			if(player.getCharacterSelector() != null)
			{
				// GameObject (CharacterSelector) found
				// Player has CharacterSelector
				return player.getCharacterSelector();
			}
			else
			{
				// no CharacterSelector
				Debug.LogError(photonPlayer.name + " has Player but no CharacterSelector set in Dictionary!!!");
			}
		}
		{
			Debug.LogError(photonPlayer.name + " has no Player set in Dictionary!!!");
		}
		return null;
	}

	public bool PrefabInUse( string prefabFileName )
	{
		List<Player> buffer = new List<Player>(playerDictionary.Values);
		foreach( Player currPlayer in buffer )
		{
			if( currPlayer.getCharacter() != null )
			{
				if(currPlayer.getCharacter().getPrefabFilename() == prefabFileName)
				{
					// already in use
					Debug.LogWarning(currPlayer.getPhotonPlayer().name + " verwendet " + prefabFileName + " schon.");
					return true;
				}
			}
		}
		return false;

	}

	public void AddPlayer(PhotonPlayer photonPlayer, Player player)
	{
		Player currentPlayer = null;
		
		if(playerDictionary.TryGetValue(photonPlayer, out currentPlayer))
		{
			// alten Wert überschreiben
			playerDictionary[photonPlayer] = player;
			Debug.LogError(photonPlayer.name + " was already in Dictionary. overwritten!");
		}
		else
		{
			// noch nicht vorhanden, item eintragen
			playerDictionary.Add(photonPlayer, player);
			Debug.Log(photonPlayer.name + " added to Dictionary.");
		}
	}
	
	public void RemovePlayer( PhotonPlayer photonPlayer )
	{
		Player removedPlayer = null;
		
		if(playerDictionary.TryGetValue(photonPlayer, out removedPlayer))
		{
			playerDictionary.Remove(photonPlayer);
			Debug.Log(photonPlayer.name + " removed from Dictionary.");
		}
		else
		{
			Debug.LogWarning(photonPlayer.name + " was not added to Dictionary.");
		}
	}

	public void SetCharacter(PhotonPlayer key, Character setCharacter)
	{
		Player currentPlayer = null;
		Character currentCharacter = null;
		
		if(playerDictionary.TryGetValue(key, out currentPlayer))
		{
			currentCharacter = currentPlayer.getCharacter();
			currentPlayer.setCharacter(setCharacter);
			if(currentCharacter.getGameObject() != null)
			{
				Debug.LogWarning(currentCharacter.getGameObject().name + " replaced");
				if(setCharacter.getGameObject() != null)
				{
					Debug.LogWarning(currentCharacter.getGameObject().name + " replaced by " + setCharacter.getGameObject().name);
				}
 			}
		}
		else
		{
			Debug.LogError(key.name + " has no Player in Dictionary, to set Character!");
		}
	}

	public void SetPlayer(PhotonPlayer key, Player value)
	{
		Player currentPlayer = null;
		
		if(playerDictionary.TryGetValue(key, out currentPlayer))
		{
			playerDictionary[key] = value;
			Debug.Log(key.name + " was already in Dictionary, value " + currentPlayer.getName() + " replaced by " + value.getName());
		}
		else
		{
			// was not in Dictionary, added KeyValuePair
			playerDictionary.Add(key, value);
		}
	}
	
	public Player GetPlayer(PhotonPlayer photonPlayer)
	{
		Player currentValue = null;
		Player result = null;

		if(playerDictionary.TryGetValue(photonPlayer, out currentValue))
		{
			result = currentValue;
		}
		
		return result;
	}

	public Dictionary<PhotonPlayer, Player>.KeyCollection Keys()
	{
		return playerDictionary.Keys;
	}

	public Dictionary<PhotonPlayer, Player>.ValueCollection Values()
	{
		return playerDictionary.Values;
	}

	public void RemoveAll()
	{
		playerDictionary.Clear();
		Debug.LogWarning("Dictionary cleared.");
	}
}
