using UnityEngine;
using System.Collections;

public class Beam : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
		{
			float oldY = other.transform.position.y;
			float oldX = other.transform.position.x;
			if(oldX < 10)
			{
				other.gameObject.transform.position = new Vector2(19.5f,oldY);
			}
			else
			{
				other.gameObject.transform.position = new Vector2(0.5f,oldY);
			}
		}
	}
}
