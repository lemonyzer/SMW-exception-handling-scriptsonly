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

	public float rigidbody2DvelocityX = 0f;
	public float rigidbody2DvelocityY = 0f;
	public float userInputvelocityX = 0f;
	public float userInputvelocityY = 0f;


	public bool invincible = false;

	public List<Sprite> animations;
	public List<Sprite> animationIdle;
	public List<Sprite> animationRun;
	public List<Sprite> animationJump;
	public List<Sprite> animationChangeRunDirection;
	public List<Sprite> animationDie;
	public List<Sprite> animationDead;
	public List<Sprite> animationSpawn;

	public float animationSpeed = 10;

	private AnimationType currentAnimationType = AnimationType.idle;

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
					setAnimation(AnimationType.headJumped);
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
						setAnimation(AnimationType.spawn);
						if(spawnProtection)
						{
							setAnimation(AnimationType.spawnProtection);
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
		int index = (int)(Time.time * animationSpeed);
		index = index % animationSprite.Count;
		spriteRenderer.sprite = animationSprite[index];
	}

	public void setAnimation(AnimationType animationType)
	{
		currentAnimationType = animationType;
	}
}
