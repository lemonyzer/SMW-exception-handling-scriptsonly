﻿using UnityEngine;
using System.Collections;

public class LevelDesigner : MonoBehaviour {

	public GameObject prefab;
	public Sprite sprite;
	public Vector2 gizmoPosition;
	public float depth = 0;
	public Color gizmoColor = Color.grey;
	public Vector3 rotation;

	void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		Gizmos.DrawWireCube(new Vector3(gizmoPosition.x+0.5F, gizmoPosition.y-0.5F, depth), new Vector3(1,1,1)); 
	}
}
