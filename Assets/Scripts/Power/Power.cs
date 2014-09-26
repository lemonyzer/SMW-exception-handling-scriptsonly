using UnityEngine;
using System.Collections;

public abstract class Power : MonoBehaviour {

	string powerName;
	bool triggerAble;
	bool startAnimation;
	float durationTime;
	int priotity;

	// Item wurde eingesammelt die diese Power/Fähigkeit freischaltet.
	public abstract void gained();
	
	// Character hat Fähigkeit verloren
	public abstract void lost();

	// Fähigkeit wurde getriggert (manuell von Spieler, von Server oder von Gegenspieler
	public abstract void activated();
}
