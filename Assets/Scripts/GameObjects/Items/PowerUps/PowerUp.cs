using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	public string powerUpName;
	public float powerUpStayTime = 8f; 

	public void StartDestroyTimer()
	{
		StartCoroutine(DestroyPowerUp());
	}

	IEnumerator DestroyPowerUp()
	{
		yield return new WaitForSeconds(powerUpStayTime);
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			Destroy(this.gameObject);
		}
		if(Network.isServer)
		{
			if(this.gameObject != null)
			{
				Network.Destroy (this.gameObject);
			}
			else
			{
				Debug.LogWarning("nothing to Destroy! already destroyed/collected?!");
			}
		}
	}
}
