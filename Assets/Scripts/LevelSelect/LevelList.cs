using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelListState
{
	[CanBeNull]
	public GameLevel[] Levels;

	[CanBeNull]
	public string Search;

	public bool? Tutorial;

	public int? SelectedIndex;
}

public class LevelList : StatefulUI<LevelListState>
{
	public GameObject LevelsGrid;
	public GameObject ButtonPrefab;
	public Button TutorialSwitch;
	public Button ChallengeSwitch;

	public Color SelectedColor;
	private ColorBlock _selectedColors;

	private void Start()
	{
		_selectedColors = new ColorBlock()
		{
			colorMultiplier = 1,
			disabledColor = SelectedColor,
			highlightedColor = SelectedColor,
			normalColor = SelectedColor,
			pressedColor = SelectedColor
		};

		State = new LevelListState
		{
			Levels = new GameLevel[0],
			Search = "",
			SelectedIndex = -1,
			Tutorial = PlayerPrefs.GetInt("LevelList.Tutorial", 0) == 1
		};

		ReadLevels();
	}

	private void ReadLevels()
	{
		FileInfo[] levelSources = new DirectoryInfo(Application.streamingAssetsPath + "/Levels").GetFiles("*.json");

		GameLevel[] levels = new GameLevel[levelSources.Length];

		for (int index = 0; index < levelSources.Length; index++)
		{
			levels[index] = JsonUtility.FromJson<GameLevel>(File.ReadAllText(levelSources[index].FullName));
			levels[index].FileName = levelSources[index].Name;
		}

		Array.Sort(levels, GameLevel.Compare);

		State = new LevelListState()
		{
			Levels = levels
		};
	}

	protected override void Render()
	{
		foreach (Transform childTransform in LevelsGrid.transform)
		{
			Destroy(childTransform.gameObject);
		}

		for (int index = 0; index < State.Levels.Length; index++)
		{
			if (State.Tutorial.Value == (State.Levels[index].Tutorial == null)) continue;;
			if (!State.Search.Equals("") && !State.Levels[index].Name.ToLower().Contains(State.Search)) continue;

			GameObject button = Instantiate(ButtonPrefab, LevelsGrid.transform);

			int i = index; // `i` is an immutable index
			button.GetComponent<Button>().onClick.AddListener(() => UpdateSelection(i));

			if (index == State.SelectedIndex)
			{
				button.GetComponent<Button>().colors = _selectedColors;
			}

			button.GetComponentInChildren<Text>().text = State.Levels[index].Name;
		}

		TutorialSwitch.colors = State.Tutorial.Value ? _selectedColors : ColorBlock.defaultColorBlock;
		ChallengeSwitch.colors = State.Tutorial.Value ? ColorBlock.defaultColorBlock : _selectedColors;
	}

	public void UpdateSearch(string search)
	{
		State = new LevelListState
		{
			Search = search.ToLower()
		};
	}

	public void UpdateTutorial(bool tutorial)
	{
		State = new LevelListState
		{
			Tutorial = tutorial
		};
		PlayerPrefs.SetInt("LevelList.Tutorial", tutorial ? 1 : 0);
	}

	private void UpdateSelection(int index)
	{
		State = new LevelListState
		{
			SelectedIndex = index
		};
	}

	public void ReloadLevels()
	{
		ReadLevels();
		State = new LevelListState
		{
			SelectedIndex = -1
		};
	}

	public void PlaySelectedLevel(int solutionIndex)
	{
		GameController.solutionNum = solutionIndex;
		GameLevel level = State.Levels[State.SelectedIndex.Value];

		GameController.gameLevel = level;

		StartCoroutine(GameController.TransitionAndLoad("CircuitGrid"));
	}
}