﻿using UnityEngine;
using System.Collections;

public class FireBallProjectile : Projectile {
	
//	public abstract override void Collecting(GameObject itemGO, PlatformCharacter collector);
//	public abstract override void Collected(PlatformCharacter collector, NetworkMessageInfo info);

//	public string resourcesFolder = "PowerUps";									<--- durch vererbung enthalten
//	public string projectilePrefabFilename = "";								<--- durch vererbung enthalten
//
//	public  Vector3 projectileSpawnPositionOffset = new Vector3(0.75f,0,0);		<--- durch vererbung enthalten

	public FireBallProjectile(string resFolder, string prefabFilename, Vector3 spawnOffset) : base(resFolder, prefabFilename, spawnOffset)
	{
	}

//	public override GameObject SpawnSingle(GameObject ownerCharacter)
//	{
//		//Resource finden und laden
//
//		GameObject projectilePrefab = (GameObject) Resources.Load(resourcesFolder+"/"+projectilePrefabFilename, typeof(GameObject));
//		if(projectilePrefab == null)
//		{
//			Debug.LogError("bulletPrefab coudn't be loaded!!!! check path / and name");
//			return null;
//			//			return;
//		}
//
//		//GameObject instantieren
////		Vector3 playerPosition = ownerCharacter.transform.position;
//
//		GameObject projectileGameObject = null;
//		if(Network.isServer)
//		{
//			projectileGameObject = (GameObject) Network.Instantiate( projectilePrefab, new Vector3(ownerCharacter.transform.localScale.x * projectileSpawnPositionOffset.x,1* projectileSpawnPositionOffset.y,1* projectileSpawnPositionOffset.z) + ownerCharacter.transform.position, Quaternion.identity, 0);
//		}
//		else if(Network.peerType == NetworkPeerType.Disconnected)
//		{
//			projectileGameObject = (GameObject) GameObject.Instantiate(projectilePrefab, new Vector3(ownerCharacter.transform.localScale.x * projectileSpawnPositionOffset.x,1* projectileSpawnPositionOffset.y,1* projectileSpawnPositionOffset.z) + ownerCharacter.transform.position, Quaternion.identity);
//		}
//
//		//GameObject Script anpassen
//
//		AuthoritativeBullet projectileScript = projectileGameObject.GetComponent<AuthoritativeBullet>();
//
//		projectileScript.ownerCharacter = ownerCharacter;// important!!!
//		projectileScript.moveDirection = new Vector3(ownerCharacter.transform.localScale.x,0,0);
//
//		projectileGameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(ownerCharacter.transform.localScale.x * AuthoritativeBullet.moveSpeed.x,1 * AuthoritativeBullet.moveSpeed.y,1* AuthoritativeBullet.moveSpeed.z);
//		
//		return projectileGameObject;
//	}
	
}
