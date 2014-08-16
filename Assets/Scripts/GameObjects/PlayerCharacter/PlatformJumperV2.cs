using UnityEngine;
using System.Collections;

/**
 * V2: 
 * no extra Layer for each Player needed!!!!
 * no JumpSaveZone needed
 **/

public class PlatformJumperV2 : MonoBehaviour {

	public LayerMask whatIsJumpAbleSaveZone;	// Floor, JumpAblePlatform, DestroyAblePlatform 
	Vector2 saveZoneCheckPosition = new Vector2(-0.5f, -0.5f);	// Position, where the the Ground will be checked

	bool isInJumpAbleSaveZone = false;
	BoxCollider2D bodyCollider;
	BoxCollider2D groundStopper;

	GameObject gameController;
//	PlatformCharacter myPlatformCharacter;
	Layer layer;
	LayerMask jumpOnPlatform;

	SpriteRenderer spriteRenderer;





	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
//		myPlatformCharacter = GetComponent<PlatformCharacter>();

		bodyCollider = GetComponent<BoxCollider2D>();
		groundStopper = transform.FindChild(Tags.groundStopper).GetComponent<BoxCollider2D>();
	}

	// Use this for initialization
	void Start () {
		whatIsJumpAbleSaveZone = 1 << layer.jumpAblePlatformSaveZone;
		jumpOnPlatform = 1 << layer.jumpAblePlatform;


		CalculateColliderEdges();
	}

	Vector2 playerBodyColliderTopLeftPos;
	Vector2 playerBodyColliderBottomRightPos;
	Vector2 playerBodyColliderBottomLeftPos;
//	Vector2 playerGroundStopperColliderBottomLeftPos;

	Vector2 platformColliderFinderTopLeftPos;
	Vector2 platformColliderFinderBottomRightPos;


	void CalculateColliderEdges()
	{
		playerBodyColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.center.x,
		                                           transform.position.y + bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Top Left
		
		playerBodyColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider.size.x*0.5f + bodyCollider.center.x,
		                                               transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Bottom Right
		
		playerBodyColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.center.x,
		                                              transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Bottom Left

//		playerGroundStopperColliderBottomLeftPos = new Vector2(transform.position.x - groundStopper.size.x*0.5f + groundStopper.center.x,
//		                                              transform.position.y - groundStopper.size.y*0.5f + groundStopper.center.y);	// Collider Bottom Left

		platformColliderFinderTopLeftPos = new Vector2(-groundStopper.size.x, groundStopper.size.y) + playerBodyColliderTopLeftPos;		// höhe (nach oben) und breite (nach links) verschieben
		platformColliderFinderBottomRightPos = new Vector2(groundStopper.size.x,0) + playerBodyColliderBottomRightPos;				// breite (nach rechts) verschieben


	}
	
	// Update is called once per frame
	void Update () {
		JumpAblePlatformV2();
	}

	void JumpAblePlatformV2()
	{
		/**
		 * OverlapArea
		 * __________
		 * |		|
		 * |		|
		 * | *....*	|		<-- Character Collider Top
		 * | |	  |	|
		 * |_|____|_|
		 *   *....*			<-- Character Collider Bottom
		 **/

		// Overlap Area in Collision with JumpOnPlatform, disable in Unity

		CalculateColliderEdges();

		Debug.DrawLine(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, Color.green);

		Collider2D platformColliderAbove = Physics2D.OverlapArea(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, jumpOnPlatform);
		if(platformColliderAbove != null)
		{
			Physics2D.IgnoreCollision(bodyCollider, platformColliderAbove, true);
			Physics2D.IgnoreCollision(groundStopper, platformColliderAbove, true);
//			Debug.LogWarning(platformColliderAbove.name + " found");
		}

		Collider2D platformColliderBelow = Physics2D.OverlapArea(playerBodyColliderBottomLeftPos,
		                                                         platformColliderFinderBottomRightPos  - new Vector2(0,1),
		                                                         jumpOnPlatform);
		if(platformColliderBelow != null)
		{
			Physics2D.IgnoreCollision(bodyCollider, platformColliderBelow, false);
			Physics2D.IgnoreCollision(groundStopper, platformColliderBelow, false);
//			Debug.LogWarning(platformColliderAbove.name + " found");
		}
	}

	/**
	 * 
	 * Wird extra abgefragt, da Spieler auch ohne selbst zu Springen eine positive vertikale Geschwindigkeit bekommen können
	 * zB.: steht auf Platform, Gegenspieler springt von unten an die Füße => Spieler macht automatischen Sprung
	 * 
	 **/
	void JumpAblePlatform()
	{

		// check if collider hits jumpablesavezone collider
		// 

		//Physics2D.OverlapArea(pointA, pointB, layers);		// pointB Diagonally opposite of pointA corner of the rectangle.
		//Physics2D.OverlapCircle(center, radius, layers);
		//Debug.DrawLine(start, end)

//		Debug.Log(gameObject.name + " spriteRenderer: " + spriteRenderer.bounds);
//		Debug.Log(gameObject.name + " transform: " + transform.position);
//		Debug.Log(gameObject.name + " renderer: " + renderer.bounds);

		/**
		 * GameObject Scale Corners ( mit hilfe des SpriteRenderers )
		 **/

		Vector2 playerTopLeftPos = new Vector2(transform.position.x - spriteRenderer.bounds.extents.x,
		                                       transform.position.y + spriteRenderer.bounds.extents.y);	// GameObject Top Left

		Vector2 playerBottomRightPos = new Vector2(transform.position.x + spriteRenderer.bounds.extents.x,
		                                           transform.position.y - spriteRenderer.bounds.extents.y);	// GameObject Bottom Right

		Debug.DrawLine(playerTopLeftPos, playerBottomRightPos, Color.cyan);
		/**
		 * BodyCollider Scale Corners
		 **/

		Vector2 playerBodyColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.center.x,
		                                               transform.position.y + bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Top Left

		Vector2 playerBodyColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider.size.x*0.5f + bodyCollider.center.x,
		                                                   transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Bottom Right

		Vector2 playerBodyColliderTopRightPos = new Vector2(transform.position.x + bodyCollider.size.x*0.5f + bodyCollider.center.x,
		                                                   transform.position.y + bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Top Right
		
		Vector2 playerBodyColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.center.x,
		                                                       transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Bottom Left


		isInJumpAbleSaveZone = Physics2D.OverlapArea(playerBodyColliderTopLeftPos, playerBodyColliderBottomRightPos, whatIsJumpAbleSaveZone);

		Color color = Color.blue;
		if(isInJumpAbleSaveZone)
			color = Color.red;

		/**
		 * RED debug rect, if BodyCollider is in JumpAbleSaveZone
		 * 	else its blue
		 * 
		 **/
		Debug.DrawLine(playerBodyColliderTopLeftPos, playerBodyColliderTopRightPos, color);
		Debug.DrawLine(playerBodyColliderTopLeftPos, playerBodyColliderBottomLeftPos, color);
		Debug.DrawLine(playerBodyColliderBottomLeftPos, playerBodyColliderBottomRightPos, color);
		Debug.DrawLine(playerBodyColliderTopRightPos, playerBodyColliderBottomRightPos, color);
//		if(gameObject.layer == layer.player1)
//			Debug.Log( gameObject.name + " is in Jumpable Save Zone: " + isInJumpAbleSaveZone);

		/**
		 * OverlapCircle Lösung, mit Kreishilfslinie 
		 **/

//		Vector2 playerCenterPos = new Vector2(transform.position.x, transform.position.y);	// center!!!
//		isInJumpAbleSaveZone = Physics2D.OverlapCircle(playerCenterPos, bodyCollider.size.x*0.75f, whatIsJumpAbleSaveZone);

//		for(float i=0; i<360; i+=Mathf.PI*0.25f)
//		{
//			float x = Mathf.Cos(i);
//			float y = Mathf.Sin(i);
//
//			Debug.DrawLine(playerCenterPos, playerCenterPos + new Vector2(x,y) * bodyCollider.size.x*0.75f, Color.black);
//		}

		if(!isInJumpAbleSaveZone)
		{
			if(rigidbody2D.velocity.y >0.1F)
			{
				Physics2D.IgnoreLayerCollision(layer.jumpAblePlatform, gameObject.layer, true);		// Kollisionsdetection ausschalten
			}
			else if(rigidbody2D.velocity.y <0.1F)
			{
				Physics2D.IgnoreLayerCollision(layer.jumpAblePlatform, gameObject.layer, false);		// Kollisionsdetection einschalten
			}
		}
		else
		{
			// is in Jumpable save zone
			// deaktiviere collision, nicht an platformen hängen bleiben
			Physics2D.IgnoreLayerCollision(layer.jumpAblePlatform, gameObject.layer,true);
		}
	}

}
