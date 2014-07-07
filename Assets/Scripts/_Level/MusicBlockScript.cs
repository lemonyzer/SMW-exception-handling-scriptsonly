using UnityEngine;
using System.Collections;
//using System.Collections.Generic;	// für Liste

public class MusicBlockScript : MonoBehaviour {

//	public List<AudioClip> backgroundMusicTracks;
	public AudioClip backgroundMusicStandard;
	public AudioClip backgroundMusicAlternative;
	private bool stdMusic = true;

	private GameObject gameController;
	private Layer layer;
	private AudioSource backgroundMusicSource;


	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
		layer = gameController.GetComponent<Layer>();
		backgroundMusicSource = gameController.GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start () {
		stdMusic = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.layer == layer.head)
		{
			if(stdMusic)
			{
				backgroundMusicSource.clip = backgroundMusicAlternative;
				backgroundMusicSource.Play();
				//				AudioSource.PlayClipAtPoint(powerUpReleaseSound,transform.position,1);
				stdMusic = false;
			}
			else
			{
				backgroundMusicSource.clip = backgroundMusicStandard;
				backgroundMusicSource.Play();
				stdMusic = true;
			}
		}
		else if(other.gameObject.name == "Feet")
		{
			Debug.Log("feet");
			other.gameObject.transform.parent.rigidbody2D.velocity = new Vector2(other.gameObject.transform.parent.rigidbody2D.velocity.x,14f);
			//trampolin sound
		}
	}

	
//	void OnCollisionEnter2D(Collision2D collision)
//	{
//		if(collision.gameObject.layer == layer.feet)
//		{
//			collision.gameObject.rigidbody2D.velocity = new Vector2(collision.gameObject.rigidbody2D.velocity.x,14f);
//		}
//	}
}
