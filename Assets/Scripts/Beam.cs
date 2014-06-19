using UnityEngine;
using System.Collections;

public class Beam : MonoBehaviour {

	bool beamableObject;

	void Start()
	{
		beamableObject=false;
	}

	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other)
	{
		beamableObject = false;

		if(other.gameObject.layer == LayerMask.NameToLayer("Player1"))
		{
			beamableObject = true;
		}
		else if(other.gameObject.layer == LayerMask.NameToLayer("Player2"))
		{
			beamableObject = true;
		}
		else if(other.gameObject.layer == LayerMask.NameToLayer("Player3"))
		{
			beamableObject = true;
		}
		else if(other.gameObject.layer == LayerMask.NameToLayer("Player4"))
		{
			beamableObject = true;
		}
		else if(other.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
		{
			beamableObject = true;
		}

		if(beamableObject)
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
