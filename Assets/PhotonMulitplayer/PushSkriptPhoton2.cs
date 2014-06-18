using UnityEngine;
using System.Collections;

public class PushSkriptPhoton2 : MonoBehaviour {
	
	float pushForce=150.0f;
	
	bool isKI=false;
	
	Transform myCharacter;
	Rigidbody2D myRigidBody2D;
	Rigidbody2D otherRigidBody2D;
	PlatformerWalker4 myPlayerController;
	PlatformerWalker4 otherPlayerController;
	
	KI myKIController;
	KI otherKIController;
	
	/** 
	 * Connection with GameController 
	 **/
	GameObject gameController;
	HashID hash;
	
	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
	}
	
	// Use this for initialization
	void Start () {
		myCharacter = this.gameObject.transform;
		
		
		myRigidBody2D = myCharacter.rigidbody2D;
		if(myRigidBody2D == null)
			Debug.LogError(myCharacter.name + " hat kein RigidBody2D");
		else
			Debug.LogError(myCharacter.name + " hat ein RigidBody2D");
		
		
		myPlayerController = GetComponent<PlatformerWalker4>();
		if(myPlayerController == null)
			Debug.LogError(myCharacter.name + " hat kein PlatformerWalker4");
		else
			isKI = false;
		
		
		myKIController = GetComponent<KI>();
		if(myKIController == null)
			Debug.LogError(myCharacter.name + " hat kein KIController");
		else
			isKI = true;
	}
	
	
	void OnCollisionEnter2D(Collision2D collision) 
	{
		/***
		 * Compare layer > 10 & layer < 14 effektiver?
		 * mit layermask layer 11,12,13,14 und verknüpfen und vergleichen?
		 ***/
		if((collision.gameObject.layer == 11) || 
		   (collision.gameObject.layer == 12) ||
		   (collision.gameObject.layer == 13) || 
		   (collision.gameObject.layer == 14))
		{
			Debug.Log(myCharacter.name + ": Collision's relative Velocity = " + collision.relativeVelocity);
			
			float relativeVelocity = Mathf.Abs(collision.relativeVelocity.x);
			
			Debug.DrawLine(myCharacter.position,
			               myCharacter.position + new Vector3(0f,0.5f,0f),
			               Color.blue,
			               2,
			               false);
			
			Debug.DrawLine(collision.contacts[0].point,		// Start
			               collision.contacts[1].point,		// End
			               Color.red,						// Color
			               2,								// Visible Time
			               false);							// depthTest
			
			if(myCharacter.position.x < collision.contacts[0].point.x)
			{
				//				// Collision rechts
				//				// KI
				//				if(isKI)
				//				{
				//					if(myKIController.facingRight)
				//					{
				//						// Gesicht zeigt in Richtung der Collision
				//						//pushForce = -collision.relativeVelocity.x * 0.5;
				//					}
				//					else
				//					{
				//						// Rücken zeigt in Richtung der Collision
				//						//pushForce = -collision.relativeVelocity.x * 0.5;
				//					}
				//				}
				//				// Collision rechts
				//				// Player
				//				else
				//				{
				//					if(myPlayerController.facingRight)
				//					{
				//						// Gesicht zeigt in Richtung der Collision
				//						//pushForce = -collision.relativeVelocity.x * 0.5;
				//					}
				//					else
				//					{
				//						// Rücken zeigt in Richtung der Collision
				//						//pushForce = -collision.relativeVelocity.x * 0.5;
				//					}
				//				}
				
				pushForce = -relativeVelocity;
				
				if(!isKI)
				{
					myPlayerController.isBouncing = true;
					myPlayerController.pushForce = pushForce;
				}
				else
				{
					myKIController.isBouncing = true;
					myKIController.pushForce = pushForce;
				}
			}
			else if(myCharacter.position.x > collision.contacts[0].point.x)
			{
				//				// Collision links
				//				// KI
				//				if(isKI)
				//				{
				//					if(myKIController.facingRight)
				//					{
				//						// Gesicht zeigt in Richtung der Collision
				//						//pushForce = collision.relativeVelocity.x * 0.5;
				//					}
				//					else
				//					{
				//						// Rücken zeigt in Richtung der Collision
				//						//pushForce = collision.relativeVelocity.x * 0.5;
				//					}
				//				}
				//				// Collision links
				//				// Player
				//				else
				//				{
				//					if(myPlayerController.facingRight)
				//					{
				//						// Gesicht zeigt in Richtung der Collision
				//						//pushForce = collision.relativeVelocity.x * 0.5;
				//					}
				//					else
				//					{
				//						// Rücken zeigt in Richtung der Collision
				//						//pushForce = collision.relativeVelocity.x * 0.5;
				//					}
				//				}
				
				pushForce = relativeVelocity;
				
				if(!isKI)
				{
					myPlayerController.isBouncing = true;
					myPlayerController.pushForce = pushForce;
				}
				else
				{
					myKIController.isBouncing = true;
					myKIController.pushForce = pushForce;
				}
			}
			
			//			foreach(ContactPoint2D contact in collision.contacts)
			//			{
			//				Debug.DrawRay(contact.point, contact.normal, Color.red, 2, false);
			//				Debug.Log("Contact Point: " + contact.point);
			//			}
			
			Debug.Log("PushSkript von: " + myCharacter.name);
			otherRigidBody2D = collision.rigidbody;		// zgriff auf Physikeigenschaften des Gegenspielers
			
			float myVelocityX = rigidbody2D.velocity.x;
			float myVelocityY = rigidbody2D.velocity.y;
			float otherVelocityX = otherRigidBody2D.velocity.x;
			float otherVelocityY = otherRigidBody2D.velocity.y;
			
			Debug.Log(myCharacter.name + " velocity.x= " + myRigidBody2D.velocity.x);
			Debug.Log(collision.gameObject.name + " velocity.x= " + collision.rigidbody.velocity.x);
			
			//
			//			if(myRigidBody2D.velocity.x > 1f)
			//			{
			//				Debug.Log(myCharacter.name + " bewegte sich nach rechts und wird jetzt nach links gedrückt");
			//				// Spieler bewegt sich nach rechts
			//				// muss also nach links gestoßen werden
			//
			////				myRigidBody2D.velocity = new Vector2(-10.0f,0f);
			//				pushForce = -10f;
			//			}
			//			else if(myRigidBody2D.velocity.x < 1f)
			//			{
			//				Debug.Log(myCharacter.name + " bewegte sich nach links und wird jetzt nach rechts gedrückt");
			//				// Spieler bewegt sich nach links
			//				// muss also nach rechts gestoßen werden
			////				myRigidBody2D.velocity = new Vector2(10.0f,0f);
			//				pushForce = 10f;
			//			}
			//			else
			//			{
			//				// Spieler bewegt sich nicht auf x-Achse
			//				// Richtung wird von Gegenspieler vorgegeben
			//				if(otherVelocityX > 0f)
			//				{
			//					Debug.Log("Gegenspieler komm von rechts");
			//					// Gegenspieler bewegt sich nach rechts
			//					// Spieler wird in die gleiche Richtung gedrückt
			////					myRigidBody2D.velocity = new Vector2(10.0f,0f);
			//					pushForce = 10f;
			//				}
			//				else if(otherVelocityX < 0f)
			//				{
			//					Debug.Log("Gegenspieler komm von links");
			//					// Gegenspieler bewegt sich nach links
			//					// Spieler wird in die gleiche Richtung gedrückt
			////					myRigidBody2D.velocity = new Vector2(-10.0f,0f);
			//					pushForce = -10f;
			//				}
			//				else
			//				{
			//					// Gegenspieler bewegt sich nicht!
			//					// facingRight checken!
			//					if(!isKI)
			//					{
			//						if(otherPlayerController.facingRight)
			//						{
			//							pushForce = 10f;
			//						}
			//						else
			//							pushForce = -10f;
			//					}
			//					else
			//					{
			//						if(otherKIController.facingRight)
			//						{
			//							pushForce = 10f;
			//						}
			//						else
			//							pushForce = -10f;
			//					}
			//				}
			//			}
			//
			//			if(!isKI)
			//			{
			//				myPlayerController.isBouncing = true;
			//				myPlayerController.pushForce = pushForce;
			//			}
			//			else
			//			{
			//				myKIController.isBouncing = true;
			//				myKIController.pushForce = pushForce;
			//			}
			//
			////			try {
			////				otherKIController = collision.transform.GetComponent<KI>();
			////			} catch(UnityException e) { }
			////
			////			try
			////			{
			////				otherPlayerController = collision.transform.GetComponent<PlayerController>();
			////			} catch(UnityException e) { }
		}
	}
}
