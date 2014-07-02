using UnityEngine;
using System.Collections;

public class SendDamageCollider : MonoBehaviour {

	public int damageValue = 1;
//	private int targetLayer = 0;		// Head
	//public bool enabled=true;


	Transform myCharacterTransform;		
	Transform targetCharacterTransform;
	Transform targetHead;

	HealthController myHealthController;
	HealthController targetHealthController;


	/** 
	 * Connection to GameController 
	 **/
	private GameObject gameController;
	private Layer layer;
	private StatsManager statsManager;

	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
		statsManager = gameController.GetComponent<StatsManager>();
//		targetLayer = layer.head;
	}


	// Use this for initialization
	void Start () {
		myCharacterTransform = transform.parent;
		myHealthController = myCharacterTransform.GetComponent<HealthController>();
		if(myHealthController == null)
			Debug.LogError( myCharacterTransform.name + "'s has no HealthController");
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D(Collider2D other)
	{
		if(!(Network.peerType == NetworkPeerType.Disconnected))
		{
			// connected
			if(!Network.isServer)
			{
				return;
			}
		}
		if(myHealthController != null)
		{
			if(!myHealthController.isHit)
			{
				//Angriff zählt nur wenn selbst nicht getroffen

				if(other.gameObject.layer == layer.head)
				{
					//Angriff zählt nur wenn anderer Collider sich in der Layer (Ebene) "Head" befindet

					if(myCharacterTransform.rigidbody2D.velocity.y < 0)
					{
						//Angriff zählt nur bei Fallbewegung

						targetHead = other.gameObject.transform;
						targetCharacterTransform = targetHead.parent;
						statsManager.HeadJump(myCharacterTransform,targetCharacterTransform);
						//targetCharacterTransform.GetComponent<HealthController>().ApplyDamage(damageValue,true);

						//AudioSource.PlayClipAtPoint(deathSound,transform.position,1);								//wird zu oft ausgeführT!!!


						/* SendMessage, Parameter vorher in Array packen!
					 *  
					 * head.SendMessage("ApplyDamage",damageValue,SendMessageOptions.DontRequireReceiver);	// BESSER ??!!!! 
					 */
					}
					else
					{
						Debug.Log( myCharacterTransform.name + ": " + "Angriff zählt nur bei Fallbewegung");
					}
					
					// Angreifenden Player nach oben schleudern
					myCharacterTransform.rigidbody2D.velocity = new Vector2(0.0F,10.0F);
				}
			}
		}
		else
		{
			Debug.LogError("Charakter hat keine HealthController");
		}
	}
}
