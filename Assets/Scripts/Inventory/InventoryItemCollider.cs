using UnityEngine;
using System.Collections;

public class InventoryItemCollider : MonoBehaviour {

	public string itemName = "";
	public float value = 1;
	public AudioClip SpawnedSound;
	public AudioClip CollectedSound;
	public AudioClip PoewrUpSound;
	public string targetTag = Tags.player;		// kann nur von Spielern eingesammelt werden

	private GameObject gameController;
	private Layer layer;
	private SpawnScript spawnScript;
	
	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
		spawnScript = gameController.GetComponent<SpawnScript>();
	}

	void Start()
	{
//		this.itemName = this.gameObject.name;
	}

	public virtual void CollectItem(GameObject player)
	{
		AudioSource.PlayClipAtPoint(CollectedSound,transform.position,1);
//		transform.GetComponent<AudioSource>().audio.loop=true;
		if(InventoryManager.inventory != null)
			InventoryManager.inventory.AddItems(itemName, value);
		else
			Debug.Log("Character hat kein Inventory");
//		
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
				CollectItem(other.gameObject);
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
				CollectItem(collision.gameObject);
			}
		}
	
	}
}
