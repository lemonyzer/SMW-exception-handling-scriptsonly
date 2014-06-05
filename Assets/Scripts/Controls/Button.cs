using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public Texture2D button;
	public Texture2D buttonPressed;
	public bool isPressed = false;

	// Update is called once per frame
	void Update () {
		isPressed = false;
		guiTexture.texture = button;

		foreach(Touch touch in Input.touches)
		{
			if(!isPressed)
			{
				//noch kein Finger in linker Bildschirmhälfte gefunden
				if( touch.position.x > (Screen.width * 0.5) )
				{
					isPressed = true;
					guiTexture.texture = buttonPressed;
				}
			}
			/*if(guiTexture.HitTest(touch.position))
			{
				isPressed = true;
				guiTexture.texture = buttonPressed;
			}*/
		}
	}
}
