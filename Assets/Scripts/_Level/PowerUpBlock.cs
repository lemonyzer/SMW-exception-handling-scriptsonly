using UnityEngine;
using System.Collections;

public class PowerUpBlock : MonoBehaviour {

	public string targetTag = "Head";
	public float powerUpRespawnTime = 15.0f;
	public bool hasPowerUp = true;

	public AudioClip powerUpReleaseSound;
	public AudioClip powerUpReloadedSound;

	public GameObject powerup;
	public float powerUpStayTime = 8.0f;

	private Animator anim;
	GameObject gameController;
	private HashID hash;

	// Use this for initialization
	void Awake() {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		anim = GetComponent<Animator>();
	}

	void Start() 
	{
		anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag != null)
		{
			if(other.gameObject.tag == targetTag)
			{
				if(hasPowerUp)
				{
					AudioSource.PlayClipAtPoint(powerUpReleaseSound,transform.position,1);
					hasPowerUp = false;
					anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
					StartCoroutine(ReloadPowerUpBlock());
					//Vector3 offset = new Vector3(.5f,.5f,0.0f);
					Vector3 offset = new Vector3(0,1,0);
					GameObject clone = (GameObject)Instantiate(powerup,transform.position + offset ,Quaternion.identity);
					clone.rigidbody2D.AddForce(new Vector2(-250.0f,350.0f));
					Destroy(clone,powerUpStayTime);

				}

			}
		}
	}

	IEnumerator ReloadPowerUpBlock()
	{
		yield return new WaitForSeconds(powerUpRespawnTime);
		hasPowerUp=true;
		anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
	}
}
