using UnityEngine;
using System.Collections;
//using System.Collections.Generic;	// für Liste

public class MusicBlockScript : MonoBehaviour {

//	public List<AudioClip> backgroundMusicTracks;
	public AudioClip backgroundMusicStandard;
	public AudioClip backgroundMusicAlternative;
	private bool stdMusic = true;

	private string targetTag = "Head";
	private GameObject gameController;
	private HashID hash;
	private AudioSource backgroundMusicSource;


	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		hash = gameController.GetComponent<HashID>();
		backgroundMusicSource = gameController.GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start () {
		stdMusic = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag != null)
		{
			if(other.gameObject.tag == targetTag)
			{
				if(stdMusic)
				{
					backgroundMusicSource.clip = backgroundMusicAlternative;
					backgroundMusicSource.Play();
//					AudioSource.PlayClipAtPoint(powerUpReleaseSound,transform.position,1);
					stdMusic = false;
				}
				else
				{
					backgroundMusicSource.clip = backgroundMusicStandard;
					backgroundMusicSource.Play();
					stdMusic = true;
				}
				
			}
		}
	}
}
