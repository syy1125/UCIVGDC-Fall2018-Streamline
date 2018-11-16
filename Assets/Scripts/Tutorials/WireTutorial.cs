using System.Collections;
using UnityEngine;

public class WireTutorial : Tutorial
{
	private IEnumerator Start()
	{
		yield return new WaitUntil(() =>
			ComponentSelection.selected == ComponentType.REDWIRE
			|| ComponentSelection.selected == ComponentType.GREENWIRE);

		Next();

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
	}
}