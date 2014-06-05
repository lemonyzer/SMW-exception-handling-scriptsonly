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
	Transform feet;
	Transform head;
	Animator anim;

	// Use this for initialization
	void Start () {
		myCharacter = this.gameObject.transform.parent;
		feet = myCharacter.Find("Feet");
		head = myCharacter.Find("Head");
		anim = myCharacter.GetComponent<Animator>();
		if(lbl_life != null)
			lbl_life.text = myCharacter.name + ": " + currentLifes;

		if(myCharacter.GetComponent<KI>() != null)
			isKI = true;
		else
			isKI = false; 
	}

	public void ApplyDamage(float damage, bool headJumped)
	{
		if(!godmode)
		{
			if(!isHit)		//nur wenn er noch nicht getroffen wurde
			{
				isHit = true;
/*new*/			anim.SetTrigger("HitTrigger");
				anim.SetBool("Hitted",true);
				anim.SetBool("Dead",true);	// zu schnell!
				anim.SetBool("Spawn",false);

				// Death Sound abspielen
				AudioSource.PlayClipAtPoint(deathSound,transform.position,1);

				// Body BoxCollider2D deaktivieren (Gegenspieler können durchlaufen)
				myCharacter.GetComponent<BoxCollider2D>().enabled = false;

				// Fuß BoxCollider2D deaktivieren (Gegenspieler nehmen keinen Schaden mehr)
				// myCharacter.Find("Feet").gameObject.SetActive(false);
				feet.gameObject.SetActive(false);
				head.gameObject.GetComponent<BoxCollider2D>().enabled = false;

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

					/* Leben auf GUI ausgeben */
					if(lbl_life != null)
					{
						lbl_life.text = myCharacter.name + ": " + currentLifes;
					}
					else
						Debug.LogError("keine Label für Leben gesetzt!");
						
					
					Debug.Log (this.gameObject.name + ": takes damage of " + damage);
					if (currentLifes <= 0)
					{
						/* Game Over...
						 * Player hat alle Leben verloren */
						anim.SetBool ("GameOver", true);
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
							anim.SetBool("HeadJumped",true);
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

	void HeadJumped() 
	{
		Debug.Log ("HeadJumped stays " + deathTime + " seconds");

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

	void ReSpawn()
	{
		//ReSpawn...
		Debug.Log (this.gameObject.name + ": ReSpawn()");

		SetSpawnPoint();
		myCharacter.renderer.enabled = true;
		myCharacter.GetComponent<BoxCollider2D>().enabled = true;
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
		respawn=true;
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
		if(isKI)
		{
			myCharacter.GetComponent<KI>().isDead = true;
			// myCharacter.GetComponent<KI>().enabled = false; // NICHT komplette Animator deaktivieren!
			//myCharacter.GetComponent<KI>().JumpAllowed = false;
			//myCharacter.GetComponent<KI>().MoveAllowed = false;
		}

		if(myCharacter.GetComponent<PlayerController>() != null)
		{
			myCharacter.GetComponent<PlayerController>().isDead = true;
			//myCharacter.GetComponent<PlayerController>().enabled = false;
		}

	}

	void startControlls()
	{
		if(isKI)
		{
			myCharacter.GetComponent<KI>().isDead = false;
			// myCharacter.GetComponent<KI>().enabled = true; // NICHT komplette Animator deaktivieren!
			//myCharacter.GetComponent<KI>().JumpAllowed = true;
			//myCharacter.GetComponent<KI>().MoveAllowed = true;
		}
		
		if(myCharacter.GetComponent<PlayerController>() != null)
		{
			myCharacter.GetComponent<PlayerController>().isDead = false;
			//myCharacter.GetComponent<PlayerController>().enabled = true;
		}
	}


	void Update()
	{
		if(respawn)
		{
			respawn=false;
			anim.SetBool ("Spawn", true);
			ReSpawn();
			// Ki und Controlls nach SpawnTime (SpawnAnimation) aktivieren
			StartCoroutine(SpawnAnimationTime());
		}
		if(enableControlls) {
			anim.SetBool ("Spawn", false);
			anim.SetBool ("SpawnProtection", true);
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
			head.gameObject.GetComponent<BoxCollider2D>().enabled = true;
			isHit = false;
			anim.SetBool ("SpawnProtection", false);
		}
	}
}
