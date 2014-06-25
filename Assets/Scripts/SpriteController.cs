using UnityEngine;
using System.Collections;
using System.Collections.Generic;			//für Liste

public class SpriteController : MonoBehaviour {

	public bool LeftRightMirrow=true;				// nur eine Textur für beide Richtungen

	public List<Sprite> animations;
	public List<Sprite> animationIdle;
	public List<Sprite> animationRun;
	public List<Sprite> animationJump;
	public List<Sprite> animationChangeRunDirection;
	public List<Sprite> animationDie;
	public List<Sprite> animationDeadCorp;
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
		animationDeadCorp.Add(animations[5]);
	}

	public enum AnimationType {
		idle,
		run,
		changeRunDirection,
		jump

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentAnimationType)
		{
			case AnimationType.idle:
				setTexture(animationIdle);
				break;
			
			case AnimationType.run:
				setTexture(animationRun);
				break;

			case AnimationType.changeRunDirection:
				setTexture(animationChangeRunDirection);
				break;

			case AnimationType.jump:
				setTexture(animationJump);
				break;

		}

	}

	void setTexture(List<Sprite> animationSprite)
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
