using System;

[Serializable]
public class LevelTest
{
	public int[] Input1 = { };
	public int[] Input2 = { };
	public int[] Output1 = { };
	public int[] Output2 = { };
}

[Serializable]
public class GameLevel
{
	[NonSerialized]
	public string FileName;

	public string Name = "Level";

	// Description is an overview of the level.
	public string[] Description = new string[0];

	// Objective is the task the player needs to achieve in a level.
	public string[] Objective = null;
	// Objective may be null, in which case it defaults to the level's description.

	public LevelTest[] Tests = new LevelTest[0];

	public static int Compare(GameLevel left, GameLevel right)
	{
		return string.CompareOrdinal(left.Name, right.Name);
	}
}