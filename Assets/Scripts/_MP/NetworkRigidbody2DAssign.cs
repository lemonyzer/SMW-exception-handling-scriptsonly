using UnityEngine;
using System.Collections;

public class NetworkRigidbody2DAssign : MonoBehaviour {

	// Use this for initialization
	void Start () {
		NetworkRigidbody2D myNetworkRigidbody = GetComponent<NetworkRigidbody2D>();
		if(networkView == null || networkView.isMine)
		{
			myNetworkRigidbody.enabled = false;
		}
		else
		{
			myNetworkRigidbody.enabled = true;
		}
	}
}
