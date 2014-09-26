﻿using UnityEngine;
using System.Collections;

public abstract class Power : MonoBehaviour {

	public double gainTimeStamp;
	public string powerName;
	public bool triggerAble;
	public bool startAnimation;
	public float durationTime;
	public int priority;

	// Item wurde eingesammelt die diese Power/Fähigkeit freischaltet.
	public abstract void gained(NetworkMessageInfo info);
	
	// Character hat Fähigkeit verloren
	public abstract void lost();

	// Fähigkeit wurde getriggert (manuell von Spieler, von Server oder von Gegenspieler
	public abstract void activate();
}
