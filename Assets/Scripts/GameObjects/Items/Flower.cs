using UnityEngine;
using System.Collections;

public class Flower : WithPower {

	PlatformCharacter collector;


	public override void Collecting(PlatformCharacter collector)
	{
		this.collector = collector;
	}

	public override void Collected(PlatformCharacter collector)
	{
		this.collector = collector;
	}
}
