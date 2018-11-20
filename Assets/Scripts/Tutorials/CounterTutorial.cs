using System.Collections;
using UnityEngine;

public class CounterTutorial : Tutorial
{
	private IEnumerator Start()
	{
		Grid grid = Grid.Instance;

		yield return null;
		
		yield return new WaitUntil(() =>
			grid.GetGridComponent(4, 1) != null
			&& grid.GetGridComponent(4, 1).GetComponent<Add>() != null);
		Next();

		yield return new WaitUntil(() =>
			grid.IsWire(grid.GetGridComponent(5, 1))
			&& grid.IsWire(grid.GetGridComponent(5, 2))
			&& grid.IsWire(grid.GetGridComponent(4, 2))
		);
		Next();

		yield return new WaitUntil(() =>
			grid.GetGridComponent(4, 0) != null
			&& grid.GetGridComponent(4, 0).GetComponent<Constant>() != null);
		Next();

		yield return new WaitUntil(() =>
			grid.GetGridComponent(4, 0) != null
			&& grid.GetGridComponent(4, 0).GetComponent<Transmitter>() != null
			&& grid.GetGridComponent(4, 0).GetComponent<Constant>() != null
			&& grid.GetGridComponent(4, 0).GetComponent<Transmitter>().OutputDirection == Vector2Int.up
			&& grid.GetGridComponent(4, 0).GetComponent<Constant>().number == 1);
		Next();

		yield return new WaitUntil(() =>
			grid.IsWire(grid.GetGridComponent(6, 1))
			&& grid.IsWire(grid.GetGridComponent(7, 1)));
		Next();
	}
}