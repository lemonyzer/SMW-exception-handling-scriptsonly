using UnityEngine;
using System.Collections;

public class InventoryItemStarCollider : MonoBehaviour {
	
	public string itemName = "";
	public float value = 1;
	public AudioClip SpawnedSound;
	public AudioClip CollectedSound;
	public AudioClip PoewrUpSound;
	private string targetTag = "ALL";		// kann von allen eingesammelt werden
	
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
		spawnScript.playerDictonary[player].getCharacter().getRageModus().startRageModus();
		//		transform.GetComponent<AudioSource>().audio.loop=true;
		if(InventoryManager.inventory != null)
			InventoryManager.inventory.AddItems(itemName, value);
		else
			Debug.Log("kein Inventory in Scene");
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
			if(targetTag == "ALL" || targetTag == other.gameObject.tag)
			{
				// PowerUp kann von allen oder nur von Player augesammelt werden
				CollectItem(other.gameObject);
			}
			else
			{
				Debug.LogWarning("Star (Trigger): falscher Tag!");
				Debug.LogWarning("targetTag: " + targetTag);
				Debug.LogWarning("gameObject: " + other.gameObject.tag);
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
			if(targetTag == "ALL" || targetTag == collision.gameObject.tag)
			{
				// PowerUp kann von allen oder nur von Player augesammelt werden
				CollectItem(collision.gameObject);
			}
			else
			{
				Debug.LogWarning("Star (Collision): falscher Tag!");
				Debug.LogWarning("targetTag: " + targetTag);
				Debug.LogWarning("gameObject: " + collision.gameObject.tag);
			}
		}
		
	}
}
