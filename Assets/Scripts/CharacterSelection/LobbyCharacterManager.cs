using UnityEngine;
using System.Collections;

public class LobbyCharacterManager : MonoBehaviour {

	public GUIText player0GUIText;
	public GUIText player1GUIText;
	public GUIText player2GUIText;
	public GUIText player3GUIText;

	public GUITexture player0GUITexture;
	public GUITexture player1GUITexture;
	public GUITexture player2GUITexture;
	public GUITexture player3GUITexture;

	public SpriteRenderer player0SpriteRenderer;
	public SpriteRenderer player1SpriteRenderer;
	public SpriteRenderer player2SpriteRenderer;
	public SpriteRenderer player3SpriteRenderer;

	private string debugmsg="";

	//Texture2D[] characterArray;
	Sprite[] characterArray;

	void Awake()
	{

		PlayerPrefs.DeleteAll();		// delete PlayerPrefs auf Server und Client

		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		debugmsg="";
		characterArray = Resources.LoadAll<Sprite>("Skins");			// alle Sliced Sprites (Spritename_0) ...
		//characterArray = Resources.LoadAll<Texture2D>("Skins");		// nur ganze Bilder 
		
		//if(Network.isServer)
		if(networkView.isMine)
		{
			
			for(int i=0; i < characterArray.Length; i=i+6)
			{
				debugmsg+=characterArray[i].name + "\n";
//				Debug.Log(characterArray[i].name);
			}
		}
	}


	public GUITexture GetPlayerGUITexture(string playerID)
	{
		if(playerID == "0")
		{
			// Server
			return player0GUITexture;
		}
		else if(playerID == "1")
		{
			return player1GUITexture;
		}
		else if(playerID == "2")
		{
			return player2GUITexture;
		}
		else if(playerID == "3")
		{
			return player3GUITexture;
		}
		else
			return null;
	}

	public void SetPlayerSprite(string playerID, string characterPrefabName)
	{
		GameObject character = GameObject.Find(characterPrefabName);
		if(character == null)
			return;
		Sprite characterSprite = character.GetComponent<SpriteRenderer>().sprite;
		if(characterSprite == null)
			return;
		SpriteRenderer targetSpriteRenderer = GetPlayerSpriteRenderer(playerID);
		if(targetSpriteRenderer != null)
		{
			targetSpriteRenderer.sprite = characterSprite;
		}
		else
		{
			Debug.LogError("LobbyCharacterManager, Player " + playerID + " hat kein SpriteRenderer"); 
		}

	}

	public SpriteRenderer GetPlayerSpriteRenderer(string playerID)
	{
		if(playerID == "0")
		{
			// Server
			return player0SpriteRenderer;
		}
		else if(playerID == "1")
		{
			return player1SpriteRenderer;
		}
		else if(playerID == "2")
		{
			return player2SpriteRenderer;
		}
		else if(playerID == "3")
		{
			return player3SpriteRenderer;
		}
		else
			return null;
	}

	/**
	 * Client / Server Funktion
	 **/
	public int GetSelectedCharacterPrefabID(Vector3 clickedPosition)
	{
		string spriteName = GetSelectedCharacterName(clickedPosition);
		int prefabID = GetCharacterPrefabIDfromName(spriteName);

		return prefabID;
	}

	/**
	 * Client / Server Funktion
	 **/
	public string GetSelectedCharacterName(Vector3 clickedPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(clickedPosition);		
		Vector2 origin = ray.origin;										// startPoint
		Vector2 direction = ray.direction;									// direction
		float distance = 100f;
		RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance);
		if(hit.collider != null)
		{
			if(hit.collider.name != "Platform")
			{
				if(hit.collider.name == "Feet" ||
				   hit.collider.name == "Head")								// Layer Hash
				{
					// Kopf oder Füße getroffen -> Parent GameObject Name
					Debug.Log("LobbyCharacterManager: " + hit.collider.transform.parent.name );
					return hit.collider.transform.parent.name;
				}
				else
				{
					Debug.Log("LobbyCharacterManager: " + hit.collider.name);
					return hit.collider.name;
				}

			}
		}
		return null;
	}
	
	
	/**
	 * Prefab ID - Prefab Filename
	 **/
	public int GetCharacterPrefabIDfromName(string name)
	{
		if(name == null)
			return -1;
		for(int i=0; i < characterArray.Length; i++)
		{
			if(characterArray[i].name == name)
			{
				Debug.Log("Prefab ID for " + name + " found: " + i);
				return i;
			}
		}
		Debug.LogError("Prefab ID for " + name + " not found!!!");
		return -1;
	}
	
}
