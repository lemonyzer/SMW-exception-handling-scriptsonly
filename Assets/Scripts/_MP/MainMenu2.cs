using UnityEngine;
using System.Collections;

public class MainMenu2 : MonoBehaviour
{
	private string connectIP = "192.168.0.129";
	
	void OnGUI()
	{
		if( GUILayout.Button( "Host" ) )
		{
			// host a game
			Network.InitializeServer( 8, 25005, true );
			
			// load level
			Application.LoadLevel( "gamescene" );
		}
		
		connectIP = GUILayout.TextField( connectIP );
		if( GUILayout.Button( "Connect" ) )
		{
			// connect
			Network.Connect( connectIP, 25005 );
		}
	}
	
	void OnConnectedToServer()
	{
		Network.isMessageQueueRunning = false;
		
		// load level
		Application.LoadLevel( "gamescene" );
	}
}