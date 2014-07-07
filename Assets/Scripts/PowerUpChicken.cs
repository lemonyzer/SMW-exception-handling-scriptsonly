using UnityEngine;
using System.Collections;

public class PowerUpChicken : MonoBehaviour {

	bool isChicken = false;
	public Animator chickenAnimator;
	public AudioClip chickenSpawnSound;
	public AudioClip chickenKillSound;
	public AudioClip chickenDieSound;
	public AudioClip chickenJumpSound;


	SpriteRenderer mySpriteRenderer;

	enum AnimationState{
		idleState,
		runState,
		jumpState,
		skidState,
		dieState,
		headJumpedState
	}
	// override PlatformCharacter
	// OR new CharacterPreferencesScript... alle CharacterEigenschaften Animator/AudioClups/CharPower
	// superclass Chararacter


	GameObject gameController;
	HashID hash;
	/**
	 * 
	 *  wenn CharacterAnimator nicht durch chickenAnimator ersetz wird
	 * 
	 **/
	Animator anim;
	void Awake()
	{
		anim = GetComponent<Animator> ();
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
		gameController = GameObject.FindGameObjectWithTag (Tags.gameController);
		hash = gameController.GetComponent<HashID> ();

	}

	void InitChickenAnimation()
	{

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate()
	{
		//if(anim.state == hash.jumpState)		//switch case FUCK u take care of breaks;
		//{
		//		mySpriteRenderer.sprite = chickenAnimation[AnimationStates.jumpState];
		//}
		//checked Animator State

	}
}
