using UnityEngine;
using System.Collections;

public class ItemCollectorScript : MonoBehaviour {

	Layer layer;
	GameObject gameController;
	NetworkView myNetworkView;
	PlatformCharacter myCharacterScript;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
		myNetworkView = this.transform.parent.GetComponent<NetworkView>();
		myCharacterScript = this.transform.parent.GetComponent<PlatformCharacter>();
	}
	
	bool CharacterCanCollectItems()
	{
		// TODO more Exceptions...

		//if(gameMode.collectItems == false)
		//	return false;

		//if(myCharacterScript.inventar.isFull())
		// return false;

		if(myCharacterScript.isDead)
			return false;

		return true;
	}

	// dump function... just returning item gameObject

	void OnTriggerEnter2D(Collider2D other)
	{
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			// itemCollector is @ gameObject that is in item layer
			// collectable items are also in layer item.
			// physics2d items only interact with items (saving unity-physic-engine calculating time)
			if(other.gameObject.layer == layer.item)		// probably unnessesary but in case Physic2D interaction model changed, or hasn't been set up correctly
			{
				//TODO layer static script
				//TODO Layer sets up Physic2D Layer ignoring/reaction

				/**
				 *	Version Simple!
				 **/
				myCharacterScript.CollectingItem(other.gameObject);
				return;

//				/**
//				 * Version with Filter
//				 **/
//				if(!CharacterCanCollectItems())
//					return;						// dont do anything if character is dead! (collecting items is not possible in 
//
//				Item currentPowerUp = other.gameObject.GetComponent<Item>();
//				if(currentPowerUp == null)
//				{
//					Debug.LogError("PowerUp has no PowerUp Script Component! Identification with Tag?");
//				}
//				else
//				{
//					Debug.Log(this.ToString() +": TriggerEnter2D with " + currentPowerUp.itemName + " ("+ currentPowerUp.name + ")" );
//
//					myCharacterScript.CollectedItem(currentPowerUp.itemName);
//					Destroy(other.gameObject);
			}
		}
	}
}
