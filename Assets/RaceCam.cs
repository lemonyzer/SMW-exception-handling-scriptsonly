using UnityEngine;
using System.Collections;

public class RaceCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private float hSpeed = 4;
	private Vector3 moveDirection = Vector3.right;

	void FixedUpdate()
	{
		if(transform.position.x <= 0)
		{
			moveDirection = Vector3.right;
		}
		else if(transform.position.x >= 60)
		{
			moveDirection = Vector3.left;
		}
		transform.Translate( moveDirection * hSpeed * Time.fixedDeltaTime);

	}
}
