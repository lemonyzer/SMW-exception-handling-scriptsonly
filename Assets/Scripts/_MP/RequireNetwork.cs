using UnityEngine;
using System.Collections;

public class RequireNetwork : MonoBehaviour
{
	public int serverSlots = 7;
	private int port = 25005;
	private string gameTypeName="mpSMW";
	private string gameName="pers0rs bouncy bude";
	private string comment="comeINandDIE";

	void Awake()
	{
		MasterServer.ipAddress = "192.168.0.174";
		MasterServer.port = 23466;
		
		Network.natFacilitatorIP = "192.168.0.174";
		Network.natFacilitatorPort = 50005;

		if( Network.peerType == NetworkPeerType.Disconnected )
		{
			Debug.Log( "Server forced to initialize" );
			Network.InitializeServer( serverSlots, port, false );
			MasterServer.RegisterHost(gameTypeName,gameName,comment);
		}
	}
}