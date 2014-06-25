using UnityEngine;
using System.Collections;

public class LobbyCharacterManager : MonoBehaviour {

	public GUIText player0GUIText;
	public GUIText player1GUIText;
	public GUIText player2GUIText;
	public GUIText player3GUIText;

	public Sprite player0CharacterSprite;
	public Sprite player1CharacterSprite;
	public Sprite player2CharacterSprite;
	public Sprite player3CharacterSprite;

	private string debugmsg="";

	//Texture2D[] characterArray;
	Sprite[] characterArray;

	void Awake()
	{
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
				Debug.Log(characterArray[i].name);
			}
		}
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
				Debug.Log(hit.collider.name);
				return hit.collider.name;
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
