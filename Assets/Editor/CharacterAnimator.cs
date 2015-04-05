﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;

public class CharacterAnimator {

	[System.Serializable]
	public class SMWAnimation
	{
		public string name;
		public int framesPerSecond;	 // sample			// 2013.12.16 Live Training 16 Dec 2013 - 2D Character Controllers (720p).mp4  @@ 26:14
		public int keyFrames;
		public int frameDistance;
		public Sprite[] sprites;
		public AnimatorState animState;
		
		public SMWAnimation(string name, int framesPerSecond, int keyFrames, Sprite[] sprites, AnimatorState animState)
		{
			this.name = name;
			this.framesPerSecond = framesPerSecond;
			this.keyFrames = keyFrames;
			this.animState = animState;
			this.sprites = sprites;
		}
	}

	public static SMWAnimation[] smwAnimations = new SMWAnimation[6];

	public static RuntimeAnimatorController Create (SmwCharacterGenerics smwCharacterGenerics, SmwCharacter smwCharacter)
	{
		string charName = smwCharacter.charName;
		if(charName == "")
		{
			Debug.LogError("smwCharacter hat keinen namen gesetzt!");
			charName = "unnamedChar";
		}
		
		string createdCharacterFolderPath = "Animations/Characters/AutoGenerated/"+charName ;
		if(!CreateFolder(createdCharacterFolderPath))
		{
			Debug.LogError("Ordner existiert/existerieren nicht und kann/können nicht angelegt werden!\n" + createdCharacterFolderPath);
			return null;
		}
		
		/**
		 * 			AssetDatabase :	All paths are relative to the project folder => paths always = "Assets/..../..." //TODO last folder no SLASH / !!!
		 **/
		
		
		//		string assetCreatedCharacterFolderPath = "Assets/" + createdCharacterFolderPath;
		UnityEditor.Animations.AnimatorController controller =  UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath( "Assets/" + createdCharacterFolderPath + "/" + charName + "_scripted_AnimatorController.controller");
		
		// Add parameters
		controller.AddParameter(HashID.p_hSpeed, AnimatorControllerParameterType.Float);
		controller.AddParameter(HashID.p_vSpeed, AnimatorControllerParameterType.Float);
		controller.AddParameter(HashID.p_grounded, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_walled, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_changeRunDirectionTrigger, AnimatorControllerParameterType.Trigger);
		
		controller.AddParameter(HashID.p_hitTrigger, AnimatorControllerParameterType.Trigger);
		controller.AddParameter(HashID.p_nextStateTrigger, AnimatorControllerParameterType.Trigger);
		controller.AddParameter(HashID.p_hitted, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_gameOver, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_headJumped, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_dead, AnimatorControllerParameterType.Bool);
		
		controller.AddParameter(HashID.p_spawn, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_spawnProtection, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_stopSpawnProtectionTrigger, AnimatorControllerParameterType.Trigger);
		
		controller.AddParameter(HashID.p_rageTrigger, AnimatorControllerParameterType.Trigger);
		controller.AddParameter(HashID.p_rageModus, AnimatorControllerParameterType.Bool);
		
		// Layer 0 State Machine
		AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
		
		// Add states
		AnimatorState idleState = controller.layers[0].stateMachine.AddState(HashID.s_Idle);
		//		idleState.motion = idleAnim;
		
		AnimatorState jumpState = controller.layers[0].stateMachine.AddState(HashID.s_JumpAndFall);
		//		jumpState.motion = jumpAnim;
		
		AnimatorState runState = controller.layers[0].stateMachine.AddState(HashID.s_Run);
		//		runState.motion = runAnim;
		
		AnimatorState skidState = controller.layers[0].stateMachine.AddState(HashID.s_ChangeRunDirection);
		//		skidState.motion = changeRunDirectionAnim;
		
		AnimatorState hittedState = controller.layers[0].stateMachine.AddState(HashID.s_Hitted);
		//		hittedState.motion = idleAnim;
		
		AnimatorState headJumpedState = controller.layers[0].stateMachine.AddState(HashID.s_HeadJumped);
		//		headJumpedState.motion = headJumpedAnim;
		
		AnimatorState gameOverState = controller.layers[0].stateMachine.AddState(HashID.s_GameOver);
		//		gameOverState.motion = headJumpedAnim;
		
		AnimatorState deadState = controller.layers[0].stateMachine.AddState(HashID.s_Dead);
		//		deadState.motion = headJumpedAnim;
		
		AnimatorState spawnState = controller.layers[0].stateMachine.AddState(HashID.s_Generic_Spawn);
		//		spawnState.motion = headJumpedAnim;
		
		AnimatorState spawnProtectionState = controller.layers[0].stateMachine.AddState(HashID.s_Generic_SpawnProtection);
		//		spawnProtectionState.motion = headJumpedAnim;
		
		
		/**
		 * Layer 1 - Overlay Layer
		 **/
		
		controller.AddLayer(HashID.l_overlay);
		// Layer 1 State Machine
		controller.layers[1].blendingMode = AnimatorLayerBlendingMode.Additive;			// setzt für die zeit die es aktiv ist die variablen und wenn deaktiviert wird variable auf vorherigen wert gesetzt
		controller.layers[1].defaultWeight = 1f;
		AnimatorStateMachine overlayStateMachine = controller.layers[1].stateMachine;
		
		AnimatorState defaultOverlayState = overlayStateMachine.AddState(HashID.s_l1_defaultState);
		AnimatorState invincibleOverlayState = overlayStateMachine.AddState(HashID.s_l1_Generic_Invincible);
		AnimatorState spawnProtectionOverlayState = overlayStateMachine.AddState(HashID.s_l1_Generic_SpawnProtection);
		
		AnimatorStateTransition leaveInvincibleEnterDefaultState = invincibleOverlayState.AddTransition(defaultOverlayState);
		//		leaveInvincibleEnterDefaultState.AddCondition(AnimatorConditionMode.If, 0, HashID.p_rageTrigger);
		leaveInvincibleEnterDefaultState.duration = 0;
		leaveInvincibleEnterDefaultState.hasExitTime = true;				//TODO achtung byTime!
		leaveInvincibleEnterDefaultState.exitTime = 1f;
		leaveInvincibleEnterDefaultState.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveSpawnProtectionEnterDefaultStateByTime = spawnProtectionOverlayState.AddTransition(defaultOverlayState);
		//		leaveInvincibleEnterDefaultState.AddCondition(AnimatorConditionMode.If, 0, HashID.p_rageTrigger);
		leaveSpawnProtectionEnterDefaultStateByTime.duration = 0;
		leaveSpawnProtectionEnterDefaultStateByTime.hasExitTime = true;			//TODO achtung byTime!
		leaveSpawnProtectionEnterDefaultStateByTime.exitTime = 1f;
		leaveSpawnProtectionEnterDefaultStateByTime.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveSpawnProtectionEnterDefaultStateByTrigger = spawnProtectionOverlayState.AddTransition(defaultOverlayState);
		leaveSpawnProtectionEnterDefaultStateByTrigger.AddCondition(AnimatorConditionMode.If, 0, HashID.p_stopSpawnProtectionTrigger);				//TODO defaultOverlayState muss kräfte invincible/spawnprotection entfernen??
		leaveSpawnProtectionEnterDefaultStateByTrigger.duration = 0;
		leaveSpawnProtectionEnterDefaultStateByTrigger.hasExitTime = false;
		leaveSpawnProtectionEnterDefaultStateByTrigger.exitTime = 1f;
		leaveSpawnProtectionEnterDefaultStateByTrigger.canTransitionToSelf = false;
		
		// Overlay Layer : AnyState Transitions to InvincibleState
		AnimatorStateTransition enterInvincibleOverlayerState = overlayStateMachine.AddAnyStateTransition(invincibleOverlayState);
		enterInvincibleOverlayerState.AddCondition(AnimatorConditionMode.If, 0, HashID.p_rageTrigger);
		enterInvincibleOverlayerState.duration = 0;
		enterInvincibleOverlayerState.hasExitTime = false;
		enterInvincibleOverlayerState.exitTime = 1f;
		enterInvincibleOverlayerState.canTransitionToSelf = false;
		
		
		// Overlay Layer : AnyState Transitions to ProtectionState
		AnimatorStateTransition enterSpawnProtectionOverlayerState = overlayStateMachine.AddAnyStateTransition(spawnProtectionOverlayState);	//TODO rename SpawnProtection to Protection
		enterInvincibleOverlayerState.AddCondition(AnimatorConditionMode.If, 0, HashID.p_spawnProtection);
		enterInvincibleOverlayerState.duration = 0;
		enterInvincibleOverlayerState.hasExitTime = false;
		enterInvincibleOverlayerState.exitTime = 1f;
		enterInvincibleOverlayerState.canTransitionToSelf = false;
		
		
		
		// State Transition Setup
		
		// Layer 0 - Base Layer
		
		float minHorizontalSpeed = 0.01f;	// setze schwellwert (treshold)
		
		AnimatorStateTransition leaveIdleEnterRun = idleState.AddTransition(runState);
		leaveIdleEnterRun.AddCondition(AnimatorConditionMode.Greater, minHorizontalSpeed, HashID.p_hSpeed);
		leaveIdleEnterRun.duration = 0;
		leaveIdleEnterRun.hasExitTime = false;
		leaveIdleEnterRun.exitTime = 1f;
		leaveIdleEnterRun.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveIdleEnterRun2 = idleState.AddTransition(runState);
		leaveIdleEnterRun2.AddCondition(AnimatorConditionMode.Less, -minHorizontalSpeed, HashID.p_hSpeed);
		leaveIdleEnterRun2.duration = 0;
		leaveIdleEnterRun2.hasExitTime = false;
		leaveIdleEnterRun2.exitTime = 1f;
		leaveIdleEnterRun2.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveRunEnterIdle = runState.AddTransition(idleState);
		leaveRunEnterIdle.AddCondition(AnimatorConditionMode.Greater, -minHorizontalSpeed, HashID.p_hSpeed);
		leaveRunEnterIdle.AddCondition(AnimatorConditionMode.Less, minHorizontalSpeed, HashID.p_hSpeed);
		leaveRunEnterIdle.duration = 0;
		leaveRunEnterIdle.hasExitTime = false;
		leaveRunEnterIdle.exitTime = 1f;
		leaveRunEnterIdle.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveIdleEnterJump = idleState.AddTransition(jumpState);
		leaveIdleEnterJump.AddCondition(AnimatorConditionMode.IfNot, 0, HashID.p_grounded);
		leaveIdleEnterJump.duration = 0;
		leaveIdleEnterJump.hasExitTime = false;
		leaveIdleEnterJump.exitTime = 1f;
		leaveIdleEnterJump.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveJumpEnterIdle = jumpState.AddTransition(idleState);
		leaveJumpEnterIdle.AddCondition(AnimatorConditionMode.If, 0, HashID.p_grounded);
		leaveJumpEnterIdle.duration = 0;
		leaveJumpEnterIdle.hasExitTime = false;
		leaveJumpEnterIdle.exitTime = 1f;
		leaveJumpEnterIdle.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveRunEnterSkid = runState.AddTransition(skidState);
		leaveRunEnterSkid.AddCondition(AnimatorConditionMode.If, 0, HashID.p_changeRunDirectionTrigger);
		leaveRunEnterSkid.duration = 0;
		leaveRunEnterSkid.hasExitTime = false;
		leaveRunEnterSkid.exitTime = 1f;
		leaveRunEnterSkid.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveSkidEnterRun = skidState.AddTransition(runState);
		//leaveSkidEnterRun.AddCondition(AnimatorConditionMode.If, 0, HashID.p_changeRunDirectionTrigger);
		leaveSkidEnterRun.duration = 0;
		leaveSkidEnterRun.hasExitTime = true;			//TODO achtung byTime!
		leaveSkidEnterRun.exitTime = 1f;
		leaveSkidEnterRun.canTransitionToSelf = false;
		
		// Any State Transistion
		AnimatorStateTransition hittedTransition = rootStateMachine.AddAnyStateTransition(hittedState);	//special TODO markt
		hittedTransition.AddCondition(AnimatorConditionMode.If, 0, HashID.p_hitTrigger);
		hittedTransition.duration = 0;
		hittedTransition.hasExitTime = false;
		hittedTransition.exitTime = 1f;
		hittedTransition.canTransitionToSelf = false;
		
		
		AnimatorStateTransition leaveHittedEnterHeadJumped = hittedState.AddTransition(headJumpedState);
		leaveHittedEnterHeadJumped.AddCondition(AnimatorConditionMode.If, 0, HashID.p_headJumped);		// TODO <-- change to Trigger? p_headJumpedTrigger
		leaveHittedEnterHeadJumped.duration = 0;
		leaveHittedEnterHeadJumped.hasExitTime = false;
		leaveHittedEnterHeadJumped.exitTime = 1f;
		leaveHittedEnterHeadJumped.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveHittedEnterDie = hittedState.AddTransition(deadState);
		leaveHittedEnterDie.AddCondition(AnimatorConditionMode.If, 0, HashID.p_dead);		// TODO <-- change to name p_dieTrigger
		leaveHittedEnterDie.duration = 0;
		leaveHittedEnterDie.hasExitTime = false;
		leaveHittedEnterDie.exitTime = 1f;
		leaveHittedEnterDie.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveHittedEnterGameOver = hittedState.AddTransition(gameOverState);
		leaveHittedEnterGameOver.AddCondition(AnimatorConditionMode.If, 0, HashID.p_gameOver);		// TODO <-- change to name p_gameOverTrigger
		leaveHittedEnterGameOver.duration = 0;
		leaveHittedEnterGameOver.hasExitTime = false;
		leaveHittedEnterGameOver.exitTime = 1f;
		leaveHittedEnterGameOver.canTransitionToSelf = false;
		
		
		AnimatorStateTransition leaveHeadJumpedEnterSpawn = headJumpedState.AddTransition(spawnState);
		leaveHeadJumpedEnterSpawn.AddCondition(AnimatorConditionMode.If, 0, HashID.p_spawn);		// TODO <-- change to name p_spawnTrigger
		leaveHeadJumpedEnterSpawn.duration = 0;
		leaveHeadJumpedEnterSpawn.hasExitTime = false;
		leaveHeadJumpedEnterSpawn.exitTime = 1f;
		leaveHeadJumpedEnterSpawn.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveDieEnterSpawn = deadState.AddTransition(spawnState);
		leaveDieEnterSpawn.AddCondition(AnimatorConditionMode.If, 0, HashID.p_spawn);		// TODO <-- change to name p_spawnTrigger
		leaveDieEnterSpawn.duration = 0;
		leaveDieEnterSpawn.hasExitTime = false;
		leaveDieEnterSpawn.exitTime = 1f;
		leaveDieEnterSpawn.canTransitionToSelf = false;
		
		AnimatorStateTransition leaveSpawnEnterIdle = spawnState.AddTransition(idleState);
		//leaveSpawnEnterIdle.AddCondition(AnimatorConditionMode.If, 0, HashID.p_spawn);		//TODO add condition to enable controlls & enable gravity & & ...
		leaveSpawnEnterIdle.duration = 0;
		leaveSpawnEnterIdle.hasExitTime = true;				//TODO achtung byTime!	//TODO <-- Achtung 	hasExitTime (nach Animation)
		leaveSpawnEnterIdle.exitTime = 1f;
		leaveSpawnEnterIdle.canTransitionToSelf = false;
		
		
		
		// init smwAnimations array
		
		int baseLayerStateCount = 0;
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"_dynamic_Idle",			1,1,	smwCharacter.charIdleSprites,idleState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"_dynamic_Run",			25,2,	smwCharacter.charRunSprites,runState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"_dynamic_Jump",			1,1,	smwCharacter.charJumpSprites,jumpState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"_dynamic_Skid",			1,1,	smwCharacter.charSkidSprites,skidState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"_dynamic_Die",			1,1,	smwCharacter.charDieSprites,deadState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"_dynamic_HeadJumped",	1,1,	smwCharacter.charHeadJumpedSprites,headJumpedState);
		

		for(int i=0; i < baseLayerStateCount; i++)
		{
			// AnimationClip
			AnimationClip tempAnimClip = new AnimationClip();
			#if !UNITY_5
			// Setting it as generic allows you to use the animation clip in the animation controller
			AnimationUtility.SetAnimationType(tempAnimClip, ModelImporterAnimationType.Generic);
			#endif
			tempAnimClip.name = smwAnimations[i].name;
			CreateAnimationClip(tempAnimClip, smwAnimations[i].sprites, smwAnimations[i].keyFrames, 1.0f/smwAnimations[i].framesPerSecond);
			smwAnimations[i].animState.motion = tempAnimClip;
			
			
			tempAnimClip.EnsureQuaternionContinuity();
			if (AssetDatabase.Contains(tempAnimClip))
			{
				Debug.LogError(tempAnimClip.name + " in AssetDatabase bereits enthalten, darf nicht erscheinen");
			}
			else
			{
				//					Debug.Log(tempAnimClip.name + " nicht in AssetDatabase vorhanden, wird jetzt gespeichert.");
				
				// asset anlegen
				//					Debug.Log("Versuche " + tempAnimClip.name + " in Ordner " + "Assets/"+createdCharacterFolderPath + " zu speicheren");
				AssetDatabase.CreateAsset(tempAnimClip, "Assets/"+createdCharacterFolderPath + "/" + tempAnimClip.name + ".asset");
			}
		}
		AssetDatabase.SaveAssets();

		//TODO:: add Generic AnimationClips to characterAnimatorController
		spawnState.motion = smwCharacterGenerics.spawnAnimClip;
		spawnProtectionOverlayState.motion = smwCharacterGenerics.protectionAnimClip;
		invincibleOverlayState.motion = smwCharacterGenerics.rageAnimClip;
		//TODO

		smwCharacter.runtimeAnimatorController = controller;
		return controller;
	}


	/// <summary>
	/// Creates the folder.
	/// </summary>
	/// <returns><c>true</c>, if folder was created, <c>false</c> otherwise.</returns>
	/// <param name="pathRelativeToAssetsPath">Path relative to assets path. eg. Prefabs/Characters/Mario</param>
	public static bool CreateFolder( string pathRelativeToAssetsPath )
	{
		string[] pathSegments = pathRelativeToAssetsPath.Split( new char[] {'/'});
		string accumulatedUnityFolder = "Assets";					// Startwert
		string accumulatedSystemFolder = Application.dataPath;		// Startwert C:/Users/Aryan/Documents/SuperMarioWars_UnityNetwork/Assets
		
		//		Debug.Log("Unity : " + accumulatedUnityFolder);
		//		Debug.Log("System: " + accumulatedSystemFolder);
		
		string lastExistedFolder = accumulatedUnityFolder;
		
		foreach( string folder in pathSegments )
		{
			accumulatedSystemFolder +=  "/" + folder;
			accumulatedUnityFolder += "/" + folder;
			
			string guidFolder = "";
			if (!System.IO.Directory.Exists(accumulatedSystemFolder))
			{
				Debug.LogWarning (accumulatedSystemFolder + " existiert nicht!\n" + 
				                  accumulatedUnityFolder + " existiert nicht!");
				
				Debug.Log("parentFolder = " + lastExistedFolder + " (letzter existierender Ordner)");
				string guidParentFolder =  AssetDatabase.AssetPathToGUID(lastExistedFolder);
				if( guidParentFolder != "" )
				{
					Debug.Log("guidParentFolder = " + guidParentFolder);
					guidFolder = AssetDatabase.CreateFolder( lastExistedFolder, folder );							// TODO  ------------ WTF ordnerangabe geht!  GUID angabe geht nicht!!!!
					if( guidFolder != "" )
					{
						Debug.Log (accumulatedSystemFolder + " wurder erfolgreich erstellt! \n" +
						           accumulatedUnityFolder + " wurder erfolgreich erstellt! \n");
					}
					else
					{
						Debug.LogError ( "Ordner " + folder + " konnte in " + lastExistedFolder + " nicht erstellt werden!");
						//						Debug.LogError (accumulatedSystemFolder + " konnte nicht erstellt werden! \n" +
						//						                accumulatedUnityFolder + " konnte nicht erstellt werden! \n");
						return false;
					}
				}
				else
				{
					Debug.LogError (accumulatedSystemFolder + " guidParentFolder konnte nicht gefunden werden! \n" +
					                accumulatedUnityFolder + " guidParentFolder konnte nicht gefunden werden! \n");
					return false;
				}
			}
			else
			{
				//				Debug.Log (accumulatedSystemFolder + " existiert! \n" +
				//				           accumulatedUnityFolder + " existiert!");
			}
			
			lastExistedFolder = accumulatedUnityFolder;
		}
		return true;
	}

	static void CreateAnimationClip(AnimationClip animClip, Sprite[] sprites, int animationLength, float singleFrameTime)
	{
		Debug.Log("Create Animation: " + animClip.name + " " + " Spritearray = " + sprites.Length + ", Animation length = " + animationLength + ", single frame time = " + singleFrameTime );
		// First you need to create e Editor Curve Binding
		EditorCurveBinding curveBinding = new EditorCurveBinding();
		
		// I want to change the sprites of the sprite renderer, so I put the typeof(SpriteRenderer) as the binding type.
		curveBinding.type = typeof(SpriteRenderer);
		// Regular path to the gameobject that will be changed (empty string means root)
		curveBinding.path = "";
		// This is the property name to change the sprite of a sprite renderer
		curveBinding.propertyName = "m_Sprite";
		
		// An array to hold the object keyframes
		ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[animationLength];
		
		for (int i = 0; i < sprites.Length; i++)
		{
			keyFrames[i] = new ObjectReferenceKeyframe();
			// set the time
			keyFrames[i].time = i*singleFrameTime;			// TODO important
			// set reference for the sprite you want
			keyFrames[i].value = sprites[i];
			
		}
		
		AnimationUtility.SetObjectReferenceCurve(animClip, curveBinding, keyFrames);
	}

}
