using UnityEngine;
using System.Collections;

public class PlatformAIControl : MonoBehaviour {

	private PlatformCharacter character;
	
	/**
	 * Debugging GUI Element
	 **/
	public GUIText debugging;
	private string debugmsg="";

	/**
	 * AI Variablen
	 **/
	public bool JumpAllowed=true;
	public bool MoveAllowed=true;
	public GameObject target;
	public GameObject closest;
	public bool targetHigher;
	public bool targetAtSameHeight;
	public float heightSaveOffset = 0.5f;
	public float targetDirection;
	public float targetDistance;
	public float jumpRange = 4.0f;
	//intelegent, 
	private float changeDirectionInterval=0.5f; // in Sekunden
	private bool ableToChangeDirection = false;
	private float deltaLastDirectionChange;
	private float deltaLastJump;

	private float inputVelocity;
	private bool inputJump;

	private GameObject gameController;
	private HashID hash;
	private Stats stats;
	
	void Awake() {

		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		stats = gameController.GetComponent<Stats>();
		hash = gameController.GetComponent<HashID>();
	}

	// Use this for initialization
	void Start () {
		character = GetComponent<PlatformCharacter>();
		deltaLastJump =0.0f;
		deltaLastDirectionChange = 0.7f;
	}
	
	// Update is called once per frame
	void Update () {
		CheckTimeBetweenLastDirectionChange();
		//FindClosestGameObjectWithPriority();
		target = FindClosestPlayerWithGameController();
		AIMove();
		character.MoveKeyboard(inputVelocity, inputJump);
	}

	void CheckTimeBetweenLastDirectionChange() {
		deltaLastDirectionChange += Time.deltaTime;
		if(deltaLastDirectionChange > changeDirectionInterval)				// Bot leichter machen
		{																	// alle halbe Sekunde darf er Richtung wechseln
			ableToChangeDirection=true;
			deltaLastDirectionChange=0.0F;
		}
		else
			ableToChangeDirection=false;
	}

//	GameObject FindClosestPlayer() {
//		
//		GameObject[] gos;
//		gos = GameObject.FindGameObjectsWithTag("Player");
//		//GameObject closest;
//		targetDistance = Mathf.Infinity;
//		Vector3 position = transform.position;
//		
//		foreach (GameObject go in gos) {
//			Vector3 diff = go.transform.position - position;
//			float curDistance = diff.sqrMagnitude;
//			if (curDistance < targetDistance) {
//				closest = go;
//				targetDistance = curDistance;
//				
//				if( diff.y < heightSaveOffset )			// save offset!!! physic has no 0.0F precision!
//				{
//					targetHigher = false;
//				}
//				else
//				{
//					targetHigher = true;
//				}
//				
//				if( -10.0f < diff.x && diff.x < 0.0f )			//between -10 and 0
//				{
//					targetDirection = -1;
//				}
//				else if( 10.0f < diff.x && diff.x < 20.0f )		//between 10 and 20
//				{
//					targetDirection = -1;
//				}
//				else 											//else (between 0 and 10)
//				{
//					targetDirection = +1;
//				}
//				
//			}
//		}
//		
//		return closest;
//	}
	
	GameObject FindClosestPlayerWithGameController() {
		
		targetDistance = Mathf.Infinity;
		Vector3 myPosition = transform.position;
		
		foreach (GameObject go in stats.playerList)
		{
			if(go != null)
			{
				if(go.layer != gameObject.layer)
				{
					// Nur andere Spieler können target werden!
					Vector3 diff = go.transform.position - myPosition;		// targetPosition - eigenePosition
																			// diff = negatativ => eigene Position höher
																			// diff = 0 		=> gleiche Höhe
																			// diff = positiv 	=> eigene Position niedriger
					float curDistance = diff.sqrMagnitude;
					if (curDistance < targetDistance) {
						closest = go;
						targetDistance = curDistance;
						
						if( (diff.y <= heightSaveOffset) && (diff.y >= -heightSaveOffset) )			// save offset!!! physic has no 0.0F precision!
						{
							targetHigher = false;
							targetAtSameHeight = true;
						}
						else if(diff.y > heightSaveOffset)
						{
							targetHigher = true;
							targetAtSameHeight = false;
						}
						else
						{
							targetHigher = false;
							targetAtSameHeight = false;
						}
						
						if( -10.0f < diff.x && diff.x < 0.0f )			//between -10 and 0
						{
							targetDirection = -1;
						}
						else if( 10.0f < diff.x && diff.x < 20.0f )		//between 10 and 20
						{
							targetDirection = -1;
						}
						else 											//else (between 0 and 10)
						{
							targetDirection = +1;
						}
						
					}
				}
			}
			//			else
			//				Debug.Log("GameObject layer: " + go.layer + " == Player(Ki) layer: " + gameObject.layer);
		}

//		Debug.Log(character.name + " my position: " + myPosition);
//		Debug.Log(character.name + " hunts " + closest.name + " with position: " + closest.transform.position);
		return closest;
	}

	// Update is called once per frame
	void AIMove () {
		/* Spotting überflüssig, da alle Mitspielderpositionen bekannt sind -> bewege zum nächsten Spieler */
		/*
		spotted = Physics2D.OverlapCircle (spotCheck.position, spottingRadius, whatIsSpotted);
		*/
		
		if(target != null)
		{
			if(!targetHigher)
			{
				//Bot ist höher oder auf gleicher Höhe
				inputJump = false;

				if(ableToChangeDirection)					// Bot leichter machen
					inputVelocity = targetDirection;

				if(targetAtSameHeight)
				{
					if(targetDistance < jumpRange) 
					{
							inputJump = true;				// Target in Jumprange, Springen!
					}
				}
			}
			else if(targetHigher)
			{
				inputJump = true;
			}
		}
		else
		{
			// kein Gegner mehr da
			inputJump = false;
			inputVelocity = 0f;
		}
	}
}
