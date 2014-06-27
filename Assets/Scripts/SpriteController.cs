using UnityEngine;
using System.Collections;
using System.Collections.Generic;			//für Liste

public class SpriteController : MonoBehaviour {

	private bool LeftRightMirrow=true;				// nur eine Textur für beide Richtungen


	public bool hittedTrigger = false;
	public bool headJumped = false;
	public bool dead = false;
	public bool spawn = false;
	public bool spawnProtection = false;
	public bool gameOver = false;
	public bool grounded = false;
	public bool walled = false;
	public bool changeRunDirectionTrigger = false;

	public bool invincible = false;
	
	public bool currentAnimationStarted = false;
	public bool currentAnimationCompletedOnce = false;
	public bool currentAnimationComplete = false;
	public bool currentAnimationAtLastFrame = false; 
	public float animationSpeed = 10;
	public bool animationStopAble = false;

	public float rigidbody2DvelocityX = 0f;
	public float rigidbody2DvelocityY = 0f;
	public float userInputvelocityX = 0f;
	public float userInputvelocityY = 0f;

	public List<Sprite> animations;
	public List<Sprite> animationIdle;
	public List<Sprite> animationRun;
	public List<Sprite> animationJump;
	public List<Sprite> animationChangeRunDirection;
	public List<Sprite> animationDie;
	public List<Sprite> animationDead;
	public List<Sprite> animationSpawn;



	private AnimationType currentAnimationType = AnimationType.idle;
	private AnimationType oldAnimationType = AnimationType.idle;

	private float myTime = 0f;

	private SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		// Idle
		animationIdle.Add(animations[0]);

		// Run
		animationRun.Add(animations[0]);
		animationRun.Add(animations[1]);

		// Jump
		animationJump.Add(animations[2]);

		// ChangeRunDirection (Skid, Drift)
		animationChangeRunDirection.Add(animations[3]);

		// Die
		animationDie.Add(animations[4]);

		// DeadCrop
		animationDead.Add(animations[5]);
	}


	public enum AnimationType {
		idle,
		run,
		changeRunDirection,
		jump,
		invincible,
		headJumped,
		die,
		dead,
		spawn,
		spawnProtection,
		gameOver
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		SelectCurrentAnimation();
		StartCurrentAnimation();
	}

	private void SelectCurrentAnimation()
	{
		if(hittedTrigger)
		{
			//hittedTrigger = false;
			if(gameOver)
			{
				setAnimation(AnimationType.gameOver);
			}
			else
			{
				if(headJumped)
				{
					setAnimation(AnimationType.headJumped);		//instantiate deadPrefab...
				}
				// !gameOver
				if(dead)
				{
					setAnimation(AnimationType.dead);
				}
				else
				{
					// !dead
					if(spawn)
					{
//						currentAnimationAtLastFrame = false;
						setAnimation(AnimationType.spawn);
						if(currentAnimationCompletedOnce)
						{
							spawnProtection = true;
						}
						if(spawnProtection)
						{
							setAnimation(AnimationType.jump);		// 50% transparenz auf alles :D
						}
					}
				}
			}
		}
		else
		{
			if(!grounded)
			{
				// am Fallen oder Springen
				setAnimation(AnimationType.jump);
			}
			else
			{
				if(changeRunDirectionTrigger)
				{
					//changeRunDirectionTrigger = false;
					setAnimation(AnimationType.changeRunDirection);
					if(currentAnimationCompletedOnce)
					{
						changeRunDirectionTrigger = false;
					}
				}
				// grounded
				if(Mathf.Abs(rigidbody2DvelocityX) > 0.1f)
				{
					setAnimation(AnimationType.run);
				}
				else
				{
					setAnimation(AnimationType.idle);
				}
			}
		}
	}

	private void StartCurrentAnimation()
	{
		switch(currentAnimationType)
		{
		case AnimationType.idle:
			setSprite(animationIdle);
			break;
			
		case AnimationType.run:
			setSprite(animationRun);
			break;
			
		case AnimationType.changeRunDirection:
			animationStopAble = false;
			setSprite(animationChangeRunDirection);
			break;
			
		case AnimationType.jump:
			setSprite(animationJump);
			break;

		case AnimationType.headJumped:
			setSprite(animationDead);
			break;

		case AnimationType.gameOver:
			setSprite(animationDie);
			break;

		case AnimationType.spawn:
			setSprite(animationSpawn);
			break;
		}
	}

	void setSprite(List<Sprite> animationSprite)
	{
		float framesPerSecond = 1.0f/Time.maximumDeltaTime;
		float minFrames = (animationSprite.Count * framesPerSecond) / animationSpeed;
		Debug.Log("maxDeltaTime= " + Time.maximumDeltaTime);
		Debug.Log("FramesPerSecond= " + framesPerSecond);
		Debug.Log("minFrames= " + minFrames);
		
		myTime += Time.deltaTime;
		int index = (int)(myTime * animationSpeed);

		if(!animationStopAble)
		{
			if(index < minFrames)
				return;
		}

		

		if(animationSprite.Count != 0)
		{
			index = index % animationSprite.Count;
			Debug.Log("index= " + index + ", myTime= " + myTime);
		}
		else
			Debug.LogError("Current AnimationType: " + currentAnimationType + " Sprite list is empty!");

		spriteRenderer.sprite = animationSprite[index];
		if(!currentAnimationStarted)
		{
			currentAnimationStarted = true;
			currentAnimationAtLastFrame = false;
			if(index == 0)
			{

			}
		}
		else
		{
			// currentAnimation läuft schon
			if(currentAnimationAtLastFrame)
			{
				if(index == 0)
				{
					currentAnimationStarted = false;
					currentAnimationCompletedOnce = true;
//					currentAnimationComplete = true;
				}
			}
			else if(!currentAnimationAtLastFrame && index == animationSprite.Count-1)
			{
				// letzter frame, Animation danach einmal durchgelaufen
				currentAnimationAtLastFrame = true;
				
				if(animationSprite.Count > 1)
				Debug.Log("index " + index + " list.count: " + animationSprite.Count);
			}
//			else
//			{
//				currentAnimationComplete = false;
//			}


		}



	}

	public void setAnimation(AnimationType animationType)
	{
		currentAnimationType = animationType;
		if(oldAnimationType != currentAnimationType)
		{
			oldAnimationType = currentAnimationType;
			myTime = 0f;
			currentAnimationStarted = false;
			currentAnimationAtLastFrame = false;
			currentAnimationComplete = false;
			currentAnimationCompletedOnce = false;
		}

	}
}
