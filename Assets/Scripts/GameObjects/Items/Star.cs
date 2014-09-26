using UnityEngine;
using System.Collections;

public class Star : WithPower {

	public RPCMode rpcMode = RPCMode.All;

	public Power power;
    
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
		collector.myNetworkView.RPC("CollectingItem", rpcMode, itemId);			// Serverseitig
	}

	public override void Collected(PlatformCharacter collector, NetworkMessageInfo info)
	{
		this.collector = collector;												// Clientseitig (wenn rpcMode == All) auch Serverseitig 
		/**
		 *
		 * PROBLEM
		 * power's can run MonoBehaviour functions (Awake(),Start(),Update(),FixedUpdate(),LateUpdate(),...) without being attached to an GameObject?
		 * http://answers.unity3d.com/questions/16369/getting-a-script-to-run-without-attaching-it-to-a.html
		 * <-- dies ist nicht der fall: MonoBehaviour functions only work attched on a gameobject. 
		 * 
		 * alle unterschiedlichen Power (: MonoBehaviour) Scripte (Rage,Shoot,Shield,...) müssen an Character GameObject hängen!
		 * 
		 * power field dieser classe gibt nur den typ an, geht das überhaupt?
		 * power = Rage; ??
		 * jetzt muss das Script (die Componente vom Typ power, NICHT vom typ Power.class)
		 * es kann nicht characterGO.GetComponent<Power>(); .... weil das GameObject mehrere Power Componenten besitzt.
		 * //TODO
		 * polymorphie?? ist es möglich mit einer der Super Klasse abstracten variablen, jetzt konkret definierten???
		 * Power's die auf Scripte zugreifen BENÖTIGEN diese Referenz, bzw müssen wissen nach welche Componente sie an den CharacterGameObjecten FINDEN müssen.
		 * 
		 * //TODO
		 **/
		power.gained();	// welche power?



	}

	//power's can run MonoBehaviour functions (Awake(),Start(),Update(),FixedUpdate(),LateUpdate(),...) without being attached to an GameObject?

}
