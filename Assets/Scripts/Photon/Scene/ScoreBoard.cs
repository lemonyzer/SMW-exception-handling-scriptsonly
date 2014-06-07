using UnityEngine;
using System.Collections;

public class ScoreBoard : Photon.MonoBehaviour
{

    public static ScoreBoard SP;
    public GUISkin skin;

    //static string newRecordMessage = "";

    private GUIText highscoreText;


    private string scoreText = "Loading scores";

    private int scoreBoardHeight = 60;
    private GameSetup gameSetupScript;

    void Awake()
    {
        SP = this;
        gameSetupScript = GetComponent<GameSetup>() as GameSetup;

        highscoreText = GetComponent<GUIText>() as GUIText;
        highscoreText.enabled = false;
    }

    void OnGUI()
    {
        GUI.skin = skin;

        scoreText = "Scoreboard:\n";
        foreach (PlatformerPlayerNode entry in gameSetupScript.playerList)
        {
            scoreText += entry.playerName + " \t" + entry.kills + " kills " + entry.deaths + " deaths\n";
        }

        GUILayout.BeginArea(new Rect((Screen.width - 185), 20, 175, scoreBoardHeight));
        GUILayout.Box(scoreText);
        GUILayout.EndArea();
    }

    PlatformerPlayerNode GetPlayer(PhotonPlayer networkP)
    {
        foreach (PlatformerPlayerNode playerInstance in gameSetupScript.playerList)
        {
            if (networkP == playerInstance.networkPlayer)
            {
                return playerInstance;
            }
        }
        Debug.LogError("GetPlayer couldn't find player!");
        return null;
    }

    public void LocalPlayerHasKilled()
    {

        PlatformerPlayerNode pNode = GetPlayer(PhotonNetwork.player);
        pNode.kills += 1;

        //Overwrite the data of other players with the new correct score
        photonView.RPC("UpdateScore", PhotonTargets.All, PhotonNetwork.player, pNode.kills, pNode.deaths);
    }

    public void LocalPlayerDied()
    {
        PlatformerPlayerNode pNode = GetPlayer(PhotonNetwork.player);
        pNode.deaths += 1;

        //Overwrite with new correct score
        photonView.RPC("UpdateScore", PhotonTargets.All, PhotonNetwork.player, pNode.kills, pNode.deaths);
    }


    [RPC]
    void UpdateScore(PhotonPlayer player, int kills, int deaths)
    {
        Debug.Log((PhotonNetwork.player == player) + "=local " + kills + "kills & deaths=" + deaths);

        PlatformerPlayerNode pNode = GetPlayer(player);
        if (pNode != null)
        {
            pNode.kills = kills;
            pNode.deaths = deaths;
        }
        else
        {
            Debug.LogError("Could not find network player " + player + " in the gamesetup playerlist!");
        }
        scoreBoardHeight = gameSetupScript.playerList.Count * 15 + 40;
    }
}

