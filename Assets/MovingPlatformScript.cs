﻿using UnityEngine;
using System.Collections;

public class MovingPlatformScript : MonoBehaviour {

	[SerializeField]
	public MovingPlatform movingPlatform;

	[SerializeField]
	public Vector3 startPos;

	[SerializeField]
	public Vector3 endPos;

	[SerializeField]
	public Vector3 moveDirection;

	[SerializeField]
	public bool hinweg = true;

	[SerializeField]
	Transform myTransform;

	Vector3 diff;
	
	float velocityTranslation = 0.24f;

	[SerializeField]
	Vector3 center;

	[SerializeField]
	Vector3 axis;

	// Use this for initialization
	void Start () {
		myTransform = this.transform;
		startPos = myTransform.position;
		if (movingPlatform.path.iPathType == (short) MovingPathType.StraightPath)
		{
			endPos.x = movingPlatform.path.endX / 32f - 10f;
			endPos.y = 15f - movingPlatform.path.endY / 32f - 7.5f;
			endPos.z = myTransform.position.z;

			moveDirection = endPos - startPos;
		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.StraightPathContinuous)
		{

		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.EllipsePath)
		{
			center.x = movingPlatform.path.dCenterX / 32f - 10f;
			center.y = 15f - movingPlatform.path.dCenterY / 32f - 7.5f;
			center.z = myTransform.position.z;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

		if (movingPlatform.path.iPathType == (short) MovingPathType.StraightPath)
		{
			/*
			 * velocity *= -1;
			 */
			if (hinweg)
			{
				diff = endPos - myTransform.position;
				if (diff.magnitude < 0.1f)
					hinweg = !hinweg;
				else
					myTransform.Translate (moveDirection * Time.deltaTime * movingPlatform.path.velocity); 
			}
			else
			{
				diff = startPos - myTransform.position;
				if (diff.magnitude < 0.1f)
					hinweg = !hinweg;
				else
					myTransform.Translate (-1 * moveDirection * Time.deltaTime * movingPlatform.path.velocity); 
			}




		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.StraightPathContinuous)
		{

		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.EllipsePath)
		{
			myTransform.RotateAround (center, Vector3.forward, Time.deltaTime * movingPlatform.path.velocity);
			myTransform.rotation = Quaternion.identity;
		}
		
	}
}
