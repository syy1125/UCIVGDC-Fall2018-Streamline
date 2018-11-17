using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class OperatorTutorial : Tutorial
{
	private IEnumerator Start()
	{
		Grid grid = Grid.Instance;

		yield return WaitForSelection(ComponentType.MULT);
		Next();

		yield return new WaitUntil(() =>
			grid.GetGridComponent(0, 1) != null
			&& grid.GetGridComponent(0, 1).GetComponent<Multiply>() != null);
		Next();

		float startTime = Time.time;
		yield return new WaitUntil(() => Time.time - startTime > 10 || Input.GetKeyDown(KeyCode.Return));
		Next();

		yield return WaitForSelection(ComponentType.REDWIRE);
		Next();

		yield return WaitForWires(new[]
		{
			new Vector2Int(0, 2),
			new Vector2Int(0, 3),
			new Vector2Int(0, 4),
			new Vector2Int(0, 5),
			new Vector2Int(0, 6),
		}, wire => wire.HasRed);
		Next();

		yield return WaitForWires(new[]
		{
			new Vector2Int(1, 1),
			new Vector2Int(2, 1),
			new Vector2Int(3, 1),
			new Vector2Int(4, 1),
			new Vector2Int(5, 1),
			new Vector2Int(6, 1),
			new Vector2Int(7, 1),
		}, wire => wire.HasRed);
		Next();

		yield return WaitForSelection(ComponentType.ADD);
		Next();

		yield return new WaitUntil(() =>
			grid.GetGridComponent(2, 7) != null
			&& grid.GetGridComponent(2, 7).GetComponent<Add>() != null);
		Next();

		yield return WaitForSelection(ComponentType.GREENWIRE);
		Next();

		yield return new WaitUntil(() =>
			grid.IsWire(grid.GetGridComponent(1, 7))
			&& grid.GetGridComponent(1, 7).GetComponent<Wire>().HasGreen);
		Next();

		yield return WaitForWires(new[]
		{
			new Vector2Int(1, 0),
			new Vector2Int(2, 0),
		}, wire => wire.HasGreen);
		Next();

		yield return WaitForWires(new[]
		{
			new Vector2Int(2, 1),
			new Vector2Int(2, 2),
			new Vector2Int(2, 3),
			new Vector2Int(2, 4),
			new Vector2Int(2, 5),
			new Vector2Int(2, 6),
		}, wire => wire.HasGreen);
		Next();

		yield return WaitForSelection(ComponentType.NONE);
		Next();
		
		yield return new WaitUntil(() => grid.Selected == new Vector2Int(2,7));
		Next();
		
		
		yield return new WaitUntil(() =>
		{
			GameObject addTile = grid.GetGridComponent(2, 7);
			if (addTile == null) return false;
			Add addOp = addTile.GetComponent<Add>();
			if (addOp == null) return false;

			Transmitter t = addTile.GetComponent<Transmitter>();
			Receiver r = addTile.GetComponent<Receiver>();

			if (t.OutputDirection != Vector2Int.right) return false;

			if (r.InputDirection1 == Vector2Int.left && r.InputDirection2 == Vector2Int.down) return true;
			if (r.InputDirection1 == Vector2Int.down && r.InputDirection2 == Vector2Int.left) return true;

			return false;
		});
		Next();

		startTime = Time.time;
		yield return new WaitUntil(() => Time.time - startTime > 10 || Input.GetKeyDown(KeyCode.Return));
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