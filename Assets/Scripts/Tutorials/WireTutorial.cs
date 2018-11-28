using System.Collections;
using UnityEngine;

public class WireTutorial : Tutorial
{
	private IEnumerator Start()
	{
		// 1
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();

		// 2
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();
		
		// 3
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();
		
		// 4
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();
		
		// 5
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
		Next();
		
		// 6
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		yield return null;
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
		yield return new WaitUntil(() =>
			ComponentSelection.selected == ComponentType.REDWIRE
			|| ComponentSelection.selected == ComponentType.GREENWIRE);

		Next();

		// 10
		Grid grid = Grid.Instance;
		yield return new WaitUntil(() =>
		{
			for (int x = 1; x <= 6; x++)
			{
				if (grid.GetGridComponent(x, grid.Height - 1) == null)
				{
					return false;
				}

				if (!grid.IsWire(grid.GetGridComponent(x, grid.Height - 1)))
				{
					return false;
				}
			}

			return true;
		});
		Next();

		// 11
		yield return new WaitUntil(() =>
		{
			for (int x = 1; x <= 6; x++)
			{
				if (grid.GetGridComponent(x, 0) == null)
				{
					return false;
				}

				if (!grid.IsWire(grid.GetGridComponent(x, 0)))
				{
					return false;
				}
			}

			return true;
		});
		Next();
		
		// 12
		yield return new WaitUntil(() => GameController.simState == SimState.RUNNING);
		Next();
		
		// 13
		yield return new WaitUntil(() => GameController.simState != SimState.RUNNING);
		Next();
	}
}