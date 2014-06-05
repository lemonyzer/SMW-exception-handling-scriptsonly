using UnityEngine;
using System.Collections;

public class HealthControllerNew : MonoBehaviour {

	public bool godmode=false;

	public AudioClip deathSound;
	public AudioClip gameOverSound;
	public AudioClip criticalHealthSound;

	public float startLifes=10f;
	public float currentLifes=5f;

	public float spawnDelay = 1.0f;
	public float spawnTime = 1.0f;
	public float spawnProtectionTime = 1.0f;
	public float restartDelay = 5.0f;

	public GUIText lbl_life=null;
	public GameObject deathPrefabRight;
	public bool respawn=false;
	public bool enableControlls=false;

	public bool isKI=false;

	public bool isHit=false;
	Transform myCharacter;
	Transform feet;
	Animator anim;

	// Use this for initialization
	void Start () {
		myCharacter = this.gameObject.transform.parent;
		feet = myCharacter.Find("Feet");
		anim = myCharacter.GetComponent<Animator>();
		if(lbl_life != null)
			lbl_life.text = myCharacter.name + ": " + currentLifes;

		if(myCharacter.GetComponent<KI>() != null)
			isKI = true;
		else
			isKI = false; 
	}

	public void ApplyDamage(float damage, bool head)
	{
		if(!godmode)
		{
			if(!isHit)		//nur wenn er noch nicht getroffen wurde
			{
				isHit = true;
				AudioSource.PlayClipAtPoint(deathSound,transform.position,1);
				// BoxCollider2D deaktivieren
				myCharacter.GetComponent<BoxCollider2D>().enabled = false;
				// verhindern dass das GameObject durch die Gravität in Boden fällt
				myCharacter.rigidbody2D.isKinematic = true;
				StartCoroutine(DamageEffect());
				
				if (currentLifes > 0)
				{
					/* Schadenswert übernehmen */
					currentLifes -= damage;
					if(currentLifes == 1) 
					{
						AudioSource.PlayClipAtPoint(criticalHealthSound,transform.position,1);
					}
					if(lbl_life != null)
						lbl_life.text = myCharacter.name + ": " + currentLifes;
					
					Debug.Log (this.gameObject.name + ": takes damage of " + damage);
					if (currentLifes <= 0)
					{
						/* Game Over...
					 * Player hat alle Leben verloren */
						
						currentLifes = 0;
						
						Debug.Log (this.gameObject.name + ": GameOver");
						anim.SetBool ("GameOver", true);
						
						GameOver();
						
					}
					else
					{
						/* Spieler hat schaden genommen aber
					 * noch verbleibende Leben */
						if(head)
						{
							/* Spieler wurde per Kopfsprung getötet
						 * 
						 */
							HeadJumped();
						}
						else
						{
							/* Spieler wurde NICHT per Kopfsprung getötet
						 * 
						 */
							Debug.Log (this.gameObject.name + ": hitted");
							anim.SetBool("HeadJumped", false);
							anim.SetBool("Hitted",true);
						}
						
					}
				}
			}
		}


	}

	void HeadJumped() 
	{
		Debug.Log (this.gameObject.name + ": HeadJumped()");
		/* Ki und Controlls deaktivieren */
		stopControlls();

		myCharacter.renderer.enabled = false;
		Debug.Log("death prefab stays: " + (spawnDelay+spawnTime+spawnProtectionTime));
		Destroy(Instantiate(deathPrefabRight,transform.position,Quaternion.identity),spawnDelay+spawnTime+spawnProtectionTime);
		StartCoroutine(ReSpawnDelay());
	}

	void GameOver() 
	{
		AudioSource.PlayClipAtPoint(gameOverSound,transform.position,1);
		Debug.Log("GameOver(): !!!");
		myCharacter.rigidbody2D.isKinematic = false;
		myCharacter.rigidbody2D.AddForce(new Vector2(0,150));
		stopControlls();
		Destroy(myCharacter.gameObject,spawnDelay+spawnTime+spawnProtectionTime);
		if(myCharacter.tag.Equals("Player"))
		{
			//StartCoroutine(PlayerGameOver());
			RestartScene();
		}
	}

	void ReSpawn()
	{
		//ReSpawn...
		Debug.Log (this.gameObject.name + ": ReSpawn()");
		float newPositionX = Random.Range(0.0f, 19.0f);
		float newPositionY = Random.Range(2f, 15.0f);
		float oldPositionZ = myCharacter.transform.position.z;
		myCharacter.transform.position = new Vector3(newPositionX,newPositionY,oldPositionZ);
		myCharacter.renderer.enabled = true;
		myCharacter.GetComponent<BoxCollider2D>().enabled = true;
		myCharacter.rigidbody2D.isKinematic = true;

		// Ki und Controlls nach SpawnTime (SpawnAnimation) aktivieren
		StartCoroutine(SpawnTime());

	}

	void RestartScene()
	{	
		Application.LoadLevel(Application.loadedLevel);
	}

	IEnumerator DamageEffect()
	{
		anim.SetBool ("Hitted", true);
		yield return new WaitForSeconds(spawnDelay);
		anim.SetBool ("Hitted", false);
		yield return new WaitForSeconds((spawnDelay+spawnTime+spawnProtectionTime));
		isHit = false;
	}

	IEnumerator ReSpawnDelay()
	{
		yield return new WaitForSeconds(spawnDelay);
		respawn=true;
	}

	IEnumerator SpawnTime()
	{
		yield return new WaitForSeconds(spawnTime);
		enableControlls=true;
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
			myCharacter.GetComponent<KI>().enabled = false;
		
		if(myCharacter.GetComponent<PlayerController>() != null)
			myCharacter.GetComponent<PlayerController>().enabled = false;
	}

	void startControlls()
	{
		if(isKI)
			myCharacter.GetComponent<KI>().enabled = true;
		
		if(myCharacter.GetComponent<PlayerController>() != null)
			myCharacter.GetComponent<PlayerController>().enabled = true;
	}


	void Update()
	{
		if(respawn)
		{
			respawn=false;
			ReSpawn();
		}
		if(enableControlls) {
			enableControlls = false;
			startControlls();
			myCharacter.rigidbody2D.isKinematic = false;
		}
			
	}
}
