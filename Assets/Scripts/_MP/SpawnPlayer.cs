using UnityEngine;
using System.Collections;

public class SpawnPlayer : MonoBehaviour
{
	public GameObject Player;
	
	IEnumerator Start()
	{
		yield return null;
		yield return null;
		Network.isMessageQueueRunning = true;
		Network.Instantiate( Player, Vector3.zero, Quaternion.identity, 0 );
	}
}