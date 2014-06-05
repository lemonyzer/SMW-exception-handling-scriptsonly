using UnityEngine;
using System.Collections;
using System.Collections.Generic;			//für Liste

public class Stats : MonoBehaviour {

	public GameObject winEffect;
	public GameObject winEffect2;
	public AudioClip winSound;
	public GameObject backGround;
	public List<GameObject> player;
	public List<GameObject> stats;
	bool allEnemyDeath=false;
	bool playerLebt=false;
	bool winEffectOn=false;
	
	Transform currCharacter;		
	Transform currHead;				//has HeadTrigger and HealthController

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		allEnemyDeath = true;
		foreach(GameObject go in player)			
		{
			if(go != null)	// falls Ein Spieler schon destroyed wurde
			{
				if(go.tag.Equals("Player"))
				{																				//abfrage fehlt ob Player gestorben!!!!
					currHead = go.transform.Find("Head");
					if(currHead.GetComponent<HealthController>().currentLifes > 0.0f)
					{
						playerLebt = true;
					}
				}
				else if(go.tag.Equals("Enemy"))
				{
					currHead = go.transform.Find("Head");
					if(currHead.GetComponent<HealthController>().currentLifes > 0.0f)
					{
						allEnemyDeath = false;
						break;
					}
				}
			}
		}
		if(allEnemyDeath)
		{
			if(player[0] != null) {
				//Win!!!
				Debug.Log("YOU WIN!!!");
				currCharacter = player[0].transform;
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
			}
		}
	}

	bool restart=false;

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
