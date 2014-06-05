using UnityEngine;
using System.Collections;

public class AnalogStick : MonoBehaviour {

	//public Texture2D analogStick;
	//public Texture2D analogStickPosition;
	public bool isPressed = false;
	public float deltaX;
	public float deltaY;
	GUITexture texture;

	public float rangeX = Screen.width/2/2;	//Pixelbreite  Bildschirmauflösung/2 /2

	void Start() {
		texture = GetComponent<GUITexture>();
		if(texture != null)
		{
			// texture.pixelInset = new Rect(texture.pixelInset.x,texture.pixelInset.y, texture.pixelInset.width,texture.pixelInset.height);
		}
	}

	// Update is called once per frame
	void Update () {
		isPressed = false;
		//guiTexture.texture = analogStick;
		
		foreach(Touch touch in Input.touches)
		{
			if(!isPressed)
			{
				//noch kein Finger in linker Bildschirmhälfte gefunden
				if( touch.position.x < (Screen.width * 0.5) )
				{
					texture.transform.position = new Vector3(touch.position.x,touch.position.y,texture.transform.position.z);
					/*if(touch.deltaPosition.x > rangeX)
					{
						deltaX = 
					}*/
					deltaX += touch.deltaPosition.x/rangeX;
					if(deltaX > 1.0f)
						deltaX = 1.0f;
					else if(deltaX < -1.0f)
						deltaX = -1.0f;

					isPressed = true;
				}
			}
			/*if(guiTexture.HitTest(touch.position))
			{

				//Debug.Log(touch.position);
				deltaX = (touch.position.x-(texture.pixelInset.x+texture.pixelInset.width * 0.5f))/(texture.pixelInset.width * 0.5f);
				deltaY = (touch.position.y-(texture.pixelInset.y+texture.pixelInset.height * 0.5f))/(texture.pixelInset.height * 0.5f);
				isPressed = true;
				//guiTexture.texture = buttonPressed;
			}*/
		}
		if(!isPressed)
		{
			deltaX = 0.0F;
		}
	}
}
