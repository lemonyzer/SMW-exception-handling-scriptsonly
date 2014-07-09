using UnityEngine;
using System.Collections;

public class ConnectToPhotonGameLobby : MonoBehaviour
{

//	bool usePhoton = true;

	bool joinedLobby = false;
	bool joinedRoom = false;

	string roomName = "bouncy";

	void Start()
	{
		// connect to Photon
		PhotonNetwork.ConnectUsingSettings( "v1.0" );
	}
	void OnJoinedLobby()
	{
		// we joined Photon and are ready to get a list of rooms
		joinedLobby = true;
	}
	void OnFailedToConnectToPhoton( DisconnectCause cause )
	{
		// some error occurred, 'cause' is an enumeration of the error that happened
	}

	// We have two options here: we either joined(by title, list or random) or created a room.
	private void OnJoinedRoom()
	{
		Debug.Log("We have joined a room.");
		StartCoroutine(MoveToGameScene());
	}

	private IEnumerator MoveToGameScene()
	{
		//Wait for the 
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		
		Debug.LogWarning("Normally we would load the game scene right now.");
        PhotonNetwork.LoadLevel("pun_CharacterSelection");
		
	}
	
	private void OnCreatedRoom()
	{
		Debug.Log("We have created a room.");
		//When creating a room, OnJoinedRoom is also called, so we don't have to do anything here.
	}

	void OnGUI()
	{
		if( !joinedLobby )
		{
			// we're still connecting to photon, display a loading GUI
		}
		else if( joinedLobby )
		{
			// we're connected to Photon, so now we can draw the lobby screen
			drawLobbySetting();
			drawLobby();
		}
	}

	void drawLobbySetting()
	{
		// if we're not inside a room, let the player create a room
		if( PhotonNetwork.room == null )
		{
			string str_RoomName = GUILayout.TextField( roomName, GUILayout.Width( 100f ), GUILayout.Height (minButtonHeight));
			if(GUILayout.Button( "Create Room", GUILayout.Width( Screen.width*0.5f - 10f ), GUILayout.Height (minButtonHeight) ) )
			{
				// create a room called "RoomNameHere", visible to the
				// lobby, allows other players to join, and allows up to 8
				// players
				PhotonNetwork.CreateRoom( str_RoomName, true, true, 8 );
			}
		}
		else
		{
			// we're connected to a room, display some info such as room
			// name, player count, etc.

			// disconnect from the current room
			// if disconnect button clicked
			if(GUILayout.Button( "Leave Room", GUILayout.Width( Screen.width*0.5f - 10f ), GUILayout.Height (minButtonHeight) ) )
			{
				PhotonNetwork.LeaveRoom();
			}
		}
	}

	void drawLobby()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5f, 0, Screen.width*0.5f, Screen.height));
		GUILayout.Label("Serverlist from Photon Server");
		// no rooms available
		if( PhotonNetwork.GetRoomList().Length == 0 )
		{
			// display a message informing the user that there aren't any rooms available to join
			if( GUILayout.Button( "no rooms found", GUILayout.Width( Screen.width*0.5f - 10f ), GUILayout.Height (minButtonHeight) ) )
			{
			}
		}
		// draw each room in a scroll view
		else
		{
			foreach( RoomInfo room in PhotonNetwork.GetRoomList() )
			{
				// draw the room info to the GUI
				if( GUILayout.Button( room.name + "", GUILayout.Width( Screen.width*0.5f - 10f ), GUILayout.Height (minButtonHeight) ) )
				{
					PhotonNetwork.JoinRoom( room.name );
				}
			}
		}
		GUILayout.EndArea();
	}

//	private string ip = "192.168.0.129";
//	private int port = 25005;
//	public int serverSlots = 7;
//
//	private string gameTypeName="mpSMW";
//	private string gameName="pers0rs bouncy bude";
//	private string comment="comeINandDIE";
//
////	private float timeoutHostList = 0.0f;
//	private float lastHostListRequest = -1000.0f;
//	private float hostListRefreshTimeout = 10.0f;
//
//
//	// are we currently trying to download a host list?
//	private bool loading = false;
////	private bool hostListRefreshed = false;
//	
//	// the current position within the scrollview
//	private Vector2 scrollPos = Vector2.zero;
//
//	private int numberOfAllPlayer;
//	private int numberOfAIPlayer;
//	private int numberOfLocalUserPlayer;


	private float minButtonHeight;

	void Awake()
	{
		if(Screen.dpi != 0)
		{
			minButtonHeight = 20f * Screen.height / Screen.dpi;
		}
		else
			minButtonHeight = 20f;

		MasterServer.ipAddress = "192.168.0.174";
		MasterServer.port = 23466;

		Network.natFacilitatorIP = "192.168.0.174";
		Network.natFacilitatorPort = 50005;
	}

//	void Start()
//	{
//
//		numberOfAllPlayer = 4;
//		numberOfAIPlayer = 0;
//		numberOfLocalUserPlayer = 1;
//		PlayerPrefs.SetInt("NumberOfAllPlayers",numberOfAllPlayer);
//		PlayerPrefs.SetInt("NumberOfAIPlayers",numberOfAIPlayer);
//		PlayerPrefs.SetInt("NumberOfLocalUserPlayers",numberOfLocalUserPlayer);
//
////		try{
////			InetAddress ownIP=InetAddress.getLocalHost();
////			ip = ownIP.getHostAddress();
////		}catch (Exception e){
////        }
//        
//        // immediately request a list of hosts
//		refreshHostList();
//	}

	void Update()
	{

		/**
		 * Android Softbutton: Back
		 **/
		if (Application.platform == RuntimePlatform.Android ||
		    Application.platform == RuntimePlatform.WindowsPlayer ||
		    Application.platform == RuntimePlatform.WindowsEditor)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				// Insert Code Here (I.E. Load Scene, Etc)
				// OR Application.Quit();
				Application.LoadLevel("MainMenu");
			}
		}
	}

//	void OnGUI()
//	{
//
//
//		// let the user enter IP address
//		GUILayout.Label( "IP Address" );
//		ip = GUILayout.TextField( ip, GUILayout.Width( 200f ) );
//		
//		// let the user enter port number
//		// port is an integer, so only numbers are allowed
//		GUILayout.Label( "Port" );
//		string port_str = GUILayout.TextField( port.ToString(), GUILayout.Width( 100f ) );
//		int port_num = port;
//		if( int.TryParse( port_str, out port_num ) )
//			port = port_num;
//		
//		// connect to the IP and port
//		if( GUILayout.Button( "Connect", GUILayout.Width( 100f ), GUILayout.Height (minButtonHeight) ) )
//		{
//			Network.Connect( ip, port );
//		}
//		
//		// host a server on the given port, only allow 1 incoming connection (one other player)
//		if( GUILayout.Button( "Host", GUILayout.Width( 100f ), GUILayout.Height (minButtonHeight) ) )
//		{
//			if(!usePhoton)
//			{
//				//InitializeServer(int connections, int listenPort, bool useNat);
//				//connections Anzahl (zusätzlich zum server selst)
//				Network.InitializeServer( serverSlots, port, true );
//			}
//			else
//			{
//
//			}
//		}
//
//		usePhoton = GUILayout.Toggle(usePhoton, "Photon Network", GUILayout.Width( 150f ), GUILayout.Height (minButtonHeight) );
//
//
//		if ( true )
//		{
////			hostListRefreshed = false;
//			GUILayout.BeginArea(new Rect(Screen.width*0.5f, 0, Screen.width*0.5f, Screen.height));
//			GUILayout.Label("Serverlist from Master Server");
//			if( loading )
//			{
//				GUILayout.Label( "Loading Serverlist..." );
//			}
//			scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.Width( Screen.width*0.5f ), GUILayout.Height( Screen.height - minButtonHeight -60f ) );
//			
//			HostData[] hosts = MasterServer.PollHostList();
//			for( int i = 0; i < hosts.Length; i++ )
//			{
//				Debug.Log("hosts: " + hosts.Length);
//				Debug.Log("hosts[" + i + "] gameName=" + hosts[i].gameName);
//				//if( GUI.Button(new Rect(10,40,210,30), hosts[i].gameName, GUILayout.ExpandWidth( true ) ) )
//				if( GUILayout.Button( hosts[i].guid + ":" + hosts[i].port + " " + hosts[i].gameName + " " + hosts[i].connectedPlayers + " " + hosts[i].playerLimit, GUILayout.Width( Screen.width*0.5f - 10f ), GUILayout.Height (minButtonHeight) ) )
//				{
//					Network.Connect( hosts[i].ip, hosts[i].port );
//				}
//			}
//			
//			if( hosts.Length == 0 )
//			{
//				GUILayout.Label( "No servers running" );
//			}
//			GUILayout.EndScrollView();
//
//			if( GUILayout.Button( "Refresh", GUILayout.Height (minButtonHeight)) || Time.realtimeSinceStartup > lastHostListRequest + hostListRefreshTimeout)
//			{
//				//				Debug.Log(Time.realtimeSinceStartup);
//				//				Debug.Log(lastHostListRequest);
//				//				Debug.Log(lastHostListRequest+hostListRefreshTimeout);
//				lastHostListRequest = Time.realtimeSinceStartup;
//				Debug.Log("Serverlist requested");
//				refreshHostList();
//			}
//			GUILayout.EndArea();
//
//		}
//	}

//	void refreshHostList()
//	{
//		// let the user know we are awaiting results from the master server
//		loading = true;
//		MasterServer.ClearHostList();
//		MasterServer.RequestHostList( gameTypeName );
//	}
//	
//	// this is called when the Master Server reports an event to the client – for example, server registered successfully, host list received, etc
//	void OnMasterServerEvent( MasterServerEvent msevent )
//	{
//		if( msevent == MasterServerEvent.HostListReceived )
//		{
//			// received the host list, no longer awaiting results
//			loading = false;
////			hostListRefreshed = true;
//			Debug.Log("MasterServerEvent.HostListReceived");
//		}
//	}
//	
//	void OnConnectedToServer()
//	{
//		Debug.Log( "Connected to server" );
//		// this is the NetworkLevelLoader we wrote earlier in the chapter – pauses the network, loads the level, waits for the level to finish, and then unpauses the network
//		NetworkLevelLoader.Instance.LoadLevel( "mp_CharacterSelection",0 );
//	}
//	
//	void OnServerInitialized()
//	{
//		Debug.Log( "Server initialized" );
//		MasterServer.RegisterHost(gameTypeName,gameName,comment);
//		NetworkLevelLoader.Instance.LoadLevel( "mp_CharacterSelection",0 );
//	}
//	
//	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
//		Debug.Log(info);
//	}
//	
//	void OnFailedToConnect(NetworkConnectionError info) {
//		Debug.Log(info);
//	}
}