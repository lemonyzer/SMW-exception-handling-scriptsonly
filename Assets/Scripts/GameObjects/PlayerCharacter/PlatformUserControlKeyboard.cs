using UnityEngine;
using System.Collections;

public class PlatformUserControlKeyboard : Photon.MonoBehaviour {
	
	private PlatformCharacter character;
	private RealOwner realOwner;
	/**
	 * Debugging GUI Element
	 **/
	public GUIText debugging;
	private string debugmsg="";
	
	/** 
	 * Input
	 **/
	private float inputVelocity = 0f;
	private bool inputJump = false;
	
	void Awake()
	{
		ApplicationPlatformCheck();		// Touchfunction only on mobile devices
	}
	
	// Use this for initialization
	void Start() {
		character = GetComponent<PlatformCharacter>();
		realOwner = GetComponent<RealOwner>();
	}
	
	void ApplicationPlatformCheck()
	{
		/**
		 * not on Mobile Devices (Android / IOs)
		 **/
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			
		}
		else if (Application.platform == RuntimePlatform.OSXEditor)
		{
			
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			
		}
		else if (Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			
		}
		else
		{
			Debug.LogWarning(this.name + ": disabled!!!");
			this.enabled = false;							// disable this script
		}
	}
	
	// Update is called once per frame
	void Update() {
		if(Network.player == realOwner.owner)
		{
			Keyboard();
//			character.MoveKeyboard(inputVelocity, inputJump);		// Transfer Input to Character
		}
	}

	void FixedUpdate()
	{
		if(Network.player == realOwner.owner)
		{
			photonView.RPC("SendMovementInput", PhotonTargets.MasterClient, inputVelocity, inputJump);
		}
	}

	// darf nur einmal vorkommen!
//	[RPC]
//	void SendMovementInput(float inputX, bool inputJump)
//	{
//		//Called on the server
//		character.MoveKeyboard(inputX, inputJump);
//	}
	
	void Keyboard() {

		//BackButton();			// jetzt in GameScene!

		inputVelocity = Input.GetAxis("Horizontal");
		inputJump = Input.GetKey(KeyCode.Space);
		if(debugging != null)
			debugging.text = debugmsg;
//		Debug.LogWarning("Velocity: " + inputVelocity);
//		Debug.LogWarning("Jump: " + inputJump);
	}

}
