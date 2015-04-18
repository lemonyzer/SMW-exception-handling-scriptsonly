
//#define NUM_AUTO_FILTERS	12
//#define MAPWIDTH			20			//width of the map
//#define MAPHEIGHT			15			//height of the map
//#define TILESETUNKNOWN	-3

#define SDL_LITTLE_ENDIAN
#define SDL_BYTEORDER
//#define SDL_BIG_ENDIAN
#undef SDL_BIG_ENDIAN
//	#define SDL_BYTEORDER = SDL_LITTLE_ENDIAN

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class MapTile
{
	[SerializeField]
	public TileType iType;
	[SerializeField]
	public int iFlags;
};

[Serializable]
public class MapBlock
{
	[SerializeField]
	public short iType;
	//	public short iSettings[NUM_BLOCK_SETTINGS];
	[SerializeField]
	public short[] iSettings;
	[SerializeField]
	public bool fHidden;

	public MapBlock()
	{
		iSettings = new short[Globals.NUM_BLOCK_SETTINGS];
	}

	public MapBlock(short iType)
	{
		this.iType = iType;
		iSettings = new short[Globals.NUM_BLOCK_SETTINGS];
		fHidden = false;
	}
};

[Serializable]
public class TilesetTile
{
	[SerializeField]
	public short iID;
	[SerializeField]
	public short iCol;
	[SerializeField]
	public short iRow;
};

[Serializable]
public class MovingPlatform {

	[SerializeField]
	public int iTileWidth;
	[SerializeField]
	public int iTileHeight;

	public MovingPlatform()
	{

	}

}

[Serializable]
public class TilesetTranslation
{
	[SerializeField]
	public short iID;
//	public char[] szName;	// TODO 128 -> TILESET_TRANSLATION_CSTRING_SIZE
	// szName ersetzt durch string!!!
	[SerializeField]
	public string Name;

	public TilesetTranslation()
	{
		iID = 0;
		Name = null;
	}
};

[Serializable]
public class MapItem
{
	public short itype;
	public short ix;
	public short iy;
};

[Serializable]
public class MapHazard
{
	public short itype;
	public short ix;
	public short iy;
	
	public short[] iparam = new short[Globals.NUMMAPHAZARDPARAMS];
	public float[] dparam = new float[Globals.NUMMAPHAZARDPARAMS];
};

[Serializable]
public class Warp
{
	public short direction;
	public short connection;
	public short id;
};

[Serializable]
public class WarpExit
{
	public short direction;
	public short connection;
	public short id;
	
	public short x; //Player location where player warps out of
	public short y; 
	
	public short lockx;  //Location to display lock icon
	public short locky;  
	
	public short warpx;  //map grid location for first block in warp
	public short warpy;
	public short numblocks;  //number of warp blocks for this warp
	
	public short locktimer;  //If > 0, then warp is locked and has this many frames left until unlock
};

[Serializable]
public class SpawnArea
{
	public short left;
	public short top;
	public short width;
	public short height;
	public short size;
};

[Serializable]
public class SDL_Rect
{
	public short x;
	public short y;
	public ushort w;
	public ushort h;
};

[Serializable]
public class Map : ScriptableObject {

	public void OnEnable()
	{
		Debug.LogWarning(this.ToString() + " OnEnable()");
		if(m_TilesetManager == null)
		{
			Debug.LogWarning("m_TilesetManager == NULL");
		}
		else
		{
			Debug.Log("m_TilesetManager is set");
		}
	}

	public TilesetTile[,,] GetMapData()
	{
		return mapdata;
	}

	//Converts the tile type into the flags that this tile carries (solid + ice + death, etc)
//	short[] g_iTileTypeConversion = new short[Globals.NUMTILETYPES] = {0, 1, 2, 5, 121, 9, 17, 33, 65, 6, 21, 37, 69, 3961, 265, 529, 1057, 2113, 4096};
	[SerializeField]
	short[] g_iTileTypeConversion = new short[] {0, 1, 2, 5, 121, 9, 17, 33, 65, 6, 21, 37, 69, 3961, 265, 529, 1057, 2113, 4096};

//	public Map(TilesetManager tilesetManager)
//	{
//		this.m_TilesetManager = tilesetManager;
//	}

	[SerializeField]
	int[] m_Version = new int[] {0, 0, 0, 0};
//	int[] g_iVersion = new int[] {0, 0, 0, 0};
	[SerializeField]
	TilesetManager m_TilesetManager;

	[SerializeField]
	int iNumPlatforms = 0;
	[SerializeField]
	int iPlatformCount = 0;
	[SerializeField]
	int iHazardCount = 0;
	[SerializeField]
	int iIceCount = 0;

	//	TilesetTile	mapdata[MAPWIDTH][MAPHEIGHT][MAPLAYERS];
	//	MapTile		mapdatatop[MAPWIDTH][MAPHEIGHT];
	//	MapBlock	objectdata[MAPWIDTH][MAPHEIGHT];
	//	IO_Block*   blockdata[MAPWIDTH][MAPHEIGHT];
	//	bool		nospawn[NUMSPAWNAREATYPES][MAPWIDTH][MAPHEIGHT];
	//	bool[] 		fAutoFilter = new bool[NUM_AUTO_FILTERS];

	[SerializeField]
	TilesetTile[,,] mapdata;	// komplett eingelesene Tiles der Map
	[SerializeField]
	MapTile[,] mapdatatop;		// Oberste Kayer der eingelesenen Map
	[SerializeField]
	MapBlock[,] objectdata;		// ka.

	[SerializeField]
	TilesetTile[,] platformTiles;
	[SerializeField]
	MapTile[,] platformTileTypes;

	[SerializeField]
	int iNumMapItems;
	[SerializeField]
	MapItem[] mapItems;

	[SerializeField]
	int iNumMapHazards;
	[SerializeField]
	MapHazard[] mapHazards;

	[SerializeField]
	short[] eyecandy; //= new short[Globals.NUMEYECANDY];
	[SerializeField]
	int musicCategoryID;

	[SerializeField]
	Warp[,] warpdata;//[MAPWIDTH][MAPHEIGHT];
	[SerializeField]
	bool[,,] nospawn;

	[SerializeField]
	int maxConnection;
	[SerializeField]
	int numwarpexits;
	[SerializeField]
	WarpExit[] warpexits;//[MAXWARPS];

	[SerializeField]
	short[] totalspawnsize;//[NUMSPAWNAREATYPES];
	[SerializeField]
	short[]	numspawnareas;//[NUMSPAWNAREATYPES];
	[SerializeField]
	SpawnArea[,] spawnareas;//[NUMSPAWNAREATYPES][MAXSPAWNAREAS];

	[SerializeField]
	int numdrawareas;

	[SerializeField]
	SDL_Rect[] drawareas;

	[SerializeField]
	int iNumRaceGoals;
	[SerializeField]
	Vector2[] racegoallocations;

	[SerializeField]
	int iNumFlagBases;
	[SerializeField]
	Vector2[] flagbaselocations;


	
//	IO_Block[,] blockdata;
	[SerializeField]
	bool[] fAutoFilter = new bool[Globals.NUM_AUTO_FILTERS];

//	char szBackgroundFile[128];	// BACKGROUND_CSTRING_SIZE
	[SerializeField]
	string szBackgroundFile;

	[SerializeField]
	TilesetTranslation[] translations;

	[SerializeField]
	int[] translationid;
	[SerializeField]
	int[] tilesetwidths;
	[SerializeField]
	int[] tilesetheights;

	[SerializeField]
	short[] iSwitches;
	
	[SerializeField]
	MovingPlatform[] platforms;
	[SerializeField]
	List<MovingPlatform> platformsList = new List<MovingPlatform>();

	[SerializeField]
	MovingPlatform[] tempPlatforms = new MovingPlatform[Globals.PLATFORMDRAWLAYERS];
	[SerializeField]
	List<MovingPlatform> tempPlatformsList = new List<MovingPlatform>();

	[SerializeField]
	MovingPlatform[] platformdrawlayer = new MovingPlatform[Globals.PLATFORMDRAWLAYERS];
	[SerializeField]
	List<MovingPlatform> platformdrawlayerList = new List<MovingPlatform>();

	public void SetTiletsetManager(TilesetManager tilesetManager)
	{
		this.m_TilesetManager = tilesetManager;
		if(m_TilesetManager != null)
			Debug.Log("<color=green>m_TilesetManager is set</color>");
		else
			Debug.LogWarning("m_TilesetManager == NULL");
	}

	public TilesetManager GetTilesetManager()
	{
		return this.m_TilesetManager;
	}

	void initSwitches()
	{
		iSwitches = new short[Globals.NUM_SWITCHES];
		for(short iSwitch = 0; iSwitch < iSwitches.Length; iSwitch++)
			iSwitches[iSwitch] = 0;
	}

	public void loadMap(string filePath, ReadType iReadType)
	{
		FileStream fs = new FileStream(filePath, FileMode.Open);
		BinaryReader binReader = new BinaryReader(fs);

		Debug.Log("FileStream.Length = " + fs.Length);

		// check if datei in pfad != null	//TODO
		if(fs.Length <= 0)
		{
			Debug.LogError("FileStream.Length <= 0");
			binReader.Close();
			fs.Close();
			return;
		}

		//Load version number
		m_Version = new int[Globals.VERSIONLENGTH];
		Debug.Log("version.length = " + m_Version.Length);
		ReadIntChunk(m_Version, (uint)m_Version.Length, binReader);
		string sversion = "";
		for(int i=0; i<m_Version.Length; i++)
		{
			if(i != m_Version.Length -1)
				sversion += m_Version[i] + ", ";  
			else
				sversion += m_Version[i];  
		}
		Debug.Log("Map Version = " + sversion);

		if(VersionIsEqualOrAfter(m_Version, 1, 8, 0, 0))
		{
			//Read summary information here
			Debug.Log("Version is Equal or After: 1, 8, 0, 0");

			try
			{
				loadMapVersionEqualOrAfter1800(binReader, iReadType, m_Version);
			}
			// Catch the EndOfStreamException and write an error message.
			catch (EndOfStreamException e)
			{
				Debug.LogError("Error writing the data.\n" + e.GetType().Name);
			}
			// Catch the EndOfStreamException and write an error message.
			catch (ObjectDisposedException e)
			{
				Debug.LogError("Error writing the data.\n" + e.GetType().Name);
			}
			// Catch the EndOfStreamException and write an error message.
			catch (IOException e)
			{
				Debug.LogError("Error writing the data.\n" + e.GetType().Name);
			}
			catch (Exception ex)
			{
				Debug.LogError("loadMapVersionEqualOrAfter1800 failed\n" + ex);
			}
			finally
			{

			}
		}

		// close stream and file
		binReader.Close();
		fs.Close();
	}

	bool VersionIsEqualOrAfter(int[] iVersion, short iMajor, short iMinor, short iMicro, short iBuild)
	{
		if(iVersion[0] > iMajor)
			return true;
		
		if(iVersion[0] == iMajor)
		{
			if(iVersion[1] > iMinor)
				return true;
			
			if(iVersion[1] == iMinor)
			{
				if(iVersion[2] > iMicro)
					return true;
				
				if(iVersion[2] == iMicro)
				{
					return iVersion[3] >= iBuild;
				}
			}
		}
		
		return false;
	}

	void loadMapVersionEqualOrAfter1800(BinaryReader binReader, ReadType iReadType, int[] version)
	{
		Debug.Log("loading map ");	//TODO mapname
		
		if(iReadType == ReadType.read_type_preview)
			Debug.LogWarning("(preview)");

		//Read summary information here
		
		int[] iAutoFilterValues = new int[Globals.NUM_AUTO_FILTERS + 1];
		ReadIntChunk(iAutoFilterValues, Globals.NUM_AUTO_FILTERS + 1, binReader);
		
		for(short iFilter = 0; iFilter < Globals.NUM_AUTO_FILTERS; iFilter++)
		{
			Debug.Log("fAutoFilter["+iFilter+"] = " + iAutoFilterValues[iFilter] + " von " + fAutoFilter.Length);
			fAutoFilter[iFilter] = iAutoFilterValues[iFilter] > 0;
		}
		
		if(iReadType == ReadType.read_type_summary)
		{
			Debug.LogWarning("summary only");
//			binReader.Close();
			return;
		}

		//clearPlatforms();

		//Load tileset information
		short iNumTilesets = (short) ReadInt(binReader);

		Debug.Log("iNumTilesets = " + iNumTilesets + " Anzahl an Tileset Translations");
		translations = new TilesetTranslation[iNumTilesets];

		short iMaxTilesetID = 0; //Figure out how big the translation array needs to be
		for(short iTileset = 0; iTileset < iNumTilesets; iTileset++)
		{
			Debug.LogWarning("Tileset Translation: " + iTileset+1 + " von " + iNumTilesets);
			
			short iTilesetID = (short) ReadInt(binReader);
			Debug.Log("iTileset = " + iTileset + ", iTilesetID = " + iTilesetID + ", iMaxTilesetID = " + iMaxTilesetID);

			translations[iTilesetID] = new TilesetTranslation();

			if(translations == null)
				Debug.LogError("translation == null");
			else
				Debug.Log("translation != null");

			if(translations[iTilesetID] == null)
				Debug.LogError("translation["+iTilesetID+"] == null");
			else
				Debug.Log("translation["+iTilesetID+"] != null");
				

			Debug.Log("translation[iTileset].iID = " + translations[iTileset].iID);
			translations[iTileset].iID = iTilesetID;
			
			if(iTilesetID > iMaxTilesetID)
				iMaxTilesetID = iTilesetID;

			// Funktioniert, erste Zeichen fehlt jedoch
			// ReadString erwartet einen 7-Bit langen int-Wert der die länge des zu lesenden Strings angibt
//			string tilesetName = ReadString(TILESET_TRANSLATION_CSTRING_SIZE,binReader);
//			Debug.Log(tilesetName);

			//TODO NOTE: char array in struct kann nicht direkt adressiert werden, kein Ahnung warum. ersetzt durch string.
//			translation[iTileset].szName = new char[TILESET_TRANSLATION_CSTRING_SIZE];
//			ReadString(translation[iTileset].szName, TILESET_TRANSLATION_CSTRING_SIZE, binReader);
//			Debug.Log(new string(translation[iTileset].szName));
//			Debug.Log("iTileset = " + iTileset + ", iID = " + iTilesetID + ", szName = " + new string(translation[iTileset].szName) + ", iMaxTilesetID = " + iMaxTilesetID); 
			//TODO NOTE: char array in struct kann nicht direkt adressiert werden, kein Ahnung warum. ersetzt durch string.

			translations[iTileset].Name = ReadString(Globals.TILESET_TRANSLATION_CSTRING_SIZE, binReader);
			Debug.Log("TilesetName in struct object: " + translations[iTileset].Name);
			Debug.Log("iTileset = " + iTileset + ", iID = " + iTilesetID + ", Name = " + translations[iTileset].Name + ", iMaxTilesetID = " + iMaxTilesetID); 
		}

		
		translationid = new int[iMaxTilesetID + 1];
		tilesetwidths = new int[iMaxTilesetID + 1];
		tilesetheights = new int[iMaxTilesetID + 1];

		for(short iTileset = 0; iTileset < iNumTilesets; iTileset++)
		{
			short iID = translations[iTileset].iID;
//			translationid[iID] = g_tilesetmanager.GetIndexFromName(translation[iTileset].szName);
			translationid[iID] = m_TilesetManager.GetIndexFromName(translations[iTileset].Name);
			
			if(translationid[iID] == (int) Globals.TILESETUNKNOWN)	//TODO achtung int cast
			{
				tilesetwidths[iID] = 1;
				tilesetheights[iID] = 1;
			}
			else
			{
				tilesetwidths[iID] = m_TilesetManager.GetTileset(translationid[iID]).width;
				tilesetheights[iID] = m_TilesetManager.GetTileset(translationid[iID]).height;
			}
			Debug.Log("Tileset width = " + tilesetwidths[iID] + ", height = " + tilesetheights[iID]);
		}

		mapdata = new TilesetTile[Globals.MAPWIDTH, Globals.MAPHEIGHT, Globals.MAPLAYERS];	// mapdata, hier werden die eingelesenen Daten gespeichert
		objectdata = new MapBlock[Globals.MAPWIDTH, Globals.MAPHEIGHT];

		//2. load map data
		Debug.LogWarning("reading and filling mapdata array BEGINN");
		for(int y = 0; y < Globals.MAPHEIGHT; y++)
		{
			for(int x = 0; x < Globals.MAPWIDTH; x++)
			{
				for(int l = 0; l < Globals.MAPLAYERS; l++)
				{
//					TilesetTile * tile = &mapdata[i][j][k];	// zeigt auf aktuelles Element in mapdata
					mapdata[x, y, l] = new TilesetTile();
					TilesetTile tile = mapdata[x, y, l];
					tile.iID = ReadByteAsShort(binReader);
					tile.iCol = ReadByteAsShort(binReader);
					tile.iRow = ReadByteAsShort(binReader);
					
					if(tile.iID >= 0)
					{
						if(tile.iID > iMaxTilesetID)
							tile.iID = 0;
						
						//Make sure the column and row we read in is within the bounds of the tileset
						if(tile.iCol < 0 || tile.iCol >= tilesetwidths[tile.iID])
							tile.iCol = 0;
						
						if(tile.iRow < 0 || tile.iRow >= tilesetheights[tile.iID])
							tile.iRow = 0;
						
						//Convert tileset ids into the current game's tileset's ids
						tile.iID = (short) translationid[tile.iID];
					}
				}
				objectdata[x,y] = new MapBlock();
				objectdata[x,y].iType = ReadByteAsShort(binReader);
				objectdata[x,y].fHidden = ReadBool(binReader);
//				Debug.LogWarning("objectdata["+x+", "+y+"].fHidden = " + objectdata[x,y].fHidden.ToString()); 
			}
		}
		Debug.LogWarning("reading and filling mapdata array DONE");
		
		//Read in background to use
		szBackgroundFile = ReadString(Globals.BACKGROUND_CSTRING_SIZE, binReader);
		Debug.Log("BackgroundFile = " + szBackgroundFile);

		initSwitches();

		//Read on/off switches
		for(short iSwitch = 0; iSwitch < iSwitches.Length; iSwitch++)
		{
			iSwitches[iSwitch] = (short)ReadInt(binReader);
			Debug.Log("readed iSwitches["+iSwitch+"] = " + iSwitches[iSwitch]);
		}

		bool fPreview;
		if(iReadType == ReadType.read_type_preview)
			fPreview = true;
		else
			fPreview = false;


		loadPlatforms(binReader, fPreview, version, translationid, tilesetwidths, tilesetheights, iMaxTilesetID);

		loadingRest(binReader, iReadType);

	}

	void loadingRest(BinaryReader binReader, ReadType iReadType)
	{
		//All tiles have been loaded so the translation is no longer needed
//		delete [] translationid;
//		delete [] tilesetwidths;
//		delete [] tilesetheights;

		Debug.LogWarning("reading more MapData");

		//Load map items (like carryable spikes and springs)
		Debug.Log("reading MapItems");
		iNumMapItems = ReadInt(binReader);
		Debug.Log("iNumMapItems = " + iNumMapItems);
		mapItems = new MapItem[iNumMapItems];
		for(int j = 0; j < iNumMapItems; j++)
		{
			mapItems[j] = new MapItem();
			mapItems[j].itype = (short) ReadInt(binReader);
			mapItems[j].ix = (short) ReadInt(binReader);
			mapItems[j].iy = (short) ReadInt(binReader);
		}
		
		//Load map hazards (like fireball strings, rotodiscs, pirhana plants)
		Debug.Log("reading MapHazards");
		iNumMapHazards = ReadInt(binReader);
		Debug.Log("iNumMapHazards = " + iNumMapHazards);
		mapHazards = new MapHazard[iNumMapHazards];
		for(short iMapHazard = 0; iMapHazard < iNumMapHazards; iMapHazard++)
		{
			mapHazards[iMapHazard].itype = (short) ReadInt(binReader);
			mapHazards[iMapHazard].ix = (short) ReadInt(binReader);
			mapHazards[iMapHazard].iy = (short) ReadInt(binReader);
			
			for(short iParam = 0; iParam < Globals.NUMMAPHAZARDPARAMS; iParam++)
				mapHazards[iMapHazard].iparam[iParam] = (short) ReadInt(binReader);
			
			for(short iParam = 0; iParam < Globals.NUMMAPHAZARDPARAMS; iParam++)
				mapHazards[iMapHazard].dparam[iParam] = ReadFloat(binReader);
		}

		eyecandy = new short[Globals.NUMEYECANDY];

		//For all layers if the map format supports it
		if(VersionIsEqualOrAfter(m_Version, 1, 8, 0, 2))
		{
			eyecandy[0] = (short)ReadInt(binReader);
			eyecandy[1] = (short)ReadInt(binReader);
		}
		
		//Read in eyecandy to use
		eyecandy[2] = (short)ReadInt(binReader);
		
		musicCategoryID = ReadInt(binReader);

		if(mapdatatop == null)
			mapdatatop = new MapTile[Globals.MAPWIDTH, Globals.MAPHEIGHT];	// wenn keine Platform in der map gefunden wurde ist Array nicht angelegt

		warpdata = new Warp[Globals.MAPWIDTH, Globals.MAPHEIGHT];
		nospawn = new bool[Globals.NUMSPAWNAREATYPES, Globals.MAPWIDTH, Globals.MAPHEIGHT];

		for(int j = 0; j < Globals.MAPHEIGHT; j++)
		{
			for(int i = 0; i < Globals.MAPWIDTH; i++)
			{
				TileType iType = (TileType)ReadInt(binReader);

				mapdatatop[i,j] = new MapTile();
				MapTile tile = mapdatatop[i,j];

				if(iType >= 0 && (int) iType < Globals.NUMTILETYPES)
				{
//					mapdatatop[i][j].iType = iType;
//					mapdatatop[i][j].iFlags = g_iTileTypeConversion[iType];
					tile.iType = iType;
					tile.iFlags = g_iTileTypeConversion[(int)iType];
				}
				else
				{
//					mapdatatop[i][j].iType = tile_nonsolid;
//					mapdatatop[i][j].iFlags = tile_flag_nonsolid;
					tile.iType = TileType.tile_nonsolid;
					tile.iFlags = (int)TileTypeFlag.tile_flag_nonsolid;
				}

				warpdata[i,j] = new Warp(); 
				warpdata[i,j].direction = (short)ReadInt(binReader);
				warpdata[i,j].connection = (short)ReadInt(binReader);
				warpdata[i,j].id = (short)ReadInt(binReader);
				
				for(short z = 0; z < Globals.NUMSPAWNAREATYPES; z++)
					nospawn[z,i,j] = ReadBool(binReader);
			}
		}
		
		//Read switch block state data
		int iNumSwitchBlockData = ReadInt(binReader);
		for(short iBlock = 0; iBlock < iNumSwitchBlockData; iBlock++)
		{
			short iCol = ReadByteAsShort(binReader);
			short iRow = ReadByteAsShort(binReader);
			
			objectdata[iCol,iRow].iSettings[0] = ReadByteAsShort(binReader);
		}
		
		if(iReadType == ReadType.read_type_preview)
		{
			Debug.LogWarning("ReadType.read_type_preview");
			return;
		}
		
		maxConnection = 0;

		numwarpexits = (short)ReadInt(binReader);
		warpexits = new WarpExit[Globals.MAXWARPS];
		for(int i = 0; i < numwarpexits && i < Globals.MAXWARPS; i++)
		{
			warpexits[i].direction = (short)ReadInt(binReader);
			warpexits[i].connection = (short)ReadInt(binReader);
			warpexits[i].id = (short)ReadInt(binReader);
			warpexits[i].x = (short)ReadInt(binReader);
			warpexits[i].y = (short)ReadInt(binReader);
			
			warpexits[i].lockx = (short)ReadInt(binReader);
			warpexits[i].locky = (short)ReadInt(binReader);
			
			warpexits[i].warpx = (short)ReadInt(binReader);
			warpexits[i].warpy = (short)ReadInt(binReader);
			warpexits[i].numblocks = (short)ReadInt(binReader);
			
			if(warpexits[i].connection > maxConnection)
				maxConnection = warpexits[i].connection;
		}
		
		//Ignore any more warps than the max
		for(int i = 0; i < numwarpexits - Globals.MAXWARPS; i++)
		{
			for(int j = 0; j < 10; j++)
			{
				ReadInt(binReader);
				Debug.Log("i="+i+", j="+j+"... Ignore any more warps than the max");
			}
		}
		
		if(numwarpexits > Globals.MAXWARPS)
			numwarpexits = Globals.MAXWARPS;

		Debug.Log("numwarpexits = " + numwarpexits);
		

		//Read spawn areas
		Debug.Log("Read spawn areas");
		numspawnareas = new short[Globals.NUMSPAWNAREATYPES];
		totalspawnsize = new short[Globals.NUMSPAWNAREATYPES];
		spawnareas = new SpawnArea[Globals.NUMSPAWNAREATYPES, Globals.MAXSPAWNAREAS];
		for(int i = 0; i < Globals.NUMSPAWNAREATYPES; i++)
		{
			totalspawnsize[i] = 0;
			numspawnareas[i] = (short)ReadInt(binReader);
			Debug.Log("numspawnareas["+i+"] = " + numspawnareas[i]);
			
			if(numspawnareas[i] > Globals.MAXSPAWNAREAS)
			{
				Debug.LogError(" ERROR: Number of spawn areas (" + numspawnareas[i] + ") was greater than max allowed (" + Globals.MAXSPAWNAREAS + ")");
//				cout << endl << " ERROR: Number of spawn areas (" << numspawnareas[i]
//				<< ") was greater than max allowed (" << MAXSPAWNAREAS << ')'
//					<< endl;
				return;
			}
			
			for(int m = 0; m < numspawnareas[i]; m++)
			{
				spawnareas[i,m] = new SpawnArea();
				spawnareas[i,m].left = (short)ReadInt(binReader);
				spawnareas[i,m].top = (short)ReadInt(binReader);
				spawnareas[i,m].width = (short)ReadInt(binReader);
				spawnareas[i,m].height = (short)ReadInt(binReader);
				spawnareas[i,m].size = (short)ReadInt(binReader);
				
				totalspawnsize[i] += spawnareas[i,m].size;
			}
			
			//If no spawn areas were identified, then create one big spawn area
			if(totalspawnsize[i] == 0)
			{
				numspawnareas[i] = 1;
				spawnareas[i,0] = new SpawnArea();
				spawnareas[i,0].left = 0;
				spawnareas[i,0].width = 20;
				spawnareas[i,0].top = 1;
				spawnareas[i,0].height = 12;
				spawnareas[i,0].size = 220;
				totalspawnsize[i] = 220;
			}
		}

		Debug.Log("reading DrawAreas");

		//Read draw areas (foreground tiles drawing optimization)
		numdrawareas = (short)ReadInt(binReader);
		Debug.Log("numdrawareas = " + numdrawareas);
		
		if(numdrawareas > Globals.MAXDRAWAREAS)
		{
			Debug.LogError(" ERROR: Number of spawn areas (" + numdrawareas + ") was greater than max allowed (" + Globals.MAXDRAWAREAS + ")");
//			cout << endl << " ERROR: Number of draw areas (" << numdrawareas
//				<< ") was greater than max allowed (" << MAXDRAWAREAS << ')'
//					<< endl;
			return;
		}

//		Rect[] test = new Rect[23];
//		test[0].x;
//		test[0].y;
//		test[0].width;
//		test[0].height;

		//Load rects to help optimize drawing the foreground
		drawareas = new SDL_Rect[Globals.MAXDRAWAREAS];
		for(int m = 0; m < numdrawareas; m++)
		{
			drawareas[m].x = (short)ReadInt(binReader);
			drawareas[m].y = (short)ReadInt(binReader);
			drawareas[m].w = (ushort)ReadInt(binReader);
			drawareas[m].h = (ushort)ReadInt(binReader);
//			drawareas[m].x = (Sint16)ReadInt(binReader);
//			drawareas[m].y = (Sint16)ReadInt(binReader);
//			drawareas[m].w = (Uint16)ReadInt(binReader);
//			drawareas[m].h = (Uint16)ReadInt(binReader);
		}

		Debug.Log("reading ExtendedDataBlocks");

		int iNumExtendedDataBlocks = ReadInt(binReader);
		Debug.Log("iNumExtendedDataBlocks = " + iNumExtendedDataBlocks);
		for(short iBlock = 0; iBlock < iNumExtendedDataBlocks; iBlock++)
		{
			short iCol = ReadByteAsShort(binReader);
			short iRow = ReadByteAsShort(binReader);
			
			short iNumSettings = ReadByteAsShort(binReader);
			Debug.Log("ExtendedDataBlocks ("+iNumSettings+") : x=" + iCol + ", y=" + iRow);
			for(short iSetting = 0; iSetting < iNumSettings; iSetting++)
				objectdata[iCol,iRow].iSettings[iSetting] = ReadByteAsShort(binReader);
		}

		Debug.Log("reading RaceGoals");

		//read mode item locations like flags and race goals
		iNumRaceGoals = (short)ReadInt(binReader);
		Debug.Log("iNumRaceGoals = " + iNumRaceGoals);
		
		racegoallocations = new Vector2[Globals.MAXRACEGOALS];
		for(int j = 0; j < iNumRaceGoals; j++)
		{
			racegoallocations[j].x = (short)ReadInt(binReader);
			racegoallocations[j].y = (short)ReadInt(binReader);
		}

		Debug.Log("reading FlagBases");

		iNumFlagBases = (short)ReadInt(binReader);
		Debug.Log("iNumFlagBases = " + iNumFlagBases);
		
		flagbaselocations = new Vector2[Globals.MAXFLAGBASES];
		for(int j = 0; j < iNumFlagBases; j++)
		{
			flagbaselocations[j].x = (short)ReadInt(binReader);
			flagbaselocations[j].y = (short)ReadInt(binReader);
		}
	}

	void loadPlatforms(BinaryReader binReader, bool fPreview, int[] version, int[] translationid, int[] tilesetwidths, int[] tilesetheights, short iMaxTilesetID)
	{
		Debug.LogWarning("reading and loading Platforms"); 

		clearPlatforms();

		// Load moving platforms
		iNumPlatforms = (short) ReadInt(binReader);
		Debug.Log("iNumPlatforms = " + iNumPlatforms);
		platforms = new MovingPlatform[iNumPlatforms];

		for(short iPlatform = 0; iPlatform < iNumPlatforms; iPlatform++)
		{
			short iWidth = (short) ReadInt(binReader);
			short iHeight = (short) ReadInt(binReader);
			Debug.Log("iPlatform = " + iPlatform + ", iWidth = " + iWidth);
			Debug.Log("iPlatform = " + iPlatform + ", iHeight = " + iHeight);

//			TilesetTile[,] tiles = new TilesetTile[iWidth, iHeight];
//			MapTile[,] types = new MapTile[iWidth, iHeight];
//
//
//			for(short iCol = 0; iCol < iWidth; iCol++)
//			{
//				tiles[iCol] = new TilesetTile[iHeight];
//				types[iCol] = new MapTile[iHeight];
//				
//				for(short iRow = 0; iRow < iHeight; iRow++)
//				{
//					//TilesetTile * tile = &tiles[iCol][iRow];
//					TilesetTile tile = tiles[iCol][iRow];

			platformTiles = new TilesetTile[iWidth, iHeight];				// geht nicht wenn Platform unterschiedliche längen und breiten auf seinen ebenen hat
			platformTileTypes = new MapTile[iWidth, iHeight];

			mapdatatop = new MapTile[iWidth, iHeight];

			for(short iCol = 0; iCol < iWidth; iCol++)
			{
				Debug.Log("Platform iCol = " + iCol);
//				tiles[iCol] = new TilesetTile[iHeight];
//				types[iCol] = new MapTile[iHeight];

				for(short iRow = 0; iRow < iHeight; iRow++)
				{
					Debug.Log("Platform iRow = " + iRow);
					
					//TilesetTile * tile = &tiles[iCol][iRow];
					platformTiles[iCol,iRow] = new TilesetTile();
					TilesetTile tile = platformTiles[iCol,iRow];
					tile = new TilesetTile();
					platformTileTypes[iCol,iRow] = new MapTile();
					mapdatatop[iCol,iRow] = new MapTile();
//			TilesetTile[][] tiles = new TilesetTile[iWidth][];
//			MapTile[][] types = new MapTile[iWidth][];

//			for(short iCol = 0; iCol < iWidth; iCol++)
//			{
//				tiles[iCol] = new TilesetTile[iHeight];
//				types[iCol] = new MapTile[iHeight];
//
//				for(short iRow = 0; iRow < iHeight; iRow++)
//				{
//					//TilesetTile * tile = &tiles[iCol][iRow];
//					TilesetTile tile = tiles[iCol][iRow];

					if(VersionIsEqualOrAfter(version, 1, 8, 0, 0))
					{
						Debug.LogWarning("VersionIsEqualOrAfter = 1, 8, 0, 0");
						tile.iID = ReadByteAsShort(binReader);
						tile.iCol = ReadByteAsShort(binReader);
						tile.iRow = ReadByteAsShort(binReader);
						
						if(tile.iID >= 0)
						{
							if(iMaxTilesetID != -1 && tile.iID > iMaxTilesetID)
								tile.iID = 0;
							
							//Make sure the column and row we read in is within the bounds of the tileset
//							if(tile.iCol < 0 || (tilesetwidths && tile.iCol >= tilesetwidths[tile.iID]))
							if(tile.iCol < 0 || (tilesetwidths != null && tile.iCol >= tilesetwidths[tile.iID]))
								tile.iCol = 0;
							
//							if(tile.iRow < 0 || (tilesetheights && tile.iRow >= tilesetheights[tile.iID]))
							if(tile.iRow < 0 || (tilesetheights != null && tile.iRow >= tilesetheights[tile.iID]))
								tile.iRow = 0;
							
							//Convert tileset ids into the current game's tileset's ids
							if(translationid != null)
								tile.iID = (short) translationid[tile.iID];
						}
						
						TileType iType = (TileType)ReadInt(binReader);
						
//						if(iType >= 0 && (iType) < Globals.NUMTILETYPES)
						if(iType >= 0 && ((int)iType) < Globals.NUMTILETYPES)
						{
							platformTileTypes[iCol,iRow].iType = iType;
//							types[iCol][iRow].iFlags = g_iTileTypeConversion[iType];
							platformTileTypes[iCol,iRow].iFlags = g_iTileTypeConversion[(int)iType];
						}
						else
						{
							platformTileTypes[iCol,iRow].iType = (int) TileType.tile_nonsolid;
							platformTileTypes[iCol,iRow].iFlags = (int) TileTypeFlag.tile_flag_nonsolid;
						}
					}
					else
					{
						Debug.LogWarning("VersionIsBefore < 1, 8, 0, 0");
						short iTile = (short) ReadInt(binReader);
						TileType type;
						
						if(iTile == Globals.TILESETSIZE)
						{
							tile.iID = Globals.TILESETNONE;
							tile.iCol = 0;
							tile.iRow = 0;
							
							type = TileType.tile_nonsolid;
						}
						else
						{
							tile.iID = m_TilesetManager.GetClassicTilesetIndex();
							tile.iCol = (short)(iTile % Globals.TILESETWIDTH);
							tile.iRow = (short)(iTile / Globals.TILESETWIDTH);
							
							type = m_TilesetManager.GetClassicTileset().GetTileType(tile.iCol, tile.iRow);
						}
						
						if(type >= 0 && (int)type < Globals.NUMTILETYPES)
						{
							platformTileTypes[iCol,iRow].iType = type;
							platformTileTypes[iCol,iRow].iFlags = g_iTileTypeConversion[(int)type];
						}
						else
						{
							mapdatatop[iCol,iRow].iType = TileType.tile_nonsolid;
							mapdatatop[iCol,iRow].iFlags = (int) TileTypeFlag.tile_flag_nonsolid;
						}
					}
				}
			}

//			short iDrawLayer = 2;
//			if(VersionIsEqualOrAfter(version, 1, 8, 0, 1))
//				iDrawLayer = (short)ReadInt(binReader);
//			
//			//printf("Layer: %d\n", iDrawLayer);
//			
//			short iPathType = 0;
//			
//			if(VersionIsEqualOrAfter(version, 1, 8, 0, 0))
//				iPathType = ReadInt(binReader);
//			
//			//printf("PathType: %d\n", iPathType);
//			
//			MovingPlatformPath * path = NULL;
//			if(iPathType == 0) //segment path
//			{
//				float fStartX = ReadFloat(binReader);
//				float fStartY = ReadFloat(binReader);
//				float fEndX = ReadFloat(binReader);
//				float fEndY = ReadFloat(binReader);
//				float fVelocity = ReadFloat(binReader);
//				
//				path = new StraightPath(fVelocity, fStartX, fStartY, fEndX, fEndY, fPreview);
//				
//				//printf("Read segment path\n");
//				//printf("StartX: %.2f StartY:%.2f EndX:%.2f EndY:%.2f Velocity:%.2f\n", fStartX, fStartY, fEndX, fEndY, fVelocity);
//			}
//			else if(iPathType == 1) //continuous path
//			{
//				float fStartX = ReadFloat(binReader);
//				float fStartY = ReadFloat(binReader);
//				float fAngle = ReadFloat(binReader);
//				float fVelocity = ReadFloat(binReader);
//				
//				path = new StraightPathContinuous(fVelocity, fStartX, fStartY, fAngle, fPreview);
//				
//				//printf("Read continuous path\n");
//				//printf("StartX: %.2f StartY:%.2f Angle:%.2f Velocity:%.2f\n", fStartX, fStartY, fAngle, fVelocity);
//			}
//			else if(iPathType == 2) //elliptical path
//			{
//				float fRadiusX = ReadFloat(binReader);
//				float fRadiusY = ReadFloat(binReader);
//				float fCenterX = ReadFloat(binReader);
//				float fCenterY = ReadFloat(binReader);
//				float fAngle = ReadFloat(binReader);
//				float fVelocity = ReadFloat(binReader);
//				
//				path = new EllipsePath(fVelocity, fAngle, fRadiusX, fRadiusY, fCenterX, fCenterY, fPreview);
//				
//				//printf("Read elliptical path\n");
//				//printf("CenterX: %.2f CenterY:%.2f Angle:%.2f RadiusX: %.2f RadiusY: %.2f Velocity:%.2f\n", fCenterX, fCenterY, fAngle, fRadiusX, fRadiusY, fVelocity);
//			}
//			
//			MovingPlatform * platform = new MovingPlatform(tiles, types, iWidth, iHeight, iDrawLayer, path, fPreview);
//			platforms[iPlatform] = platform;
//			platformdrawlayer[iDrawLayer].push_back(platform);

		}

	}

	void clearPlatforms()
	{
//		foreach(MovingPlatform mp in platformdrawlayerList)
//		{
//
//		}
		platformdrawlayerList.Clear();

//		for(short iLayer = 0; iLayer < platformdrawlayerList ; iLayer++)
//			platformdrawlayer[iLayer].clear();


		if(platformsList != null)
		{
			platformsList.Clear();
		}

//		if(platforms != null)
//		{
//			for(short iPlatform = 0; iPlatform < iNumPlatforms; iPlatform++)
//			{
//				platforms[iPlatform] = null;
//				//platforms.delete	// list
//			}
//			platforms = NULL;
//		}
		
		iNumPlatforms = 0;
		
//		std::list<MovingPlatform*>::iterator iter = tempPlatforms.begin(), lim = tempPlatforms.end();
//		while (iter != lim)
//		{
//			delete (*iter);
//			++iter;
//		}
//		for(int i=0; i<tempPlatforms.Length; i++)
//		{
//			tempPlatforms[i] = null;
//		}

		tempPlatformsList.Clear();
		
//		tempPlatforms.clear();
	}

	public void OnGUI()
	{
	}

	public void OnGUI_Preview()
	{

		OnGUI_Preview_PlatformTiles();

//		previewSliderPosition = EditorGUILayout.BeginScrollView(previewSliderPosition);
		OnGUI_Preview_Mapdata();
//		EditorGUILayout.EndScrollView();

//		previewObjectDataSliderPosition = EditorGUILayout.BeginScrollView(previewObjectDataSliderPosition);	
		OnGUI_Preview_Objectdata();
//		EditorGUILayout.EndScrollView();
	}

	Vector2 previewPlatformTilesSliderPosition = Vector2.zero;
	
	public void OnGUI_Preview_PlatformTiles()
	{
		if(platformTiles != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			previewPlatformTilesSliderPosition = EditorGUILayout.BeginScrollView(previewPlatformTilesSliderPosition);
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{
					
					string tileString = "";
					
					TilesetTile platformTile = platformTiles[x,y];


					if( platformTile == null)
					{
						GUILayout.Label("null");
					}
					else
					{
						//							if(tile.iCol == 0 && tile.iRow == 0)
						//							{
						//								tileString +="<color=red>";
						//							}
						
						if(platformTile.iCol == 0 && platformTile.iRow == 0)
						{
							GUI.skin.textArea.fixedWidth = 12;
							GUI.skin.textArea.stretchWidth = false;
							tileString += platformTile.iID.ToString("D2");
						}
						else
						{
							tileString += platformTile.iID.ToString("D2")+","+platformTile.iCol.ToString("D2")+","+platformTile.iRow.ToString("D2")+"\n";
						}

					}
					
					EditorGUILayout.TextArea(tileString);
					
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.LabelField("platformTiles empty");
		}
	}

	Vector2 previewObjectDataSliderPosition = Vector2.zero;

	public void OnGUI_Preview_Objectdata()
	{
		if(objectdata != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			previewObjectDataSliderPosition = EditorGUILayout.BeginScrollView(previewObjectDataSliderPosition);
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{
					
					string mapBlockString = "";
					
					MapBlock mapBlock = objectdata[x,y];
						
					if( mapBlock == null)
					{
						GUILayout.Label("null");
					}
					else
					{
//						mapBlockString += mapBlock.fHidden.ToString()+","+mapBlock.iSettings.ToString()+","+mapBlock.iType.ToString("D2");
//						mapBlockString += mapBlock.fHidden.ToString()+","+mapBlock.iType.ToString("D2");
						mapBlockString += mapBlock.fHidden ? "1" : "0"+"\n" + 
							mapBlock.iType.ToString("D3") + "\n";

						for(int i=0; i<mapBlock.iSettings.Length; i++)
						{
							if(mapBlock.iSettings[i] != 0)
								mapBlockString += mapBlock.iSettings[i].ToString("D2") + ",";
						}
								
					}
					
					EditorGUILayout.TextArea(mapBlockString);
					
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.LabelField("objectdata empty");
		}
	}

	Vector2 previewSliderPosition = Vector2.zero;

//	public GUIStyle textFieldStlye = new GUIStyle();

	public void OnGUI_Preview_Mapdata()
	{
//		textFieldStlye = new GUIStyle();
//		textFieldStlye.richText = false;
//		textFieldStlye.fixedWidth = 16+4+16+4+16;
//		//textFieldStlye.stretchWidth = true;

		if(mapdata != null)
		{
			previewSliderPosition = EditorGUILayout.BeginScrollView(previewSliderPosition);
			EditorGUILayout.BeginHorizontal();
//			GUILayout.Space(10);
//			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			for(int y = 0; y < Globals.MAPHEIGHT; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for(int x = 0; x < Globals.MAPWIDTH; x++)
				{

					string tileString = "";

					for(int l = 0; l < Globals.MAPLAYERS; l++)
					{
//						EditorGUILayout.BeginVertical();
//						GUILayout.BeginVertical();

						TilesetTile tile = mapdata[x,y,l];

						if( tile == null)
						{
							GUILayout.Label("null");
						}
						else
						{
//							if(tile.iCol == 0 && tile.iRow == 0)
//							{
//								tileString +="<color=red>";
//							}

							if(tile.iCol == 0 && tile.iRow == 0)
							{
								GUI.skin.textArea.fixedWidth = 12;
								GUI.skin.textArea.stretchWidth = false;
								tileString += tile.iID.ToString("D2");
							}
							else
							{
								tileString += tile.iID.ToString("D2")+","+tile.iCol.ToString("D2")+","+tile.iRow.ToString("D2");
							}

//							if(tile.iCol == 0 && tile.iRow == 0)
//							{
//								tileString +="</color>";
//							}

							if(l == Globals.MAPLAYERS -1)
							{
								// no line end
							}
							else
							{
								tileString += "\n";
							}

//							GUILayout.Label(tile.iID+","+tile.iCol+","+tile.iRow);
//							GUILayout.Label(tile.iCol + "");
//							GUILayout.Label(tile.iRow + "");
						}
//						GUILayout.Label(mapdata[x,y,l].iID.ToString());
//						EditorGUILayout.LabelField(x+" "+y+" "+l, GUILayout.ExpandWidth(false));
//						string mapDataField = "iID = " + mapdata[x,y,l].iID;
//						EditorGUILayout.LabelField(mapDataField);
						//mapdata[x,y,l] = EditorGUILayout.IntField();



//						GUILayout.EndVertical();
//						EditorGUILayout.EndVertical();
					}

					EditorGUILayout.TextArea(tileString);
//					EditorGUILayout.TextArea(tileString, textFieldStlye);

				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndScrollView();

		}
		else
		{
			EditorGUILayout.LabelField("mapdata empty");
		}
	}

	void saveMap(string filePath)
	{
		FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
		BinaryWriter binWriter = new BinaryWriter(fs);

		//First write the map compatibility version number 
		//(this will allow the map loader to identify if the map needs conversion)
		WriteInt(Globals.version[0], binWriter); //Major
		WriteInt(Globals.version[1], binWriter); //Minor
		WriteInt(Globals.version[2], binWriter); //Micro
		WriteInt(Globals.version[3], binWriter); //Build
		
		bool[,] usedtile = new bool[Globals.MAPWIDTH, Globals.MAPHEIGHT];

		for(int iPlatform = 0; iPlatform < iNumPlatforms; iPlatform++)
		{
			for(short iCol = 0; iCol < platforms[iPlatform].iTileWidth; iCol++)
			{
				for(short iRow = 0; iRow < platforms[iPlatform].iTileHeight; iRow++)	
				{
					
				}
			}
		}
				
		iPlatformCount++;
		iHazardCount++;
		iIceCount++;

		binWriter.Close();
		fs.Close();
    }

	void WriteInt(int value, BinaryWriter binWriter)
    {
		//	fwrite(&out, sizeof(Uint32), 1, outFile);
		binWriter.Write(value);
	}

	bool ReadBool(BinaryReader binReader)
	{
		bool b;
//		fread(&b, sizeof(Uint8), 1, inFile);
		b = binReader.ReadBoolean();
		
		return b;
	}

	short ReadByteAsShort(BinaryReader binReader)
	{
		byte b;

//		fread(&b, sizeof(Uint8), 1, inFile);
		b = binReader.ReadByte();
		
		return (short)b;
	}

	/// <summary>
	/// Reads the int.
	/// </summary>
	/// <returns>The int.</returns>
	/// <param name="inFile">In file.</param>
	int ReadInt(BinaryReader binReader)
	{
		int inValue;
//		fread(&inValue, sizeof(Uint32), 1, inFile);
		inValue = (int) binReader.ReadUInt32();
		
		#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
		// kopiere value zum bearbeiten der byte reihenfolge
		int t = inValue;

		inValue = (int) ReverseBytes((UInt32)t);

//		((char*)&inValue)[0] = ((char*)&t)[3];
//		((char*)&inValue)[1] = ((char*)&t)[2];
//		((char*)&inValue)[2] = ((char*)&t)[1];
//		((char*)&inValue)[3] = ((char*)&t)[0];
		#endif
		
		return inValue;
	}


	/// <summary>
	/// Reads the int chunk. (Datenblock)
	/// </summary>
	#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
	void ReadIntChunk(int[] mem, uint iQuantity, BinaryReader binReader)
	{
		for(uint i=0; i<iQuantity; i++)
		{
			mem[i] = (int) binReader.ReadUInt32();

			// kopiere value
			int t = mem[i];

			// Reverse Byte Order - reordner the 4 bytes in Integer (32 bit)
			mem[i] = (int) ReverseBytes((uint)t);
		}
	}

	// reverse byte order (32-bit)
	public static UInt32 ReverseBytes(UInt32 value)
	{
		return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
			(value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
	}

	#else
	void ReadIntChunk(int[] mem, uint iQuantity, BinaryReader binReader)
	{
//		fread(mem, sizeof(Uint32), iQuantity, inFile);
		for(uint i=0; i<iQuantity; i++)
		{
			mem[i] = (int) binReader.ReadUInt32();
		}
	}
	#endif

	float ReadFloat(BinaryReader binReader)
	{
														//TODO ready ReadBytes(4), vielleicht konvertiert ReadSingle bereits falsch
		float inValue = binReader.ReadSingle();			// float ReadSingle()
//		fread(&inValue, sizeof(float), 1, inFile);
		
		#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
		float t = inValue;
		
		inValue = (float) ReverseBytes((UInt32)t);
		#endif
		
		return inValue;
	}


	string ReadString(uint size, BinaryReader binReader)
	{
		// string länge auslesen
		//		int iLen = ReadInt(inFile);
		int iLen = ReadInt(binReader);
		Debug.Log("iLen = " + iLen + " = cstring länge mit NULL Terminator");

		if(iLen < 0)
		{
			Debug.LogError("string länge < 0!");
			return null;
		}
		else if (iLen > Globals.TILESET_TRANSLATION_CSTRING_SIZE)
		{
			Debug.LogError("string länge > max. länge (" + Globals.TILESET_TRANSLATION_CSTRING_SIZE + ") ");
			return null;
		}
		
		//		char * szReadString = new char[iLen];
		char[] szReadString = new char[iLen];
		
		//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadString = binReader.ReadChars(iLen);
		
		//		szReadString[iLen - 1] = 0;
		szReadString[iLen - 1] = '\0';	// cstrin NULL Terminated 
		
		//Prevent buffer overflow  5253784 5253928
		//		strncpy(szString, szReadString, size - 1);		// -> size = TILESET_TRANSLATION_CSTRING_SIZE
		//		szString[size - 1] = 0;
		//TODO NOTE: szString hat im Struct eine länge von 128, nicht über disen Speicherbereich hinaus schreiben!
		/* copy to sized buffer (overflow safe): */ 
		//strncpy ( str2, str1, sizeof(str2) );
		
		string readString = new string(szReadString);
		
		Debug.Log("readString = " + readString);
		
		return readString;
	}


//	void ReadString(char * szString, short size, FILE * inFile)
	void ReadString(char[] szString, uint size, BinaryReader binReader)
	{
		Debug.LogError(this.ToString() + " DON'T USE ME");
		
		// string länge auslesen
//		int iLen = ReadInt(inFile);
		int iLen = ReadInt(binReader);
		Debug.Log("iLen = " + iLen + " (cstring länge)");

		if(iLen < 0)
		{
			Debug.LogError("string länge < 0!");
			return;
		}
		else if (iLen > Globals.TILESET_TRANSLATION_CSTRING_SIZE)
		{
			Debug.LogError("string länge > max. länge (" + Globals.TILESET_TRANSLATION_CSTRING_SIZE + ") ");
			return;
		}

//		char * szReadString = new char[iLen];
		char[] szReadString = new char[iLen];

//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadString = binReader.ReadChars(iLen);

//		szReadString[iLen - 1] = 0;
		szReadString[iLen - 1] = '\0';	//TODO check string/char line end in cpp 
		
		//Prevent buffer overflow  5253784 5253928
		//		strncpy(szString, szReadString, size - 1);		// -> size = TILESET_TRANSLATION_CSTRING_SIZE
		//		szString[size - 1] = 0;
		//TODO NOTE: szString hat im Struct eine länge von 128, nicht über disen Speicherbereich hinaus schreiben!
		/* copy to sized buffer (overflow safe): */ 
		//strncpy ( str2, str1, sizeof(str2) );
		szString = szReadString;					//TODO TODO szString zeigt auf die selbe reference
		szString = new char[iLen];					//TODO TODO szString muss eine eigene reference haben, nur der inhalt soll kopiert werden
		Array.Copy(szReadString, szString, iLen);	// char Array kopieren
		string test = new string(szString);			//TODO löscht diese anweisung den Inhalt aus szString?
		string test2 = new string(szString);		//TODO löscht diese anweisung den Inhalt aus szString?
//		string test3 = string.Join("", szString);	//TODO löscht diese anweisung den Inhalt aus szString?
//		string charToString = new string(CharArray, 0, CharArray.Count());
		Debug.Log("szString = " + new string(szString));	//TODO NEIN: Inhalt noch vorhanden
		Debug.Log("szString = " + test);	
		Debug.Log("szString = " + test2);	
//		Debug.Log("szString = " + test3);	
//		delete [] szReadString;
	}

	string ReadNativString(uint size, BinaryReader binReader)
	{
		// Funktioniert mit dieser Dateistruktur NICHT,
		// in der Datei steht ein 32-bit langer Integer-Wert
		// BinaryReader.ReadString() erwartet einen 7-Bit langen Interger-Wert

		string szString;
		// string länge auslesen
		//		int iLen = ReadInt(inFile);
		// TODO BinaryReader.ReadString() erwartet als erste Information die Stringlänge
//		int iLen = ReadInt(binReader);
//		Debug.Log("iLen = " + iLen + " (string länge)");
		

		//		char * szReadString = new char[iLen];
		string szReadString ;
		
		//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadString = binReader.ReadString();	// TODO achtung was macht es?

		szString = szReadString;
		return szString;
	}

}