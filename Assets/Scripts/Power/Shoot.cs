using UnityEngine;
using System.Collections;

public class Shoot : Power {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log(this.ToString() + " " + this.gameObject.name);

	}

	void FixedUpdate()
	{

	}

	public override void gained (NetworkMessageInfo info)
	{
		throw new System.NotImplementedException ();
	}
	
	public override void lost ()
	{
		throw new System.NotImplementedException ();
	}
	
	public override void activate ()
	{
		throw new System.NotImplementedException ();
    }
}
