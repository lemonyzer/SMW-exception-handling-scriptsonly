using UnityEngine;
using System.Collections;

public class Power : MonoBehaviour {

	string powerName;
	bool triggerAble;
	bool startAnimation;
	float durationTime;
	int priotity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void gained()
	{
		// Item wurde eingesammelt die diese Power/Fähigkeit freischaltet.
	}

	void lost()
	{
		// Character hat Fähigkeit verloren
	}
}
