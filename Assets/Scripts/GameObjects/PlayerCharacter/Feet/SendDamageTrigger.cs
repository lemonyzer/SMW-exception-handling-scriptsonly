using UnityEngine;
using System.Collections;

public class SendDamageTrigger : MonoBehaviour {

	public int damageValue = 1;
//	private int targetLayer = 0;		// Head
	//public bool enabled=true;


	GameObject myCharacterGameObject;		
	GameObject targetCharacterGameObject;
	GameObject targetHead;

	PlatformCharacter myPlatformCharacterScript;

//	HealthController myHealthController;
//	HealthController targetHealthController;


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
		myCharacterGameObject = transform.parent.gameObject;
		myPlatformCharacterScript = myCharacterGameObject.GetComponent<PlatformCharacter>();
//		myHealthController = myCharacterGameObject.GetComponent<HealthController>();
//		if(myHealthController == null)
//			Debug.LogError( myCharacterGameObject.name + "'s has no HealthController");
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
		if(myPlatformCharacterScript != null)
		{
			if(!myPlatformCharacterScript.isHit)
			{
				//Angriff zählt nur wenn selbst nicht getroffen

				if(other.gameObject.layer == layer.head)
				{
					//Angriff zählt nur wenn anderer Collider sich in der Layer (Ebene) "Head" befindet

					if(myCharacterGameObject.rigidbody2D.velocity.y < 0)
					{
						//Angriff zählt nur bei Fallbewegung

						Debug.Log(targetHead.name);
						Debug.Log(other.transform.name);
						Debug.Log(other.transform.parent.name);

						targetHead = other.gameObject;
						targetCharacterGameObject = targetHead.transform.parent.gameObject;



						if(myCharacterGameObject.rigidbody2D.velocity.y < targetCharacterGameObject.rigidbody2D.velocity.y)
						{
							// Angriff zählt nur wenn Gegenspieler nicht durch mich durchspringt
							statsManager.HeadJump(myCharacterGameObject,targetCharacterGameObject);			// Alternative: statsManager oder HealthController können auch SpawnProtection abfragen!
						}
						else
						{
							Debug.LogWarning(targetCharacterGameObject.name + " durchspringt " + myCharacterGameObject.name + ", feet-head trigger zählt nicht als angriff");
							Debug.Log(myCharacterGameObject.name + " " + myCharacterGameObject.rigidbody2D.velocity.y + " < " + targetCharacterGameObject.rigidbody2D.velocity.y + " " + targetCharacterGameObject.name);
						}
						//targetCharacterGameObject.GetComponent<HealthController>().ApplyDamage(damageValue,true);

						//AudioSource.PlayClipAtPoint(deathSound,transform.position,1);								//wird zu oft ausgeführT!!!


						/* SendMessage, Parameter vorher in Array packen!
					 *  
					 * head.SendMessage("ApplyDamage",damageValue,SendMessageOptions.DontRequireReceiver);	// BESSER ??!!!! 
					 */
					}
					else
					{
						Debug.LogWarning( myCharacterGameObject.name + ": " + "Angriff zählt nur bei Fallbewegung");
					}
					
					// Angreifenden Player nach oben schleudern
					//myCharacterGameObject.rigidbody2D.velocity = new Vector2(0.0F,10.0F);
					myCharacterGameObject.rigidbody2D.AddForce(new Vector2(0f,10f));
				}
			}
		}
		else
		{
			Debug.LogError("Charakter hat kein HealthController");
		}
	}


	void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.layer == layer.head)
		{
			//myCharacterGameObject.rigidbody2D.velocity = new Vector2(0.0F,10.0F);
			myCharacterGameObject.rigidbody2D.AddForce(new Vector2(0f,10f));
		}
	}
}
