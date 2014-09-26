using UnityEngine;
using System.Collections;

public class Rage : Power {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void gained ()
	{
		//if(gamemode.powerprefs.rage == auto)
		activated();
		//else
		//powerbtn aktivieren
		//timer aktivieren...
	}

	public override void lost ()
	{
		throw new System.NotImplementedException ();
	}

	public override void activated ()
	{
		this.GetComponent<RageModus>().StartRageModus();			// später wird statt weiteres script... alles in diesem script durchgeführt was diese poewr betrifft!

	}
}
