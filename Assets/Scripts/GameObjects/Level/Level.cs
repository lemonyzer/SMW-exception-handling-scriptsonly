using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	GameObject background;
	SpriteRenderer bgSpriteRenderer;

	bool backgroundFound = false;

	void Awake()
	{
		background = GameObject.FindGameObjectWithTag(Tags.background);
		if(background != null)
		{
			bgSpriteRenderer = background.GetComponent<SpriteRenderer>();
			backgroundFound = true;
		}
		else
		{
			Debug.LogError("kein Background gesetzt, kein RandomSpawnPoint berechenbar!!!");
		}
	}

	public Vector3 getRandomSpawnPosition()
	{
		if(backgroundFound)
		{
//			float x,y,z=0;
//			x = Camera.main.transform.position.x;
//			y = Camera.main.transform.position.y;
			float z = 0;

			float left = bgSpriteRenderer.sprite.bounds.center.x - Camera.main.transform.position.x;
			float bottom = bgSpriteRenderer.sprite.bounds.center.y - Camera.main.transform.position.y;

			float width = bgSpriteRenderer.sprite.bounds.extents.x*2 - left;
			float height = bgSpriteRenderer.sprite.bounds.extents.y*2 - bottom;

			// Beam Zone abziehen
			left++;
			width--;

			// Floor abziehen
			bottom++;
			height--;
			return new Vector3(Random.Range(left,width),Random.Range(bottom,height),z);
		}
		else
			return Vector3.zero;
	}

	// Use this for initialization
	void Start () {
		Debug.Log("Cam Position: " + Camera.main.transform.position);
		Debug.Log("Renderer Bounds: " + bgSpriteRenderer.bounds);
		Debug.Log("Sprite Bounds: " + bgSpriteRenderer.sprite.bounds);
	}
}
