using UnityEngine;
using System.Collections;
using System.Collections.Generic;			//für Liste

public class Stats : MonoBehaviour {

	private GameObject statsGameObject;
	private string statsmsg;
	public GameObject winEffect;
	public GameObject winEffect2;
	public AudioClip winSound;
	private GameObject backGround;
	public List<GameObject> playerList;
	public List<GameObject> statsList;
//	bool allEnemyDeath=false;
	bool playerLebt=true;
	bool winEffectOn=false;
	bool restart=false;

	Transform currCharacter;		
	Transform currHead;				//has HeadTrigger and HealthController

	void Awake()
	{

	}

	// Use this for initialization
	void Start () {
		statsGameObject = new GameObject();
		statsGameObject.AddComponent<GUIText>();
		statsGameObject.guiText.fontSize = 14;
		statsGameObject.guiText.text = "Stats";
		statsGameObject.guiText.color = Color.black;
		statsGameObject.guiText.pixelOffset = new Vector2(Screen.width-300,Screen.height);
//		Instantiate(statsGameObject);
		//stats.text="testsetest";
		//		stats.pixelOffset = new Rect(Screen.width-200,Screen.height-20,200,100);
//		stats.pixelOffset = new Vector2(Screen.width-300,Screen.height-20);
        //Instantiate(stats,new Vector2(5f,5f),Quaternion.identity);
//		Instantiate(stats);
	}

	public void AddPlayer(GameObject newPlayer)
	{
		playerList.Add(newPlayer);
	}
	
	// Update is called once per frame
	void Update () {
		statsmsg = "Stats\n";
		if(playerList.Count == 0)
		{
			//noch Kein Spieler in der Liste
		}
		else
		{

//			allEnemyDeath = true;
			playerLebt = false;
			foreach(GameObject go in playerList)			
			{
				if(go != null)	// falls Ein Spieler schon destroyed wurde
				{
					float currentLifes;
					currHead = go.transform.Find(Tags.head);
					currentLifes = currHead.GetComponent<HealthController>().currentLifes;
					statsmsg += go.name + ": " + currentLifes + "\n";

					if(go.tag.Equals(Tags.player))
					{																				//abfrage fehlt ob Player gestorben!!!!
						if(currentLifes > 0f)
						{
							playerLebt = true;
						}
					}
					else if(go.tag.Equals(Tags.enemy))
					{
						if(currentLifes > 0f)
						{
//							allEnemyDeath = false;
						}
					}
				}
				else
					playerList.Remove(go);
			}
		}
		if(playerLebt && playerList.Count == 1)
		{
//			if(playerList[0] != null) {
//				//Win!!!
				Debug.Log("YOU WIN!!!");
				currCharacter = playerList[0].transform;
				if(winEffect!=null)
				{
					if(!winEffectOn)
					{
						backGround.renderer.enabled = false;
						winEffect.SetActive(true);
						if(winEffect2!=null)
							winEffect2.SetActive(true);
						if(winSound!=null) {
							transform.GetComponent<AudioSource>().Stop();
							AudioSource.PlayClipAtPoint(winSound,transform.position,1);
						}
						StartCoroutine(RestartSceneDelay());
                        winEffectOn = true;
                    }
                    else
                    {
                        winEffect.transform.position = currCharacter.position;
                        winEffect2.transform.position = currCharacter.position;
                        if(restart)
                            RestartScene();
                    }
                    
                }
//      	}
        }
		else
		{
			statsGameObject.guiText.text = statsmsg;
        }
    }
    
    
    
    IEnumerator RestartSceneDelay()
	{
		yield return new WaitForSeconds(10.0F);
		restart=true;
	}

	void RestartScene()
	{	
		Application.LoadLevel(Application.loadedLevel);
	}
}
