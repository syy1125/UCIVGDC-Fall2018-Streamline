using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDisplayState
{
}

public class LevelDisplay : StatefulUI<LevelDisplayState>
{
	public Text Title;
	public Text Description;
	public GameObject[] LoadButtons;

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
			Description.text = string.Join(" ", level.Description);
			for(int i = 0; i < LoadButtons.Length; i++){
				LoadButtons[i].SetActive(true);
			}
		}
		else
		{
			Title.text = "";
			Description.text = "";
			for(int i = 0; i < LoadButtons.Length; i++){
				LoadButtons[i].SetActive(false);
			}
		}
	}
}