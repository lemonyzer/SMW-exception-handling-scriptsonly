using UnityEngine;
using System.Collections;

public class RequireNetwork : MonoBehaviour
{
	void Awake()
	{
		if( Network.peerType == NetworkPeerType.Disconnected )
			Network.InitializeServer( 1, 25005, true );
	}
}