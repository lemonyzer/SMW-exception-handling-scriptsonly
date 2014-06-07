﻿using UnityEngine;
using System.Collections;

public class HashID : MonoBehaviour {

	public int hittedState;
	public int deadState;
	public int dieState;
	public int headJumpedState;
	public int spawnState;
	public int spawnProtectionState;
	public int idleState;
	public int runState;
	public int changeRunDirectionState;
	public int jumpState;

	public int speedFloat;
	public int hSpeedFloat;
	public int vSpeedFloat;
	public int groundBool;
	public int groundedBool;
	public int wallBool;
	public int walledBool;
	public int hittedBool;
	public int gameOverBool;
	public int headJumpedBool;
	public int spawnBool;
	public int deadBool;
	public int hitTrigger;
	public int spawnProtectionBool;
	
	void Awake() {
		hittedState =			Animator.StringToHash("Base Layer.Hitted");
		deadState = 			Animator.StringToHash("Base Layer.Dead");
		dieState = 				Animator.StringToHash("Base Layer.Die");
		headJumpedState = 		Animator.StringToHash("Base Layer.HeadJumped");
		spawnState = 			Animator.StringToHash("Base Layer.Spawn");
		spawnProtectionState = 	Animator.StringToHash("Base Layer.SpawnProtection");
		idleState = 			Animator.StringToHash("Base Layer.Idle");
		runState = 				Animator.StringToHash("Base Layer.Run");
		changeRunDirectionState = Animator.StringToHash("Base Layer.ChangeRunDirection");
		jumpState = 			Animator.StringToHash("Base Layer.Jump");

		speedFloat = Animator.StringToHash("Speed");				//duplicates
		hSpeedFloat = Animator.StringToHash("hSpeed");				//duplicates

		vSpeedFloat = Animator.StringToHash("vSpeed");

		groundBool = Animator.StringToHash("Ground");				//duplicates
		groundedBool = Animator.StringToHash("Grounded");			//duplicates

		wallBool = Animator.StringToHash("Wall");					//duplicates
		walledBool = Animator.StringToHash("Walled");				//duplicates

		hittedBool = Animator.StringToHash("Hitted");
		gameOverBool = Animator.StringToHash("GameOver");
		headJumpedBool = Animator.StringToHash("HeadJumped");
		spawnBool = Animator.StringToHash("Spawn");
		deadBool = Animator.StringToHash("Dead");
		hitTrigger = Animator.StringToHash("HitTrigger");
		spawnProtectionBool = Animator.StringToHash("SpawnProtection");
	}
}
