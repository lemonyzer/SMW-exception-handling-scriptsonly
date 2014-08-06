using UnityEngine;
using System.Collections;

public class AnimatorController : MonoBehaviour {

	public bool godmode = false;

	public AudioClip deathSound;
	public AudioClip gameOverSound;
	public AudioClip criticalHealthSound;

	public float startLifes=10f;
	public float currentLifes=5f;

	public float deathTime = 3.0f;				//how long crops stays!
	public float spawnAnimationTime = 1.0f;
	public float spawnProtectionTime = 1.0f;
	public float restartDelay = 5.0f;
	
	public GameObject deathPrefabRight;
	public GameObject HeadJumpedPrefabRight;
	public bool respawn=false;
	public bool enableControlls=false;
	public bool disableSpawnProtection = false;


	GameObject myCharacter;
//	BoxCollider2D myCharacterCollider2D;
//	Transform feet;
//	BoxCollider2D feetCollider2D;
//	Transform head;
//	BoxCollider2D headCollider2D;

	PlatformCharacter myPlatformCharacterScript;
	ReSpawnScript myReSpawnScript;
	Animator anim;

	GameObject gameController;
	private HashID hash;
	private StatsManager statsManager;
	private Layer layer;

	/**
	 * Collider / Trigger
	 **/
	// root: PlayerLayer
	BoxCollider2D myBodyCollider;
	BoxCollider2D myBodyTrigger;
	// child Feet: FeetLayer
	BoxCollider2D myFeetTrigger;
	// child Head: HeadLayer
	BoxCollider2D myHeadTrigger;
	// child GroundStopper: PlayerLayer
	BoxCollider2D myGroundStopperCollider;
	
	void InitColliderAndTrigger()
	{
		BoxCollider2D[] myBody = GetComponents<BoxCollider2D>();
		if(myBody == null)
			return;
		foreach(BoxCollider2D coll in myBody)
		{
			if(coll.isTrigger)
				myBodyTrigger = coll;
			else
				myBodyCollider = coll;
		}
		myFeetTrigger = transform.Find(Tags.feet).GetComponent<BoxCollider2D>();
		myHeadTrigger = transform.Find(Tags.head).GetComponent<BoxCollider2D>();
//		myGroundStopperCollider = transform.Find(Tags.groundStopper).GetComponent<BoxCollider2D>();
	}

	void Awake() {
		myCharacter = this.gameObject;
		InitColliderAndTrigger();
		myReSpawnScript = GetComponent<ReSpawnScript>();
//		Debug.Log(gameObject.name + " HealthController -> Awake()");
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		statsManager = gameController.GetComponent<StatsManager>();
		layer = gameController.GetComponent<Layer>();
	}

	// Use this for initialization
	void Start () {

//		myCharacterCollider2D = myCharacter.GetComponent<BoxCollider2D>();
//		if(myCharacterCollider2D == null)
//			Debug.LogError(myCharacter.name + " has no BoxCollider2D");
//
//		feet = myCharacter.transform.Find(Tags.feet);
//		feetCollider2D = feet.GetComponent<BoxCollider2D>();
//		if(feetCollider2D == null)
//			Debug.LogError(myCharacter.name + "'s feet has no FEET BoxCollider2D");
//
//
//		head = myCharacter.transform.Find(Tags.head);
//		headCollider2D = head.GetComponent<BoxCollider2D>();
//		if(headCollider2D == null)
//			Debug.LogError(myCharacter.name + "'s head has no HEAD BoxCollider2D");

		anim = myCharacter.GetComponent<Animator>();

		myPlatformCharacterScript = myCharacter.GetComponent<PlatformCharacter>();
		if(myPlatformCharacterScript == null)
			Debug.LogError(myCharacter.name + " has no PlatformCharacter Script");
	}

	void SetCharacterColliderHeadJumped()
	{
		// Layer Collisionen mit Gegenspieler und PowerUps ignorieren, GameObject soll aber auf Boden/Platform fallen und liegen bleiben
		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player1,true);
		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player2,true);
		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player3,true);
		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player4,true);
		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.powerUp,true);
		
		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		myBodyCollider.enabled = true;												//aktiviert, da collision ignoriert werden! und spieler auf boden liegen bleiben soll
		myBodyTrigger.enabled = false;
//					myCharacterCollider2D.enabled = false;
		
		// FeetCollider deaktivieren (Gegenspieler nehmen keinen Schaden mehr)
		myFeetTrigger.enabled = false;
		myHeadTrigger.enabled = false;
//					feet.gameObject.SetActive(false);
//					headCollider2D.enabled = false;

//					// verhindern dass das GameObject durch die Gravität in Boden fällt
//					myCharacter.rigidbody2D.isKinematic = true;
		
		/* Ki und Controlls deaktivieren */
		stopControlls();
	}

	void SetCharacterColliderDead()
	{
		SetCharacterColliderHeadJumped();

		// zusätzlich durch Boden fallen
		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.floor,true);
		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.destroyAbleBlock,true);
		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.jumpAblePlatform,true);


	}

	public void HeadJumpAnimation()
	{
		myPlatformCharacterScript.isHit = true;
		anim.SetBool(hash.spawnBool,false);
		anim.SetBool(hash.gameOverBool,false);
		anim.SetBool(hash.headJumpedBool,false);
		anim.SetBool(hash.deadBool,false);
		anim.SetTrigger(hash.hitTrigger);			// Lösung!

		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

		HeadJumped();
		myReSpawnScript.StartReSpawn();
		myPlatformCharacterScript.isHit = false;
    }

	public void ShootedAnimation()
	{
		myPlatformCharacterScript.isHit = true;
		anim.SetBool(hash.spawnBool,false);
		anim.SetBool(hash.gameOverBool,false);
		anim.SetBool(hash.headJumpedBool,false);
		anim.SetBool(hash.deadBool,false);
		anim.SetTrigger(hash.hitTrigger);			// Lösung!
		
		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);
        
        NoHeadJump();
		myReSpawnScript.StartReSpawn();
		myPlatformCharacterScript.isHit = false;
    }
    
    public void InvincibleAttackAnimation()
	{
		myPlatformCharacterScript.isHit = true;
		anim.SetBool(hash.spawnBool,false);
		anim.SetBool(hash.gameOverBool,false);
		anim.SetBool(hash.headJumpedBool,false);
		anim.SetBool(hash.deadBool,false);
		anim.SetTrigger(hash.hitTrigger);			// Lösung!

		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

		NoHeadJump();
		myReSpawnScript.StartReSpawn();
		myPlatformCharacterScript.isHit = false;
    }

	public void GameOverAnimation()
	{
		// Death Sound abspielen
		AudioSource.PlayClipAtPoint(gameOverSound,transform.position,1);
		anim.SetBool (hash.gameOverBool, true);
		SetCharacterColliderDead();
		DeadAnimationPhysics();
		GameOver();
    }

	void NoHeadJump()
	{
		//Animation setzen
		anim.SetBool(hash.deadBool,true);
		SetCharacterColliderDead();

		DeadAnimationPhysics();
	}

	void HeadJumped() 
	{
		//Animation setzen
		anim.SetBool(hash.headJumpedBool,true);
		SetCharacterColliderHeadJumped();

		myCharacter.rigidbody2D.velocity = Vector2.zero;
	}

//	void HeadJumped() 
//	{
////		Debug.Log ("HeadJumped stays " + deathTime + " seconds");
//
//		myCharacter.renderer.enabled = false;
//
//		// Deathanimation positioning
//		Vector3 offset = new Vector3(0.0f,-0.5f,0.0f);
//
//		// Show Deathanimation and Destroy after deathTime seconds
//		if(deathPrefabRight != null)
//			Destroy(Instantiate(HeadJumpedPrefabRight,transform.position + offset ,Quaternion.identity),deathTime);
//	}
//
//	void NotHeadJumped() 
//	{
//		//		Debug.Log ("HeadJumped stays " + deathTime + " seconds");
//		
//		myCharacter.renderer.enabled = false;
//		
//		// Deathanimation positioning
//		Vector3 offset = new Vector3(0.0f,-0.5f,0.0f);
//		
//		// Show Deathanimation and Destroy after deathTime seconds
//		if(deathPrefabRight != null)
//		{
//			GameObject deathPrefab = (GameObject) Instantiate(deathPrefabRight,transform.position + offset ,Quaternion.identity);
//			deathPrefab.rigidbody2D.velocity = new Vector2(0f, 20f);
//			Destroy(deathPrefab,deathTime);
//		}
//	}

	void DeadAnimationPhysics()
	{
		rigidbody2D.velocity = new Vector2(0f, 20f);
	}

	void GameOver() 
	{
		AudioSource.PlayClipAtPoint(gameOverSound,transform.position,1);
		Debug.Log (this.gameObject.name + ": GameOver");

		myCharacter.rigidbody2D.fixedAngle = false;
		myCharacter.rigidbody2D.AddTorque(20);
		stopControlls();
		StartCoroutine(PlayerGameOver());		// nach gameovertime character nicht mehr fallen lassen, kinematic setzen und ausserhalb camera einblenden (resourcen schonen)
	}

	IEnumerator PlayerGameOver()
	{
		yield return new WaitForSeconds(5f);
		myCharacter.renderer.enabled = false;
		myCharacter.rigidbody2D.isKinematic = true;
		myCharacter.rigidbody2D.velocity = Vector2.zero;
	}

/*
	IEnumerator DamageEffect()
	{
		anim.SetBool ("Hitted", true);
		//yield return new WaitForSeconds(spawnDelay);
		yield return new WaitForSeconds((deathTime+spawnTime+spawnProtectionTime));
		anim.SetBool ("Hitted", false);
		isHit = false;
	}
*/

	IEnumerator SpawnAnimationTime()
	{
		yield return new WaitForSeconds(spawnAnimationTime);
		enableControlls=true;
	}

	IEnumerator SpawnProtection()
	{
		yield return new WaitForSeconds(spawnProtectionTime);
		disableSpawnProtection=true;
	}

	void stopControlls()
	{
		if(myPlatformCharacterScript != null)
		{
			myPlatformCharacterScript.isDead = true;
			//myCharacter.GetComponent<PlatformCharacter>().enabled = false;
		}
	}

	void startControlls()
	{
		if(myPlatformCharacterScript != null)
		{
			myPlatformCharacterScript.isDead = false;
			//myCharacter.GetComponent<PlatformCharacter>().enabled = true;
		}
	}
}
