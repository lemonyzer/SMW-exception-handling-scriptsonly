using UnityEngine;
using System.Collections;
using System;

public class Character : IComparable<Character>
{
	private string characterName;
	private Sprite characterSprite;
	private GameObject characterPrefab;

	private Animator platformAnimator;
	private PlatformCharacter platformCharacter;
	private HealthController healthController;
	private PushSkript pushSkript;
	private RageModus rageModus;

	private PlatformAIControl platformAIControl;
	private PlatformUserControlAnalogStickAndButton platformUserControlMobile;
	private PlatformUserControlKeyboard platformUserControlPC;

	private bool isAI;

	// Constructor
	public Character(GameObject prefab, bool isAI)
	{
		this.characterPrefab = prefab;
		this.isAI = isAI;
		this.characterName = prefab.name;
		this.characterSprite = prefab.GetComponent<SpriteRenderer>().sprite;
		this.platformAnimator = prefab.GetComponent<Animator>();
		this.platformCharacter = prefab.GetComponent<PlatformCharacter>();
		if(platformCharacter == null)
			Debug.LogError(characterName + " hat kein PlatformCharacter script!!!");

		this.healthController = prefab.GetComponent<HealthController>();
		if(healthController == null)
			Debug.LogError(characterName + " hat kein HealthController script!!!");

		this.rageModus = prefab.GetComponent<RageModus>();
		if(rageModus == null)
			Debug.LogError(characterName + " hat kein rageModus script!!!");

		this.platformUserControlMobile = prefab.GetComponent<PlatformUserControlAnalogStickAndButton>();
		this.platformUserControlPC = prefab.GetComponent<PlatformUserControlKeyboard>();
		this.platformAIControl = prefab.GetComponent<PlatformAIControl>();
		if(isAI)
		{
			this.platformUserControlMobile.enabled = false;
			this.platformUserControlPC.enabled = false;
			this.platformAIControl.enabled = true;
		}
		else
		{
			this.platformUserControlMobile.enabled = true;
			this.platformUserControlPC.enabled = true;
			this.platformAIControl.enabled = false;
		}
	}

	public Animator GetAnimator()
	{
		return this.platformAnimator;
	}

	public HealthController getHealthController()
	{
		return this.healthController;
	}

	public PlatformCharacter getPlatformCharacter()
	{
		return this.platformCharacter;
	}

	public RageModus getRageModus()
	{
		return this.rageModus;
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
