using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
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

	public LevelTest[] Tests = new LevelTest[0];

	public static int Compare(GameLevel left, GameLevel right)
	{
		return string.CompareOrdinal(left.Name, right.Name);
	}
}

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance;

	public GameObject ButtonPrefab;

	private GameLevel[] _levels;
	private string _search = "";

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Debug.Log("Multiple `LevelManager`s instantiated. Destroying the extra ones.");
			Destroy(gameObject);
			return;
		}

		ReadLevels();
		UpdateDisplay();
	}

	private void ReadLevels()
	{
		FileInfo[] levelSources = new DirectoryInfo(Application.streamingAssetsPath + "/Levels").GetFiles("*.json");

		_levels = new GameLevel[levelSources.Length];

		for (int index = 0; index < levelSources.Length; index++)
		{
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
			if (!_search.Equals("") && !_levels[index].Name.ToLower().Contains(_search)) continue;

			GameObject button = Instantiate(ButtonPrefab, transform);

			int i = index; // `i` is an immutable index
			button.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(PlayLevel(i)));

			button.GetComponentInChildren<Text>().text = _levels[index].Name;
		}
	}

	public void UpdateSearch(string search)
	{
		_search = search.ToLower();
		UpdateDisplay();
	}

	public void ReloadLevels()
	{
		ReadLevels();
		UpdateDisplay();
	}

	private IEnumerator PlayLevel(int index)
	{
		AsyncOperation load = SceneManager.LoadSceneAsync("");

		while (!load.isDone) yield return null;
	}
}