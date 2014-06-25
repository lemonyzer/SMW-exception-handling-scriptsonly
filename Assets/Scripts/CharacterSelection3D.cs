using UnityEngine;
using System.Collections;

public class CharacterSelection3D : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetSelectedCharacter();
	}


	GameObject GetSelectedCharacter()
	{
		if(Input.GetMouseButtonUp(0))
		{

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);		
			Vector3 origin = ray.origin;										// startPoint
			Vector3 direction = ray.direction;									// direction
			float distance = 100f;
			RaycastHit hit;
			Physics.Raycast(origin,direction,out hit,distance);
			if(hit.collider != null)
			{
				Debug.Log(hit.collider.transform.parent.name);
				return hit.collider.transform.parent.gameObject;
			}
		}
		return null;
	}
}
