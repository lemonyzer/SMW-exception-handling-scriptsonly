using UnityEngine;
using System.Collections;

public class RealOwner : MonoBehaviour {

	public PhotonPlayer owner;
	public Behaviour characterControls;

	void Awake()
	{
		this.enabled = false;
		characterControls = GetComponent<PlatformUserControlAnalogStickAndButton>();
	}

	[RPC]
	void SetCharacterControlsOwner(PhotonPlayer player)
	{
		owner = player;
		if (player == PhotonNetwork.player)
		{
			//Hey thats us! We can control this player: enable this script (this enables Update());
			characterControls.enabled = true;
		}
	}
	
}
