using UnityEngine;
using System.Collections;

public class GamePrefs : ScriptableObject {

	public enum GameMode {
		Classic,
		Reverse,
		Tagging,
		Race,
		Survival
	}

	public enum MultiplayerMode {
		OfflineBots,
		OfflineVS,
		UnityNetwork,
		OnlinePhoton,
		OnlineULink
	}

	private int[][] spawnZone;

	private GameMode gameMode;
	private MultiplayerMode multiplayerMode;

	private int gameSlots;
	private string gameScene;
	private string nextScene;

	private int numberOfTeams;
	private int teamSlots;

	private int aiSlots;
	public enum AILevel {
		Easy,
		Medium,
		Hard,
		Impossible
	}
	private AILevel aiLevel;

	/**
	 * Getter / Setter Methods
	 **/

	public GameMode GetGameMode()
	{
		return gameMode;
	}
	public void SetGameMode(GameMode mode)
	{
		gameMode = mode;
	}

	public MultiplayerMode GetMultiplayerMode()
	{
		return multiplayerMode;
	}
	public void SetMultiplayerMode(MultiplayerMode mode)
	{
		multiplayerMode = mode;
	}

	public int GetGameSlots()
	{
		return gameSlots;
	}
	public void SetGameSlots(int slots)
	{
		gameSlots = slots;
		Debug.Log("GameSlots: " + slots);
	}

	public int GetTeamSlots()
	{
		return teamSlots;
	}
	public void SetTeamSlots(int slots)
	{
		teamSlots = slots;
	}

	public int GetNumberOfTeams()
	{
		return numberOfTeams;
	}
	public void SetNumberOfTeams(int quantity)
	{
		numberOfTeams = quantity;
	}

	public int GetAISlots()
	{
		return aiSlots;
	}
	public void SetAISlots(int slots)
	{
		aiSlots = slots;
	}


}
