using UnityEngine;
using System.Collections;

public class RageModus : MonoBehaviour {

	private float rageTime = 3*3.202f;
	public bool isInRageModus = false;
	private float oldMaxSpeed;

	private GameObject invincibleSound;

	Color[] rageAnimationColors;
	int currentAnimColorIndex = 0;

	private float rageMaxSpeed;
	/** 
	 * Character Animation 
	 **/
	//	private SpriteController spriteController;
//	public Animator anim;									// Animator State Machine

	/** 
	 * Connection to GameController 
	 **/
	private GameObject gameController;
	private HashID hash;
	private Layer layer;
	private StatsManager statsManager;

	/**
	 * Connection to other Body parts
	 **/
	private Collider2D bodyCollider2D;
	private Collider2D headCollider2D;
	private Collider2D feetCollider2D;

	private PlatformCharacter myPlatformCharacter;
	private SpriteRenderer mySpriteRenderer;
	void InitRageAnimation()
	{
		rageAnimationColors = new Color[6];

//		for(int i=0; i<rageAnimationColors.Length; i++)
//		{
//			rageAnimationColors[i].r = 1;
//			rageAnimationColors[i].g = 0;
//			rageAnimationColors[i].b = 0;
//			rageAnimationColors[i].a = 1;
//		}
		int i=0;
		rageAnimationColors[i] = new Color(1f, 0f, 0f, 1f);
//		rageAnimationColors[i].r = 1;
//		rageAnimationColors[i].g = 0;
//		rageAnimationColors[i].b = 0;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(1f, 1f, 0f, 1f);
//		rageAnimationColors[i].r = 1;
//		rageAnimationColors[i].g = 1;
//		rageAnimationColors[i].b = 0;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(0f, 1f, 0f, 1f);
//		rageAnimationColors[i].r = 0;
//		rageAnimationColors[i].g = 1;
//		rageAnimationColors[i].b = 0;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(0f, 1f, 1f, 1f);
//		rageAnimationColors[i].r = 0;
//		rageAnimationColors[i].g = 1;
//		rageAnimationColors[i].b = 1;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(0f, 0f, 1f, 1f);
//		rageAnimationColors[i].r = 0;
//		rageAnimationColors[i].g = 0;
//		rageAnimationColors[i].b = 1;
//		rageAnimationColors[i].a = 1;
		i++;
		rageAnimationColors[i] = new Color(1f, 1f, 1f, 1f);
//		rageAnimationColors[i].r = 1;
//		rageAnimationColors[i].g = 1;
//		rageAnimationColors[i].b = 1;
//		rageAnimationColors[i].a = 1;
//		rageAnimationColors[1] = Color.yellow;
//		rageAnimationColors[2] = Color.green;
//		rageAnimationColors[3] = Color.cyan;
//		rageAnimationColors[4] = Color.blue;
//		rageAnimationColors[5] = Color.white;
	}

	void Awake()
	{
		invincibleSound = GameObject.FindGameObjectWithTag(Tags.invincibleSound);
		InitRageAnimation();
		mySpriteRenderer = GetComponent<SpriteRenderer>();
		myPlatformCharacter = GetComponent<PlatformCharacter>();
//		anim = GetComponent<Animator>();
		bodyCollider2D = GetComponent<BoxCollider2D>();
		headCollider2D = transform.FindChild(Tags.head).GetComponent<BoxCollider2D>();
		feetCollider2D = transform.FindChild(Tags.feet).GetComponent<BoxCollider2D>();
		
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		layer = gameController.GetComponent<Layer>();
		statsManager = gameController.GetComponent<StatsManager>();
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void LateUpdate () {
		if(isInRageModus)
		{
//			Debug.Log("currentAnimColor:" + currentAnimColor);
//			Debug.Log("SpriteRenderer Color:" + mySpriteRenderer.color);
			currentAnimColorIndex = currentAnimColorIndex % rageAnimationColors.Length;
			mySpriteRenderer.color = rageAnimationColors[currentAnimColorIndex];
//			Debug.Log("new SpriteRenderer Color:" + mySpriteRenderer.color);
			currentAnimColorIndex++;

		}
	}

	void playSound()
	{
		if(invincibleSound != null)
		{
			gameController.audio.Stop();
			invincibleSound.audio.Play();
		}
	}

	void stopSound()
	{
		if(invincibleSound != null)
		{
			invincibleSound.audio.Stop();
			gameController.audio.Play();
		}
	}

	public void startRageModus()
	{
		oldMaxSpeed = myPlatformCharacter.getMaxSpeed();
		rageMaxSpeed = oldMaxSpeed * 1.2f;
		myPlatformCharacter.setMaxSpeed(rageMaxSpeed);
		headCollider2D.enabled = false;
		feetCollider2D.enabled = false;

		playSound();

//		bodyCollider2D.isTrigger = false;

//		disableCollision();
		
		isInRageModus = true;
		myPlatformCharacter.isInRageModus = true;
//		if(anim != null)
//		{
//			anim.SetBool(hash.rageModusBool,true);
//			anim.SetTrigger(hash.rageTrigger);
//		}
		Debug.LogError(gameObject.name + "isInRageModus: On");
		//		InventoryManager.inventory.SetItems("Star(Clone)",0f);
		StartCoroutine(RageTime());
	}

	IEnumerator RageTime()
	{
		yield return new WaitForSeconds(rageTime);
		stopRageModus();
	}

	public void stopRageModus()
	{
		Debug.LogError(gameObject.name + "isInRageModus: Off");
//		if(anim != null)
//		{
//			anim.SetBool(hash.rageModusBool,false);
//		}
		mySpriteRenderer.color = Color.white;
//		enableCollision();

		stopSound();

		isInRageModus = false;
		myPlatformCharacter.isInRageModus = false;
		headCollider2D.enabled = true;
		feetCollider2D.enabled = true;
		myPlatformCharacter.setMaxSpeed(oldMaxSpeed);
//		bodyCollider2D.isTrigger = false;
		
		//anim.SetBool(hash.hasPowerUpBool,hasPowerUp);
		//AudioSource.PlayClipAtPoint(powerUpReloadedSound,transform.position,1);
	}

	void disableCollision()
	{
		int currentLayer = gameObject.layer;
		for(int i=0; i<4; i++)
		{
			if(i==0)
				currentLayer = layer.player1;
			if(i==1)
				currentLayer = layer.player2;
			if(i==2)
				currentLayer = layer.player3;
			if(i==3)
				currentLayer = layer.player4;
			
			if(gameObject.layer != currentLayer)
				Physics2D.IgnoreLayerCollision(gameObject.layer,currentLayer,true);
		}
	}

	void enableCollision()
	{
		int currentLayer = gameObject.layer;
		for(int i=0; i<4; i++)
		{
			if(i==0)
				currentLayer = layer.player1;
			if(i==1)
				currentLayer = layer.player2;
			if(i==2)
				currentLayer = layer.player3;
			if(i==3)
				currentLayer = layer.player4;
			
			if(gameObject.layer != currentLayer)
				Physics2D.IgnoreLayerCollision(gameObject.layer,currentLayer,false);
		}
	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		// Network.peerType == PeerState.Disconnected
		if(Network.isServer || networkView == null)
		{
			if(isInRageModus)
			{
				if(this.gameObject.layer != collision.gameObject.layer)										// Spieler aus eigenem Team(layer) nicht zerstören
				{
					bool enemyObject = false;
					if(collision.gameObject.layer == layer.player1)
					{
						enemyObject = true;
					}
					else if(collision.gameObject.layer == layer.player2)
					{
						enemyObject = true;
					}
					else if(collision.gameObject.layer == layer.player3)
					{
						enemyObject = true;
					}
					else if(collision.gameObject.layer == layer.player4)
					{
						enemyObject = true;
					}
					//				else if(other.gameObject.layer == layer.powerUp)
					//				{
					//					enemyObject = true;
					//				}
					
					if(enemyObject)
					{
						//networkView.RPC
						//other.gameObject.GetComponent<NetworkView>().RPC(
						statsManager.InvincibleAttack(this.gameObject, collision.gameObject);			// Layerfilter -> wir sind auf PlatformCharacter ebene (nicht im child feet/head)
						//other.gameObject.GetComponent<HealthController>().ApplyDamage(this.gameObject, 1 ,true);
					}
				}
			}
		}
	}

}
