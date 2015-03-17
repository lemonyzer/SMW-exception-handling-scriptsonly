﻿using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Character : IComparable<Character>
{
	public int id;
	public int name;
	public bool inUse;

	private string prefabFilename;
	private GameObject characterGameObject;
	private bool isAI;

	private NetworkView gameObjectsNetworkView;
//	private PhotonView gameObjectsPhotonView;

	private string characterName;

	private SpriteRenderer spriteRenderer;		// 2D 
	private Sprite characterAvatarSprite;
	
	private MeshRenderer meshRenderer;			// 3D
	private Texture characterAvatarTexture;

	private Renderer renderer;					// 3D & 2D ?


	public Behaviour characterScript;
	public Behaviour characterAIScript;
	public Behaviour characterInputControlsScript;
//	public PartyCharacter3D partyCharacter3D;
//	public PartyCharacter3DAI partyCharacter3DAI;
//	public PartyCharacter3DControls partyCharacter3DControls;

	public Component bodyCollider;
	public Component bodyCollider2D;

//	public SphereCollider sphereCollider;

	public Character()
	{

	}

	// Constructor
	public Character(string file, GameObject instantiatedPrefab, bool isAI)
	{
		this.prefabFilename = file;
		this.characterGameObject = instantiatedPrefab;
		this.isAI = isAI;
		this.characterName = instantiatedPrefab.name;

//		this.gameObjectsPhotonView = instantiatedPrefab.GetComponent<PhotonView>();

		this.spriteRenderer = instantiatedPrefab.GetComponent<SpriteRenderer> ();

		if(spriteRenderer != null)
		{
			// 2D
			// Character GameObject hat einen oder meherere SpriteRenderer
			this.characterAvatarSprite = instantiatedPrefab.GetComponent<SpriteRenderer>().sprite;
		}
		else
		{
			// kein SpriteRenderer
		}

		meshRenderer = instantiatedPrefab.GetComponent<MeshRenderer>();
		if(meshRenderer != null)
		{
			// 3D
			// Character GameObject hat einene oder mehrere MeshRenderer
			characterAvatarTexture = meshRenderer.material.mainTexture;
		}

		characterScript = instantiatedPrefab.GetComponent<PlatformCharacter>();
//		characterAIScript = instantiatedPrefab.GetComponent<PlatformAIControl>();
		characterInputControlsScript = instantiatedPrefab.GetComponent<PlatformUserControl>();
		bodyCollider2D = instantiatedPrefab.GetComponent<Collider2D>();
	}


	public int CompareTo(Character other)
	{
		if(other == null)
		{
			return 1;
		}

		if(characterName != other.characterName)
			return 1;
		else
			return 0;
	
	}

	public void RemoveCharacterGameObject()
	{
		this.characterGameObject = null;
	}

	public void SetCharacterGameObject(GameObject go)
	{
		this.characterGameObject = go;
	}

	public string getPrefabFilename()
	{
		return prefabFilename;
	}

	public string getName()
	{
		return characterName;
	}

	public void setName(string name)
	{
		characterName = name;
	}

	public Sprite getAvatarSprite()
	{
		return characterAvatarSprite;
	}

	public Texture getAvatarTexture()
	{
		return characterAvatarTexture;
	}

	public GameObject getGameObject()
	{
		return characterGameObject;
	}

//	public PhotonView getGameObjectsPhotonView()
//	{
//		return gameObjectsPhotonView;
//	}

//	public int getGameObjectsViewID()
//	{
//		return gameObjectsPhotonView.viewID;
//	}

	public void setPrefab(GameObject prefab)
	{
		characterGameObject = prefab;
	}
}
