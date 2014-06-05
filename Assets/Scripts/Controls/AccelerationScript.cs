﻿using UnityEngine;
using System.Collections;

public class AccelerationScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Input.acceleration.x * Time.deltaTime, 0,
		                    -Input.acceleration.z * Time.deltaTime);
	}
}
