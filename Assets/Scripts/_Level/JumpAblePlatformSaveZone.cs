using UnityEngine;
using System.Collections;

public class JumpAblePlatformSaveZone : MonoBehaviour {

	public bool isCorrectGameObject;
	string playerLayer;
	PlatformCharacter playerController;

	private GameObject gameController;
	private Layer layer;
	
	
	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
	}
	
	void Start()
	{
    }

	bool Check(Collider2D other)
	{
		if(other.gameObject.layer == layer.player1)
		{
			return true;
		}
		else if(other.gameObject.layer == layer.player2)
		{
			return true;
		}
		else if(other.gameObject.layer == layer.player3)
		{
			return true;
		}
		else if(other.gameObject.layer == layer.player4)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other)
	{
		isCorrectGameObject = Check(other);

        if(isCorrectGameObject)
		{
			playerController = other.gameObject.GetComponent<PlatformCharacter>();
//			playerController.isInJumpAbleSaveZone = true;

//			Debug.LogError( other.gameObject.name +  ": enters Save Zone => JumpAblePlatform Collision OFF! " + other.gameObject.layer);
			Physics2D.IgnoreLayerCollision(layer.jumpAblePlatform, other.gameObject.layer,true);
        }
	}

	void OnTriggerExit2D(Collider2D other)
	{
		isCorrectGameObject = Check(other);
				
		if(isCorrectGameObject)
		{
			playerController = other.gameObject.GetComponent<PlatformCharacter>();
//			playerController.isInJumpAbleSaveZone = false;

//			Debug.LogError( other.gameObject.name +  ": leaving Save Zone => JumpAblePlatform Collision On! " + other.gameObject.layer);
			Physics2D.IgnoreLayerCollision(layer.jumpAblePlatform, other.gameObject.layer,false);
        }
	}

}
