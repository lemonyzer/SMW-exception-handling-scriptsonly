using UnityEngine;
using System.Collections;

public class PushSkript : MonoBehaviour {

	string targetTag="Player";
	string targetTag2="Enemy";
	float pushForce=150.0f;

	bool bounced;

	Transform myCharacter;
	Rigidbody2D myRigidBody2D;
	PlayerController myPlayerController;
	PlayerController otherPlayerController;


	// Use this for initialization
	void Start () {
		bounced=false;
		myCharacter = this.gameObject.transform;
		if(myCharacter == null)
			Debug.LogError("Character zum Script nicht gefunden");
		else
			Debug.Log("Character zum Script gefunden!!");

		myRigidBody2D = myCharacter.rigidbody2D;
		if(myRigidBody2D == null)
			Debug.LogError("Character hat kein RigidBody2D");
		else
			Debug.LogError("RigidBody2D gefunden!!");

		myPlayerController = GetComponent<PlayerController>();
		if(myPlayerController == null)
			Debug.LogError("Character hat kein PlayerController");
	}
	
	// Update is called once per frame
	void Update () {
		if(bounced)
		{
			bounced = false;
			myRigidBody2D.velocity = new Vector2(10.0f,0.0f);
		}
	}

	void OnCollisionEnter2D(Collision2D collision) 
	{

		if(collision.gameObject.tag == targetTag)
		{
			bounced = true;
			foreach(ContactPoint2D contact in collision.contacts)
			{
				Debug.DrawRay(contact.point, contact.normal, Color.red, 2, false);
			}
			Debug.Log("OnCollisionEnter2D: other=" + targetTag);
			Debug.Log("velocity.x=" + collision.gameObject.rigidbody2D.velocity.x);
			Debug.Log("velocity.x=" + collision.transform.rigidbody2D.velocity.x);
			Debug.Log("velocity.x=" + collision.transform.gameObject.rigidbody2D.velocity.x);
			float myVelocityX = rigidbody2D.velocity.x;
			float otherVelocityX = collision.transform.gameObject.rigidbody2D.velocity.x;
			if(otherVelocityX > 0.0F)
			{
				// Gegenspieler bewegt sich nach rechts
//				myRigidBody2D.velocity = new Vector2(10.0f,0.0f);
				//rigidbody2D.AddForce(new Vector2(100f*otherVelocityX,0.0f));

				// bounces in other direction
				//collision.transform.rigidbody2D.velocity = new Vector2(-pushForce,collision.gameObject.rigidbody2D.velocity.y); 	
				
				//ready ONLY!!!! (collision.gameObject.rigidbody2D)
				//collision.gameObject.rigidbody2D.AddForce(new Vector2(-pushForce,0f));
				//collision.gameObject.rigidbody2D.velocity = new Vector2(-pushForce,collision.gameObject.rigidbody2D.velocity.y); 		//ready ONLY!!!!
			}
			else if(collision.gameObject.rigidbody2D.velocity.x < 0.0F)
			{
				// Gegenspieler bewegt sich nach links
//				myRigidBody2D.velocity = new Vector2(-10.0F,0.0F);

				// bounces in other direction
				//collision.transform.rigidbody2D.velocity = new Vector2(pushForce,collision.gameObject.rigidbody2D.velocity.y); 		
				
				//ready ONLY!!!! (collision.gameObject.rigidbody2D)
				//collision.gameObject.rigidbody2D.AddForce(new Vector2(pushForce,0f));
				//collision.gameObject.rigidbody2D.velocity = new Vector2(pushForce,collision.gameObject.rigidbody2D.velocity.y);
			}
			else
			{
				// Gegenspieler bewegt sich nicht auf X Achse
				
				//collision.gameObject.rigidbody2D.AddForce(new Vector2(myVelocityX*10.0f,0f));
			}
		}
	}
}
