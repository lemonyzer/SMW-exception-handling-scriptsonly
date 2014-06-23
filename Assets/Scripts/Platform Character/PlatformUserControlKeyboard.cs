using UnityEngine;
using System.Collections;

public class PlatformUserControlKeyboard : MonoBehaviour {
	
	private PlatformCharacter character;
	
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
		Keyboard();
		character.MoveKeyboard(inputVelocity, inputJump);		// Transfer Input to Character
	}
	
	void Keyboard() {

		if (Input.GetKey(KeyCode.Escape))
		{
			MasterServer.UnregisterHost();
			for(int i=0;i<Network.connections.Length;i++)
			{
				Network.CloseConnection(Network.connections[i],true);
			}
			Network.Disconnect();
			Application.LoadLevel("MainMenuOld");
			return;
		}

		inputVelocity = Input.GetAxis("Horizontal");
		inputJump = Input.GetKey(KeyCode.Space);
		if(debugging != null)
			debugging.text = debugmsg;
//		Debug.LogWarning("Velocity: " + inputVelocity);
//		Debug.LogWarning("Jump: " + inputJump);
	}
}
