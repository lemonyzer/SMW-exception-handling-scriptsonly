using UnityEngine;
using System.Collections;

public class JumpAblePlatformSaveZonePhoton : MonoBehaviour {
	
	public bool isCorrectGameObject;
	string playerLayer;
	PlayerControllerPhoton playerController;
	KI kiController;
	
	void Start()
	{
		isCorrectGameObject=false;
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other)
	{
		isCorrectGameObject=false;
		
		//if(other.gameObject.layer == LayerMask.NameToLayer("Player1"))
		if(other.gameObject.layer == 11)
		{
			isCorrectGameObject=true;
			
		}
		//else if(other.gameObject.layer == LayerMask.NameToLayer("Player2"))
		else if(other.gameObject.layer == 12)
		{
			isCorrectGameObject=true;
			
		}
		//else if(other.gameObject.layer == LayerMask.NameToLayer("Player3"))
		else if(other.gameObject.layer == 13)
		{
			isCorrectGameObject=true;
			
		}
		//else if(other.gameObject.layer == LayerMask.NameToLayer("Player4"))
		else if(other.gameObject.layer == 14)
		{
			isCorrectGameObject=true;
			
		}
		
		
		if(isCorrectGameObject)
		{
			playerController = other.gameObject.GetComponent<PlayerControllerPhoton>();
			if(playerController == null)
			{
				// Player muss KI sein
				kiController  = other.gameObject.GetComponent("KI") as KI;
				kiController.isInJumpAbleSaveZone = true;
			}
			else
			{
				playerController.isInJumpAbleSaveZone = true;
			}
			//			Debug.LogError( other.gameObject.name +  ": enters Save Zone => JumpAblePlatform Collision OFF! " + other.gameObject.layer);
			//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),other.gameObject.layer,true);
			Physics2D.IgnoreLayerCollision(18,other.gameObject.layer,true);
			
		}
		
		/*
		if(other.gameObject.tag != null)
		{
			if(other.gameObject.tag == targetTag)
			{
				Debug.LogError("Collision OFF!");
				Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),true);
			}
		}
*/
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		
		isCorrectGameObject=false;
		
		//if(other.gameObject.layer == LayerMask.NameToLayer("Player1"))
		if(other.gameObject.layer == 11)
		{
			isCorrectGameObject=true;
			
		}
		//else if(other.gameObject.layer == LayerMask.NameToLayer("Player2"))
		else if(other.gameObject.layer == 12)
		{
			isCorrectGameObject=true;
			
		}
		//else if(other.gameObject.layer == LayerMask.NameToLayer("Player3"))
		else if(other.gameObject.layer == 13)
		{
			isCorrectGameObject=true;
			
		}
		//else if(other.gameObject.layer == LayerMask.NameToLayer("Player4"))
		else if(other.gameObject.layer == 14)
		{
			isCorrectGameObject=true;
			
		}
		
		
		if(isCorrectGameObject)
		{
			playerController = other.gameObject.GetComponent<PlayerControllerPhoton>();
			if(playerController == null)
			{
				// Player muss KI sein
				kiController  = other.gameObject.GetComponent("KI") as KI;
				kiController.isInJumpAbleSaveZone = false;
			}
			else
			{
				playerController.isInJumpAbleSaveZone = false;
			}
			//			Debug.LogError( other.gameObject.name +  ": leaving Save Zone => JumpAblePlatform Collision On! " + other.gameObject.layer);
			//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),other.gameObject.layer,false);
			Physics2D.IgnoreLayerCollision(18,other.gameObject.layer,false);
			
		}
		
		/*
		if(other.gameObject.tag != null)
		{
			if(other.gameObject.tag == targetTag)
			{
				Debug.LogError("Collision ON!");
				Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),false);
			}
		}*/
	}
	
}
