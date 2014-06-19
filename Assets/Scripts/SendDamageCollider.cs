using UnityEngine;
using System.Collections;

public class SendDamageCollider : MonoBehaviour {

	public int damageValue = 1;
	public string targetTag ="Head";
	//public bool enabled=true;


	Transform myCharacter;		
	Transform myHead;				//has HeadTrigger and HealthController
	Transform targetCharacter;
	Transform targetHead;

	HealthController myHealthController;
	HealthController targetHealthController;

	// Use this for initialization
	void Start () {
		myCharacter = transform.parent;
		myHead = myCharacter.Find("Head");
		myHealthController = myHead.GetComponent<HealthController>();
		if(myHealthController == null)
			Debug.LogError( myCharacter.name + "'s Head has no HealthController");
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D(Collider2D other)
	{
		if(myHead != null)
		{
			if(!myHealthController.isHit)
			{
				//Angriff zählt nur wenn selbst nicht getroffen

				if(other.gameObject.tag != null)
				{
					if(other.gameObject.tag == targetTag)
					{
//						foreach(ContactPoint2D contact in collision.contacts)
//						{
//							Debug.DrawRay(contact.point, contact.normal, Color.red, 2, false);
//						}
						//Angriff zählt nur wenn anderer Collider den richtigen Tag besitzt ("Head")
						if(myCharacter.rigidbody2D.velocity.y < 0)
						{
							//Angriff zählt nur bei Fallbewegung
							targetCharacter = other.gameObject.transform.parent;
							targetHead = targetCharacter.Find("Head");
							//AudioSource.PlayClipAtPoint(deathSound,transform.position,1);								//wird zu oft ausgeführT!!!
							targetHead.GetComponent<HealthController>().ApplyDamage(damageValue,true);
//							Debug.Log( myCharacter.name + ": " + "kollision mit Kopf von " + targetCharacter.name);
							
							/* SendMessage, Parameter vorher in Array packen!
							 *  
							 * head.SendMessage("ApplyDamage",damageValue,SendMessageOptions.DontRequireReceiver);	// BESSER ??!!!! 
							 */
						}
						else
						{
							Debug.Log( myCharacter.name + ": " + "Angriff zählt nur bei Fallbewegung");
						}

						// Angreifenden Player nach oben schleudern
						myCharacter.rigidbody2D.velocity = new Vector2(0.0F,10.0F);
					}
				}
			}
		}
		else
		{
			Debug.LogError("Charakter hat keine Hitcollider 'Head'");
		}

	}
}
