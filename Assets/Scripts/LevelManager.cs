using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class GameLevel
{
	public string Name;
	public int[] Input1;
	public int[] Input2;
	public int[] Output1;
	public int[] Output2;

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
		UpdateLevels();
	}

	private void UpdateLevels()
	{
		FileInfo[] levelSources = new DirectoryInfo(Application.streamingAssetsPath + "/Levels").GetFiles("*.json");

		_levels = new GameLevel[levelSources.Length];

		for (int index = 0; index < levelSources.Length; index++)
		{
			Debug.Log(levelSources[index].FullName);
			_levels[index] = JsonUtility.FromJson<GameLevel>(File.ReadAllText(levelSources[index].FullName));
		}

		Array.Sort(_levels, GameLevel.Compare);

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
		UpdateLevels();
	}
}