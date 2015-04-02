﻿using UnityEngine;
using System.Collections;

using UnityEditor;
using UnityEditor.Sprites;

using System;
using System.Collections.Generic;
//using System.Xml;

public class CharacterCreationHelper : EditorWindow {

	public SmwCharacter smwCharacter;

	public SpriteAlignment spriteAlignment = SpriteAlignment.Center;
	public Vector2 customOffset = new Vector2(0.5f, 0.5f);

	public Sprite unslicedSprite;
	public Sprite[] slicedSprite;
//	public Texture2D texture2d;
	public int subSpritesCount = 6;
	public int pixelPerUnit = 32;
	public int pixelSizeWidth = 32;
	public int pixelSizeHeight = 32;

	[MenuItem ("Window/Character Editor %#e")]
	static void Init () {
		GetWindow (typeof (CharacterCreationHelper));
	}

//	void OnEnable ()
//	{
//		if(EditorPrefs.HasKey("ObjectPath"))
//		{
//			string objectPath = EditorPrefs.GetString("ObjectPath");
//			smwCharacter = AssetDatabase.LoadAssetAtPath (objectPath, typeof(SmwCharacter)) as SmwCharacter;
//		}
//	}

	void OnGUI ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("SMW Character Editor", EditorStyles.boldLabel);
		if (smwCharacter != null)
		{
			// wenn ein SMW Character gesetzt ist
			if (GUILayout.Button("Show SMW Character SO"))
			{
				// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = smwCharacter;
			}
		}
		if (GUILayout.Button("New Character"))
		{
			// neuen Character erzeugen
			smwCharacter = CreateSmwCharacter.CreateAssetAndSetup();
		}
		GUILayout.EndHorizontal ();

		smwCharacter = EditorGUILayout.ObjectField("SMW Character SO", smwCharacter, typeof(SmwCharacter), false) as SmwCharacter;
//		GUI.enabled = false;

//		GUILayout.BeginVertical ();
//		texture2d = EditorGUILayout.ObjectField("Texture2D", texture2d, typeof(Texture2D), false) as Texture2D;
//		GUILayout.EndVertical ();

//		xmlAsset = EditorGUILayout.ObjectField("XML Source", xmlAsset, typeof (TextAsset), false) as TextAsset;

		GUILayout.BeginHorizontal ();
		unslicedSprite = EditorGUILayout.ObjectField("Unsliced Sprite", unslicedSprite, typeof(Sprite), false) as Sprite;

		UnityEditor.TextureImporter myImporter = null;
		bool enabled_SpriteSet = GUI.enabled;
		if(unslicedSprite != null)
		{
			myImporter = UnityEditor.TextureImporter.GetAtPath ( AssetDatabase.GetAssetPath(unslicedSprite) ) as TextureImporter ;
		}
		else
		{
			enabled_SpriteSet = false;
			GUI.enabled = false;
		}
		//smwCharacter.SetCharSprites( EditorGUILayout.ObjectField("Sliced Sprite", smwCharacter. );
		
		//		UnityEditor.TextureImporter importer = new UnityEditor.TextureImporter();
		//importer.assetPath = AssetDatabase.GetAssetPath(sprite); // Read-only

		GUILayout.EndHorizontal ();

		// GUI SubSpriteCount
		subSpritesCount = EditorGUILayout.IntSlider("Sub Sprites #", subSpritesCount, 1, 6);

		// GUI pixelPerUnit
		pixelPerUnit = EditorGUILayout.IntField("Pixel per Unit", pixelPerUnit);

		GUILayout.BeginHorizontal ();
		// GUI: Pivot
		spriteAlignment = (SpriteAlignment) EditorGUILayout.EnumPopup("Pivot", spriteAlignment);
//		bool customPivotEnabled = enabled_SpriteSet;
		if (spriteAlignment != SpriteAlignment.Custom) {
			// deaktiviere custom Offset
			GUI.enabled = false;
		}
		// GUI: Custom Pivot
		EditorGUILayout.Vector2Field("Custom Offset", customOffset);
		GUILayout.EndHorizontal ();

		GUI.enabled = enabled_SpriteSet;

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button("Sprite Info"))
		{
			// Spriteinfo ausgeben
			SpriteAssetInfo(myImporter);
		}
		
//		if (GUILayout.Button("Setup Sprite"))
//		{
//			SetupSprite(myImporter);
//			
//			// Spriteinfo ausgeben
//			SpriteAssetInfo(myImporter);
//		}
		if (GUILayout.Button("meta. Slice"))
		{
			//Grid Slice
			PerformMetaSlice(unslicedSprite);
		}

		GUI.enabled = false;
//		if (GUILayout.Button("man. Slice"))
//		{
//			//Grid Slice
//			ManualSliceSprite(myImporter);
//		}
		GUILayout.EndHorizontal ();
		GUI.enabled = true;
	}

//	[SerializeField] private TextAsset xmlAsset;
//	public TextureImporter importer;

	// thx to http://www.toxicfork.com/154/importing-xml-spritesheet-into-unity3d

	private void PerformMetaSlice(Sprite sprite)
	{
		if(sprite != null)
		{
			UnityEditor.TextureImporter myImporter = null;
			myImporter = UnityEditor.TextureImporter.GetAtPath ( AssetDatabase.GetAssetPath(sprite) ) as TextureImporter ;

			bool failed = false;
			List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

			slicedSprite = new Sprite[subSpritesCount];
			// Calculate SpriteMetaData (sliced SpriteSheet)
			for(int i=0; i<slicedSprite.Length; i++)
			{
				try {

					SpriteMetaData spriteMetaData = new SpriteMetaData
					{
						alignment = (int)spriteAlignment,
						border = new Vector4(),
						name = System.IO.Path.GetFileNameWithoutExtension(myImporter.assetPath) + "_" + i,
						pivot = GetPivotValue(spriteAlignment, customOffset),
						rect = new Rect(i*pixelSizeWidth, 	0, pixelSizeWidth, 	pixelSizeHeight)
					};

					metaDataList.Add(spriteMetaData);

				}
				catch (Exception exception) {
					failed = true;
					Debug.LogException(exception);
				}
			}

//XML not needed
//		XmlDocument document = new XmlDocument();
//		document.LoadXml(xmlAsset.text);
		
//		XmlElement root = document.DocumentElement;
//		if (root.Name == "TextureAtlas")
//		{
//			bool failed = false;
//			
//			Texture2D texture = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture2D;
//			int textureHeight = texture.height;
//			
//			List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();
//			
//			foreach (XmlNode childNode in root.ChildNodes)
//			{
//				if (childNode.Name == "SubTexture") {
//					try {
//						int width = Convert.ToInt32(childNode.Attributes["width"].Value);
//						int height = Convert.ToInt32(childNode.Attributes["height"].Value);
//						int x = Convert.ToInt32(childNode.Attributes["x"].Value);
//						int y = textureHeight - (height + Convert.ToInt32(childNode.Attributes["y"].Value));
//						
//						SpriteMetaData spriteMetaData = new SpriteMetaData
//						{
//							alignment = (int)spriteAlignment,
//							border = new Vector4(),
//							name = childNode.Attributes["name"].Value,
//							pivot = GetPivotValue(spriteAlignment, customOffset),
//							rect = new Rect(x, y, width, height)
//						};
//						
//						metaDataList.Add(spriteMetaData);
//					}
//					catch (Exception exception) {
//						failed = true;
//						Debug.LogException(exception);
//					}
//				}
//				else
//				{
//					Debug.LogError("Child nodes should be named 'SubTexture' !");
//					failed = true;
//				}
//			}
			
			if (!failed) {
				myImporter.spritePixelsPerUnit = pixelPerUnit;
				myImporter.spriteImportMode = SpriteImportMode.Multiple; 
				myImporter.spritesheet = metaDataList.ToArray();
				
				EditorUtility.SetDirty(myImporter);
				
				try
				{
					AssetDatabase.StartAssetEditing();
					AssetDatabase.ImportAsset(myImporter.assetPath);
				}
				finally
				{
					AssetDatabase.StopAssetEditing();
					//Close();
				}
			}
		}
		else
		{
			Debug.LogError("sprite == null");
		}
	}



	//SpriteEditorUtility
	public static Vector2 GetPivotValue(SpriteAlignment alignment, Vector2 customOffset)
	{
		switch (alignment)
		{
		case SpriteAlignment.Center:
			return new Vector2(0.5f, 0.5f);
		case SpriteAlignment.TopLeft:
			return new Vector2(0.0f, 1f);
		case SpriteAlignment.TopCenter:
			return new Vector2(0.5f, 1f);
		case SpriteAlignment.TopRight:
			return new Vector2(1f, 1f);
		case SpriteAlignment.LeftCenter:
			return new Vector2(0.0f, 0.5f);
		case SpriteAlignment.RightCenter:
			return new Vector2(1f, 0.5f);
		case SpriteAlignment.BottomLeft:
			return new Vector2(0.0f, 0.0f);
		case SpriteAlignment.BottomCenter:
			return new Vector2(0.5f, 0.0f);
		case SpriteAlignment.BottomRight:
			return new Vector2(1f, 0.0f);
		case SpriteAlignment.Custom:
			return customOffset;
		default:
			return Vector2.zero;
		}
	}


































	void ManualSliceSprite(TextureImporter importer)
	{
		if(importer != null && unslicedSprite != null)
		{
			SpriteAssetInfo(importer);

			// kopiere original SpriteSheet
			string copyPath =  System.IO.Path.GetDirectoryName(importer.assetPath) + "/" +
				System.IO.Path.GetFileNameWithoutExtension(importer.assetPath) + " sliced.asset";
			Debug.Log("copy Path = " + copyPath);
			AssetDatabase.CopyAsset(importer.assetPath, copyPath);
//			importer.SaveAndReimport();	

			// lade kopiertes SpriteSheet
			Sprite newSprite = AssetDatabase.LoadAssetAtPath(copyPath, typeof(Sprite)) as Sprite;
//			TextureImporter copyImporter = UnityEditor.TextureImporter.GetAtPath ( AssetDatabase.GetAssetPath(newSprite) ) as TextureImporter ;
//			SpriteAssetInfo(copyImporter);
//			AssetDatabase.CreateAsset(newSprite, importer.assetPath + " sliced");

			slicedSprite = new Sprite[subSpritesCount];
//	ok		Debug.Log("slicedSprite Array länge: " + slicedSprite.Length);

			// Slice SpriteSheet
			for(int i=0; i<slicedSprite.Length; i++)
			{
//	ok			Debug.Log("Loop i = " + i);
//	ok			Debug.Log("Sprite rect = " + unslicedSprite.rect);
				// left, top, width, height
				Rect rect = new Rect(i*pixelSizeWidth, 	0, pixelSizeWidth, 	pixelSizeHeight);

				// setze Pivot für jedes Sprite
				Vector2 pivot = new Vector2 (0.5f, 0.5f);

				// erhalte sliced Texture
				slicedSprite[i] = Sprite.Create(unslicedSprite.texture, rect, pivot, pixelPerUnit);

				// setze name 
				slicedSprite[i].name = System.IO.Path.GetFileNameWithoutExtension(importer.assetPath) + "_" + i; 
			}

			if(slicedSprite[0] != null)
			{
				if(slicedSprite[0].texture != null)
				{

					for (int i=subSpritesCount-1; i>=0; i--)
					{
						AssetDatabase.AddObjectToAsset(slicedSprite[i], newSprite);
					}

					// ist nicht gleiches format wie mit Sprite Editor geschnitten
					//AssetDatabase.CreateAsset( slicedSprite[0], importer.assetPath + "_sliced_" + ".asset");
//					AssetDatabase.AddObjectToAsset(slicedSprite[1], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[2], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[3], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[4], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[5], unslicedSprite);

					// geht nicht: unslicedSprite ist schon als Asset gespeichert meldet Console
//					AssetDatabase.CreateAsset( unslicedSprite, importer.assetPath + "_sliced_" + ".asset");
//					AssetDatabase.AddObjectToAsset(slicedSprite[0], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[1], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[2], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[3], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[4], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[5], unslicedSprite);
					//AssetDatabase.AddObjectToAsset(slicedSprite[1], importer.assetPath + " sliced.asset");
					
					// Reimport the asset after adding an object.
					// Otherwise the change only shows up when saving the project
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newSprite));
				}
			}

		}
		else
		{
			Debug.LogError("importer oder unslicedSprite == null");
		}
	}

	void SetupSprite(TextureImporter importer)
	{
		importer.spritePixelsPerUnit = pixelPerUnit;
		importer.spriteImportMode = SpriteImportMode.Multiple;
		importer.SaveAndReimport();									// sobald am Asset was geändert wurde speichern
	}

	void SpriteAssetInfo(TextureImporter importer)
	{
		// Spriteinfo ausgeben
		Debug.Log("Path = " + importer.assetPath );
		Debug.Log("Import Mode = " + importer.spriteImportMode.ToString() );
		Debug.Log("Pixel Per Unit = " + importer.spritePixelsPerUnit.ToString() );
	}

}
