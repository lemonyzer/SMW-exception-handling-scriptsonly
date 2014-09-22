using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectToGame : MonoBehaviour
{
	public string registeredGameName = "smw_alpha";
	public string registeredGameType = "platformer";
	public string registeredGameComment = "classic";

	public string testStatus = "Testing network connection capabilities.";
	public string testMessage = "Test in progress";
	public string shouldEnableNatMessage;
	public bool doneTesting = false;
	public bool probingPublicIP = false;
	private int serverPort = 25005;
	public ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;
	public float timer = 0;
	// Indicates if the useNat parameter be enabled when starting a server
	public bool useNat = false;

	public int clientSlots = 10;

	private string ip = "tofast.ddns.net";
	private int port = 25005;


	// masterserver host list

	// are we currently trying to download a host list?
	bool loadingHostList = false;
	// the current position within the scrollview
	private Vector2 scrollPos = Vector2.zero;


	private string ipPC = "192.168.0.129";
	private string ipMWhite = "192.168.0.113";
	private string ipMBlack = "192.168.0.146";

	List<Host> hostList;

	class Host
	{
		public string name;
		public string ip;

		public Host(string name, string ip)
		{
			this.name = name;
			this.ip = ip;
		}
	}

	NatTest natTest;

	void Awake()
	{
		hostList = new List<Host>();
		hostList.Add(new Host("PC", ipPC));
		hostList.Add(new Host("MWhite", ipMWhite));
		hostList.Add(new Host("MBlack", ipMBlack));

		InitGUIStyle ();

		//UPnPTest();
		natTest = new NatTest();
		natTest.Start();

		myIP = Network.player.ipAddress;
		myExternalIP = Network.player.externalIP;
//		if(Network.HavePublicAddress())
//		{
//			myExternalIP = Network.player.externalIP;
//		}
//		else
//		{
//			myExternalIP = "unknown";
//		}
	}

	/**
	 * GUIStyle
	 **/
	GUIStyle buttonStyle;
	float minButtonHeight;
	
	void InitGUIStyle()
	{
		buttonStyle = new GUIStyle ();
		buttonStyle.stretchWidth = false;
		
		if(Screen.dpi != 0)
		{
			minButtonHeight = 20f * Screen.height / Screen.dpi;
		}
		else
		{
			minButtonHeight = 20f;
		}
	}

	public string upnpStatus = "Test not started";

	void UPnPTest()
	{
		upnpStatus = "Test running";
        if(NAT.DiscoverWithTryCatch())
		{
			upnpStatus = "You have an UPnP-enabled router and your IP is: "+NAT.GetExternalIP();
		}
		else
			upnpStatus = "You do not have an UPnP-enabled router.";
    }

	bool currentTestRunning = false;

	void Start()
	{
		// immediately request a list of hosts
		refreshHostList();
	}

	void refreshHostList()
	{
		// let the user know we are awaiting results from the master server
		loadingHostList = true;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList( registeredGameType );
	}
	
	// this is called when the Master Server reports an event to the client – for example, server registered successfully, host list received, etc
	void OnMasterServerEvent( MasterServerEvent msevent )
	{
		if( msevent == MasterServerEvent.HostListReceived )
		{
			// received the host list, no longer awaiting results
			loadingHostList = false;
		}
	}

	bool useNatToConnect = true;

	void OnGUI()
	{
//		if(Network.peerType == NetworkPeerType.Client ||
//		   Network.peerType == NetworkPeerType.Server)
//			return;
		GUILayout.Label("IP: " + myIP);
		GUILayout.Label("external IP: " + myExternalIP);
		GUILayout.Label("UPnP Status: " + upnpStatus);

		GUILayout.Label("Current Status: " + testStatus);
		GUILayout.Label("Test result : " + testMessage);
		GUILayout.Label(shouldEnableNatMessage);


		// let the user enter IP address
		GUILayout.Label( "IP Address" );
		ip = GUILayout.TextField( ip, GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) );

		GUILayout.BeginHorizontal();
		// let the user enter port number
		// port is an integer, so only numbers are allowed
		GUILayout.Label( "Port" );
		string port_str = GUILayout.TextField( port.ToString(), GUILayout.Width( 70f ), GUILayout.MinHeight(minButtonHeight) );
		int port_num = port;
		if( int.TryParse( port_str, out port_num ) )
			port = port_num;

		// connect to the IP and port
		if( GUILayout.Button( "Connect", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			Network.Connect( ip, port );
		}
		GUILayout.EndHorizontal();

//		if (!doneTesting)
//		{
		if(currentTestRunning)
		{
			TestConnection();
		}
		else
		{
		}
		//			return;
//		}

		GUI.enabled = !currentTestRunning;
		if( GUILayout.Button( "Test Connection", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			if(!doneTesting)
			{
				// test lief noch nicht
//				Network.InitializeServer( clientSlots, port, useNat );
//				MasterServer.RegisterHost(registeredGameType, registeredGameName, registeredGameComment);

				TestConnection();
				currentTestRunning = true;
				currentTestFinished = false;
				myIP = Network.player.ipAddress;
                myExternalIP = Network.player.externalIP;
			}
			else
			{
				// test wurde schon einmal gestartet -> server ist bereits initialisiert!
				doneTesting = false;
				TestConnection(true);
				currentTestRunning = true;
			}
			
			//			You can call RegisterHost more than once while a server is running 
			//			to update the information stored on the Master Server. For example, 
			//			if the server changes to a new level, you might call RegisterHost 
			//			again to update the lobby.
		}

		GUI.enabled = true;
		// host a server on the given port, only allow 3 incoming connection (3 other players)
		if( GUILayout.Button( "Host offline", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			Network.Disconnect();
			hosting = true;
			Network.InitializeServer( clientSlots, port, useNat );
			
			//			You can call RegisterHost more than once while a server is running 
			//			to update the information stored on the Master Server. For example, 
			//			if the server changes to a new level, you might call RegisterHost 
			//			again to update the lobby.
		}
		GUI.enabled = doneTesting;
		if( GUILayout.Button( "Host online", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			Network.Disconnect();
			hosting = true;
			Network.InitializeServer( clientSlots, port, useNat );
			MasterServer.RegisterHost(registeredGameType, registeredGameName, registeredGameComment);


//			You can call RegisterHost more than once while a server is running 
//			to update the information stored on the Master Server. For example, 
//			if the server changes to a new level, you might call RegisterHost 
//			again to update the lobby.
		}
		GUI.enabled = true;

		GUILayout.BeginArea(new Rect(Screen.width * 0.5f, 0, Screen.width * 0.5f, Screen.height));
		GUILayout.Label("Servers");

		useNatToConnect = GUILayout.Toggle (useNatToConnect, "Use NAT punchthrough and connect with MasterServer help");

		if( GUILayout.Button( "Refresh", GUILayout.MinHeight(minButtonHeight) ) )
		{
			refreshHostList();
		}
		
		if( loadingHostList )
		{
			GUILayout.Label( "Loading..." );
		}
		else
		{
			scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.Width( 200f ), GUILayout.Height( 200f ) );
			
			HostData[] hosts = MasterServer.PollHostList();
			for( int i = 0; i < hosts.Length; i++ )
			{
				if( GUILayout.Button( hosts[i].gameName, GUILayout.ExpandWidth( true ) ) )
				{
					if(useNatToConnect)
					{
						Debug.Log("Connecting to " + hosts[i].guid + " with help of NAT punchthrough" );
						Network.Connect( hosts[i].guid );
					}
					else
					{
						Debug.Log("Connecting to " + hosts[i].ToString() + " no NAT punchthrough" );
						Network.Connect( hosts[i] );
					}
				}
			}
			
			if( hosts.Length == 0 )
			{
				GUILayout.Label( "No servers running" );
			}
			
			GUILayout.EndScrollView();
		}

		foreach(Host host in hostList)
		{
			if( GUILayout.Button( host.ip + " " + host.name,  GUILayout.MinHeight(minButtonHeight) ) )
			{
				Network.Connect( host.ip, port );
			}
		}
		GUILayout.EndArea();

	}

	void OnConnectedToServer()
	{
		Debug.Log( "Connected to server" );
		// this is the NetworkLevelLoader we wrote earlier in the chapter – pauses the network, loads the level, waits for the level to finish, and then unpauses the network
		NetworkLevelLoader.Instance.LoadLevel( Scenes.unityNetworkGameRoom );
	}

	private bool hosting = false;

	void OnServerInitialized()
	{
		if(!hosting)
		{
			// server wurde initiiert um TestConnection aufbauen zu lassen. (Test Server... keine neue Scene laden!)
			TestConnection();
			currentTestRunning = true;
			currentTestFinished = false;
			myIP = Network.player.ipAddress;
			myExternalIP = Network.player.externalIP;
//			if(Network.HavePublicAddress())
//			{
//				myExternalIP = Network.player.externalIP;
//			}
//			else
//			{
//				myExternalIP = "unknown";
//			}
			return;
		}

		Debug.Log( "Server initialized" );
		NetworkLevelLoader.Instance.LoadLevel( Scenes.unityNetworkGameRoom );
	}

	string myIP = "unknown";
	string myExternalIP = "unknown";
	bool currentTestFinished = false;

	void TestConnection(bool forceTest = false) {
		// Start/Poll the connection test, report the results in a label and 
		// react to the results accordingly
		connectionTestResult = Network.TestConnection(forceTest);
		switch (connectionTestResult) {
		case ConnectionTesterStatus.Error: 
			testMessage = "Problem determining NAT capabilities";
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.Undetermined: 
			testMessage = "Undetermined NAT capabilities";
			doneTesting = false;
			break;
			
		case ConnectionTesterStatus.PublicIPIsConnectable:
			testMessage = "Directly connectable public IP address.";
			useNat = false;
			doneTesting = true;
			break;
			
			// This case is a bit special as we now need to check if we can 
			// circumvent the blocking by using NAT punchthrough
		case ConnectionTesterStatus.PublicIPPortBlocked:
			testMessage = "Non-connectable public IP address (port " + serverPort +" blocked), running a server is impossible.";
			useNat = false;
			// If no NAT punchthrough test has been performed on this public 
			// IP, force a test
			if (!probingPublicIP) {
				connectionTestResult = Network.TestConnectionNAT();
				probingPublicIP = true;
				testStatus = "Testing if blocked public IP can be circumvented";
				timer = Time.time + 10;
			}
			// NAT punchthrough test was performed but we still get blocked
			else if (Time.time > timer) {
				probingPublicIP = false; 		// reset
				useNat = true;
				doneTesting = true;
			}
			break;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			testMessage = "Public IP address but server not initialized,\n"+	"it must be started to check server accessibility. Restart\n"+ "connection test when ready.";
			break;
			
		case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
			testMessage = "Limited NAT punchthrough capabilities. Cannot\n"+ "connect to all types of NAT servers. Running a server\n"+ "is ill advised as not everyone can connect.";
			useNat = true;
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
			testMessage = "Limited NAT punchthrough capabilities. Cannot\n"+ "connect to all types of NAT servers. Running a server\n"+ "is ill advised as not everyone can connect.";
			useNat = true;
			doneTesting = true;
			break;


		case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
			testMessage = "NAT punchthrough capable. Can connect to all\n"+ "servers and receive connections from all clients. Enabling\n"+ "NAT punchthrough functionality.";
			useNat = true;
			doneTesting = true;
			break;
		case ConnectionTesterStatus.NATpunchthroughFullCone:
			testMessage = "NAT punchthrough capable. Can connect to all\n"+ "servers and receive connections from all clients. Enabling\n"+ "NAT punchthrough functionality.";
			useNat = true;
			doneTesting = true;
			break;
			
		default: 
			testMessage = "Error in test routine, got " + connectionTestResult;
			break;
		}
		if (doneTesting) {
			currentTestRunning = false;
			if (useNat)
				shouldEnableNatMessage = "When starting a server the NAT\n"+ "punchthrough feature should be enabled (useNat parameter)";
			else
				shouldEnableNatMessage = "NAT punchthrough not needed";
			testStatus = "Done testing";
		}
	}
}