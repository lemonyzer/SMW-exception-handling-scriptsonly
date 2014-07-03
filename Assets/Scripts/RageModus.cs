using UnityEngine;
using System.Collections;

public class RageModus : MonoBehaviour {

	public bool isInRageModus = false;
	private float oldMaxSpeed;

	private float rageMaxSpeed;
	/** 
	 * Character Animation 
	 **/
	//	private SpriteController spriteController;
	public Animator anim;									// Animator State Machine

	/** 
	 * Connection to GameController 
	 **/
	private GameObject gameController;
	private HashID hash;
	private Layer layer;
	private StatsManager statsManager;

	/**
	 * Connection to other Body parts
	 **/
	private Collider2D bodyCollider2D;
	private Collider2D headCollider2D;
	private Collider2D feetCollider2D;

	private PlatformCharacter myPlatformCharacter;
	
	void Awake()
	{
		myPlatformCharacter = GetComponent<PlatformCharacter>();
		anim = GetComponent<Animator>();
		bodyCollider2D = GetComponent<BoxCollider2D>();
		headCollider2D = transform.FindChild(Tags.head).GetComponent<BoxCollider2D>();
		feetCollider2D = transform.FindChild(Tags.feet).GetComponent<BoxCollider2D>();
		
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		layer = gameController.GetComponent<Layer>();
		statsManager = gameController.GetComponent<StatsManager>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startRageModus()
	{
		oldMaxSpeed = myPlatformCharacter.getMaxSpeed();
		rageMaxSpeed = oldMaxSpeed * 1.25f;
		myPlatformCharacter.setMaxSpeed(rageMaxSpeed);
		headCollider2D.enabled = false;
		feetCollider2D.enabled = false;
//		bodyCollider2D.isTrigger = false;
		
		isInRageModus = true;
		myPlatformCharacter.isInRageModus = true;
		if(anim != null)
		{
			anim.SetBool(hash.rageModusBool,true);
			anim.SetTrigger(hash.rageTrigger);
		}
		Debug.LogError(gameObject.name + "isInRageModus: On");
		//		InventoryManager.inventory.SetItems("Star(Clone)",0f);
		StartCoroutine(RageTime());
	}

	IEnumerator RageTime()
	{
		yield return new WaitForSeconds(8.0f);
		stopRageModus();
	}

	public void stopRageModus()
	{
		Debug.LogError(gameObject.name + "isInRageModus: Off");
		if(anim != null)
		{
			anim.SetBool(hash.rageModusBool,false);
		}
		isInRageModus = false;
		myPlatformCharacter.isInRageModus = false;
		headCollider2D.enabled = true;
		feetCollider2D.enabled = true;
		myPlatformCharacter.setMaxSpeed(oldMaxSpeed);
//		bodyCollider2D.isTrigger = false;
		
		//anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		//AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		// Network.peerType == PeerState.Disconnected
		if(Network.isServer || networkView == null)
		{
			if(isInRageModus)
			{
				if(this.gameObject.layer != collision.gameObject.layer)										// Spieler aus eigenem Team(layer) nicht zerstören
				{
					bool enemyObject = false;
					if(collision.gameObject.layer == layer.player1)
					{
						enemyObject = true;
					}
					else if(collision.gameObject.layer == layer.player2)
					{
						enemyObject = true;
					}
					else if(collision.gameObject.layer == layer.player3)
					{
						enemyObject = true;
					}
					else if(collision.gameObject.layer == layer.player4)
					{
						enemyObject = true;
					}
					//				else if(other.gameObject.layer == layer.powerUp)
					//				{
					//					enemyObject = true;
					//				}
					
					if(enemyObject)
					{
						//networkView.RPC
						//other.gameObject.GetComponent<NetworkView>().RPC(
						statsManager.InvincibleAttack(this.gameObject, collision.gameObject);			// Layerfilter -> wir sind auf PlatformCharacter ebene (nicht im child feet/head)
						//other.gameObject.GetComponent<HealthController>().ApplyDamage(this.gameObject, 1 ,true);
					}
				}
			}
		}
	}

}
