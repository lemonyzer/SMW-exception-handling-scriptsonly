using UnityEngine;
using System.Collections;

public class InventoryCollider : MonoBehaviour {

	public string itemName = "";
	public float value = 1;
	public AudioClip SpawnedSound;
	public AudioClip CollectedSound;
	public string targetTag = Tags.player;		// kann nur von Spielern eingesammelt werden

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
		if((other.gameObject.layer == 11) || 
		   (other.gameObject.layer == 12) ||
		   (other.gameObject.layer == 13) || 
		   (other.gameObject.layer == 14))
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
		if((collision.gameObject.layer == 11) || 
		   (collision.gameObject.layer == 12) ||
		   (collision.gameObject.layer == 13) || 
		   (collision.gameObject.layer == 14))
		{
			if(targetTag == "" || targetTag == collision.gameObject.tag)
			{
				// PowerUp kann von allen oder nur von Player augesammelt werden
				CollectItem();
			}
		}
	
	}
}
