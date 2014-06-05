using UnityEngine;
using System.Collections;

public class PushSkript : MonoBehaviour {

	public string targetTag="Enemy";
	public float pushForce=150.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		if(other.tag == targetTag)
		{
			other.transform.gameObject.rigidbody2D.AddForce(new Vector2(-pushForce,0));
		}
	}
}
