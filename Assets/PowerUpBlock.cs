using UnityEngine;
using System.Collections;

public class PowerUpBlock : Photon.MonoBehaviour {

	public string targetTag = "Head";
	public float powerUpRespawnTime = 8.0f;
	public bool hasPowerUp = true;

	public GameObject powerup;
	public float powerUpStayTime = 8.0f;

	// Use this for initialization
	void Start () {
	
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

					hasPowerUp = false;
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
	}
}
