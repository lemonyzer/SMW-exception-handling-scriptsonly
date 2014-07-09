using UnityEngine;
using System.Collections;

public class GamePrefs : ScriptableObject {

	private int gameSlots;
	private string gameScene;
	private int aiSlots;
	private int aiLevel;

	public int GetGameSlots()
	{
		return gameSlots;
	}

	public void SetGameSlots(int slots)
	{
		gameSlots = slots;
	}

	public int GetAISlots()
	{
		return aiSlots;
	}
}
