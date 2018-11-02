using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class LevelTest
{
	public int[] Input1;
	public int[] Input2;
	public int[] Output1;
	public int[] Output2;
}

[Serializable]
class GameLevel
{
	[NonSerialized]
	public string FileName;
	
	public string Name;
	public string Description;

	public LevelTest[] Tests;

	public static int Compare(GameLevel left, GameLevel right)
	{
		return String.CompareOrdinal(left.Name, right.Name);
	}
}

public class LevelManager : MonoBehaviour
{
	public GameObject ButtonPrefab;

	private GameLevel[] _levels;

	private void Awake()
	{
		LoadLevels();
		UpdateDisplay();
	}
	
	private void LoadLevels()
	{
		FileInfo[] levelSources = new DirectoryInfo(Application.streamingAssetsPath + "/Levels").GetFiles("*.json");

		_levels = new GameLevel[levelSources.Length];

		for (int index = 0; index < levelSources.Length; index++)
		{
			Debug.Log(levelSources[index].FullName);
			_levels[index] = JsonUtility.FromJson<GameLevel>(File.ReadAllText(levelSources[index].FullName));
			_levels[index].FileName = levelSources[index].Name;
		}

		Array.Sort(_levels, GameLevel.Compare);
	}

	private void UpdateDisplay()
	{
		foreach (Transform childTransform in transform)
        		{
        			Destroy(childTransform.gameObject);
        		}
        
        		for (int index = 0; index < _levels.Length; index++)
        		{
        			GameObject button = Instantiate(ButtonPrefab, transform);
        			button.GetComponentInChildren<Text>().text = _levels[index].Name;
        		}
	}

	public void Reload()
	{
		LoadLevels();
		UpdateDisplay();
	}
}