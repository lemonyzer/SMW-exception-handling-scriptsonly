using UnityEngine;
using System.Collections;

public class DestroyAbleBlock : MonoBehaviour {

	public string targetTag = "Head";
	public float powerUpRespawnTime = 8.0f;

	public AudioClip destroyBlockSound;
	public GameObject destroyedBlockPrefab;
	public float destroyedBlockPrefabStayTime = 5.0f;

	private BoxCollider2D myTriggerZone;
	private GameObject myBlock;
	private BoxCollider2D myBlockCollider;

	void Awake()
	{
		myTriggerZone = GetComponent<BoxCollider2D>();
		myBlock = this.transform.parent.gameObject;
		myBlockCollider = myBlock.GetComponent<BoxCollider2D>();
	}

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
				other.gameObject.transform.parent.rigidbody2D.velocity = new Vector2(other.gameObject.transform.parent.rigidbody2D.velocity.x,0f);
				AudioSource.PlayClipAtPoint(destroyBlockSound,transform.position,1);

//				foreach(BoxCollider2D collider in myColliders)
//				{
//					collider.enabled = false;
//				}
				myBlock.renderer.enabled = false;
				myBlockCollider.enabled = false;
				myTriggerZone.enabled = false;

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
			}
		}
	}

//	IEnumerator ReloadPowerUpBlock()
//	{
//		yield return new WaitForSeconds(powerUpRespawnTime);
//		hasPowerUp=true;
//		AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
//	}
}
