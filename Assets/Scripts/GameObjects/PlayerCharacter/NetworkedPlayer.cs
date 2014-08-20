using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NetworkedPlayer : Photon.MonoBehaviour
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

	void Awake()
	{
		ownerScript = GetComponent<RealOwner> ();
		characterScript = GetComponent<PlatformCharacter> ();
		inputScript = GetComponent<PlatformUserControl> ();
	}

	void Start()
	{

	}

	// simulate movement local
	// send input and calculated position to server / masterclient
	void FixedUpdate()
	{
		if( ownerScript.owner == PhotonNetwork.player )
		{
			// get current move state
			move moveState = new move( inputScript.inputHorizontal , inputScript.inputJump, PhotonNetwork.time );
			
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
			photonView.RPC( "ProcessInput", PhotonTargets.MasterClient, moveState.HorizontalAxis, moveState.jump, transform.position );
		}
	}
	
	[RPC]
	void ProcessInput( float recvedInputHorizontal, bool recvedInputJump, Vector3 recvedPosition, PhotonMessageInfo info )
	{
		// aktuell gehören photonviews dem masterclient
		//		if( photonView.isMine )
		//			return;
		if (ownerScript.owner == PhotonNetwork.player)
		{
			// this character is owned by local player... don't run simulation
			// master client muss sich selbst nicht kontrollieren
			return;
		}
		
		if( !PhotonNetwork.isMasterClient )
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
			photonView.RPC( "CorrectState", info.sender, transform.position );
		}
	}
	
	[RPC]
	void CorrectState( Vector3 correctPosition, PhotonMessageInfo info )
	{
		// find past state based on timestamp
		int pastState = -1;											// FIX?
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
		if (ownerScript.owner == PhotonNetwork.player)	return;  // don't run interpolation on the local object

		// dieser Character gehört nicht lokalem Spieler
		// zwischen empfangenen positionen wird interpoliert um bewegung zu glätten

		if( stateCount == 0 ) return; // no states to interpolate
		
		double currentTime = PhotonNetwork.time;
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

	// Authorative & unreliable!!! [RPC] is book method
	[RPC]
	void netUpdate( Vector3 position, PhotonMessageInfo info )
	{
		//Problem: aktuell gehören photonViews dem MasterClient
		if( ownerScript.owner != PhotonNetwork.player )
		{
			// dieser Character gehört nicht lokalem Spieler
			bufferState( new networkState( position, info.timestamp ) );
			Debug.Log("----------------------------------------------");
			Debug.Log("current Time = " + Time.time);
			Debug.Log("current Network Time = " + PhotonNetwork.time);
			Debug.Log("RPC Time = " + info.timestamp);
			Debug.Log("rtt = " + (PhotonNetwork.time - info.timestamp));
			Debug.Log("prtt = " + PhotonNetwork.networkingPeer.RoundTripTime);
			Debug.Log("prttS = " + PhotonNetwork.networkingPeer.RoundTripTimeVariance);
			Debug.Log("----------------------------------------------");
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


	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		
		// Send data to server
		if (stream.isWriting)
		{
			float time = Time.time;
			Vector3 pos = transform.position;
			stream.Serialize(ref pos);
			stream.Serialize(ref time);
		}
		// Read data from remote client
		else
		{
			float time = 0;
			Vector3 pos = Vector3.zero;
			stream.Serialize(ref pos);
			stream.Serialize(ref time);
			Debug.Log("recved Time = " + time);
			Debug.Log("Time.time = " + Time.time);
			Debug.Log("differenz = " + (Time.time-time));
			netUpdate(pos, info);
		}
	}
}