﻿using UnityEngine;
using System.Collections;

public class Beam : MonoBehaviour {

//	bool beamableObject;

	SpriteRenderer backgroundSpriteRenderer;

	float backgroundWidth;
	float backgroundCenterPositionX;
	float leftBeamZoneX;
	float rightBeamZoneX;

	float saveBeamOffsetX = 0.5f;

	/** 
	 * Connection to GameController 
	 **/
	private GameObject gameController;
	private Layer layer;

	BoxCollider2D[] beamCollider;

	void Awake()
	{
		beamCollider = GetComponents<BoxCollider2D>();
		SortBeamCollider();
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
	}

	void SortBeamCollider()
	{
		if(beamCollider != null)
		{
			if(beamCollider.Length > 1)
			{
//				Debug.Log(beamCollider.Length + " Beamcollider");
//				Debug.Log(beamCollider[0].center);
//				Debug.Log(beamCollider[1].center);
				if(beamCollider[0].center.x <= beamCollider[1].center.x)
				{
					// alles ok, collider mit index 0 = links, collider mit index 1 = rechts
				}
				else
				{
					// tauschen!
					BoxCollider2D temp = beamCollider[0];
					beamCollider[0] = beamCollider[1];
					beamCollider[1] = temp;
				}
//				Debug.Log(beamCollider[0].center);
//				Debug.Log(beamCollider[1].center);
			}
		}
	}

	void Start()
	{
		backgroundSpriteRenderer = this.GetComponent<SpriteRenderer>();

		backgroundCenterPositionX = backgroundSpriteRenderer.bounds.center.x;
		backgroundWidth = backgroundSpriteRenderer.bounds.size.x;
//		leftBeamZoneX = backgroundCenterPositionX - (backgroundWidth * 0.5f) + saveBeamOffsetX;	// + !!!
//		rightBeamZoneX = backgroundCenterPositionX + (backgroundWidth * 0.5f) - saveBeamOffsetX;	// - !!!
		//transform position fehlt
		leftBeamZoneX = transform.position.x + beamCollider[0].center.x + beamCollider[0].size.x*0.5f + saveBeamOffsetX;
		rightBeamZoneX = transform.position.x + beamCollider[1].center.x - beamCollider[0].size.x*0.5f - saveBeamOffsetX;
//		Debug.Log(backgroundSpriteRenderer.bounds);
//		Debug.Log(leftBeamZoneX);
//		Debug.Log(rightBeamZoneX);
	}

//	void OnCollisionEnter2D ( Collision2D collision )
//	{
//		bool beamableParentObject = false;
//		if(collision.gameObject.layer == layer.groundStopper)		// cant use player ... colliders are disabled during spawnprotection!
//		{
//			beamableParentObject = true;
//		}
//		if(beamableParentObject)
//		{
//			float oldY = collision.transform.parent.position.y;
//			float oldX = collision.transform.parent.position.x;
//			if(oldX < backgroundCenterPositionX)
//			{
//				collision.transform.parent.position = new Vector2(rightBeamZoneX,oldY);
//			}
//			else
//			{
//				collision.transform.parent.position = new Vector2(leftBeamZoneX,oldY);
//			}
//		}
//	}

	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other)
	{

//		if(Network.peerType != NetworkPeerType.Disconnected)
//		{
//			if(!Network.isServer)
//			{
//				return;
//			}
//		}

//		GameObject original;
		bool beamableObject = false;
		bool beamableParentObject = false;

		//Simple Debug
//		Debug.Log(this.ToString() + ": OnTriggerEnter2D() " + other.name);

//		if(other.gameObject.layer == layer.groundStopper)		// cant use player ... colliders are disabled during spawnprotection!
//		{
//			beamableParentObject = true;						// players are beamed in characterScript because of unity physic engine simulation lag
//		}
//		else if(other.gameObject.layer == layer.player1)
//		{
//			beamableObject = true;
//		}
//		else if(other.gameObject.layer == layer.player2)
//		{
//			beamableObject = true;
//		}
//		else if(other.gameObject.layer == layer.player3)
//		{
//			beamableObject = true;
//		}
//		else if(other.gameObject.layer == layer.player4)
//		{
//			beamableObject = true;
//		}
		if(other.gameObject.layer == layer.item)
		{
			beamableObject = true;
		}
		else if(other.gameObject.layer == layer.bullet)
		{
			beamableObject = true;
		}

		if(beamableParentObject)
		{
			float oldY = other.transform.parent.position.y;
			float oldX = other.transform.parent.position.x;
			if(oldX < backgroundCenterPositionX)
			{
				other.transform.parent.position = new Vector2(rightBeamZoneX,oldY);
			}
			else
			{
				other.transform.parent.position = new Vector2(leftBeamZoneX,oldY);
			}
		}

		if(beamableObject)
		{
//			original = other.transform.gameObject;
			float oldY = other.transform.position.y;
			float oldX = other.transform.position.x;
			if(oldX < backgroundCenterPositionX)
			{
				other.gameObject.transform.position = new Vector2(rightBeamZoneX,oldY);
			}
			else
			{
				other.gameObject.transform.position = new Vector2(leftBeamZoneX,oldY);
			}
//			GameObject clone = (GameObject) Instantiate(original,new Vector2(oldX,oldY),Quaternion.identity);
//			clone.layer = layer.floor;
//			Destroy(clone,2f);
		}

	}
}
