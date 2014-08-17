using UnityEngine;
using System.Collections;

public class ReSpawnScript : MonoBehaviour {
	public bool debugSpawn = false;

	float reSpawnDelayTime = 2f;

	bool spawnProtection = false;
	float spawnProtectionTime = 2f;
	Color[] spawnProtectionAnimation;
	

	Animator anim;
	GameObject gameController;
	HashID hash;
	Layer layer;

	Level currentLevel;

	GameObject myCharacter;
	SpriteRenderer mySpriteRenderer;
	PlatformCharacter myPlatformCharacterScript;
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
		foreach(BoxCollider2D coll in myBody)
		{
			if(coll.isTrigger)
				myBodyTrigger = coll;
			else
				myBodyCollider = coll;
		}
		myFeetTrigger = transform.Find(Tags.feet).GetComponent<BoxCollider2D>();
		myHeadTrigger = transform.Find(Tags.head).GetComponent<BoxCollider2D>();
		myGroundStopperCollider = transform.Find(Tags.groundStopper).GetComponent<BoxCollider2D>();
	}

	void SetSpawnAnimationCharacterCollider()
	{
		rigidbody2D.isKinematic = true;
		// Layer Collisionen mit Gegenspieler und PowerUps ignorieren, GameObject soll aber auf Boden/Platform fallen und liegen bleiben

		myGroundStopperCollider.enabled = true;	// wurde bei invincible attack deaktiviert...
		
		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		myBodyCollider.enabled = false;
		// Body Trigger aktivieren, PowerUps einsammeln
		myBodyTrigger.enabled = true;
		//					myCharacterCollider2D.enabled = false;
		
		// FeetCollider aktivieren (Gegenspieler nehmen Schaden)
		myFeetTrigger.enabled = false;
		// HeadTrigger deaktivieren, (in SpawnProtection nicht angreifbar)
		myHeadTrigger.enabled = false;
		//					feet.gameObject.SetActive(false);
		//					headCollider2D.enabled = false;
		
		//					// verhindern dass das GameObject durch die Gravität in Boden fällt
		//					myCharacter.rigidbody2D.isKinematic = true;
		
		/* Ki und Controlls deaktivieren */
		myPlatformCharacterScript.isDead = true;
	}

	void SetSpawnProtectionCharacterCollider()
	{
		rigidbody2D.isKinematic = false;
		// Layer Collisionen mit Gegenspieler und PowerUps ignorieren, GameObject soll aber auf Boden/Platform fallen und liegen bleiben
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player1,true);
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player2,true);
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player3,true);
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player4,true);

//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.powerUp,false);
		
		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		myBodyCollider.enabled = false;												//aktiviert, da collision ignoriert werden! und spieler auf boden liegen bleiben soll
		// Body Trigger aktivieren, PowerUps einsammeln
		myBodyTrigger.enabled = true;
		//					myCharacterCollider2D.enabled = false;
		
		// FeetCollider aktivieren (Gegenspieler nehmen Schaden)
		myFeetTrigger.enabled = true;
		// HeadTrigger deaktivieren, (in SpawnProtection nicht angreifbar)
		myHeadTrigger.enabled = false;
		//					feet.gameObject.SetActive(false);
		//					headCollider2D.enabled = false;
		
		//					// verhindern dass das GameObject durch die Gravität in Boden fällt
		//					myCharacter.rigidbody2D.isKinematic = true;
		
		/* Ki und Controlls aktivieren */
		myPlatformCharacterScript.isDead = false;
	}

	void SetSpawnCompleteCharacterCollider()
	{
		rigidbody2D.isKinematic = false;
		// Layer Collisionen mit Gegenspieler und PowerUps ignorieren, GameObject soll aber auf Boden/Platform fallen und liegen bleiben
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player1,false);
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player2,false);
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player3,false);
//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.player4,false);

//		Physics2D.IgnoreLayerCollision(myCharacter.layer,layer.powerUp,false);
		
		// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
		myBodyCollider.enabled = true;												//aktiviert, da collision ignoriert werden! und spieler auf boden liegen bleiben soll
		// Body Trigger aktivieren, PowerUps einsammeln
		myBodyTrigger.enabled = true;
		//					myCharacterCollider2D.enabled = false;
		
		// FeetCollider aktivieren (Gegenspieler nehmen Schaden)
		myFeetTrigger.enabled = true;
		// HeadTrigger deaktivieren, (in SpawnProtection nicht angreifbar)
		myHeadTrigger.enabled = true;
		//					feet.gameObject.SetActive(false);
		//					headCollider2D.enabled = false;
		
		//					// verhindern dass das GameObject durch die Gravität in Boden fällt
		//					myCharacter.rigidbody2D.isKinematic = true;
		
		/* Ki und Controlls aktivieren */
		myPlatformCharacterScript.isDead = false;
	}

	void Awake()
	{
		myCharacter = this.gameObject;
		myPlatformCharacterScript = GetComponent<PlatformCharacter>();
		InitColliderAndTrigger();
		InitSpawnProtectionAnimation ();
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
		anim = GetComponent<Animator>();
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		currentLevel = gameController.GetComponent<Level>();
		layer = gameController.GetComponent<Layer>();
	}

	// SpawnArea
	public void StartReSpawn()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("StartReSpawn()");
		StartCoroutine(SpawnDelay());
//			// disable SpriteRenderer		// nur wenn states von Animator (dead und headjumped) nicht genutzt werden (DeadPrefab mit floorcollider)
		// set new random SpawnPosition
		// spawn Animation!
				

		// animator state = spawnAnimation
		// emable SpriteRenderer
		// wait for SpawnAnimation to finish (yield alternative ?? check Animator is in StateSpawnProtection)
		// after SpawnAnimation
		// enable Controlls

	}

	IEnumerator SpawnDelay()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnDelay()");
		yield return new WaitForSeconds(reSpawnDelayTime);
		StartSpawnAnimation();
	}

	public void StartSpawnAnimation()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("StartSpawnAnimation()");
		myCharacter.renderer.enabled = false;				// sieht besser aus

		// neue Position halten
		rigidbody2D.isKinematic = true;

		// Random Spawn Position
		SetSpawnPosition();

		// Spawn Animation
		anim.SetBool(hash.spawnBool, true);

		// Kinematic = true, alle Collider & Trigger aus
		SetSpawnAnimationCharacterCollider();
		myCharacter.renderer.enabled = true;				// sieht besser aus
	}

	void SetSpawnPosition()
	{
//		float newPositionX = Random.Range(0.0f, 19.0f);
//		float newPositionY = Random.Range(2f, 15.0f);
//		float oldPositionZ = myCharacter.transform.position.z;
//		myCharacter.gameObject.transform.position = new Vector3(newPositionX,newPositionY,oldPositionZ);
		this.transform.position = currentLevel.getRandomSpawnPosition();
	}

	IEnumerator SpawnProtection()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("CoRoutine: SpawnProtection()");
		spawnProtection = true;
		yield return new WaitForSeconds(spawnProtectionTime);
		spawnProtection = false;
		SpawnComplete();
	}

	void SpawnComplete()
	{
		if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
			Debug.LogWarning("SpawnComplete()");
		mySpriteRenderer.color = new Color(1f,1f,1f,1f);	// transparenz entfernen
		SetSpawnCompleteCharacterCollider();
	}


	void InitSpawnProtectionAnimation()
	{
		spawnProtectionAnimation = new Color[1];
		spawnProtectionAnimation [0] = new Color (1f, 1f, 1f, 0.5f);	// alpha channel = 0.5
	}


	void LateUpdate()
	{
		if(!spawnProtection)
		{
			if(anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.spawnProtectionState)
			{
				if(debugSpawn && myCharacter.name.StartsWith("Carbuncle"))
					Debug.LogWarning("SpawnProtectionState");
				spawnProtection = true;	// coroutine ist zu langsam, wird sonst zweimal gestartet!
				anim.SetTrigger(hash.nextStateTrigger);	// spawnprotection state verlassen
				// Spawn Animation finished!
				// nach SpawnAnimation Collider & Trigger auf SpawnProtection setzen
				SetSpawnProtectionCharacterCollider();
				// SpawnProtection Timer starten
				StartCoroutine(SpawnProtection());


			}
			else
			{

			}
		}
		else //if(spawnProtection)
		{
			mySpriteRenderer.color = spawnProtectionAnimation[0];
		}
	}
}
