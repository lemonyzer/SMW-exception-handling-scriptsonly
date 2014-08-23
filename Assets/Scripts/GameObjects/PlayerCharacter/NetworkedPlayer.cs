using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour
{
	// how far back to rewind interpolation?
	public float InterpolationBackTime = 0.1f;
	
	// a snapshot of values received over the network
	private struct networkState
	{
		public Vector3 Position;
		public double Timestamp;

		public float InputHorizontal;
		public bool InputJump;
		
		public networkState( Vector3 pos, double time, float inputHorizontal, bool inputJump )
		{
			this.InputHorizontal = inputHorizontal;
			this.InputJump = inputJump;
			this.Position = pos;
			this.Timestamp = time;
		}
	}
	
	// represents a move command sent to the server
	private struct move
	{
		public float HorizontalAxis;
		public bool jump;
		public double Timestamp;
		public Vector3 Position;
		
		public move( float horiz, bool jump, double timestamp )
		{
			this.HorizontalAxis = horiz;
			this.jump = jump;
			this.Timestamp = timestamp;
			this.Position = new Vector3(0f,0f,0f);
		}
	}
	
	// we'll keep a buffer of 20 states
	networkState[] stateBuffer = new networkState[ 20 ];
	int stateCount = 0; // how many states have been recorded
	
	// a history of move commands sent from the client to the server
	List<move> moveHistory = new List<move>();
	
	PlatformUserControl inputScript;
	PlatformCharacter characterScript;
	RealOwner ownerScript;
	NetworkView myNetworkView;

	Transform characterBoxCollider;

	void Awake()
	{
		ownerScript = GetComponent<RealOwner> ();
		characterScript = GetComponent<PlatformCharacter> ();
		inputScript = GetComponent<PlatformUserControl> ();
		myNetworkView = GetComponent<NetworkView>();
		characterBoxCollider = transform.Find("BoxCollider");		// looks in transform childs
																	// GameObject.Find looks in compete Scene!
	}

	void Start()
	{

	}

	// simulate movement local
	// send input and calculated position to server / masterclient
	void FixedUpdate()
	{
		if( ownerScript.owner == Network.player )
		{
			// simulate
			characterScript.Simulate();

			// this is my character
			// get current move state
			move moveState = new move( inputScript.inputHorizontal , inputScript.inputJump, Network.time );

			// information required to locate the own boxcollider (should be 100 ms behind the charactersprite)
			moveState.Position = this.transform.position;

			// buffer move state
			moveHistory.Insert( 0, moveState );
			
			// cap history at 200
			if( moveHistory.Count > 200 )
			{
				moveHistory.RemoveAt( moveHistory.Count - 1 );
			}

			// send state to server
			if(Network.isClient)
			{
				myNetworkView.RPC( "ProcessInput", RPCMode.Server, moveState.HorizontalAxis, moveState.jump, transform.position );
			}
			else if(Network.isServer)
			{
				// cant send from server to server!
			}
		}
		else
		{
			/**
			 * 
			 * !!!!!!!!!! 	Prediction / "Extrapolation"		!!!!!!
			 * 
			 **/
			// this character is from other player
			if(stateCount > 0)
			{
				// wir haben mindestens ein paket
				networkState last = stateBuffer[0];
				inputScript.inputHorizontal = last.InputHorizontal;	// predict that user is still moving in same direction.
				//inputScript.inputJump = last.InputJump;
				characterScript.Simulate();
			}
		}
	}

	// server
	[RPC]
	void ProcessInput( float recvedInputHorizontal, bool recvedInputJump, Vector3 recvedPosition, NetworkMessageInfo info )
	{
//		Debug.Log(this.ToString() + ": ProcessInput");
		// aktuell gehören photonviews dem masterclient
		//		if( photonView.isMine )
		//			return;
		if (ownerScript.owner == Network.player)
		{
			// this character is owned by local player... don't run simulation
			// master client muss sich selbst nicht kontrollieren
			return;
		}
		
		if( !Network.isServer )
		{
			// nur master client bekommt input und kontrolliert andere spieler
			return;
		}
		
		// execute input
		inputScript.inputHorizontal = recvedInputHorizontal;
		inputScript.inputJump = recvedInputJump;
		characterScript.Simulate();

		// compare results
		if( Vector3.Distance( transform.position, recvedPosition ) > 0.1f )
		{
			// error is too big, tell client to rewind and replay
			myNetworkView.RPC( "CorrectState", info.sender, transform.position );
		}
	}

	double timeDiff = 0;

	// local client character only
	[RPC]
	void CorrectState( Vector3 correctPosition, NetworkMessageInfo info )
	{
		// find past state based on timestamp
		int pastState = -1;											// FIX?: -1	//replay begins with 0<=pastState, if array is empty can't access element 0!!!
		for( int i = 0; i < moveHistory.Count; i++ )
		{
			if( moveHistory[ i ].Timestamp <= info.timestamp )
			{
				pastState = i;
				break;
			}
		}
		
		// rewind position
		if(true)
		{
			//v1 3 frames to smooth correction
			//a) Coroutine triggers every deltaFixedUpdateTime
			//b) FixedUpdate
			transform.position = correctPosition;

		}


		Debug.Log("pastState ID: " + pastState);

		// replay already sent controlInput
		// because the movement commands are already sent to server!
		for( int i = 0; i <= pastState; i++ )
		{
			inputScript.inputHorizontal = moveHistory[ i ].HorizontalAxis;
			inputScript.inputJump = moveHistory[ i ].jump;
			characterScript.Simulate();
		}
		
		// clear
		moveHistory.Clear();
	}
	
	private float updateTimer = 0f;

	int GetPastState(double timeStamp)
	{
		// moveHistory[0] immer aktuellster State, wenn nicht leer!
		int pastState = -1;											
		for( int i = 0; i < moveHistory.Count; i++ )
		{
			if( moveHistory[ i ].Timestamp <= timeStamp )
			{
				pastState = i;
				break;
			}
		}
		return pastState;
	}

	// raw position data stored in moveHisory, no backsimulation needed
//	// local Character BoxCollider draw on 100ms back position
//	void SimulationBack()
//	{
//	}

	void LateUpdate()
	{
		if(ownerScript.owner == Network.player)
		{
			// shows my characterposition 100ms in the past (Network Time - 100 ms)... is almost equal to the authorativ position from server
			double serverTime = Network.time - InterpolationBackTime;
			int pastState = GetPastState( serverTime );
//			Debug.Log("BoxCollider pastState = " + pastState);
			if(pastState >= 0)
			{
				// buffer moveHistory[0] immer aktuellste
				//				for(int i=0, i<=pastState)
				//				{
				//					SimulationBack();
				//				}
				characterBoxCollider.transform.position = Vector3.zero + moveHistory[pastState].Position;
				Debug.DrawLine(this.transform.position, moveHistory[pastState].Position, Color.red, 5f);
			}
			else
			{
				characterBoxCollider.transform.position = this.transform.position;
				//				characterBoxCollider.SetActive(false);
			}
		}
		else
		{

			/***
			 * 
			 *	PREDICTION			SIMPLE (only Player Input from latest received netUpdate (OnSerialzationView)
			 *
			 **/
			// predicts characterposition (Network Time + 100ms) 

			if(stateCount > 0)
			{
				Vector3 predictedPosition = new Vector3(transform.position.x,transform.position.y,transform.position.z);
				Vector3 moveDirectionPredicted = new Vector3(stateBuffer[0].InputHorizontal * characterScript.getMaxSpeed() * Time.fixedDeltaTime,0f,0f);
				int steps = (int) (InterpolationBackTime/Time.fixedDeltaTime);
				//Debug.Log("Steps =" + steps);
				// vorher position wieder auf character position setzen, dann kann vorrausberechnet werden
				characterBoxCollider.position = transform.position;
				for(int i=0; i < steps; i++)
				{
					characterBoxCollider.Translate( moveDirectionPredicted );
				}
			}
		}
	}


	// Server and other clients characters
	void Update()
	{
		// in OnSerializeView() --- unreliable/reliable posibility! ...
		// Server owns all Character GameObjects therefore he knows the correct authorative position ( with pingdelay behind controlling client ) 

		// is this the server? send out position updates every 1/10 second
//		if( Network.isServer )
//		{
//			updateTimer += Time.deltaTime;
//			if( updateTimer >= 0.1f )
//			{
//				updateTimer = 0f;
//				myNetworkView.RPC( "netUpdate", RPCMode.Others, transform.position );
//			}
//		}

		if(Network.isServer)
		{
//			Debug.Log(stateCount); 		// always 0  -> noData to interpolate!
			return;						// Server doesn't need interpolation!
		}

		// NetworkView of character is always owned by server!
//		if( myNetworkView.isMine ) return; 						// don't run interpolation on the local object
		if (ownerScript.owner == Network.player)	return;  // don't run interpolation on the local object
		if( stateCount == 0 ) return; // no states to interpolate
		
		double currentTime = Network.time;
		double interpolationTime = currentTime - InterpolationBackTime;
		
		// the latest packet is newer than interpolation time - we have enough packets to interpolate
		if( stateBuffer[ 0 ].Timestamp > interpolationTime )
		{
			for( int i = 0; i < stateCount; i++ )
			{
				// find the closest state that matches network time, or use oldest state
				if( stateBuffer[ i ].Timestamp <= interpolationTime || i == stateCount - 1 )
				{
					// the state closest to network time
					networkState lhs = stateBuffer[ i ];
					
					// the state one slot newer
					networkState rhs = stateBuffer[ Mathf.Max( i - 1, 0 ) ];
					
					// use time between lhs and rhs to interpolate
					double length = rhs.Timestamp - lhs.Timestamp;
					float t = 0f;
					if( length > 0.0001 )
					{
						t = (float)( ( interpolationTime - lhs.Timestamp ) / length );
					}
					
					transform.position = Vector3.Lerp( lhs.Position, rhs.Position, t );
					break;
				}
			}
		}
	}
	bool oneTimeInfo = true;
	// Authorative & unreliable replaced - Bookmethod [RPC]
	// buffers only on Clients, on Characters not owned by local player
	[RPC]
	void netUpdate( Vector3 position, float inputHorizontal, bool inputJump, NetworkMessageInfo info )
	{
		// hier kann einiges optimiert werden

		// The short keyword is used to define a variable of type short. Short is an integer (ie no decimals are allowed). Range of short: –32,768 through 32,767. 
		// positionen ebenfalls  (wird sowieso dazwischen interpoliert)
		// position über vector2 - 2d game!
		// inputHorizontal: 1 char/2 booleam (left/right)/short/int
		// input all in one char [jumpBit, powerUpBit, leftBit, rightBit, X, X, X, X]

		//Problem: aktuell gehören photonViews dem MasterClient
		if( ownerScript.owner != Network.player )
		{
			// dieser Character gehört nicht lokalem Spieler
			bufferState( new networkState( position, info.timestamp, inputHorizontal, inputJump ) );
		}
		else
		{
			// Dieser Character gehört lokalem Spieler
			if(oneTimeInfo)
			{
				oneTimeInfo=false;
				Debug.Log(this.ToString() + ": my Character");
			}
		}
	}
	
	// save new state to buffer
	void bufferState( networkState state )
	{
		// shift buffer contents to accomodate new state
		for( int i = stateBuffer.Length - 1; i > 0; i-- )
		{
			stateBuffer[ i ] = stateBuffer[ i - 1 ];
		}
		
		// save state to slot 0
		stateBuffer[ 0 ] = state;
		
		// increment state count (up to buffer size)
		stateCount = Mathf.Min( stateCount + 1, stateBuffer.Length );
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		
		// Code runs on NetworkView Owner
		// Send data to all Clients
		if (stream.isWriting)
		{
			Vector3 authorativePos = transform.position;				// authorative calculated position
			float receivedHorizontal = inputScript.inputHorizontal;		// received input from the character owner send to all clients (for prediction use and animation)
			bool receivedInputJump = inputScript.inputJump;				// received input from the character owner send to all clients (for prediction use and animation)
			stream.Serialize(ref authorativePos);
			stream.Serialize(ref receivedHorizontal);
			stream.Serialize(ref receivedInputJump);
		}
		// Read data from Server
		else
		{
			Vector3 authorativePos = Vector3.zero;
			float receivedHorizontal = 0;
			bool receivedInputJump = false;
			stream.Serialize(ref authorativePos);
			stream.Serialize(ref receivedHorizontal);
			stream.Serialize(ref receivedInputJump);
			//netUpdate(pos, info);			// will buffer state only on characters not controlled by local player
			netUpdate(authorativePos, receivedHorizontal, receivedInputJump, info);			// will buffer state only on characters not controlled by local player
		}
	}

//	void OnGUI()
//	{
//		if(stateCount > 0)
//		{
//			GUI.Box()
//		}
//	}
}