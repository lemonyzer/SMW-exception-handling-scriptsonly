using UnityEngine;
using System.Collections;
using System;

public class Player
{
	private string name;
	private Character character;
	private Character characterClone;
	private int points;
	private int kills;
	private int deads;
//	private int 
	
	// Constructor
	public Player(string name, Character character)
	{
		this.name = name;
		this.character = character;
	}
	
//	public int CompareTo(Character other)
//	{
//		if(other == null)
//		{
//			return 1;
//		}
//		
//		if(characterName != other.characterName)
//			return 1;
//		else
//			return 0;
//		
//	}
	
	public string getName()
	{
		return name;
	}
	
	public void setName(string name)
	{
		this.name = name;
	}
	
//	public Sprite getSprite()
//	{
//		return characterSprite;
//	}
//	
//	public void setSprite(Sprite sprite)
//	{
//		characterSprite = sprite;
//	}
	
//	public GameObject getPrefab()
//	{
//		return characterPrefab;
//	}
//	
//	public void setPrefab(GameObject prefab)
//	{
//		characterPrefab = prefab;
//	}

	public Character getCharacter()
	{
		return character;
	}
	
	public void setCharacter(Character character)
	{
		this.character = character;
	}
}
