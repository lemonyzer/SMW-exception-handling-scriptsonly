using UnityEngine;
using System.Collections;

public class Flower : WithPower {

	public Power powerScript;
	public string powerScriptName;
	public PlatformCharacter collector;
	public int itemId;

//	public override PlatformCharacter collector {
//		get {
//			throw new System.NotImplementedException ();
//		}
//		set {
//			throw new System.NotImplementedException ();
//		}
//	}
//
//	public override Power powerScript {
//		get {
//			throw new System.NotImplementedException ();
//		}
//		set {
//			powerScript = new Shoot();
//		}
//	}
//	
//	public override string powerScriptName {
//		get {
//			throw new System.NotImplementedException ();
//        }
//        set {
//			powerScriptName = "Rage";
//        }
//    }
//	
//	public override int itemId {
//		get {
//			throw new System.NotImplementedException ();
//        }
//        set {
//            itemId = value;
//        }
//    }

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
