using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectToGame : MonoBehaviour
{
	private string ip = "192.168.0.129";
	private int port = 25005;

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

	void Awake()
	{
		hostList = new List<Host>();
		hostList.Add(new Host("PC", ipPC));
		hostList.Add(new Host("MWhite", ipMWhite));
		hostList.Add(new Host("MBlack", ipMBlack));

		InitGUIStyle ();
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

	void OnGUI()
	{
		// let the user enter IP address
		GUILayout.Label( "IP Address" );
		ip = GUILayout.TextField( ip, GUILayout.Width( 200f ), GUILayout.MinHeight(minButtonHeight) );

		// let the user enter port number
		// port is an integer, so only numbers are allowed
		GUILayout.Label( "Port" );
		string port_str = GUILayout.TextField( port.ToString(), GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) );
		int port_num = port;
		if( int.TryParse( port_str, out port_num ) )
			port = port_num;

		// connect to the IP and port
		if( GUILayout.Button( "Connect", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			Network.Connect( ip, port );
		}

		foreach(Host host in hostList)
		{
			if( GUILayout.Button( host.name, GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
			{
				Network.Connect( host.ip, port );
			}
		}

		// host a server on the given port, only allow 3 incoming connection (3 other players)
		if( GUILayout.Button( "Host", GUILayout.Width( 100f ), GUILayout.MinHeight(minButtonHeight) ) )
		{
			Network.InitializeServer( 3, port, true );
		}
	}

	void OnConnectedToServer()
	{
		Debug.Log( "Connected to server" );
		// this is the NetworkLevelLoader we wrote earlier in the chapter – pauses the network, loads the level, waits for the level to finish, and then unpauses the network
		NetworkLevelLoader.Instance.LoadLevel( Scenes.unityNetworkGameRoom );
	}

	void OnServerInitialized()
	{
		Debug.Log( "Server initialized" );
		NetworkLevelLoader.Instance.LoadLevel( Scenes.unityNetworkGameRoom );
	}
}