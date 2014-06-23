using UnityEngine;
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

		// constructor
		public networkState( Vector3 pos, double time )
		{
			this.Position = pos;
			this.Timestamp = time;
		}
	}
	
	// we'll keep a buffer of 20 states
	networkState[] stateBuffer = new networkState[ 20 ];
	int stateCount = 0; // how many states have been recorded
	
	void Update()
	{
		if( networkView.isMine ) return; // don't run interpolation on the local object
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
	
	void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info )
	{
		Vector3 position = Vector3.zero;
		if( stream.isWriting )
		{
			position = transform.position;
			stream.Serialize( ref position );
		}
		else
		{
			stream.Serialize( ref position );
			bufferState( new networkState( position, info.timestamp ) );
		}
	}
	
	// save new state to buffer
	void bufferState( networkState state )
	{
		// shift buffer contents to accomodate new received state
		for( int i = stateBuffer.Length - 1; i > 0; i-- )
		{
			stateBuffer[ i ] = stateBuffer[ i - 1 ];
		}
		
		// save new received state to slot 0
		stateBuffer[ 0 ] = state;

		// increment state count (up to buffer size)
		stateCount = Mathf.Min( stateCount + 1, stateBuffer.Length );
	}
}