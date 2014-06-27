using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

	public bool godmode=false;

	public AudioClip deathSound;
	public AudioClip gameOverSound;
	public AudioClip criticalHealthSound;

	public float startLifes=10f;
	public float currentLifes=5f;

	public float deathTime = 3.0f;				//how long crops stays!
	public float spawnAnimationTime = 1.0f;
	public float spawnProtectionTime = 1.0f;
	public float restartDelay = 5.0f;

	public GUIText lbl_life=null;
	public GameObject deathPrefabRight;
	public bool respawn=false;
	public bool enableControlls=false;
	public bool disableSpawnProtection = false;

	public bool isKI=false;

	public bool isHit=false;

	Transform myCharacter;
	BoxCollider2D myCharacterCollider2D;
	Transform feet;
	BoxCollider2D feetCollider2D;
	Transform head;
	BoxCollider2D headCollider2D;

	PlatformCharacter myPlatformCharacterScript;

	Animator anim;

	GameObject gameController;
	private HashID hash;

	void Awake() {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
	}

	// Use this for initialization
	void Start () {

		myCharacter = this.gameObject.transform.parent;

		myCharacterCollider2D = myCharacter.GetComponent<BoxCollider2D>();
		if(myCharacterCollider2D == null)
			Debug.LogError(myCharacter.name + " has no BoxCollider2D");

		feet = myCharacter.Find("Feet");
		feetCollider2D = feet.GetComponent<BoxCollider2D>();
		if(feetCollider2D == null)
			Debug.LogError(myCharacter.name + "'s feet has no BoxCollider2D");


		head = myCharacter.Find("Head");
		headCollider2D = head.GetComponent<BoxCollider2D>();
		if(headCollider2D == null)
			Debug.LogError(myCharacter.name + "'s head has no BoxCollider2D");

		anim = myCharacter.GetComponent<Animator>();
		if(lbl_life != null)
			lbl_life.text = myCharacter.name + ": " + currentLifes;

		myPlatformCharacterScript = myCharacter.GetComponent<PlatformCharacter>();
		if(myPlatformCharacterScript == null)
			Debug.LogError(myCharacter.name + " has no PlatformCharacter Script");
	}

	public void ApplyDamage(float damage, bool headJumped)
	{
		if(!myPlatformCharacterScript.isInRageModus)
		{
			if(!godmode)
			{
				if(!isHit)		//nur wenn er noch nicht getroffen wurde
				{
					isHit = true;
					anim.SetTrigger(hash.hitTrigger);
					anim.SetBool(hash.hittedBool,true);
					anim.SetBool(hash.deadBool,true);	// zu schnell!
					anim.SetBool(hash.spawnBool,false);

					// Death Sound abspielen
					AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

					// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
					myCharacterCollider2D.enabled = false;

					// Fuß BoxCollider2D & SendDamageScript deaktivieren (Gegenspieler nehmen keinen Schaden mehr)
					// myCharacter.Find("Feet").gameObject.SetActive(false);
					feet.gameObject.SetActive(false);
					headCollider2D.enabled = false;
					//head.gameObject.GetComponent<BoxCollider2D>().enabled = false;	//BAD PROGRAMMING!

					// verhindern dass das GameObject durch die Gravität in Boden fällt
					myCharacter.rigidbody2D.isKinematic = true;

	//				myCharacter.rigidbody2D.velocity = new Vector2(0.0f,0.0f);

					/* Ki und Controlls deaktivieren */
					stopControlls();

	//				StartCoroutine(DamageEffect());
					
					if (currentLifes > 0)
					{
						/* Leben vorhanden */
						/* Schadenswert übernehmen */
						currentLifes -= damage;
						if(currentLifes == 1) 
						{
							/* nur noch ein Leben verbleibend */
							AudioSource.PlayClipAtPoint(criticalHealthSound,transform.position,1);
						}

	//					/* Leben auf GUI ausgeben */
	//					if(lbl_life != null)
	//					{
	//						lbl_life.text = myCharacter.name + ": " + currentLifes;
	//					}
	//					else
	//						Debug.LogError("keine Label für Leben gesetzt!");
							
						
	//					Debug.Log (this.gameObject.name + ": takes damage of " + damage);
						if (currentLifes <= 0)
						{
							/* Game Over...
							 * Player hat alle Leben verloren */
							anim.SetBool (hash.gameOverBool, true);
							currentLifes = 0;
							GameOver();
						}
						else
						{
							/* Spieler hat schaden genommen aber
						 	* noch verbleibende Leben */
							if(headJumped)
							{
								/* Spieler wurde per Kopfsprung getötet
							 	* 
							 	*/

								//Animation setzen
								anim.SetBool(hash.headJumpedBool,true);
								//Animation 3 sekunden laufen lassen
								HeadJumped();
								StartCoroutine(SpawnDelay());
							}
							else
							{
								/* Spieler wurde NICHT per Kopfsprung getötet
							 	* 
							 	*/
								Debug.Log (this.gameObject.name + ": shooted, timout,... my bad!");
							}
							
						}
					}
				}
				else
				{
					Debug.LogWarning("is already Hitted!");
				}
			}
		}

	}

	void HeadJumped() 
	{
//		Debug.Log ("HeadJumped stays " + deathTime + " seconds");

		//myCharacter.renderer.enabled = false;

		// Deathanimation positioning
		Vector3 offset = new Vector3(0.0f,-0.5f,0.0f);

		// Show Deathanimation and Destroy after deathTime seconds
		//Destroy(Instantiate(deathPrefabRight,transform.position + offset ,Quaternion.identity),deathTime);
	}

	void GameOver() 
	{

		AudioSource.PlayClipAtPoint(gameOverSound,transform.position,1);
		Debug.Log (this.gameObject.name + ": GameOver");

		// Spieler aus Physic nehmen
		myCharacter.rigidbody2D.isKinematic = false;
		feet.gameObject.SetActive(false);
		myCharacter.rigidbody2D.AddForce(new Vector2(0.0f,500.0f));
		myCharacter.rigidbody2D.fixedAngle = false;
		myCharacter.rigidbody2D.AddTorque(20);
		stopControlls();
		Destroy(myCharacter.gameObject,deathTime+spawnAnimationTime+spawnProtectionTime);
		if(myCharacter.tag.Equals("Player"))
		{
			//StartCoroutine(PlayerGameOver());
			//fader!
			RestartScene();
		}
	}

	void SetSpawnPoint ()
	{
		float newPositionX = Random.Range(0.0f, 19.0f);
		float newPositionY = Random.Range(2f, 15.0f);
		float oldPositionZ = myCharacter.transform.position.z;
		myCharacter.gameObject.transform.position = new Vector3(newPositionX,newPositionY,oldPositionZ);
	}

	public void ReSpawn()
	{
		//ReSpawn...
		anim.SetBool(hash.spawnBool, true);
		Debug.Log (this.gameObject.name + ": ReSpawn()");

		SetSpawnPoint();
		myCharacter.renderer.enabled = true;
		myCharacterCollider2D.enabled=true;
		//myCharacter.GetComponent<BoxCollider2D>().enabled = true;	//BAD PROGRAMMING!
		myCharacter.rigidbody2D.isKinematic = true;
	}

	void RestartScene()
	{	
		Application.LoadLevel(Application.loadedLevel);
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
	IEnumerator SpawnDelay()
	{
		yield return new WaitForSeconds(deathTime);
/*		anim.SetBool("HeadJumped", false);
		anim.SetBool ("Spawn", true);
*/
		//respawn=true;
		ReSpawn();
		StartCoroutine(SpawnAnimationTime());
	}

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

	IEnumerator PlayerGameOver()
	{
		Debug.Log("Restart in " + restartDelay + " seconds");
		yield return new WaitForSeconds(restartDelay);
		RestartScene();
		/* Funktioneirt nicht, mit if RestartScene im Update immer abfragen, sehr resourcen verschwendent!!!! 
		 */
	}

	void stopControlls()
	{
//		if(isKI)
//		{
//			myKIScript.isDead = true;
//			// myCharacter.GetComponent<KI>().enabled = false; // NICHT komplette Animator deaktivieren!
//			//myCharacter.GetComponent<KI>().JumpAllowed = false;
//			//myCharacter.GetComponent<KI>().MoveAllowed = false;
//		}

		if(myPlatformCharacterScript != null)
		{
			myPlatformCharacterScript.isDead = true;
			//myCharacter.GetComponent<PlatformCharacter>().enabled = false;
		}

	}

	void startControlls()
	{
//		if(isKI)
//		{
//			myKIScript.isDead = false;
//			// myCharacter.GetComponent<KI>().enabled = true; // NICHT komplette Animator deaktivieren!
//			//myCharacter.GetComponent<KI>().JumpAllowed = true;
//			//myCharacter.GetComponent<KI>().MoveAllowed = true;
//		}
		
		if(myPlatformCharacterScript != null)
		{
			myPlatformCharacterScript.isDead = false;
			//myCharacter.GetComponent<PlatformCharacter>().enabled = true;
		}
	}


	void Update()
	{
//		if(respawn)
//		{
//			respawn=false;
//			anim.SetBool(hash.spawnBool, true);
//			ReSpawn();
//			// Ki und Controlls nach SpawnTime (SpawnAnimation) aktivieren
//			StartCoroutine(SpawnAnimationTime());
//		}
		if(enableControlls) {
			anim.SetBool(hash.spawnBool, false);
			anim.SetBool(hash.spawnProtectionBool, true);
			enableControlls = false;

			// Ki und Controlls aktivieren
			startControlls();

			// Spieler wieder in Physic einbinden
			myCharacter.rigidbody2D.isKinematic = false;

			// Spieler kann wieder angreifen
			feet.gameObject.SetActive(true);

			// Spieler noch nicht angreifbar, erst nach Ablauf der SpawnProtection
			StartCoroutine(SpawnProtection());
		}
		if(disableSpawnProtection)
		{
			disableSpawnProtection = false;
//			anim.SetBool("Dead",false);
//			anim.SetBool("Hitted",false);

			//Spieler kann wieder angegriffen werden
			headCollider2D.enabled = true;
			isHit = false;
			anim.SetBool(hash.spawnProtectionBool, false);
		}
	}
}
