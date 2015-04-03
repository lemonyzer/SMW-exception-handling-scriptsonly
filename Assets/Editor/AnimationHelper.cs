﻿using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

//using System.Collections;


//[CustomEditor(typeof(SMWAnimation))]
//public class SMWAnimationEditor : Editor
//{
//	public override void OnInspectorGUI()
//	{
//		serializedObject.Update();
//		var controller = target as SMWAnimation;
//		//EditorGUIUtility.LookLikeInspector();
//		SerializedProperty tps = serializedObject.FindProperty ("targetPoints");
//		EditorGUI.BeginChangeCheck();
//		EditorGUILayout.PropertyField(tps, true);
//		if(EditorGUI.EndChangeCheck())
//			serializedObject.ApplyModifiedProperties();
//		EditorGUIUtility.LookLikeControls();
//	}
//}

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


public class AnimationHelper : EditorWindow {


    
    public SMWAnimation[] smwAnimations = new SMWAnimation[6];

	public GameObject target;

	public AnimationClip idleAnim;
	public AnimationClip runAnim;
	public AnimationClip changeRunDirectionAnim;
	public AnimationClip jumpAnim;

	public AnimationClip headJumpedAnim;
	public AnimationClip dieAnim;
	public AnimationClip gameOverAnim;
	public AnimationClip spawnAnim;
	public AnimationClip spawnProtectionAnim;
	public AnimationClip rageAnim;

	public SmwCharacter smwCharacter;

	[MenuItem ("Window/Animation Helper")]
	static void OpenWindow ()
	{
		// Get existing open window or if none, make a new one:
		GetWindow<AnimationHelper> ();
	}


	void OnGUI () {
		target = EditorGUILayout.ObjectField("Target Object", target, typeof(GameObject), true) as GameObject;

		smwCharacter = EditorGUILayout.ObjectField("SMW Character SO", smwCharacter, typeof(SmwCharacter), false) as SmwCharacter;

		//smwAnimations = EditorGUILayout.PropertyField("SMW Character SO", animations, typeof(SMWAnimation), false) as SMWAnimation;

//		idleAnim = EditorGUILayout.ObjectField("Idle", idleAnim, typeof(AnimationClip), false) as AnimationClip;
//		runAnim = EditorGUILayout.ObjectField("Run", runAnim, typeof(AnimationClip), false) as AnimationClip;
//		jumpAnim = EditorGUILayout.ObjectField("Jump", jumpAnim, typeof(AnimationClip), false) as AnimationClip;
//		headJumpedAnim = EditorGUILayout.ObjectField("HeadJumped", headJumpedAnim, typeof(AnimationClip), false) as AnimationClip;
//		dieAnim = EditorGUILayout.ObjectField("Die", dieAnim, typeof(AnimationClip), false) as AnimationClip;


		if(GUILayout.Button("Create"))
		{
			if(target == null)
			{
				Debug.LogError ("No target for animator controller set.");
				return;
			}

			Create ();
		}
	}

	void Create ()
	{
		UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/" + target.name + " scripted Animator.controller");

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
		leaveInvincibleEnterDefaultState.hasExitTime = true;
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
		leaveSkidEnterRun.hasExitTime = true;
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
		leaveSpawnEnterIdle.hasExitTime = true;													//TODO <-- Achtung 	hasExitTime (nach Animation)
		leaveSpawnEnterIdle.exitTime = 1f;
		leaveSpawnEnterIdle.canTransitionToSelf = false;


        
        // init smwAnimations array
		string charName = "TestCharacter";
		int baseLayerStateCount = 0;
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"dynamicIdle",			1,1,	smwCharacter.charIdleSprites,idleState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"dynamicRun",			25,2,	smwCharacter.charRunSprites,runState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"dynamicJump",			1,1,	smwCharacter.charJumpSprites,jumpState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"dynamicSkid",			1,1,	smwCharacter.charSkidSprites,skidState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"dynamicDie",			1,1,	smwCharacter.charDieSprites,deadState);
		smwAnimations[baseLayerStateCount++] = new SMWAnimation(charName+"dynamicHeadJumped",	1,1,	smwCharacter.charHeadJumpedSprites,headJumpedState);


		// ordner anlegen, wenn noch nicht vorhanden
//		string guidAnimationsFolder;
//		string guidCharactersFolder;
//		string guidAutoGeneratedFolder;
//		string guidCurrentCharacterFolder;
//
//		string path = "Assets/Animations";
//		if(!AssetDatabase.Contains (path))
//			guidAnimationsFolder = AssetDatabase.CreateFolder("Assets", "Animations");
//		else
//			Debug.Log(path + " existiert");
//
//		path += "/Characters";
//		if(!AssetDatabase.Contains (path))
//			guidCharactersFolder = AssetDatabase.CreateFolder( AssetDatabase.GUIDToAssetPath(guidAnimationsFolder) , "Characters");
//		else
//			Debug.Log(path + " existiert");
//
//		path += "/_AutoGenerated";
//		if(!AssetDatabase.Contains (path))
//			guidAutoGeneratedFolder = AssetDatabase.CreateFolder( AssetDatabase.GUIDToAssetPath(guidCharactersFolder) , "_AutoGenerated");
//		else
//			Debug.Log(path + " existiert");
//
//		path += "/" + charName;
//		if(!AssetDatabase.Contains (path))
//			guidCurrentCharacterFolder = AssetDatabase.CreateFolder( AssetDatabase.GUIDToAssetPath(guidAutoGeneratedFolder) , charName);
//		else
//			Debug.Log(path + " existiert");

		string folderPath = "Animations/Characters/AutoGenerated/" + charName ;
		if( CreateFolder(folderPath) )
		{
			string assetFolderPath = "Assets/" + folderPath;

			for(int i=0; i < baseLayerStateCount; i++)
			{
				AnimationClip tempAnimClip = new AnimationClip();
				#if !UNITY_5
				// Setting it as generic allows you to use the animation clip in the animation controller
				AnimationUtility.SetAnimationType(tempAnimClip, ModelImporterAnimationType.Generic);
	            #endif
				tempAnimClip.name = smwAnimations[i].name;
				CreateAnimationClip(tempAnimClip, smwAnimations[i].sprites, smwAnimations[i].keyFrames, 1.0f/smwAnimations[i].framesPerSecond);
				smwAnimations[i].animState.motion = tempAnimClip;

				if (AssetDatabase.Contains(tempAnimClip))
				{
					Debug.LogError(tempAnimClip.name + " in AssetDatabase bereits enthalten, darf nicht erscheinen");
				}
				else
				{
					Debug.Log(tempAnimClip.name + " nicht in AssetDatabase vorhanden, wird jetzt gespeichert.");

//					string tempPath = "Animations/Characters/AutoGenerated/" + charName;
//					CreateAssetInFolder(tempAnimClip, tempPath, tempAnimClip.name );
					// asset anlegen
					Debug.Log("Versuche " + tempAnimClip.name + " in Ordner " + assetFolderPath + " zu speicheren");
					AssetDatabase.CreateAsset(tempAnimClip, assetFolderPath + "/" + tempAnimClip.name + ".asset");
					Debug.Log("Erfolgreich: " + tempAnimClip.name + " in Ordner " + assetFolderPath + " gespeichert");
				}
			}
			AssetDatabase.SaveAssets();

			//TODO
			target.GetComponent<Animator>().runtimeAnimatorController = controller;
            //TODO
		}
		else
		{
			Debug.LogError("Ordner existiert/existerieren nicht und kann/können nicht angelegt werden!\n" + folderPath);
		}


		// AnimationClip

//		AnimationClip animClip = new AnimationClip();
//
//#if !UNITY_5
//		// Setting it as generic allows you to use the animation clip in the animation controller
//		AnimationUtility.SetAnimationType(animClip, ModelImporterAnimationType.Generic);
//#endif
//
//		animClip.name = "dynamic_Idle";
//		CreateAnimationClip(animClip, smwCharacter.charIdleSprites, 1, singleSpriteTime);
//
//		// for correct transform editing
//		animClip.EnsureQuaternionContinuity();
//		idleState.motion = animClip;
//
//		animClip.name = "dynamic_Run";
//		CreateAnimationClip(animClip, smwCharacter.charIdleSprites, 2, singleSpriteTime);
//		
//		// for correct transform editing
//		animClip.EnsureQuaternionContinuity();
//		idleState.motion = animClip;



	}


	// creates Folders if missing
	public bool CreateFolder( string ParentFolder )
	{
		string[] pathSegments = ParentFolder.Split( new char[] {'/'});
		string accumulatedUnityFolder = "Assets";					// Startwert
		string accumulatedSystemFolder = Application.dataPath;		// Startwert C:/Users/Aryan/Documents/SuperMarioWars_UnityNetwork/Assets

		Debug.Log("Unity : " + accumulatedUnityFolder);
		Debug.Log("System: " + accumulatedSystemFolder);

		foreach( string folder in pathSegments )
		{
			accumulatedSystemFolder +=  "/" + folder;
			accumulatedUnityFolder += "/" + folder;
            
			string guidFolder = "";
			if (!System.IO.Directory.Exists(accumulatedSystemFolder))
			{
				Debug.LogWarning (accumulatedSystemFolder + " existiert nicht!");
				guidFolder = AssetDatabase.CreateFolder( accumulatedUnityFolder, folder );
				if( guidFolder != "" )
				{
					Debug.Log (accumulatedSystemFolder + " wurder erfolgreich erstellt! \n" +
					           accumulatedUnityFolder + " wurder erfolgreich erstellt! \n");
				}
				else
				{
					Debug.LogError (accumulatedSystemFolder + " konnte nicht erstellt werden! \n" +
					                accumulatedUnityFolder + " konnte nicht erstellt werden! \n");
					return false;
				}
			}
			else
			{
				Debug.Log (accumulatedSystemFolder + " existiert! \n" +
				           accumulatedUnityFolder + " existiert!");
			}
        }
		return true;
    }
    
    
    //	float singleSpriteTime = 0.0833f;
    
    void CreateAnimationClip(AnimationClip animClip, Sprite[] sprites, int animationLength, float singleFrameTime)
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

//	int timeForKey(int i)
//	{
//		// Here you put the code that will return the time
//		// for the ith keyframe
//		
//		// Example: the time of each frame will be its index
//		return i;
//	}
//	
//	Sprite spriteForKey(int i)
//	{
//		// Here you put the code that will return the sprite that you want
//		// for the ith keyframe
//		
//		// Example: assuming you have a sprite array "_sprites" populated with all the sprites for each keyframe
//		// in order
//		return _sprites[i];
//	}
}
