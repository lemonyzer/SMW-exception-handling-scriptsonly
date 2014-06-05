using UnityEngine;
using System.Collections;

public class JumpAblePlatformSaveZone : MonoBehaviour {

	public string targetTag ="Player";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other)
	{
		Debug.LogError( other.gameObject.name +  " Collision OFF!");
		if(other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
		{
			Debug.LogError("Collision OFF!");
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),true);
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
