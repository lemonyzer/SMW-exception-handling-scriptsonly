using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float MoveSpeed = 5f;
	
	void Update()
	{
		if( networkView == null || networkView.isMine )
		{
			transform.Translate( new Vector3( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ), 0f ) * MoveSpeed * Time.deltaTime );

			/**
			 * Windows Escape
			 **/
			if (Application.platform == RuntimePlatform.WindowsPlayer ||
			    Application.platform == RuntimePlatform.WindowsWebPlayer ||
			    Application.platform == RuntimePlatform.WindowsEditor)
			{
				if (Input.GetKey(KeyCode.Escape))
				{
					// Insert Code Here (I.E. Load Scene, Etc)
					// OR Application.Quit();
					Application.LoadLevel("MainMenuOld");
					return;
				}
			}
		}
	}
}