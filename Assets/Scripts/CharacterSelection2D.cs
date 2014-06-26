using UnityEngine;
using System.Collections;
//using UnityEditor;

public class CharacterSelection2D : MonoBehaviour {

	public static string suffixID = "_Prefab_Id";
	public static string suffixName = "_Prefab_Name";

	private bool debugShown = false;

	private Vector3 clickedPosition = Vector3.zero;
	public AudioClip characterInUseSound;
	public AudioClip characterSelected;				// Prefab Sound später abspielen

//	public GUIText player0GUIText;
//	public GUIText player1GUIText;
//	public GUIText player2GUIText;
//	public GUIText player3GUIText;

	public Sprite player0CharacterSprite;
	public Sprite player1CharacterSprite;
	public Sprite player2CharacterSprite;
	public Sprite player3CharacterSprite;
	
	private string debugmsg="";

	private LobbyCharacterManager lobbyCharacterManager;

//	string[] assetsPaths;

	void Awake()
	{
//		PlayerPrefs.DeleteAll();		CharacterSelection2D ist eine NetzwerkInstanz! -> PlayerPrefs auf Server löschen zB. in Class LobbyCharacterManager
	}

	void Start()
	{
		lobbyCharacterManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LobbyCharacterManager>();

	}

	// ArgumentException: You can only call GUI functions from inside OnGUI.
	void OnGUI()
	{
		if(EveryPlayerHasValidCharacter())
		{
			if(GUILayout.Button( "Start Game", GUILayout.Width( 100f ) ))
			{
				networkView.RPC("StartGame", RPCMode.All, "mp_classic_selected_character");
			}
		}
	}

	// Update is called once per frame
	void Update () {


		// Client Funktion
		// setze Clickposition und sende an Server (mit RPC)
		if(networkView == null || networkView.isMine)
		{

			if (Input.GetKey(KeyCode.Escape))
			{
				if(Network.isServer)
				{
					MasterServer.UnregisterHost();
					Debug.LogWarning("MasterServer.UnregisterHost();");

					foreach(NetworkPlayer player in Network.connections)
					{
						Network.CloseConnection(player,true);
						Debug.LogWarning("Network.CloseConnection("+player.ToString()+",true);");
					}
// schlecht!
//					for(int i=0;i<Network.connections.Length;i++)
//					{
//						Network.CloseConnection(Network.connections[i],true);
//						Debug.LogWarning("Network.CloseConnection(Network.connections["+i+"],true);");
//					}
				}
				Network.Disconnect();
				Debug.LogWarning("Network.Disconnect();");
				Application.LoadLevel("mp_Multiplayer");
				return;
			}

			//debugmsg = networkView.owner.ToString() + "\n";
			//Debug.Log(networkView.owner.ToString());		// Server: 0
															// Clients: 1 to Network.connections.Length;
			GetClickPosition();

//			if(networkView.owner.ToString() == "0")
//			{
//				if(!debugShown)
//				{
//					debugShown = true;
//					Debug.Log("Server");
//				}
//				if(player0GUIText != null)
//					player0GUIText.text = debugmsg;
//			}
//			else if(networkView.owner.ToString() == "1")
//			{
//				if(!debugShown)
//				{
//					debugShown = true;
//					Debug.Log("Client 1");
//				}
//				if(player1GUIText != null)
//					player1GUIText.text = debugmsg;
//			}
//			else if(networkView.owner.ToString() == "2")
//			{
//				if(!debugShown)
//				{
//					debugShown = true;
//					Debug.Log("Client 2");
//				}
//				if(player2GUIText != null)
//					player2GUIText.text = debugmsg;
//			}
//			else if(networkView.owner.ToString() == "3")
//			{
//				if(!debugShown)
//				{
//					debugShown = true;
//					Debug.Log("Client 3");
//				}
//				if(player3GUIText != null)
//					player3GUIText.text = debugmsg;
//			}
		}
	}

	void SetPlayerCharater( string playerId, string characterPrefabName)
	{
		string key = playerId + suffixName;
		key = key.ToLower();
		PlayerPrefs.SetString(key, characterPrefabName);
	}

	bool PlayerHasValidCharacter( string playerId )
	{
		string key = playerId;
		key = key + suffixName;														//suffixName!!
		key = key.ToLower();
		if(PlayerPrefs.HasKey(key))
		{
			string value = PlayerPrefs.GetString(key);
			if(value == "")															// PrefabName = "" --> ERROR
			{
				Debug.LogError("PlayerPrefs " + key + " value is an empty string"); 
				return false;
			}
			if(GameObject.Find(value) != null)										// GameObject with PrefabName found? no --> ERROR
			{
				return true;
			}
			else
			{
				Debug.LogError("PlayerPrefs " + key + " with value " + value + " no GameObject in Scene found");
				return false;
			}
		}
		else
		{
			Debug.LogError("Player " + playerId + " has no character selected");
			return false;
		}

	}

	bool ServerPlayerHasValidCharacter()
	{
		// Server Character
		string playerID = "0";
		if(PlayerHasValidCharacter(playerID))
		{
			return true;
		}
		return false;
	}

	bool EveryPlayerHasValidCharacter()
	{
		// Server Character
		if(!ServerPlayerHasValidCharacter())
		{
			return false;
		}

		// Clients
		foreach(NetworkPlayer player in Network.connections)
		{
			if(!PlayerHasValidCharacter(player.ToString()))
				return false;
		}
		return true;
	}

	[RPC]
	void StartGame(string sceneName)
	{
		// starte das gewünschte Level auf allen Clients
		// Hash / static string scenename/levelname
		NetworkLevelLoader.Instance.LoadLevel(sceneName,0);
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
					if(Network.isServer)										// wenn Client auch Server ist
					{
						// Client ist auch Server
//						NetworkMessageInfo myInfo = new NetworkMessageInfo();
//						NetworkPlayer myPlayer = networkView.owner;
//	schreib schutz		myInfo.sender = myPlayer;
//	schreib schutz		myInfo.networkView = this.networkView;
						CharacterClicked(clickedPosition);											// Server ruft Funktion selbst auf
//						networkView.RPC("CharacterClicked", RPCMode.Server, clickedPosition);		// Server bekommt diese RPC (RPCMode.Server) nicht
					}
					else
					{
						// Client ist nur Client
						networkView.RPC("CharacterClicked", RPCMode.Server, clickedPosition);		// RPC geht von Client an Server
					}
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
//		Debug.LogWarning("RPC CharacterClicked");
		if(!Network.isServer)
			return;

		string playerClickedID = info.sender.ToString();
		bool characterInUse = false;

		string characterName = lobbyCharacterManager.GetSelectedCharacterName(recvPos);
		GameObject prefab = GameObject.Find(characterName);
		if( prefab != null)
		{
			// Prefab (GameObject) in Scene gefunden
			Debug.Log("Server: Prefab " + characterName + " gefunden.");

			characterInUse = CheckPrefabInUse(characterName);
			
			if(!characterInUse)
			{
				SetPlayerCharater(playerClickedID, characterName);	// Register CharacterPrefab with Player in PlayerPref
				// kein Spieler hat diesen Character gewählt, Client Character zuteilen und freigabe mitteilen.
				
				// Zuteilung allen Clients mitteilen
				networkView.RPC( "AllowSelectedCharacter", RPCMode.All, playerClickedID, characterName );	// RPC geht von Server an alle
				// RPCMode.All wird auch auf Server ausgeführt															// allen Clients Characterauswahl des Clients(playerClickedID) mitteilen
				//			AllowSelectedCharacter(playerClickedID, recvPos);											// RPC auch am Server ausführen
				// auch Master Clients Characterauswahl des Clients(playerClickedID) mitteilen
			}
			else
			{
				// Character schon in Verwendung
				
				// anfragendem Client mitteilen (spielt sound ab)
				networkView.RPC( "SelectedCharacterInUse", info.sender );									// RPC geht von Server an requested Client
			}
		}
		else
		{
			// keine Prefab (GameObject) mit passendem Name in Scene gefunden
			Debug.LogError("Server: Prefab " + characterName + " NICHT gefunden.");
			return;																			// RPC abbrechen!
		}


	}

	/**
	 *	Server Funktion (RPC geht nicht von Server -> Server )
	 **/
	void CharacterClicked(Vector3 recvPos)
	{
//		Debug.LogWarning("local CharacterClicked");
		if(!Network.isServer)
			return;

		string playerClickedID = networkView.owner.ToString();
		bool characterInUse = false;
		
		string characterName = lobbyCharacterManager.GetSelectedCharacterName(recvPos);
		GameObject prefab = GameObject.Find(characterName);

		if( prefab != null)
		{
			// Prefab (GameObject) in Scene gefunden
			Debug.Log("MasterClient: Prefab " + characterName + " gefunden.");

			characterInUse = CheckPrefabInUse(characterName);
			
			if(!characterInUse)
			{
				SetPlayerCharater(playerClickedID, characterName);	// Register CharacterPrefab with Player in PlayerPref
				// kein Spieler hat diesen Character gewählt, Client Character zuteilen und freigabe mitteilen.
				
				// Zuteilung allen Clients mitteilen
				networkView.RPC( "AllowSelectedCharacter", RPCMode.All, playerClickedID, characterName );							// allen Clients Serverauswahl mitteilen (MasterClient bekommt diese RPC auch!)
			}
			else
			{
				// Character schon in Verwendung
				
				// Master Client hat angefragt ->  spielt sound ab
				SelectedCharacterInUse();																						// Läuft local auf Server
			}
		}
		else
		{
			// keine Prefab (GameObject) mit passendem Name in Scene gefunden
			Debug.LogError("Server: Prefab " + characterName + " NICHT gefunden.");
			return;																			// RPC abbrechen!
		}
	}


	/**
	 * RPC des Clients, (Server fordert Client auf diese Funktion zu starten)
	 **/
	[RPC]
	void AllowSelectedCharacter(string networkPlayerID, string characterPrefabName, NetworkMessageInfo info)
	{
		Debug.LogWarning("RPC AllowSelectedCharacter");
		MarkCharacter(networkPlayerID, characterPrefabName);
		//		transform.position = recvPos;
		//		renderer.enabled = true;
	}
//	void AllowSelectedCharacter(string networkPlayerID, Vector3 recvPos)
//	{
//		Debug.LogWarning("AllowSelectedCharacter");
//		MarkCharacter(networkPlayerID, recvPos);
//		//		transform.position = recvPos;
//		//		renderer.enabled = true;
//	}


	/**
	 * RPC des Clients, (Server fordert Client auf diese Funktion zu starten)
	 **/
	[RPC]
	void SelectedCharacterInUse(NetworkMessageInfo info)
	{
		// Character already in Use
		Debug.LogWarning("RPC Character already in Use");
		AudioSource.PlayClipAtPoint(characterInUseSound,transform.position,1);
	}
	void SelectedCharacterInUse()
	{
		// Character already in Use
		Debug.LogWarning("Character already in Use");
		AudioSource.PlayClipAtPoint(characterInUseSound,transform.position,1);
	}


	/**
	 * Charakter selection Animation 
	 **/
	void MarkCharacter(string networkPlayerID, string characterPrefabName)
	{
		// Character Sound 
		Debug.Log("Player " + networkPlayerID + " hat Character " + characterPrefabName + " gewählt");
		AudioSource.PlayClipAtPoint(characterSelected,transform.position,1);

		//Network.Instantiate();..
		net_DoSpawn( getRandomPosition(), characterPrefabName );
	}

	Vector3 getRandomPosition()
	{
		return new Vector3(Random.Range(0.0f, 19.0f), Random.Range(2f, 15.0f), 0f);
	}

	[RPC]
	void net_DoSpawn( Vector3 position, string characterPrefabName )
	{
		// The object PikachuLanRigidBody2D must be a prefab in the project view.
		// spawn the player paddle
		GameObject myCharacter = GameObject.Find (characterPrefabName);
		Network.Instantiate( myCharacter, position, Quaternion.identity,0 );
	}


	/**
	 * Check in PlayerPrefs
	 **/
	bool CheckPrefabInUse(string characterPrefabName)
	{
		/*
		 * ACHTUNG!!!! Server hat ID 0, NetworkPlayer geht bei 1 los!
		 * //foreach(NetworkPlayer player in Network.connections)
		 * 
		 * wenn player 2 disconnected, wird slot nicht direkt freigegeben!
		 * Debug.LogWarning("Verbindungsanzahl: " + Network.connections.Length);
		 */
		
		// Charactere der Clients checken
		foreach(NetworkPlayer player in Network.connections)
		{
			if(PlayerHasValidCharacter(player.ToString()))
			{
				if(GetPlayerCharacter(player.ToString()) == characterPrefabName)
				{
					return true;
				}
			}
		}
		// Charactere des Master Clients (Server) checken
		if(ServerPlayerHasValidCharacter())
		{
			if(GetPlayerCharacter("0") == characterPrefabName)
			{
				return true;
			}
		}
		return false;
	}

	string GetPlayerCharacter(string playerId)
	{
		string key = playerId + suffixName;
		key = key.ToLower();
		return PlayerPrefs.GetString(key);
	}

//	/**
//	 * Check in PlayerPrefs
//	 **/
//	bool CheckPrefabIdInUse(int characterPrefabID)
//	{
//		/*
//		 * 
//		 * ACHTUNG!!!! Server hat ID 0, NetworkPlayer geht bei 1 los!
//		 * //foreach(NetworkPlayer player in Network.connections)
//		 * 
//		 * wenn player 2 disconnected, wird slot nicht direkt freigegeben!
//		 */
//
//		Debug.LogWarning("Verbindungsanzahl: " + Network.connections.Length);
//
//		string key;
//		int value;
//
//		// Charactere der Clients checken
//		foreach(NetworkPlayer player in Network.connections)
//		{
//			key = player.ToString() + suffixID;		// Key um in PlayerPrefs nach einträgen zu schauen
//			key = key.ToLower();
//			value = PlayerPrefs.GetInt(key);			// Value 
//			if(value == characterPrefabID)
//			{
//				return true;
//			}
//		}
//		// Charactere des Master Clients (Server) checken
//		key = "0" + suffixID;
//		key = key.ToLower();
//		if(PlayerPrefs.GetInt(key) == characterPrefabID)
//			return true;
//		return false;
//    }

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
