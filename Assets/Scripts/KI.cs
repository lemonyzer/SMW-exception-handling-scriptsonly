using UnityEngine;
using System.Collections;

public class KI : MonoBehaviour {

	public bool isDead=false;

	public AudioClip jumpSound;
	public AudioClip changeRunDirectionSound;
	public AudioClip wallJumpSound;

	Animator anim;

	public Vector2 moveDirection = Vector2.zero;
	//CharacterController characterController;
	//public float gravity=10;
	public float maxSpeed = 6f;
	public Vector2 jumpForce = new Vector2(10.0f, 14.0f);
	float velocity = 0;
	public bool inputJump = false;
	bool facingRight = true;

	bool grounded = false;
	bool walled = false;
	public Vector2 groundCheckPosition = new Vector2(0, -0.5f);
	public Vector2 wallCheckPosition = new Vector2(0.5f, 0);
	float groundRadius = 0.2f;
	float wallRadius = 0.1f;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;

	/* KI Variablen */
	public bool JumpAllowed=true;
	public bool MoveAllowed=true;
	public GameObject target;
	public GameObject closest;
	public bool targetHigher;
	public float targetDirection;
	public float targetDistance;
	public float jumpRange=4.0f;
	//intelegent, 
	public float changeDirectionInterval=0.7f; // 0,7 sekunden
	public bool ableToChangeDirection = false;
	public float deltaLastDirectionChange = 0.0F;
	public float deltaLastJump = 0.0F;

	void Start() {
		//anim = gameObject.transform.parent.gameObject.GetComponent<Animator>();
		anim = GetComponent<Animator>();
	}

	void Update() 
	{
		FixCheckPosition();
	}

	GameObject FindClosestPlayer() {

		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("Player");
		//GameObject closest;
		targetDistance = Mathf.Infinity;
		Vector3 position = transform.position;

		foreach (GameObject go in gos) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < targetDistance) {
				closest = go;
				targetDistance = curDistance;

				if( diff.y < 0.1f )			// save offset!!! physic has no 0.0F precision!
				{
					targetHigher = false;
				}
				else
				{
					targetHigher = true;
				}

				if( -10.0f < diff.x && diff.x < 0.0f )			//between -10 and 0
				{
					targetDirection = -1;
				}
				else if( 10.0f < diff.x && diff.x < 20.0f )		//between 10 and 20
				{
					targetDirection = -1;
				}
				else 											//else (between 0 and 10)
				{
					targetDirection = +1;
				}

			}
		}

		return closest;
	}
	
	// Update is called once per frame
	void KiMove () {
		/* Spotting überflüssig, da alle Mitspielderpositionen bekannt sind -> bewege zum nächsten Spieler */
		/*
		spotted = Physics2D.OverlapCircle (spotCheck.position, spottingRadius, whatIsSpotted);
		*/
		
		target = FindClosestPlayer();
		if(target != null)
		{
			if(!targetHigher)
			{
				StopJump();
				//Bot ist höher oder auf gleicher Höhe
				if(ableToChangeDirection)					//Bot leichter machen
					moveDirection.x = targetDirection;
				if(grounded || walled)
				{
					//moveDirection.x = richtung;
					if(targetDistance < jumpRange) {
						StartJump();
					}
				}
			}
			else if(targetHigher)
			{
				StartJump();
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(!isDead)
		{
			deltaLastDirectionChange += Time.deltaTime;
			if(deltaLastDirectionChange > changeDirectionInterval)				//Bot leichter machen
			{												//alle halbe Sekunde darf er Richtung wechseln
				ableToChangeDirection=true;
				deltaLastDirectionChange=0.0F;
			}
			else
				ableToChangeDirection=false;
			FixSetAnim();
			KiMove();
			//		FixInputCheck();					//Tastatur
			//		FixInputTouchCheck ();				//Mobile
			FixMove();							//Jump, Wall-Jump, rechts, links Bewegung	
			JumpAblePlatform();
		}
	}
	void FixCheckPosition()
	{
		Vector2 playerPos = new Vector2(rigidbody2D.transform.position.x,rigidbody2D.transform.position.y);
		grounded = Physics2D.OverlapCircle (playerPos+groundCheckPosition, groundRadius, whatIsGround);
		walled = Physics2D.OverlapCircle (playerPos+wallCheckPosition, wallRadius, whatIsWall);
//		//Boden unter den Füßen
//		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
//		//Gesicht an Wand (nur Gesicht, kein Rücken!)
//		walled = Physics2D.OverlapCircle (wallCheck.position, wallRadius, whatIsWall);
	}
	void FixSetAnim() 
	{
		anim.SetBool ("Grounded", grounded);
		anim.SetBool ("Walled", walled);
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
	}

	void FixMove()
	{
		//rigidbody2D.velocity = new Vector2 (moveDirection.x * maxSpeed, rigidbody2D.velocity.y);
		//Alte Kraft in X Richtung wird ignoriert!
		//velocity enthält alte Kraft -/+
		velocity = (moveDirection.x ) * maxSpeed;
		//velocity = rigidbody2D.velocity.x + (moveDirection.x + deltaX) * maxSpeed;				//schwammig!!!! bei Flip() Kraftrichtung auch wechseln
		anim.SetFloat("hSpeed", Mathf.Abs (velocity));
		//abs für beide Richtungen!!! richtung behalten!!
		if(Mathf.Abs(velocity) < maxSpeed)
		{
			//rigidbody2D.AddForce( new Vector2 (velocity,0));
			rigidbody2D.velocity = new Vector2 (velocity, rigidbody2D.velocity.y);
		}
		else 
		{
			//rigidbody2D.AddForce( new Vector2 ((moveDirection.x + deltaX)*maxSpeed,0));
			rigidbody2D.velocity = new Vector2 ((moveDirection.x)*maxSpeed, rigidbody2D.velocity.y);
		}
		if (velocity > 0 && !facingRight)
			Flip ();
		else if (velocity < 0 && facingRight)
			Flip ();
		
		//mit CharacterController
		//		moveDirection.y -= gravity; // nicht nötig, Physic2D!
		//		if(characterController.isGrounded) 
		//		{
		//			if(inputJump)
		//			{
		//				characterController.Move(moveDirection * Time.deltaTime);
		//			}
		//		}
		
		if (grounded && inputJump) {
			//Springen
			AudioSource.PlayClipAtPoint(jumpSound,transform.position,1);				//Jump
			anim.SetBool("Grounded",false);
			rigidbody2D.velocity = new Vector2(0.0F, jumpForce.y);
			//igidbody2D.AddForce(new Vector2(0, jumpForce.y));
		}
		/* WallJump */
		else if (!grounded && walled && inputJump) {
			//von Wand wegspringen
			AudioSource.PlayClipAtPoint(wallJumpSound,transform.position,1);				//WallJump
			rigidbody2D.velocity = new Vector2(0,0);		//alte Geschwindigkeit entfernen
			Flip ();										//Charakter drehen 
			anim.SetBool("Walled",false);
			//rigidbody2D.velocity = jumpForce;
			//			rigidbody2D.AddForce(new Vector2(300, 300));
			//			rigidbody2D.AddForce(jumpForce);
			rigidbody2D.velocity = new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y);
			//rigidbody2D.AddForce(new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y)); //Kraft in Richtung localScale.x anwenden
		}

	}

	void Flip() {
		if(grounded)
			AudioSource.PlayClipAtPoint(changeRunDirectionSound,transform.position,1);				//ChangeDirection
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void StartJump() {
		if(JumpAllowed)
			inputJump = true;
	}
	
	void StopJump() {
		inputJump = false;
	}

	void JumpAblePlatform()
	{
		if(rigidbody2D.velocity.y >0.0F)
		{
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),gameObject.layer,true);
			//Physics2D.IgnoreCollision(platform.collider2D, collider2D,true);
		}
		else
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),gameObject.layer,false);
		//Physics2D.IgnoreCollision(platform.collider2D, collider2D,false);
	}
	
	void ForceJumpAblePlatform()
	{
		Debug.Log("Force Jump-Able-Platform");
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),gameObject.layer,true);
	}
}
