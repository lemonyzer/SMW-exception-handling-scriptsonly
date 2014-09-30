﻿using UnityEngine;
using System.Collections;

public abstract class ItemScript : MonoBehaviour {

	//public abstract Item item { set; get; }		// durch Methode Item GetItem() ersetzt.

	public abstract Item GetItem();
//	abstract public float itemStayTime { get; set;}			// debug..geht nicht (coroutine)

	public abstract void StartDestroyTimer ();




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


}
