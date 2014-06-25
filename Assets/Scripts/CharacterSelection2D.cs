using UnityEngine;
using System.Collections;
using UnityEditor;

public class CharacterSelection2D : MonoBehaviour {
	
	private Vector3 clickedPosition = Vector3.zero;
	public AudioClip characterInUseSound;

	public GUIText player0GUIText;
	public GUIText player1GUIText;
	public GUIText player2GUIText;
	public GUIText player3GUIText;

	public Sprite player0CharacterSprite;
	public Sprite player1CharacterSprite;
	public Sprite player2CharacterSprite;
	public Sprite player3CharacterSprite;
	
	private string debugmsg="";

	Texture2D[] characterArray;
//	string[] assetsPaths;

	void Awake()
	{

		characterArray = Resources.LoadAll<Texture2D>("Skins");
		for(int i=0; i < characterArray.Length; i++)
		{
			Debug.Log(characterArray[i].name);
		}

//		assetsPaths = AssetDatabase.GetAllAssetPaths();
//		foreach (string assetPath in assetsPaths) {
//			if (assetPath.Contains (yourPrefabsFolderPath)) {
//				prefabsPaths.Add(assetPath);
//			}
//		}
	}

	void Start()
	{
		debugmsg="";
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.Escape))
		{
			if(Network.isServer)
			{
				MasterServer.UnregisterHost();
				for(int i=0;i<Network.connections.Length;i++)
				{
					Network.CloseConnection(Network.connections[i],true);
				}
			}
			Network.Disconnect();
			Application.LoadLevel("MainMenuOld");
			return;
		}

		// Client Funktion
		// setze Clickposition und sende an Server (mit RPC)
		if(networkView == null || networkView.isMine)
		{
			//debugmsg = networkView.owner.ToString() + "\n";
			//Debug.Log(networkView.owner.ToString());		// Server: 0
															// Clients: 1 to Network.connections.Length;
			GetClickPosition();

			if(networkView.owner.ToString() == "0")
			{
				Debug.Log("Server");
				if(player0GUIText != null)
					player0GUIText.text = debugmsg;
			}
			else if(networkView.owner.ToString() == "1")
			{
				Debug.Log("Client 1");
				if(player1GUIText != null)
					player1GUIText.text = debugmsg;
			}
			else if(networkView.owner.ToString() == "2")
			{
				Debug.Log("Client 2");
				if(player2GUIText != null)
					player2GUIText.text = debugmsg;
			}
			else if(networkView.owner.ToString() == "3")
			{
				Debug.Log("Client 3");
				if(player3GUIText != null)
					player3GUIText.text = debugmsg;
			}
		}
	}

	/**
	 * Client Funktion, mit RPC an Server
	 **/
	void GetClickPosition()
	{
		if(Input.GetMouseButtonUp(0))
		{
			debugmsg = "clicked" + "\n";
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);		
			Vector2 origin = ray.origin;										// startPoint
			Vector2 direction = ray.direction;									// direction
			float distance = 100f;
			RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance);
			if(hit.collider != null)
			{
				//				Debug.Log(hit.collider.name);
				if(hit.collider.name != "Platform")
				{
					clickedPosition = Input.mousePosition;
					networkView.RPC("CharacterClicked", RPCMode.Server, clickedPosition);		//RPC an Server
					Debug.Log(hit.collider.name);
					debugmsg += hit.collider.name;
				}
			}
		}
	}

	/**
	 * RPC des Servers, (Client fordert Server auf diese Funktion zu starten)
	 * Server checks if Character at clicked Position is already in Use
	 * if not in use rigister character in PlayerPrefs and send 
	 * RPC to Animate selection on all Clients
	 * if in use RPC to requested Client and play characterInUseSound
	 **/
	[RPC]
	void CharacterClicked(Vector3 recvPos, NetworkMessageInfo info)
	{
		if(!Network.isServer)
			return;

		bool characterInUse = false;

		string characterName = GetSelectedCharacterName(recvPos);
		int characterPrefabID = GetCharacterPrefabIDfromName(characterName);
		if( characterPrefabID != -1)
		{
			// Prefab ID gefunden
			Debug.Log("Prefab ID zu " + characterName + ": " + characterPrefabID);
		}
		else
		{
			// keine Prefab ID zu Character Name gefunden
			Debug.LogError("keine Prefab ID zu Character Name: " + characterName);
			return;																			// RPC abbrechen!
		}

		foreach(NetworkPlayer player in Network.connections)
		{
			string key = player.ToString() + "_PrefabID";		// Key um in PlayerPrefs nach einträgen zu schauen
			key = key.ToLower();
			int value = PlayerPrefs.GetInt(key);			// Value 
			if(value == characterPrefabID)
			{
				// ein anderer Spieler hat diesen Character bereits, Schleife ggf. abbrechen
				characterInUse = true;
			}
		}

		if(!characterInUse)
		{
			string key = info.networkView.owner.ToString() + "_PrefabID";			// Register CharacterPrefab with Player in PlayerPref
			key = key.ToLower();
			int value = characterPrefabID;
			PlayerPrefs.SetInt(key,value);
			// kein Spieler hat diesen Character gewählt, Client Character zuteilen und freigabe mitteilen.
			// Zuteilung allen Clients mitteilen
			networkView.RPC( "SelectCharacter", RPCMode.All, info.sender.ToString(), recvPos );
		}
		else
		{
			// Character schon in Verwendung
			// anfragendem Client mitteilen (spielt sound ab)
			networkView.RPC( "SelectCharacterInUse", info.sender );
		}
	}


	/**
	 * RPC des Clients, (Server fordert Client auf diese Funktion zu starten)
	 **/
	[RPC]
	void SelectCharacter(string networkPlayerID, Vector3 recvPos, NetworkMessageInfo info)
	{
		MarkCharacter(networkPlayerID, recvPos);
//		transform.position = recvPos;
//		renderer.enabled = true;
	}


	/**
	 * RPC des Clients, (Server fordert Client auf diese Funktion zu starten)
	 **/
	[RPC]
	void SelectCharacterInUse(NetworkMessageInfo info)
	{
		// Character already in Use
		Debug.LogWarning("Character already in Use");
	}


	/**
	 * Charakter selection Animation 
	 **/
	void MarkCharacter(string networkPlayerID, Vector3 recvPos)
	{
		// Character Sound 
		Debug.Log("Player " + networkPlayerID + " hat einen Character gewählt: " + recvPos);

	}


//	/**
//	 * RPC des Servers, (Client fordert Server auf diese Funktion zu starten)
//	 **/
//	[RPC]
//	void ServerRPC(Vector3 pos, NetworkMessageInfo info)
//	{
//		if(networkView.isMine)
//			return;
//		if(!Network.isServer)
//			return;
//		
//	}
	
	/**
	 * Client / Server Funktion
	 **/
	string GetSelectedCharacterName(Vector3 clickedPosition)
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
	int GetCharacterPrefabIDfromName(string name)
	{
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
	
//	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
//		Vector3 netPosition;
//		if (stream.isWriting) {
//			netPosition = this.position;
//			stream.Serialize(ref netPosition);
//		} else {
//			netPosition = Vector3.zero;
//			stream.Serialize(ref netPosition);
//			this.position = netPosition;
//		}
//	}
}
