using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformerPlayerNode
{
    public string playerName;
    public PhotonPlayer networkPlayer;

    public int kills = 0;
    public int deaths = 0;

}
public class GameSetup : Photon.MonoBehaviour
{
    

    public Transform playerPref;
    public PlatformerChat4 chatScript;
    public string playerName = "";

    //Server-only playerlist
	public List<PlatformerPlayerNode> playerList = new List<PlatformerPlayerNode>();

    void Awake()
    {
        playerName = PlayerPrefs.GetString("playerName" + Application.platform);

        chatScript = GetComponent<PlatformerChat4>() as PlatformerChat4;
        PhotonNetwork.isMessageQueueRunning = true;
        Screen.lockCursor = true;

        if (PhotonNetwork.isMasterClient)
        {
			//This Script runs on Master Client
            chatScript.SetShowChatWindow(true);
            photonView.RPC("AddPlayer", PhotonTargets.AllBuffered);

            foreach (GameObject go in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (PhotonNetwork.isNonMasterClientInRoom)
        {
			//This Script runs on n Slave Client
            photonView.RPC("AddPlayer", PhotonTargets.AllBuffered);
            chatScript.SetShowChatWindow(true);

            foreach (GameObject go in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
			//This Script runs not on Master nor Slave Client
            //How did we even get here without connection?
            Screen.lockCursor = false;
            Application.LoadLevel((Application.loadedLevel - 1));
        }
    }


    void OnPhotonPlayerDisconneced(PhotonPlayer player)
    {        
        //Remove player from the server list
		foreach (PlatformerPlayerNode entry in playerList as List<PlatformerPlayerNode>)
        {
            if (entry.networkPlayer == player)
            {
                chatScript.addGameChatMessage(entry.playerName + " disconnected " + player);
                playerList.Remove(entry);
                break;
            }
        }
    }


    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        if (PhotonNetwork.isMasterClient)
        {
			//Master Client adds Message to Chat
            chatScript.addGameChatMessage(player+" connected" );
        }
    }
    
    [RPC]
    //Sent by newly connected clients, recieved by server
    void AddPlayer(PhotonMessageInfo info)
    {
        PhotonPlayer netPlayer = info.sender;

        PlatformerPlayerNode newEntry = new PlatformerPlayerNode();
        newEntry.playerName = netPlayer.name;
        newEntry.networkPlayer = netPlayer;
        playerList.Add(newEntry);

        if (PhotonNetwork.isMasterClient)
        {
			//Master Client adds Message to Chat
            chatScript.addGameChatMessage(netPlayer.name + " joined the game");
        }
    }

    //Called via Awake()
    void OnNetworkLoadedLevel()
    {
        // Randomize starting location
        GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
        Debug.Log("spawns: " + spawnpoints.Length);

        Transform spawnpoint = spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
        PhotonNetwork.Instantiate(playerPref.name, spawnpoint.position, spawnpoint.rotation, 0);
    }
    void OnDisconnectedFromPhoton()
    {
        //Load main menu
        Screen.lockCursor = false;
        Application.LoadLevel((Application.loadedLevel - 1));
    }



}