using UnityEngine;
using System.Collections;

public class PlayerControllerMP : MonoBehaviour {

	public bool isDead = false;
	public bool JumpAllowed=true;
	public bool MoveAllowed=true;

	public AudioClip jumpSound;
	public AudioClip changeRunDirectionSound;
	public AudioClip wallJumpSound;

	public Vector2 moveDirection = Vector2.zero;
	//CharacterController characterController;
	//public float gravity=10;
	public float maxSpeed = 10.0f;
	public Vector2 jumpForce = new Vector2(10.0F, 14.0F);
	float velocity = 0;
	public bool inputJump = false;
	public bool inputMove = false;
	bool facingRight = true;

	Animator anim;

	public bool grounded = false;
	public bool walled = false;
	public Vector2 groundCheckPosition = new Vector2(0, -0.5f);
	public Vector2 wallCheckPosition = new Vector2(0.5f, 0);
	//public Transform groundCheck;
	//public Transform wallCheck;
	float groundRadius = 0.2f;
	float wallRadius = 0.1f;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;


	/* Android */
	public Touch myTouch;
	float deltaX=0;
	/* Input */
	bool inputTouchJump;
	public float rangeX = Screen.width/2/2;	//Pixelbreite  Bildschirmauflösung/2 /2
	bool buttonIsPressed = false;
	bool stickIsPressed = false;
	/* / Input */
	//float deltaY=0;
//	public Button buttonA;
//	public Button buttonB;
//	public AnalogStick leftStick;
	/* / Android */
	
	Transform player;

	// Use this for initialization
	void Start () {
		//characterController = GetComponent<CharacterController>();	//aktuelle Componente des Gameobjects zuweisen 



	}

	void Update() {
		if(!isDead)
		{
			InputCheck ();
			InputTouchCheck ();	
			JumpAblePlatform();
		}

	}
	void InputCheck()
	{
		/* Run */
		//Pfeil nach links = -1 
		//Pfeil nach rechts = +1
		//Links, Rechts
		moveDirection.x = Input.GetAxis ("Horizontal");
		if(moveDirection.x != 0)
		{
			inputMove = true;
//			Debug.Log("moveDirection= " + moveDirection.x);
//			Vector3 temp = transform.position + new Vector3(moveDirection.x,0,0);
//			gameObject.transform.position = temp;

		}
		else
		{
			inputMove = false;
//			Vector3 temp = transform.position + new Vector3(moveDirection.x,0,0);
//			gameObject.transform.position = temp;
		}

		/* Jump Keyboard */
		//moveDirection.y =  Input.GetAxis("Vertical");
		if (Input.GetKeyDown (KeyCode.Space))
			inputJump = true;
		else
			inputJump = false;

		/* Jump Mouse (and Touch) */
		//Achtung: gilt auf für Touch!
		//		if (Input.GetMouseButtonDown (0)) {
		//			Debug.Log ("Achtung bei Touch!!!");
		//			inputJump = true;
		//		}
	}
	void InputTouchCheck() 
	{
		buttonIsPressed=false;
		stickIsPressed=false;
		foreach(Touch touch in Input.touches)
		{
			if(!buttonIsPressed)
			{
				//noch kein Finger in linker Bildschirmhälfte gefunden
				if( touch.position.x > (Screen.width * 0.5) )
				{
					buttonIsPressed = true;
				}
			}
			if(!stickIsPressed)
			{
				//noch kein Finger in linker Bildschirmhälfte gefunden
				if( touch.position.x < (Screen.width * 0.5) )
				{
					deltaX += touch.deltaPosition.x/rangeX;
					if(deltaX > 1.0f)
						deltaX = 1.0f;
					else if(deltaX < -1.0f)
						deltaX = -1.0f;
					
					stickIsPressed = true;
				}
			}
		}
		if(!stickIsPressed) {
			deltaX = 0.0F;
		}
		inputTouchJump=buttonIsPressed;

//		if(Input.touchCount > 0)
//		{
//			/* mindestens ein Finger auf dem Display */
//			if(leftStick.isPressed)
//			{
//				deltaX = leftStick.deltaX;
//			}
//			else
//				deltaX=0;
//			
//			inputTouchJump=buttonA.isPressed;
//		}
//		else
//		{
//			/* kein Finger auf dem Display */
//			deltaX=0;
//			inputTouchJump=false;
//		}

	}

	// Update is called once per frame
	void FixedUpdate () {
		player = GameObject.FindWithTag("Player").transform;
		anim = player.GetComponent<Animator>();
//		if(player != null) {
//			Debug.Log("Player found");
//			Vector3 temp = player.position + new Vector3(0,1,0);
//			if(inputJump) {
//				player.position = temp;
//				player.rigidbody2D.velocity = new Vector2(3,3);
//			}
//		}
//		else {
//			Debug.Log("Player NOT found!");
//		}

		if(player != null)
		{
			if(!isDead)
			{
				FixCheckPosition();
				FixSetAnim();
				FixMove();							//Jump, Wall-Jump, rechts, links Bewegung					
				JumpAblePlatform();
			}
		}

	}
	void FixCheckPosition()
	{
		Vector2 playerPos = new Vector2(player.position.x, player.position.y);

		//Boden unter den Füßen
		grounded = Physics2D.OverlapCircle (playerPos+groundCheckPosition, groundRadius, whatIsGround);

		//Gesicht an Wand (nur Gesicht, kein Rücken!)
		walled = Physics2D.OverlapCircle (playerPos+wallCheckPosition, wallRadius, whatIsWall);
//		grounded = true;
//		walled= false;
	}
	void FixSetAnim() 
	{
		if(anim != null)
		{
			anim.SetBool ("Ground", grounded);
			anim.SetBool ("Wall", walled);
			anim.SetFloat ("vSpeed", player.rigidbody2D.velocity.y);
		}
		else
			Debug.LogError("Animator not set");

	}
	void FixMove()
	{
		//rigidbody2D.velocity = new Vector2 (moveDirection.x * maxSpeed, rigidbody2D.velocity.y);
		//Alte Kraft in X Richtung wird ignoriert!
		//velocity enthält alte Kraft -/+
		velocity = (moveDirection.x + deltaX) * maxSpeed;
		//velocity = rigidbody2D.velocity.x + (moveDirection.x + deltaX) * maxSpeed;				//schwammig!!!! bei Flip() Kraftrichtung auch wechseln
		if(anim != null)
		{
			anim.SetFloat("Speed", Mathf.Abs (velocity));
		}
		else
			Debug.LogError("Animator not set");
		//abs für beide Richtungen!!! richtung behalten!!
		if(Mathf.Abs(velocity) < maxSpeed)
		{
			//rigidbody2D.AddForce( new Vector2 (velocity,0));
			player.rigidbody2D.velocity = new Vector2 (velocity, player.rigidbody2D.velocity.y);
		}
		else 
		{
			//rigidbody2D.AddForce( new Vector2 ((moveDirection.x + deltaX)*maxSpeed,0));
			player.rigidbody2D.velocity = new Vector2 ((moveDirection.x + deltaX)*maxSpeed, player.rigidbody2D.velocity.y);
		}
		if (velocity > 0 && !facingRight)
		{
			Flip ();
		}
		else if (velocity < 0 && facingRight)
		{
			Flip ();
		}

		/* mit CharacterController
		//		moveDirection.y -= gravity; // nicht nötig, Physic2D!
		//		if(characterController.isGrounded) 
		//		{
		//			if(inputJump)
		//			{
		//				characterController.Move(moveDirection * Time.deltaTime);
		//			}
		//		}
		*/
		
		if (grounded && (inputJump || inputTouchJump)) {
			//Springen
			AudioSource.PlayClipAtPoint(jumpSound,transform.position,1);				//Jump
			anim.SetBool("Ground",false);
			player.rigidbody2D.velocity = new Vector2(0.0F,jumpForce.y);								//<--- besser für JumpAblePlatforms
			//rigidbody2D.fixedAngle = false;
			//rigidbody2D.AddTorque(10);
			//rigidbody2D.AddForce(new Vector2(0.0F, jumpForce.y));
			//ForceJumpAblePlatform();
		}
		else if (!grounded && walled && (inputJump || inputTouchJump)) {
			//von Wand wegspringen
			AudioSource.PlayClipAtPoint(wallJumpSound,transform.position,1);				//WallJump
			player.rigidbody2D.velocity = new Vector2(0,0);		//alte Geschwindigkeit entfernen
			Flip ();										//Charakter drehen 
			anim.SetBool("Wall",false);
			//rigidbody2D.velocity = jumpForce;
//			rigidbody2D.AddForce(new Vector2(300, 300));
//			rigidbody2D.AddForce(jumpForce);
			//rigidbody2D.AddForce(new Vector2((transform.localScale.x)*jumpForce.x, jumpForce.y)); //Kraft in Richtung localScale.x anwenden
			player.rigidbody2D.velocity = new Vector2((player.transform.localScale.x)*jumpForce.x, jumpForce.y);								//<--- besser für JumpAblePlatforms
		}
	}

	void StartJump() {
		if(JumpAllowed)
			inputJump = true;
	}

	void StopJump() {
		inputJump = false;
	}


	void Flip() {
		// Drift sound abspielen
		if(grounded)
			AudioSource.PlayClipAtPoint(changeRunDirectionSound,transform.position,1);				//ChangeDirection

		// Richtungvariable anpassen
		facingRight = !facingRight;

		// WallCheck anpassen
		wallCheckPosition *= -1;

		// Transform spiegeln
		Vector3 theScale = player.transform.localScale;
		theScale.x *= -1;
		player.transform.localScale = theScale;

	}

	/* Collider2D
	void OnTriggerEnter2D(Collider2D other) {
//		Debug.LogError(other.gameObject.tag);
		if(other.gameObject.tag == "Enemy")
		{
			other.gameObject.SetActive(false);
		}
		if(other.gameObject.tag == "Dead")
		{

		}
		if(other.gameObject.tag == "PlatformTrigger")
		{
			Debug.LogError("Collision Off");
			//other.gameObject.collider2D.enabled = false;
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
			                               LayerMask.NameToLayer("JumpAblePlatform"),
			                               rigidbody2D.velocity.y > 0);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.tag == "PlatformTrigger")
		{
			Debug.LogError("Collision On");
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
			                               LayerMask.NameToLayer("JumpAblePlatform"),
			                               false);
		}
	}
*/
	void JumpAblePlatform()
	{
		if(player)
		{
			if(player.rigidbody2D.velocity.y >0.0F)
			{
				Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),true);
				//Physics2D.IgnoreCollision(platform.collider2D, collider2D,true);
			}
			else
				Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),false);
			//Physics2D.IgnoreCollision(platform.collider2D, collider2D,false);
		}
	}

	void ForceJumpAblePlatform()
	{
		Debug.Log("Force Jump-Able-Platform");
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpAblePlatform"),LayerMask.NameToLayer("Player"),true);
	}

}
