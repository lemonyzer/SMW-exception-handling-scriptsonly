using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour {


//	abstract public Item item { get; set;}
//	abstract public float itemStayTime { get; set;}			// debug..geht nicht (coroutine)

//	public abstract void StartDestroyTimer();

	public float itemStayTime = 8f;

	public string itemName;
	public Item item;

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

	/**
	 * Inspector can't handle Polymorphie Scripts /without MonoBehaviour inheritance
	 **/

	// Item item kann nicht per  Inspector gesetzt und in Prefab gespeichert werden. Um if's zu entgehend muss ItemScript in Items aufgeteilt werden!
	//	public string itemName = "Star";			// alternative mit if's ... ItemScript string
	//	
	//	void OnTriggerEnter2D(Collider2D other)
	//	{
	//		if(itemName == "Star")
	//		{
	//			item = new Star();
	//			item.Collecting(this.gameObject, other.gameObject.GetComponent<PlatformCharacter>());
	//		}
	//		else if(itemName == "Flower")
	//		{
	//			item = new Flower();
	//			item.Collecting(this.gameObject, other.gameObject.GetComponent<PlatformCharacter>());
	//		}
	//	}


	Layer layer;
	
	public void Awake()
	{
		layer = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Layer>();
		item = new Star();

		if(itemName == "Star")
		{
			item = new Star();
		}
		else if(itemName == "Flower")
		{
			item = new Flower();
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.layer == layer.item)
		{
			if(other.gameObject.name == Tags.itemCollector)
			{
				// Player gefunden
				if(item == null)
				{
					Debug.LogError(this.gameObject.name + " hat kein Item im Inspektor gesetzt!!!");
				}
				else
				{
					//V0: kann im PlatformCharacter noch controllieren ob dieser das Item einsammeln darf! 
					other.transform.parent.GetComponent<PlatformCharacter>().CollectingItem(this);
					
					//V1 
					//item.Collecting(this.gameObject, other.transform.parent.GetComponent<PlatformCharacter>());
				}
			}
		}
	}


}
