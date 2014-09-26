using UnityEngine;
using System.Collections;

public class Star : WithPower {

	public RPCMode rpcMode = RPCMode.All;

	public override Power powerScript {
		get {
			throw new System.NotImplementedException ();
		}
		set {
			powerScript = new Rage();														// review!!! VERIFY
		}
	}

	public override string powerScriptName {
		get {
			throw new System.NotImplementedException ();
		}
		set {
			powerScriptName = typeof(Rage).ToString();									// review!!! VERIFY
		}
	}
    
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
		 * Daher sollte jedes Item eine Power referenz beinhalten. Um unterschiedliche Powerreferenzen zu halten public abstract Power powerRef;
		 * //TODO
		 **/

		// power.gained();	// welche power? bei jedem character unterschiedlich!

		// Generic List!!! die kann es auch!
		// iTunes U
		// 07_Java_2013_11_25_ernsthafte_Polymorphie_mit_Interfaces
		// Minute 46

		// PROBLEM statt Rage sollte eine Variable stehen						// ich möchte nicht auf MonoBehaviour Funktionnen zugreifen ich möchte auf ChildClass funktionen zugreifen wie activate!
		//collector.gameObject.GetComponent<Rage>().activated();

		//TODO isinstancOf in C# finden

		Power characterPowerScript = collector.gameObject.GetComponent(powerScriptName) as Power;
		if(characterPowerScript != null)
		{
			//characterPowerScript.activated();
			Debug.LogError("GetComponent(string) hat funktioniert!");
		}
		else
		{
			Debug.LogError("GetComponent(string) hat nicht funktioniert!");
		}

		// "variabel" wird mit .GetComponent<variable>() nicht funktionieren. außer variable ist vom typ string!
		// collector.gameObject.GetComponent<Rage>().activated();		//<<-- wird nicht mit GetComponent<>() funktionieren!
        // collector.gameObject.GetComponent<power.GetType()>().activated();		//<<-- wird nicht mit GetComponent<>() funktionieren!

		characterPowerScript = collector.gameObject.GetComponent( powerScript.GetType() ) as Power;
		characterPowerScript = collector.gameObject.GetComponent( powerScript.GetType().Name ) as Power;
		if(characterPowerScript != null)
		{
			characterPowerScript.activated();
			Debug.LogError("GetComponent(powerScript.GetType().Name) hat funktioniert!");
		}
		else
		{
			Debug.LogError("GetComponent(powerScript.GetType().Name) hat nicht funktioniert!");
        }

		//characterPowerScript = collector.gameObject.GetComponent(typeof(Power) ) as Power;		// geht nicht, es nach einer speziellen Power gesucht!!!! dies würde die erste Componente liefern die vom Typ Power ist!
		//characterPowerScript = collector.gameObject.GetComponent(Types.GetType(power) ) as Power;
	}

	//power's can run MonoBehaviour functions (Awake(),Start(),Update(),FixedUpdate(),LateUpdate(),...) without being attached to an GameObject?

}
