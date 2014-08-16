﻿using UnityEngine;
using System.Collections;

public class BreakableBlock : Photon.MonoBehaviour {
	
	public string targetTag = "Head";

	public AudioClip destroyBlockSound;
	public GameObject destroyedBlockPrefab;
	public float destroyedBlockPrefabStayTime = 5.0f;

//	private BoxCollider2D myTriggerZone;
	private GameObject myBlock;
	private BoxCollider2D myBlockCollider;

	private GameObject gameController;
	private Layer layer;

	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();

//		myTriggerZone = GetComponent<BoxCollider2D>();
		myBlock = this.gameObject;
		myBlockCollider = myBlock.GetComponent<BoxCollider2D>();
	}

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(PhotonNetwork.isMasterClient)
		{
			Debug.Log("OnTriggerEnter2D: " + other.name);
			if(other.gameObject.layer == layer.head)
			{
				// Abfrage ob Trigger/Collision am unteren Rand des Blocks
				Debug.Log("Parent: " + other.gameObject.transform.parent.name);
				if(HeadTriggerUnderBlock(other))
				{
					if(other.gameObject.transform.parent.rigidbody2D.velocity.y > 0f)			// nur zerstören wenn Spieler nach oben springt
					{
						other.gameObject.transform.parent.rigidbody2D.velocity = Vector2.zero;	// collision simulieren (player stoppt bei trigger erkennung kurz)
						photonView.RPC("Breaking", PhotonTargets.AllBuffered);				// all buffered, level is changeing!!
					}
				}
			}
		}
	}

	bool HeadTriggerUnderBlock(Collider2D other)
	{
		//BoxCollider2D headCollider = other.gameObject.GetComponent<BoxCollider2D>();
		float blockBottomPos = this.transform.position.y - this.transform.localScale.y*0.5f;
		float headTriggerUpEdgePos = other.transform.position.y + ((BoxCollider2D)other).size.y*0.5f;
		
						Debug.Log("Block bottom Position: " + blockBottomPos);
						Debug.Log("Head Trigger UpEdge Position: " + headTriggerUpEdgePos);
		
		// other shit (not needed)
		//				Debug.Log("((BoxCollider2D)other).size.y: " + ((BoxCollider2D)other).size);
		//				Debug.Log("other.bounds: " + other.bounds);
		
		float diff = blockBottomPos - headTriggerUpEdgePos;
		Debug.LogWarning("Difference: " + diff);
		//				if(Mathf.Abs(diff) < 0.02)
		//					Debug.LogWarning("Head is under the PowerUpBlock; " + diff);
		//				else
		//					Debug.LogWarning("Head is NOT under the PowerUpBlock; " + diff);
		if(Mathf.Abs(diff) < PhysicTolerances.TriggerColliderDifference)				// Unity Physics ungenauigkeit/tolleranz
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	[RPC]
	void Breaking()
	{
		Debug.LogWarning("RPC Breaking");
		if(destroyBlockSound != null)
			AudioSource.PlayClipAtPoint(destroyBlockSound,transform.position,1);
		else
			Debug.LogError("no Destroy Sound set in Unity Inspector!");

		myBlock.renderer.enabled = false;
		myBlockCollider.enabled = false;
//		myTriggerZone.enabled = false;
		
		Vector3 offset = new Vector3(0f,0f,0f);
		GameObject cloneTopLeft = (GameObject)Instantiate(destroyedBlockPrefab,transform.position+offset, Quaternion.identity);
		cloneTopLeft.rigidbody2D.AddForce(new Vector2(-250.0f,350.0f));
		cloneTopLeft.rigidbody2D.AddTorque(1000f);
		
		GameObject cloneTopRight = (GameObject)Instantiate(destroyedBlockPrefab,transform.position+offset, Quaternion.identity);
		cloneTopRight.rigidbody2D.AddForce(new Vector2(+250.0f,350.0f));
		cloneTopRight.rigidbody2D.AddTorque(-1000f);
		
		GameObject cloneBottomLeft = (GameObject)Instantiate(destroyedBlockPrefab,transform.position+offset, Quaternion.identity);
		cloneBottomLeft.rigidbody2D.AddForce(new Vector2(-150.0f,150.0f));
		cloneBottomLeft.rigidbody2D.AddTorque(1000f);
		
		GameObject cloneBottomRight = (GameObject)Instantiate(destroyedBlockPrefab,transform.position+offset, Quaternion.identity);
		cloneBottomRight.rigidbody2D.AddForce(new Vector2(+150.0f,150.0f));
		cloneBottomRight.rigidbody2D.AddTorque(-1000f);
		
		Destroy(cloneTopLeft,destroyedBlockPrefabStayTime);
		Destroy(cloneTopRight,destroyedBlockPrefabStayTime);
		Destroy(cloneBottomLeft,destroyedBlockPrefabStayTime);
		Destroy(cloneBottomRight,destroyedBlockPrefabStayTime);
		this.enabled = false;
	}

//	//Variante 0 
//	void OnSerializeNetworkView( BitStream stream )
//	{
//		//write position, direction, and speed to network
//		if( stream.isWriting )
//		{
//			//destroyPosition
//			Vector3 pos = transform.position;
//			bool destroy = this.destroyed;
//			stream.Serialize( ref pos );
//			stream.Serialize( ref destroy );
//		}
//		// read position, direction, and speed from network
//		else
//		{
//			//destroyPosition
//			Vector3 pos = Vector3.zero;
//			bool destroy = false;
//			stream.Serialize( ref pos );
//			stream.Serialize( ref destroy );
//			this.destroyed = destroy;
////			this.destroyPosition = pos;
//		}
//	}

//	IEnumerator ReloadPowerUpBlock()
//	{
//		yield return new WaitForSeconds(powerUpRespawnTime);
//		hasPowerUp=true;
//		AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
//	}
}
