using UnityEngine;
using System.Collections;
using System;

public class Character : IComparable<Character>
{
	private string characterName;
	private Sprite characterSprite;
	private GameObject characterPrefab;

	// Constructor
	public Character(string name, Sprite sprite, GameObject prefab)
	{
		this.characterName = name;
		this.characterSprite = sprite;
		this.characterPrefab = prefab;
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

	public string getName()
	{
		return characterName;
	}

	public void setName(string name)
	{
		characterName = name;
	}

	public Sprite getSprite()
	{
		return characterSprite;
	}

	public void setSprite(Sprite sprite)
	{
		characterSprite = sprite;
	}

	public GameObject getPrefab()
	{
		return characterPrefab;
	}

	public void setPrefab(GameObject prefab)
	{
		characterPrefab = prefab;
	}
}
