using UnityEngine;
using System.Collections;

public class SceneFader : MonoBehaviour {

	public Texture2D texture;
	public float fadeSpeed=2;

	private int nextLevel=1;
	private Rect screenRect;
	private Color currentColor;
	private bool isStarting = true;
	private bool isEnding = false;


	void Awake()
	{
		screenRect = new Rect(0,0,Screen.width,Screen.height);
		currentColor = Color.black;
	}

	// Use this for initialization
	void Start () {
		if(isStarting)
			FadeIn();

		if(isEnding)
			FadeOut();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if(isStarting || isEnding)
		{
			GUI.depth = 0;		//falls andere GUI Elemente auch angezeigt werden wird dieses hier über allen angezeigt (am Nächsten zur Kamera)
			GUI.color = currentColor;
			GUI.DrawTexture(screenRect,texture,ScaleMode.StretchToFill);
		}
	}

	void FadeIn()
	{
		currentColor = Color.Lerp (currentColor,Color.clear,fadeSpeed * Time.deltaTime);
		
		if(currentColor.a <= 0.05F)
		{
			//opazität = 1/transparenz
			currentColor = Color.clear;
			isStarting = false;
		}
	}

	void FadeOut()
	{
		currentColor = Color.Lerp (currentColor,Color.black,fadeSpeed * Time.deltaTime);
		
		if(currentColor.a >= 0.95F)
		{
			//opazität = 1/transparenz
			currentColor.a =1;
			//Application.LoadLevel(nextLevel);
			Application.LoadLevelAsync(nextLevel);		//Scene wird parallel geladen.
		}
	}

	public void switchScene(int nextSceneIndex)
	{
		nextLevel = nextSceneIndex;
		isEnding = true;
		isStarting = false;
	}
}
