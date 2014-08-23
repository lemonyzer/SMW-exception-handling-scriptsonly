using UnityEngine;
using System.Collections;

public class ConnectionStats : MonoBehaviour {

	/*
	 * Problem Clientseitig:
	 * 			p2p besteht nur mit server!
	 * 			keine verbindung zu anderen clients... pings nicht ohne übertragung einsehbar
	 * 			Network.connections[] ... enthält nur verbindung zum Server
	 * 
	 * Serverseitig (klar)
	 * 			Network.connections[] ... enthält Verbindungen zu allen Clients
	 * 
	 */
	void OnGUI()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return;
		
		if(Network.isServer)
		{
			GUILayout.BeginArea(new Rect(10f, 100f, Screen.width-20f, 80f ));
			GUILayout.BeginHorizontal();
			
			NetworkPlayer[] clients = Network.connections;
			foreach(NetworkPlayer currentClient in clients)
			{
				GUILayout.BeginVertical();
				//				GUILayout.Box(currentClient.ipAddress + " exinterp cnt: " + extrapolationCount);
				GUILayout.Box(currentClient.ipAddress + " last Ping: " + Network.GetLastPing(currentClient));
				GUILayout.Box(currentClient.ipAddress + " avg Ping: " + Network.GetAveragePing(currentClient));
				GUILayout.EndVertical();
			}
			
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			return;
		}
		
//		if(Network.isClient)
//		{
//			GUILayout.BeginArea(new Rect(10f, 100f, 250f, 80f ));
//			GUILayout.BeginHorizontal();
//			
//			NetworkPlayer[] clients = Network.connections;
//			foreach(NetworkPlayer currentClient in clients)
//			{
//				GUILayout.BeginVertical();
////				GUILayout.Box(currentClient.ipAddress + " exinterp cnt: " + extrapolationCount);
//				GUILayout.Box(currentClient.ipAddress + " last Ping: " + Network.GetLastPing(currentClient));
//				GUILayout.Box(currentClient.ipAddress + " avg Ping: " + Network.GetAveragePing(currentClient));
//				GUILayout.EndVertical();
//			}
//			
//			GUILayout.EndHorizontal();
//			GUILayout.EndArea();
//			
//			return;
//		}
		
		GameObject[] playerCharacters = GameObject.FindGameObjectsWithTag(Tags.player);
		
		GUILayout.BeginArea(new Rect(10f, 100f, Screen.width-20f, 80f ));
		GUILayout.BeginHorizontal();
		foreach(GameObject go in playerCharacters)
		{
			if(go != null)
			{
				RealOwner currentOwnerScript = go.GetComponent<RealOwner>();
				if(currentOwnerScript.owner != Network.player)
				{
					GUILayout.BeginVertical();
					
					NetworkedPlayer netScript = go.GetComponent<NetworkedPlayer>();
					if(netScript.extrapolation)
					{
						GUILayout.Box(currentOwnerScript.owner.ipAddress + " exinterp cnt: " + netScript.extrapolationCount);
					}
					GUILayout.Box(currentOwnerScript.owner.ipAddress + " last Ping: " + Network.GetLastPing(currentOwnerScript.owner));
					GUILayout.Box(currentOwnerScript.owner.ipAddress + " avg Ping: " + Network.GetAveragePing(currentOwnerScript.owner));
					
					GUILayout.EndVertical();
				}
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		//		if(extrapolation)
		//		{
		//			GUI.Box(new Rect(5f,Screen.height-100f,150f,20f), "Extrapolation: " + extrapolationCount);
		//		}
	}
}
