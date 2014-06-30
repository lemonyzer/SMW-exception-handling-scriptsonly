using UnityEngine;
using System.Collections;

public class InventoryCollider : MonoBehaviour {

	public string itemName = "";
	public float value = 1;
	public AudioClip SpawnedSound;
	public AudioClip CollectedSound;
	public string targetTag = Tags.player;		// kann nur von Spielern eingesammelt werden

	private GameObject gameController;
	private HashID hash;
	private Layer layer;
	
	
	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		layer = gameController.GetComponent<Layer>();
	}

	void Start()
	{
		this.itemName = this.gameObject.name;
	}

	void CollectItem()
	{
		AudioSource.PlayClipAtPoint(CollectedSound,transform.position,1);
//		transform.GetComponent<AudioSource>().audio.loop=true;
		if(InventoryManager.inventory != null)
			InventoryManager.inventory.AddItems(itemName, value);
		else
			Debug.Log("Character hat kein Inventory");
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		/***
		 * Compare layer > 10 & layer < 14 effektiver?
		 * mit layermask layer 11,12,13,14 und verknüpfen und vergleichen?
		 ***/
		if((other.gameObject.layer == layer.player1) || 
		   (other.gameObject.layer == layer.player2) ||
		   (other.gameObject.layer == layer.player3) || 
		   (other.gameObject.layer == layer.player4))
		{
			if(targetTag == "" || targetTag == other.gameObject.tag)
			{
				// PowerUp kann von allen oder nur von Player augesammelt werden
				CollectItem();
			}
		}
		
	}

	void OnCollisionEnter2D(Collision2D collision) 
	{
		/***
		 * Compare layer > 10 & layer < 14 effektiver?
		 * mit layermask layer 11,12,13,14 und verknüpfen und vergleichen?
		 ***/
		if((collision.gameObject.layer == layer.player1) || 
		   (collision.gameObject.layer == layer.player2) ||
		   (collision.gameObject.layer == layer.player3) || 
		   (collision.gameObject.layer == layer.player4))
		{
			if(targetTag == "" || targetTag == collision.gameObject.tag)
			{
				// PowerUp kann von allen oder nur von Player augesammelt werden
				CollectItem();
			}
		}
	
	}
}
