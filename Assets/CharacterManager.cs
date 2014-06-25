using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

//	public Sprite player;
//	private Vector3 position;
//
//	Dictionary<string, Character> characterDictonary = new Dictionary<string, Character>();
//	
//	// Use this for initialization
//	void Start () {
//		
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		
//
//		if(!Network.isServer)
//		{
//			return;
//		}
//		// Authorative Server
//		// nur auf Master Client (Server) ausführen.
//		string characterName = GetSelectedCharacterName();
//		int characterPrefabID = GetCharacterPrefabIDfromName(characterName);
//		if( characterPrefabID != -1)
//		{
//			// Prefab ID gefunden
//			Debug.Log("Prefab ID zu " + characterName + ": " + characterPrefabID);
//		}
//		else
//		{
//			// keine Prefab ID zu Character Name gefunden
//			Debug.LogError("keine Prefab ID zu Character Name: " + characterName);
//			characterPrefabID = 0;
//		}
//		
//		PlayerPrefs.SetInt(networkView.owner.ToString(),characterPrefabID);
//		
//	}
//	
//	/**
//	 * Client Funktion
//	 **/
//	void GetClickPosition()
//	{
//		if(Input.GetMouseButtonUp(0))
//		{
//			position = Input.mousePosition;
//			Ray ray = Camera.main.ScreenPointToRay(position);		
//			Vector2 origin = ray.origin;										// startPoint
//			Vector2 direction = ray.direction;									// direction
//			float distance = 100f;
//			RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance);
//			if(hit.collider != null)
//			{
//				//				Debug.Log(hit.collider.name);
//				if(hit.collider.name != "Platform")
//				{
//					position = Input.mousePosition;
//					Debug.Log(hit.collider.name);
//				}
//			}
//		}
//	}
//	
//	/**
//	 * Server Funktion
//	 **/
//	string GetSelectedCharacterName()
//	{
//		Ray ray = Camera.main.ScreenPointToRay(position);		
//		Vector2 origin = ray.origin;										// startPoint
//		Vector2 direction = ray.direction;									// direction
//		float distance = 100f;
//		RaycastHit2D hit = Physics2D.Raycast(origin,direction,distance);
//		if(hit.collider != null)
//		{
//			if(hit.collider.name != "Platform")
//			{
//				Debug.Log(hit.collider.name);
//				return hit.collider.name;
//			}
//		}
//		return null;
//	}
//	
//	int GetCharacterPrefabIDfromName(string name)
//	{
//		return -1;
//	}
//	
//	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
//		Vector3 netPosition;
//		if (stream.isWriting) {
//			netPosition = this.position;
//			stream.Serialize(ref netPosition);
//		} else {
//			netPosition = Vector3.zero;
//			stream.Serialize(ref netPosition);
//			this.position = netPosition;
//		}
//	}
}