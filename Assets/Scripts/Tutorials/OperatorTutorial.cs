using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class OperatorTutorial : Tutorial
{
	private IEnumerator Start()
	{
		Grid grid = Grid.Instance;

		// 1
		yield return WaitForSelection(ComponentType.ADD);
		Next();

		// 2
		yield return new WaitUntil(() =>
			grid.GetGridComponent(1, 0) != null
			&& grid.GetGridComponent(1, 0).GetComponent<Add>() != null);
		Next();

		// 3
		yield return WaitForSelection(ComponentType.REDWIRE);
		Next();

		// 4
		yield return WaitForWires(new[]
		{
			new Vector2Int(1, 1),
			new Vector2Int(1, 2),
			new Vector2Int(1, 3),
			new Vector2Int(1, 4),
			new Vector2Int(1, 5),
			new Vector2Int(1, 6),
			new Vector2Int(1, 7),
		}, wire => wire.HasRed);
		Next();
		
		// 5
		yield return WaitForSelection(ComponentType.NONE);
		Next();
		
		// 6
		yield return new WaitUntil(() =>
			grid.GetGridComponent(1, 0) != null
			&& grid.GetGridComponent(1, 0).GetComponent<Add>() != null
			&& grid.Selected == new Vector2Int(1, 0));
		Next();
		
		// 7
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		Next();
	}

	private CustomYieldInstruction WaitForSelection(ComponentType selection)
	{
		return new WaitUntil(() => ComponentSelection.selected == selection);
	}

	private CustomYieldInstruction WaitForWires(Vector2Int[] positions, Predicate<Wire> validate)
	{
		Grid grid = Grid.Instance;

		return new WaitUntil(() =>
		{
			foreach (Vector2Int position in positions)
			{
				if (!grid.IsWire(grid.GetGridComponent(position)))
				{
					return false;
				}

				if (!validate(grid.GetGridComponent(position).GetComponent<Wire>()))
				{
					return false;
				}
			}

			return true;
		});
	}
}