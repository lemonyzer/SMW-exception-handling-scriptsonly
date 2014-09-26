using UnityEngine;
using System.Collections;

public class Flower : WithPower {

	public override PlatformCharacter collector {
		get {
			throw new System.NotImplementedException ();
		}
		set {
			collector = value;
		}
	}
	
	public override int itemId {
		get {
			throw new System.NotImplementedException ();
        }
        set {
            itemId = value;
        }
    }

	public override void Collecting(PlatformCharacter collector)
	{
		this.collector = collector;

	}

	public override void Collected(PlatformCharacter collector, NetworkMessageInfo info)
	{
		this.collector = collector;
		collector.hasItem = true;
		collector.power1 = new Shoot();
	}
}
