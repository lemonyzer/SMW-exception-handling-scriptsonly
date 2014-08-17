﻿using UnityEngine;
using System.Collections;

public class PowerUpBlock : Photon.MonoBehaviour {

//	public string targetTag = "Head";
	private float powerUpRespawnTime = 0.5f;
	public bool hasPowerUp = true;

	public AudioClip powerUpReleaseSound;
	public AudioClip powerUpReloadedSound;

	public GameObject[] powerups;
//	public float powerUpStayTime = 8.0f;

	private Animator anim;
	GameObject gameController;
	private HashID hash;
	private Layer layer;

	private GameObject powerupClone;


	// Use this for initialization
	void Awake() {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		layer = gameController.GetComponent<Layer>();
		anim = GetComponent<Animator>();

		LoadRandomPowerUp();
	}

	void LoadRandomPowerUp()
	{
		// resources durchsuchen, ein powerup wählen und laden
	}

	void Start() 
	{
//		anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		anim.SetTrigger(hash.powerUpBlockLoadedTrigger);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(PhotonNetwork.isMasterClient)
		{
			if(other.gameObject.layer == layer.head)
			{
				if(HeadTriggerUnderBlock(other))
				{
					if(other.gameObject.transform.parent.rigidbody2D.velocity.y >= 0f)			// nur zerstören wenn Spieler nach oben springt
					{
						if(hasPowerUp)
						{
							photonView.RPC("ReleasingRandomPowerUp", PhotonTargets.All);
						}
					}
					else
					{
						Debug.LogError(this.ToString() + ": nicht gesprungen!");
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
		
		//				Debug.Log("Block bottom Position: " + blockBottomPos);
		//				Debug.Log("Head Trigger UpEdge Position: " + headTriggerUpEdgePos);
		
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


	IEnumerator ReloadPowerUpBlock()
	{
		yield return new WaitForSeconds(powerUpRespawnTime);
		if(PhotonNetwork.isMasterClient)
		{
			photonView.RPC("BlockReloaded", PhotonTargets.All);
		}

		//PhotonView.RPC
	}

	[RPC]
	void ReleasingRandomPowerUp()
	{
		AudioSource.PlayClipAtPoint(powerUpReleaseSound,transform.position,1);
		hasPowerUp = false;
		anim.SetTrigger(hash.powerUpBlockReleaseTrigger);
		//anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		StartCoroutine(ReloadPowerUpBlock());

		if(PhotonNetwork.isMasterClient)
		{
			//Vector3 offset = new Vector3(.5f,.5f,0.0f);
			Vector3 offset = new Vector3(0,1,0);
			int i = Random.Range(0, powerups.Length-1);
			powerupClone = (GameObject)PhotonNetwork.Instantiate( powerups[i].name, transform.position + offset, Quaternion.identity,0 );
			//GameObject powerupClone = (GameObject)Instantiate(powerups[i],transform.position + offset ,Quaternion.identity);
			if(powerupClone.rigidbody2D != null)
			{
				int direction = RandomSign();
				powerupClone.rigidbody2D.velocity = new Vector2(direction*10f,8f);
				//powerupClone.rigidbody2D.AddForce(new Vector2(-250.0f,350.0f));
			}
			else
			{
				
			}
			powerupClone.GetComponent<PowerUp>().StartDestroyTimer();
//			StartCoroutine(DestroyPowerUp());	// BAD Programming!! powerupClone loses referenz if new PowerUp Spawns...
//			Destroy(powerupClone,powerUpStayTime);
		}
	}

	int RandomSign()
	{
		return Random.value < .5? 1 : -1;
	}


//	IEnumerator DestroyPowerUp()
//	{
//		yield return new WaitForSeconds(powerUpStayTime);
//		if(powerupClone != null)											
//			PhotonNetwork.Destroy (powerupClone);					// BAD Programming!! powerupClone loses referenz if new PowerUp Spawns...
//	}

	[RPC]
	void BlockReloaded()
	{
		hasPowerUp=true;
		//anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		anim.SetTrigger(hash.powerUpBlockLoadedTrigger);
		AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
	}

}
