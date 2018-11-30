using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Rendering;

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
		yield return null;
		Next();

		// 8
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();

		// 9
		yield return new WaitUntil(() => ComponentEditor.selection == ArrowSelection.IN2);
		Next();

		// 10
		yield return new WaitUntil(() =>
			grid.GetGridComponent(1, 0) != null
			&& grid.GetGridComponent(1, 0).GetComponent<Receiver>() != null
			&& grid.GetGridComponent(1, 0).GetComponent<Receiver>().InputDirection2 == new Vector2Int(-1, 0)
			&& grid.Selected == new Vector2Int(1, 0)
			&& grid.GetGridComponent(1, 0).GetComponent<Add>() != null);
		Next();

		// 11
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();

		// 12
		yield return WaitForSelection(ComponentType.DIV);
		Next();

		// 13
		yield return new WaitUntil(() =>
			grid.GetGridComponent(4, 0) != null
			&& grid.GetGridComponent(4, 0).GetComponent<Divide>() != null);
		Next();

		// 14
		yield return WaitForSelection(ComponentType.CONSTANT);
		Next();

		// 15
		yield return new WaitUntil(() =>
			grid.GetGridComponent(4, 1) != null
			&& grid.GetGridComponent(4, 1).GetComponent<Constant>() != null);
		Next();

		// 16
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();

		// 17
		yield return new WaitUntil(() =>
			grid.GetGridComponent(4, 1) != null
			&& grid.GetGridComponent(4, 1).GetComponent<Constant>() != null
			&& grid.Selected == new Vector2Int(4, 1)
			&& grid.GetGridComponent(4, 1).GetComponent<Transmitter>() != null
			&& grid.GetGridComponent(4, 1).GetComponent<Constant>().number == 2
			&& grid.GetGridComponent(4, 1).GetComponent<Transmitter>().OutputDirection == Vector2Int.down);
		Next();
		
		// 18
		yield return WaitForSelection(ComponentType.NONE);
		Next();
		
		// 19
		yield return new WaitUntil(() =>
			grid.GetGridComponent(4, 0) != null
			&& grid.GetGridComponent(4, 0).GetComponent<Divide>() != null
			&& grid.Selected == new Vector2Int(4, 0));
		Next();
		
		// 20
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();
		
		// 21
		yield return new WaitUntil(() =>
			grid.GetGridComponent(4, 0) != null
			&& grid.GetGridComponent(4, 0).GetComponent<Divide>() != null
			&& grid.Selected == new Vector2Int(4, 0)
			&& grid.GetGridComponent(4, 0).GetComponent<Receiver>() != null
			&& grid.GetGridComponent(4, 0).GetComponent<Transmitter>() != null
			&& grid.GetGridComponent(4, 0).GetComponent<Receiver>().InputDirection1 == Vector2Int.left
			&& grid.GetGridComponent(4, 0).GetComponent<Receiver>().InputDirection2 == Vector2Int.up
			&& grid.GetGridComponent(4, 0).GetComponent<Transmitter>().OutputDirection == Vector2Int.right);
		Next();
		
		// 22
		yield return WaitForSelection(ComponentType.REDWIRE);
		Next();
		
		// 23
		yield return WaitForWires(new[]
		{
			new Vector2Int(2, 0),
			new Vector2Int(3, 0),
			new Vector2Int(5, 0),
			new Vector2Int(6, 0),
		}, wire => wire.HasRed);
		Next();
		
		// 24
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();
		
		// 25
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();
		
		// 26
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();
		
		// 27
		yield return new WaitUntil(() => GameController.simState == SimState.RUNNING);
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