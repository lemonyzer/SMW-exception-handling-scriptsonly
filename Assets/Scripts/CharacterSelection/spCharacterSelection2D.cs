using UnityEngine;
using System.Collections;
//using UnityEditor;

public class spCharacterSelection2D : MonoBehaviour
{
	private int currentPlayer = 0;

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
	
	private string debugmsg = "";

	private LobbyCharacterManager lobbyCharacterManager;
	private Layer layer;

//	string[] assetsPaths;

	void Awake()
	{
//		PlayerPrefs.DeleteAll();		CharacterSelection2D ist eine NetzwerkInstanz! -> PlayerPrefs auf Server löschen zB. in Class LobbyCharacterManager
	}

	void Start()
	{
		lobbyCharacterManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LobbyCharacterManager>();
		layer = lobbyCharacterManager.GetComponent<Layer>();
	}



	void BackButton()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			if(networkView == null)
			{
				Debug.Log("SinglePlayer");
				Application.LoadLevel("MainMenu");
				return;
			}
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
	}

	// Update is called once per frame
	void Update () {


		// Client Funktion
		// setze Clickposition und sende an Server (mit RPC)
		if(networkView == null || networkView.isMine)
		{

			BackButton();

			//debugmsg = networkView.owner.ToString() + "\n";
			//Debug.Log(networkView.owner.ToString());		// Server: 0
															// Clients: 1 to Network.connections.Length;
			GetClickPosition();
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
				if(hit.collider.gameObject.layer == layer.player1 ||
				   hit.collider.gameObject.layer == layer.player2 ||
				   hit.collider.gameObject.layer == layer.player3 ||
				   hit.collider.gameObject.layer == layer.player4)
				{
					clickedPosition = Input.mousePosition;
					if(networkView == null)
					{
						// SinglePlayer
//						Debug.Log("SinglePlayer");
						CharacterClickedSinglePlayer(clickedPosition);
					}
					else
					{
						// Multiplayer
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

			characterInUse = lobbyCharacterManager.CheckPrefabInUse(characterName);
			
			if(!characterInUse)
			{
				lobbyCharacterManager.SetPlayerCharacter(playerClickedID, characterName);	// Register CharacterPrefab with Player in PlayerPref
				Debug.Log("?Eingetragen: " + lobbyCharacterManager.GetPlayerCharacter(playerClickedID));
				Debug.Log("?Eingetragen: " + lobbyCharacterManager.GetPlayerCharacter(playerClickedID));
				// kein Spieler hat diesen Character gewählt, Client Character zuteilen und freigabe mitteilen.
				
				// Zuteilung allen Clients mitteilen
	 			networkView.RPC( "AllowSelectedCharacter", RPCMode.All, playerClickedID, characterName );	// RPC geht von Server an alle
//				MarkCharacter(playerClickedID,characterName);
				// RPCMode.All wird auch auf Server ausgeführt															// allen Clients Characterauswahl des Clients(playerClickedID) mitteilen
				//			AllowSelectedCharacter(playerClickedID, recvPos);											// RPC auch am Server ausführen
				// auch Master Clients Characterauswahl des Clients(playerClickedID) mitteilen
				Vector3 pos = new Vector3(0f, 6.75f, 0f);
				networkView.RPC( "net_DoSpawn", info.sender, pos, characterName);
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
			
			characterInUse = lobbyCharacterManager.CheckPrefabInUse(characterName);
			
			if(!characterInUse)
			{
				lobbyCharacterManager.SetPlayerCharacter(playerClickedID, characterName);	// Register CharacterPrefab with Player in PlayerPref
				Debug.Log("?Eingetragen: " + lobbyCharacterManager.GetPlayerCharacter(playerClickedID));
				// kein Spieler hat diesen Character gewählt, Client Character zuteilen und freigabe mitteilen.
				
				// Zuteilung allen Clients mitteilen
				networkView.RPC( "AllowSelectedCharacter", RPCMode.All, playerClickedID, characterName );							// allen Clients Serverauswahl mitteilen (MasterClient bekommt diese RPC auch!)
				//MarkCharacter(playerClickedID,characterName);
				DoSpawnServerPlayer(getRandomSpawnPosition(),characterName);
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

	void CharacterClickedSinglePlayer(Vector3 recvPos)
	{
		string playerClickedID = ""+currentPlayer;
		bool characterInUse = false;
		
		string characterName = lobbyCharacterManager.GetSelectedCharacterName(recvPos);
		GameObject prefab = GameObject.Find(characterName);
		
		if( prefab != null)
		{
			// Prefab (GameObject) in Scene gefunden
			Debug.Log("MasterClient: Prefab " + characterName + " gefunden.");
			
			characterInUse = lobbyCharacterManager.CheckPrefabInUseSinglePlayer(characterName);
			
			if(!characterInUse)
			{
				lobbyCharacterManager.SetPlayerCharacter(playerClickedID, characterName);	// Register CharacterPrefab with Player in PlayerPref
				Debug.Log("?Eingetragen: " + lobbyCharacterManager.GetPlayerCharacter(playerClickedID));
				MarkCharacter(playerClickedID,characterName);
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



//	The owner of a NetworkView is defined by the owner of the NetworkViewID. The owner of a NetworkViewID can't be changed. The owner is set to the peer that creates the ViewID.
//
//	Whenever someone calls Network.Instantiate and the instantiated object contains a NetworkView, Unity will automatically create / allocate a ViewID for each NetworkView. Those get send to the other users via RPC. The person that calls Network.Instantiate is automatically the owner of the object.
//
//	To create / allocate a ViewID manually you can use Network.AllocateViewID, but you have to take care of distributing it across the Network, usually with RPCs. This would be the only way to "change" the owner after an object has been instantiated.
//
//	So in your case just make sure the objects get instantiated by the desired owner.
//
//	If you want to change the owner of a NetworkView, you have to use an RPC and send a new NetworkViewID (allocated by the new owner) to everyone and replace the old one. Keep in mind when your prefab contains multiple NetworkViews, you need a new ViewID for each of them.

//	[RPC]
//	void AllocateSelectedCharacter(string characterPrefabName, NetworkMessageInfo info)
//	{
////		if(networkView.owner.ToString() == networkPlayerID)
////		{
//			// anfragender Client
//			GameObject myCharacter = GameObject.Find (characterPrefabName);
//			Debug.Log("Old myCharacter.networkView.viewID: " + myCharacter.networkView.viewID.ToString());
//			NetworkViewID nvID = Network.AllocateViewID();
//			myCharacter.networkView.viewID = nvID;
//			Debug.Log("Now myCharacter.networkView.viewID: " + myCharacter.networkView.viewID.ToString());
//			myCharacter.GetComponent<PlatformCharacter>().enabled = true;
//			myCharacter.GetComponent<PlatformUserControlAnalogStickAndButton>().enabled = true;
//			myCharacter.GetComponent<PlatformUserControlKeyboard>().enabled = true;
////		}
//	}


	/**
	 * RPC des Clients, (Server fordert Clients auf diese Funktion zu starten)
	 * Server teilt Clients mit welcher Player einen neuen Character gewählt hat
	 **/
	[RPC]
	void AllowSelectedCharacter(string networkPlayerID, string characterPrefabName, NetworkMessageInfo info)
	{
		Debug.LogWarning("RPC AllowSelectedCharacter");
		MarkCharacter(networkPlayerID, characterPrefabName);
		//		transform.position = recvPos;
		//		renderer.enabled = true;
	}

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
	void MarkCharacter(string playerID, string characterPrefabName)
	{
		// Character Sound 
		Debug.Log("Player " + playerID + " hat Character " + characterPrefabName + " gewählt");
		AudioSource.PlayClipAtPoint(characterSelected,transform.position,1);


		lobbyCharacterManager.SetPlayerSprite(playerID, characterPrefabName);

		if(networkView == null)
		{
			SpawnPlayer(getRandomSpawnPosition(), characterPrefabName);
			currentPlayer++;
			if(currentPlayer == 4)
				currentPlayer = 0;
		}

		//Network.Instantiate();..
//		net_DoSpawn( getRandomPosition(), characterPrefabName );
	}


	Vector3 getRandomSpawnPosition()
	{
		return new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(2f, 15.0f), 0f);
	}

	[RPC]
	void net_DoSpawn( Vector3 position, string characterPrefabName )
	{
		// The object PikachuLanRigidBody2D must be a prefab in the project view.
		// spawn the player paddle

// wäre Besser?! (alle GameObjects in scene, keine "manipulation") .... geht aber nicht, GameObject vorher clonen mit Instantiate(....)
//		GameObject myCharacter = GameObject.Find (characterPrefabName);

		GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + characterPrefabName, typeof(GameObject)); // in Resources Folder! \Assests\Resources\characterPrefabName
//		PlatformCharacter myPlatformCharacter = myCharacter.GetComponent<PlatformCharacter>();
//		AudioSource.PlayClipAtPoint(myPlatformCharacter.jumpSound,transform.position,1);
		if(myCharacter != null)
			Network.Instantiate( myCharacter, position, Quaternion.identity,0 );
	}

	void DoSpawnServerPlayer( Vector3 position, string characterPrefabName )
	{
		GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + characterPrefabName, typeof(GameObject)); // in Resources Folder! \Assests\Resources\characterPrefabName
		if(myCharacter != null)
			Network.Instantiate( myCharacter, position, Quaternion.identity,0 );
	}


	void SpawnPlayer( Vector3 position, string characterPrefabName )
	{
		GameObject myCharacter = (GameObject) Resources.Load(LobbyCharacterManager.resourcesPath + characterPrefabName, typeof(GameObject)); // in Resources Folder! \Assests\Resources\characterPrefabName
		if(myCharacter != null)
			Instantiate( myCharacter, position, Quaternion.identity);
	}


}
