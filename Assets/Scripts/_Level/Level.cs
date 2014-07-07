using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	GameObject background;
	SpriteRenderer bgSpriteRenderer;

	void Awake()
	{
		background = GameObject.FindGameObjectWithTag(Tags.background);
		bgSpriteRenderer = background.GetComponent<SpriteRenderer>();
	}

	public Vector3 getRandomSpawnPosition()
	{
		float x,y,z=0;
		x = Camera.main.transform.position.x;
		y = Camera.main.transform.position.y;
		z = 0;

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

	// Use this for initialization
	void Start () {
		Debug.Log("Cam Position: " + Camera.main.transform.position);
		Debug.Log("Renderer Bounds: " + bgSpriteRenderer.bounds);
		Debug.Log("Sprite Bounds: " + bgSpriteRenderer.sprite.bounds);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
