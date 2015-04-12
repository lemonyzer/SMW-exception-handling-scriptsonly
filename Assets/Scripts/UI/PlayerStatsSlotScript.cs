﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatsSlotScript : MonoBehaviour {

	public delegate void ClickAction(PlayerStatsSlotScript clickedSlot);
	public static event ClickAction OnClicked;

	public NetworkPlayer netPlayer;
	public Player player;

	//All
	public Image border;
	public Image slotAvatar;
	public Text slotName;
	public Text slotWho;
	public Text slotKills;
	public Text slotPoints;
	public Text slotLifes;
	public Text lastPing;		//Client ping to Server, (Servercharacter)
	public Text avgPing;		//Client avg ping to Server, (Servercharacter)

	// Server
	public Text correctPosSendCount;
	public Text lastPosDiff;
	public Text avgPosDiff;

	//Client
	public Text exinterpCount;	//other Client & Server Character
	public Text droppedCount;	//Client dropped Packages from Server (my & other Client and Servercharacter)
	

	// Use this for initialization
	public void Awake () {																// Awake wird auch bei instanziierung ausgeführt!!!
		Debug.Log(this.ToString() + " Wake()");
		if(this.player != null)
			Debug.LogError(this.ToString() + " " + this.player.GetHashCode());
		border = GetComponent<Image>();
		slotAvatar = transform.FindChild("SlotImage").GetComponent<Image>();
		slotName = transform.FindChild("SlotName").GetComponent<Text>();
		slotWho = transform.FindChild("SlotWho").GetComponent<Text>();

		slotPoints = transform.FindChild("SlotPoints").GetComponent<Text>();
		slotKills = transform.FindChild("SlotKills").GetComponent<Text>();
		slotLifes = transform.FindChild("SlotLifes").GetComponent<Text>();

		lastPing = transform.FindChild("SlotPing").GetComponent<Text>();
		avgPing = transform.FindChild("SlotAvaragePing").GetComponent<Text>();

		exinterpCount = transform.FindChild("SlotExinterpCount").GetComponent<Text>();
		droppedCount = transform.FindChild("SlotDropCount").GetComponent<Text>();

	}

	public void UpdateSlot(NetworkPlayer slotNetPlayer, Player slotPlayer)
	{
		Debug.Log(this.ToString() + " UpdateSlot() " + slotPlayer.getUserName());

		if(this.player != null)
			Debug.LogError(this.ToString() + " " + this.player.GetHashCode() + " "  + slotPlayer.getUserName());
		else
			Debug.LogError(this.ToString() + " " + "this.player == null");
			
		if(slotPlayer != null)
			Debug.LogError(this.ToString() + " " + slotPlayer.GetHashCode() + " "  + slotPlayer.getUserName());
		else
			Debug.LogError(this.ToString() + " " + "slotPlayer == null" + " "  + slotPlayer.getUserName());

		if(slotPlayer == null)
			return;

		this.player = slotPlayer;		// Zuordnung

		if(this.player != null)
			Debug.LogError(this.ToString() + " NACH ZUORDNUNG " + this.player.GetHashCode() + " "  + slotPlayer.getUserName());
		else
			Debug.LogError(this.ToString() + " NACH ZUORDNUNG " + "this.player == null" + " "  + slotPlayer.getUserName());

		this.netPlayer = slotNetPlayer;

		if(slotAvatar == null)
		{
			Debug.LogError("slotAvatar not Set, Start() needs to run first!" + " "  + slotPlayer.getUserName());
		}
		slotAvatar.sprite = player.characterScriptableObject.charIdleSprites[0];
		slotName.text = player.getNetworkPlayer().ipAddress + " " + slotPlayer.getUserName();

		if(Network.isClient)
		{
			if(player.getNetworkPlayer() == Network.connections[0])
			{
				slotWho.text = "Server";
			}
			else if(player.getNetworkPlayer()  == Network.player)
			{
				slotWho.text = "Me! (Client)";
			}
			else
			{
				slotWho.text = "other Client";
			}
		}
		else if(Network.isServer)
		{
			if(player.getNetworkPlayer()  == Network.player)
			{
				slotWho.text = "Me! (Server)";
			}
			else
			{
				slotWho.text = "Client";
			}
		}

	}

	public void SlotClicked()
	{
		if(Network.isServer)
		{
			// Server kann alle Character setzen
			if(OnClicked != null)
			{
				// we have event listeners
				Debug.LogError(this.ToString() + " " + player.GetHashCode());
				OnClicked(this);
			}
			else
			{
				Debug.LogError(this.ToString() + "no OnClicked() listeners");
			}
		}
		else
		{
			// Client kann nur seinen eigenen
		}
	}

}
