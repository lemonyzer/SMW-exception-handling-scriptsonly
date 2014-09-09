using UnityEngine;
using System.Collections;

public class AuthoritativeBullet : MonoBehaviour {

	public NetworkPlayer owner;
	public GameObject ownerCharacter;


	GameObject gameController;
	Layer layer;
	StatsManager statsManager;

	public static Vector3 moveSpeed = new Vector3(5,5,0);
	public Vector3 moveDirection = new Vector3(1,0,0);
	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
		statsManager = gameController.GetComponent<StatsManager>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.layer == layer.body)
		{
			if(other.gameObject != ownerCharacter)
			{
				statsManager.BulletHit(ownerCharacter, other.gameObject );
				Network.Destroy(this.gameObject);
			}
			else
			{
				Debug.LogWarning(this.ToString() +", own Bullet!");
			}
		}
	}
}
