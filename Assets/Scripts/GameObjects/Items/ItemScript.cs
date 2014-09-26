using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour {

	public Item item;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public string itemName;
	public float itemStayTime = 8f; 
	
	public void StartDestroyTimer()
	{
		StartCoroutine(DestroyPowerUp());
	}
	
	IEnumerator DestroyPowerUp()
	{
		yield return new WaitForSeconds(itemStayTime);
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			Destroy(this.gameObject);
		}
		if(Network.isServer)
		{
			if(this.gameObject != null)
			{
				Network.RemoveRPCs(this.networkView.viewID);
				Network.Destroy(this.gameObject);
			}
			else
			{
				Debug.LogWarning("nothing to Destroy! already destroyed/collected?!");
			}
		}
	}

}
