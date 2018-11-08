using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplayState
{
}

public class LevelDisplay : StatefulUI<LevelDisplayState>
{
	public Text Title;
	public Text Description;
	public GameObject LoadButton;

	public LevelList LevelList;

	private Func<LevelListState> _getLevelListState;

	private void Start()
	{
		_getLevelListState = LinkState(LevelList);
		Render();
	}

	protected override void Render()
	{
		LevelListState state = _getLevelListState();

		if (state != null && state.SelectedIndex >= 0)
		{
			GameLevel level = state.Levels[state.SelectedIndex.Value];
			Title.text = level.Name;
			Description.text = level.Description;
			LoadButton.SetActive(true);
		}
		else
		{
			Title.text = "";
			Description.text = "";
			LoadButton.SetActive(false);
		}
	}
}