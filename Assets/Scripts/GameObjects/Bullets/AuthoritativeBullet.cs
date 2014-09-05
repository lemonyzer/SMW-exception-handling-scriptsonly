using UnityEngine;
using System.Collections;

public class AuthoritativeBullet : MonoBehaviour {

	public NetworkPlayer owner;
	public GameObject ownerCharacter;
	Vector3 groundCheckPositionOffset;
	Vector3 groundCheckPosition;
	SpriteRenderer bulletSpriteRenderer;

	GameObject gameController;
	Layer layer;
	StatsManager statsManager;

	public static Vector3 moveSpeed = new Vector3(-5,5,0);

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
		statsManager = gameController.GetComponent<StatsManager>();
		bulletSpriteRenderer = this.GetComponent<SpriteRenderer>();

		groundCheckPositionOffset = new Vector3(0,bulletSpriteRenderer.bounds.extents.y,0);
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Network.isServer || Network.peerType == NetworkPeerType.Disconnected)
		{
			Debug.DrawLine(groundCheckPosition, groundCheckPosition + new Vector3(1,0,0));

			groundCheckPosition = this.transform.position - groundCheckPositionOffset;		// sprite pivot need to be in center position
			if(Physics2D.OverlapPoint(groundCheckPosition, layer.whatIsGround))
			{
				Debug.Log(this.ToString() +": bounce");
				rigidbody2D.velocity = moveSpeed;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.layer == layer.ground ||
		   collision.gameObject.layer == layer.jumpAblePlatform ||
		   collision.gameObject.layer == layer.block)
		{
			Debug.Log(this.ToString() +": UnityPhysics -> BOUNCE");
			rigidbody2D.velocity = moveSpeed;
		}
		else if(collision.gameObject.layer == layer.player)
		{
			statsManager.BulletHit(ownerCharacter, collision.gameObject );
		}

	}
}
