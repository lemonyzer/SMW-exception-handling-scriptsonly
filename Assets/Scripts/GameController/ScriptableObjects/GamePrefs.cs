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
		Bots,
		UnityNetwork,
		OnlinePhoton,
		OnlineULink
	}

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

	public int GetGameMode()
	{
		return gameMode;
	}
	public void SetGameMode(int mode)
	{
		gameMode = mode;
	}

	public int GetMultiplayerMode()
	{
		return multiplayerMode;
	}
	public void SetMultiplayerMode(int mode)
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
		Debug.Log("GameSlots initialisiert: " + slots);
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
