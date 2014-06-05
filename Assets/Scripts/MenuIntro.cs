using UnityEngine;
using System.Collections;

public class MenuIntro : MonoBehaviour {

	string buttonText = "Start";
	private SceneFader sceneFader;
	public float speed = 0.1F;
	void Awake()
	{
		sceneFader = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneFader>();
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(10,10,100,50),buttonText))
		{
			//Application.LoadLevel(1);
			sceneFader.switchScene(1);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
