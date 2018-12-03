using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CounterTutorial : Tutorial
{
	private IEnumerator Start()
	{
        base.Start();
        Grid grid = Grid.Instance;

		// 1
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();

		// 2
		ModifyBackdrops(backdrop =>
		{
			Image image = backdrop.GetComponent<Image>();
			image.raycastTarget = false;
			image.CrossFadeAlpha(0, Manager.MoveDuration, false);
		});
		yield return new WaitUntil(() =>
			grid.GetGridComponent(5, 1) != null
			&& grid.GetGridComponent(5, 1).GetComponent<Add>() != null);
		Next();

		// 3
		yield return new WaitUntil(() =>
			grid.IsWire(grid.GetGridComponent(6, 1))
			&& grid.IsWire(grid.GetGridComponent(6, 2))
			&& grid.IsWire(grid.GetGridComponent(5, 2))
			&& (
				grid.GetGridComponent(6, 1).GetComponent<Wire>().HasRed
				&& grid.GetGridComponent(6, 2).GetComponent<Wire>().HasRed
				&& grid.GetGridComponent(5, 2).GetComponent<Wire>().HasRed
				|| grid.GetGridComponent(6, 1).GetComponent<Wire>().HasGreen
				&& grid.GetGridComponent(6, 2).GetComponent<Wire>().HasGreen
				&& grid.GetGridComponent(5, 2).GetComponent<Wire>().HasGreen
			));
		Next();

		// 4
		yield return new WaitUntil(() =>
			grid.GetGridComponent(5, 0) != null
			&& grid.GetGridComponent(5, 0).GetComponent<Constant>() != null);
		Next();

		// 5
		ModifyBackdrops(backdrop =>
		{
			Image image = backdrop.GetComponent<Image>();
			image.raycastTarget = true;
			image.CrossFadeAlpha(1, Manager.MoveDuration, false);
		});
		yield return new WaitUntil(() =>
			grid.GetGridComponent(5, 0) != null
			&& grid.GetGridComponent(5, 0).GetComponent<Constant>() != null
			&& grid.Selected == new Vector2Int(5, 0)
			&& grid.GetGridComponent(5, 0).GetComponent<Constant>().number == 1
			&& grid.GetGridComponent(5, 0).GetComponent<Transmitter>().OutputDirection == Vector2Int.up);
		Next();

		// 6
		ModifyBackdrops(backdrop =>
		{
			Image image = backdrop.GetComponent<Image>();
			image.raycastTarget = false;
			image.CrossFadeAlpha(0, Manager.MoveDuration, false);
		});
		yield return new WaitUntil(() =>
			grid.IsWire(grid.GetGridComponent(6, 1))
			&& grid.IsWire(grid.GetGridComponent(6, 2))
			&& grid.IsWire(grid.GetGridComponent(5, 2))
			&& grid.IsWire(grid.GetGridComponent(7, 1))
			&& (
				grid.GetGridComponent(6, 1).GetComponent<Wire>().HasRed
				&& grid.GetGridComponent(6, 2).GetComponent<Wire>().HasRed
				&& grid.GetGridComponent(5, 2).GetComponent<Wire>().HasRed
				&& grid.GetGridComponent(7, 1).GetComponent<Wire>().HasRed
				|| grid.GetGridComponent(6, 1).GetComponent<Wire>().HasGreen
				&& grid.GetGridComponent(6, 2).GetComponent<Wire>().HasGreen
				&& grid.GetGridComponent(5, 2).GetComponent<Wire>().HasGreen
				&& grid.GetGridComponent(7, 1).GetComponent<Wire>().HasGreen
			));
		Next();

		// 7
		yield return new WaitUntil(() => GameController.simState == SimState.RUNNING);
		Next();
	}
}