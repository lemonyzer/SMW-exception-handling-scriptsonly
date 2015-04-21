﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileScript : MonoBehaviour {

	public Tileset tileSet;
	public int tilesetPosX;
	public int tilesetPosY;

	public TilesetTile tilesetTile;
	public MapTile mapTile;
	public MapBlock mapBlock;
	public TileType tileType;
	public TileTypeFlag tileTypeFlag;

	// http://answers.unity3d.com/questions/62539/how-to-display-empty-game-object-in-the-editor.html
	// Handles	http://docs.unity3d.com/ScriptReference/Handles.html
	// Gizmos	http://unity3d.com/support/documentation/ScriptReference/Gizmos.html
	//			http://docs.unity3d.com/Manual/ViewModes.html
	// UnityUI	

//	void OnDrawGizmos()
//	{
//
//		if(tilesetTile != null)
//		{
////            Gizmos.DrawIcon
//        }
//		else if(mapTile != null)
//		{
//            
//        }
//		else if(mapBlock != null)
//		{
//            
//        }
//		else if(tileType != null)
//		{
//            
//        }
//		else if(tileTypeFlag != null)
//		{
//            
//        }
//
//
//	}
}
