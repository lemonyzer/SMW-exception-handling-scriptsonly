using UnityEngine;
using System.Collections;

public class KI : MonoBehaviour {

	public bool isDead=false;
	public bool isInJumpAbleSaveZone=false;
	public bool isBouncing = false;	// move speed changed while bouncing with other player 
	public float pushForce;
	public float pushTime = 0f;

	public AudioClip jumpSound;
	public AudioClip changeRunDirectionSound;
	public AudioClip wallJumpSound;
	
	Animator anim;
	public bool changedRunDirection = false;				

	public Vector2 moveDirection = Vector2.zero;
	//CharacterController characterController;
	//public float gravity=10;
	public float maxSpeed = 6f;
	public Vector2 jumpForce = new Vector2(10.0f, 14.0f);
	float velocity = 0;
	public bool inputKIJump = false;
	public bool facingRight = true;

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
	public float changeDirectionInterval=0.5f; // 0,7 sekunden
	public bool ableToChangeDirection = false;
	public float deltaLastDirectionChange;
	public float deltaLastJump;

	GameObject gameController;
	private HashID hash;
	Stats stats;

	void Awake() {
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		stats = gameController.GetComponent<Stats>();
		hash = gameController.GetComponent<HashID>();
	}

	void Start() {
		//anim = gameObject.transform.parent.gameObject.GetComponent<Animator>();
		anim = GetComponent<Animator>();
		isInJumpAbleSaveZone=false;
		deltaLastJump =0.0f;
		deltaLastDirectionChange = 0.7f;
	}

	void Update() 
	{
		FixCheckPosition();
		if(ableToChangeDirection)
		{
			target = FindClosestPlayerWithGameController();
		}
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

	GameObject FindClosestPlayerWithGameController() {

		targetDistance = Mathf.Infinity;
		Vector3 position = transform.position;

		foreach (GameObject go in stats.playerList)
		{
			if(go.layer != gameObject.layer)
			{
				// Nur andere Spieler können target werden!
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
//			else
//				Debug.Log("GameObject layer: " + go.layer + " == Player(Ki) layer: " + gameObject.layer);
		}
		
		return closest;
	}
	
	// Update is called once per frame
	void KiMove () {
		/* Spotting überflüssig, da alle Mitspielderpositionen bekannt sind -> bewege zum nächsten Spieler */
		/*
		spotted = Physics2D.OverlapCircle (spotCheck.position, spottingRadius, whatIsSpotted);
		*/

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
	
	void CheckTimeBetweenLastDirectionChange() {
		deltaLastDirectionChange += Time.deltaTime;
		if(deltaLastDirectionChange > changeDirectionInterval)				// Bot leichter machen
		{																	// alle halbe Sekunde darf er Richtung wechseln
			ableToChangeDirection=true;
			deltaLastDirectionChange=0.0F;
		}
		else
			ableToChangeDirection=false;
	}

	// Update is called once per frame
	void FixedUpdate() {
		if(!isDead)
		{
			CheckTimeBetweenLastDirectionChange();
			FixSetAnim();
			KiMove();
			FixMove();							
			JumpAblePlatform();
		}
	}
	void FixCheckPosition()
	{
		Vector2 playerPos = new Vector2(rigidbody2D.transform.position.x,rigidbody2D.transform.position.y);
		grounded = Physics2D.OverlapCircle (playerPos+groundCheckPosition, groundRadius, whatIsGround);
		walled = Physics2D.OverlapCircle (playerPos+wallCheckPosition, wallRadius, whatIsWall);
	}
	void FixSetAnim() 
	{
		anim.SetBool (hash.groundedBool, grounded);
		anim.SetBool (hash.walledBool, walled);
		anim.SetFloat (hash.vSpeedFloat, rigidbody2D.velocity.y);
	}

	void FixMove()
	{
		velocity = (moveDirection.x);
		if(isBouncing)
		{
			//Alte Kraft in X Richtung wird nicht komplett überschrieben!

			if(pushForce > 0f)
			{
				if(velocity > 0f)
				{
					velocity *= maxSpeed;		// wenn Spieler in die gleiche Richtung wie pushForce sich bewegt,
					// volle Geschwindigkeit nehmen  
				}
				else
					velocity *= maxSpeed * 0.2f;
			}
			else if(pushForce < 0f)
			{
				if(velocity < 0f)
				{
					velocity *= maxSpeed;
				}
				else
					velocity *= maxSpeed * 0.2f;
			}
			
			pushTime += Time.deltaTime;
			float pushSpeed;
			pushForce = pushForce - (pushForce * 4f * Time.deltaTime);
			pushSpeed = pushForce;
//			Debug.LogError(this.gameObject.transform.name+ " pushSpeed = " + pushSpeed);
			if(Mathf.Abs(pushSpeed) < 1)
			{
//				Debug.LogError(this.gameObject.transform.name+ " pushSpeed = 0");
				isBouncing = false;
				pushTime = 0f;
			}
			else
			{
				velocity += pushSpeed;
//				Debug.LogError(this.gameObject.transform.name+ " velocity = " + velocity);
			}
			
		}
		else // if(!isBouncing)
		{
			velocity *= maxSpeed;
		}

		/**
		 * maxSpeed check
		 **/
		if(Mathf.Abs(velocity) > maxSpeed)
		{
			// neue velocity überschreitet maxSpeed!!!
			if(velocity < (-1.0f*maxSpeed))
			{
				velocity = -1.0f*maxSpeed;
				//rigidbody2D.velocity = new Vector2((-1.0f)*maxSpeed, rigidbody2D.velocity.y);
			}
			else if(velocity > maxSpeed)
			{
				velocity = maxSpeed;
				//rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
			}
		}
		
		// gedrosselte velocity übernehmen
		rigidbody2D.velocity = new Vector2(velocity, rigidbody2D.velocity.y);


		/**
		 * Animator status Update
		 **/
		if(anim != null)
		{
			anim.SetFloat(hash.hSpeedFloat, velocity);
		}
		else
			Debug.LogError("Animator not set");
		
		/**
		 * Check Direction Change
		 **/
		if(velocity > 0f && !facingRight)
		{
			Flip();
		}
		else if(velocity < 0f && facingRight)
		{
			Flip();
		}
		else
		{
			changedRunDirection = false;
		}
		
		if(grounded && inputKIJump) {
			//Springen
			AudioSource.PlayClipAtPoint(jumpSound,transform.position,1);				//Jump
			anim.SetBool(hash.groundedBool,false);
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,jumpForce.y);		//<--- besser für JumpAblePlatforms	
			//rigidbody2D.AddForce(new Vector2(0.0F, jumpForce.y));						//<--- klappt nicht 100% mit JumpAblePlatforms
		}
		/* WallJump */
		else if(!grounded && walled && inputKIJump) {
			//von Wand wegspringen
			AudioSource.PlayClipAtPoint(wallJumpSound,transform.position,1);			//WallJump
			rigidbody2D.velocity = new Vector2(0,0);									//alte Geschwindigkeit entfernen
			Flip();																		//Charakter drehen 
			anim.SetBool(hash.groundedBool,false);
			anim.SetBool(hash.walledBool,false);
			rigidbody2D.velocity = new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y);	//<--- besser für JumpAblePlatforms
		}

	}
	
	void StartJump() {
		if(JumpAllowed)
			inputKIJump = true;
	}
	
	void StopJump() {
		inputKIJump = false;
	}

	void Flip() {

		// Drift sound abspielen
		if(grounded)
		{
			changedRunDirection = true;
			anim.SetTrigger(hash.changeRunDirectionTrigger);	// Start Change Run Direction Animation
			AudioSource.PlayClipAtPoint(changeRunDirectionSound,transform.position,1);				//ChangeDirection
		}
		
		// Richtungvariable anpassen
		facingRight = !facingRight;
		
		// WallCheck anpassen
		wallCheckPosition *= -1;
		
		// Transform spiegeln
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void JumpAblePlatform()
	{
		if(!isInJumpAbleSaveZone)
		{
			if(rigidbody2D.velocity.y >0.0F)
			{
				Physics2D.IgnoreLayerCollision(18,gameObject.layer,true);
			}
			else
			{
				Physics2D.IgnoreLayerCollision(18,gameObject.layer,false);
			}
		}
	}
}
