using UnityEngine;
using System.Collections;

public class DestroyAbleBlock : MonoBehaviour {

	private bool destroyed = false;
	private Vector3 destroyPosition;

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
		if(destroyed)
		{
			AudioSource.PlayClipAtPoint(destroyBlockSound,transform.position,1);
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
			this.enabled = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag != null)
		{
			if(other.gameObject.tag == targetTag)
			{
//				other.gameObject.transform.parent.rigidbody2D.velocity = new Vector2(other.gameObject.transform.parent.rigidbody2D.velocity.x,0f);
				destroyed = true;

//				foreach(BoxCollider2D collider in myColliders)
//				{
//					collider.enabled = false;
//				}


			}
		}
	}

	void OnSerializeNetworkView( BitStream stream )
	{
		//write position, direction, and speed to network
		if( stream.isWriting )
		{
			//destroyPosition
			Vector3 pos = transform.position;
			bool destroy = this.destroyed;
			stream.Serialize( ref pos );
			stream.Serialize( ref destroy );
		}
		// read position, direction, and speed from network
		else
		{
			//destroyPosition
			Vector3 pos = Vector3.zero;
			bool destroy = false;
			stream.Serialize( ref pos );
			stream.Serialize( ref destroy );
			this.destroyed = destroy;
			this.destroyPosition = pos;
		}
	}

//	IEnumerator ReloadPowerUpBlock()
//	{
//		yield return new WaitForSeconds(powerUpRespawnTime);
//		hasPowerUp=true;
//		AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
//	}
}
