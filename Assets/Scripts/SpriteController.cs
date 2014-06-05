using UnityEngine;
using System.Collections;
using System.Collections.Generic;			//für Liste

public class SpriteController : MonoBehaviour {

	public bool LeftRightMirrow=true;
	public List<Texture2D> animationIdleRight;
	public List<Texture2D> animationIdleLeft;
	public List<Texture2D> animationGoRight;
	public List<Texture2D> animationGoLeft;
	public List<Texture2D> animationJumpRight;
	public List<Texture2D> animationJumpLeft;
	public List<Texture2D> animationAttackRight;
	public List<Texture2D> animationAttackLeft;
	public float speed = 1;
	public AnimationType currentAnimationType = AnimationType.idleRight;

	public enum AnimationType {
		idleRight,
		idleLeft,
		goRight,
		goLeft,
		jumpRight,
		jumpLeft,
		attackRight,
		attackLeft,
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentAnimationType)
		{
			case AnimationType.idleRight:
				setTexture(animationIdleRight);
				break;
			case AnimationType.idleLeft:
				setTexture(animationIdleLeft);
				break;
			case AnimationType.goRight:
				setTexture(animationGoRight);
				break;
			case AnimationType.goLeft:
				setTexture(animationGoLeft);
				break;
			case AnimationType.jumpRight:
				setTexture(animationJumpRight);
				break;
			case AnimationType.jumpLeft:
				setTexture(animationJumpLeft);
				break;
			case AnimationType.attackRight:
				setTexture(animationAttackRight);
				break;
			case AnimationType.attackLeft:
				setTexture(animationAttackLeft);
				break;
		}

	}

	void setTexture(List<Texture2D> animationSprite)
	{
		int index = (int)(Time.time * speed);
		index = index % animationSprite.Count;
		renderer.material.mainTexture = animationSprite[index];
	}

	public void setAnimation(AnimationType animationType)
	{
		currentAnimationType = animationType;
	}
}
