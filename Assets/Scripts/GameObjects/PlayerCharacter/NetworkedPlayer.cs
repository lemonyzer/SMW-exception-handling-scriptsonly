﻿using UnityEngine;
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
		
		public networkState( Vector3 pos, double time )
		{
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
		
		public move( float horiz, bool jump, double timestamp )
		{
			this.HorizontalAxis = horiz;
			this.jump = jump;
			this.Timestamp = timestamp;
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

	void Awake()
	{
		ownerScript = GetComponent<RealOwner> ();
		characterScript = GetComponent<PlatformCharacter> ();
		inputScript = GetComponent<PlatformUserControl> ();
		myNetworkView = GetComponent<NetworkView>();
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
			// get current move state
			move moveState = new move( inputScript.inputHorizontal , inputScript.inputJump, Network.time );
			
			// buffer move state
			moveHistory.Insert( 0, moveState );
			
			// cap history at 200
			if( moveHistory.Count > 200 )
			{
				moveHistory.RemoveAt( moveHistory.Count - 1 );
			}
			
			// simulate
			characterScript.Simulate();
			
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
	}
	
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
	
	[RPC]
	void CorrectState( Vector3 correctPosition, NetworkMessageInfo info )
	{
		// find past state based on timestamp
		int pastState = 0;											// FIX? -1
		for( int i = 0; i < moveHistory.Count; i++ )
		{
			if( moveHistory[ i ].Timestamp <= info.timestamp )
			{
				pastState = i;
				break;
			}
		}
		
		// rewind
		transform.position = correctPosition;

		Debug.Log("pastState: " + pastState);

		// replay
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
	void Update()
	{
		// in OnSerializeView() --- unreliable/reliable posibility! ... Server owns all Character GameObjects therefore he knows the correct authorative position

		// is this the server? send out position updates every 1/10 second
//		if( PhotonNetwork.isMasterClient )
//		{
//			updateTimer += Time.deltaTime;
//			if( updateTimer >= 0.1f )
//			{
//				updateTimer = 0f;
//				photonView.RPC( "netUpdate", PhotonTargets.Others, transform.position );
//			}
//		}
		
		//if( photonView.isMine ) return; 						// don't run interpolation on the local object
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
	[RPC]
	void netUpdate( Vector3 position, NetworkMessageInfo info )
	{
		//Problem: aktuell gehören photonViews dem MasterClient
		if( ownerScript.owner != Network.player )
		{
			// dieser Character gehört nicht lokalem Spieler
			bufferState( new networkState( position, info.timestamp ) );
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
		
		// Send data to server
		if (stream.isWriting)
		{
			Vector3 pos = transform.position;
			stream.Serialize(ref pos);
		}
		// Read data from remote client
		else
		{
			Vector3 pos = Vector3.zero;
			stream.Serialize(ref pos);
			netUpdate(pos, info);
		}
	}
}