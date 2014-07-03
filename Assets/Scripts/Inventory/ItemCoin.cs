using UnityEngine;
using System.Collections;

public class ItemCoin : InventoryItemCollider {

	void Awake()
	{
//		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
//		layer = gameController.GetComponent<Layer>();
//		spawnScript = gameController.GetComponent<SpawnScript>();
	}

	void Start()
	{
		this.itemName = this.gameObject.name+"test";
	}

	public override void CollectItem(GameObject player)
	{
		base.CollectItem(player);


		Destroy(gameObject);
	}
}
