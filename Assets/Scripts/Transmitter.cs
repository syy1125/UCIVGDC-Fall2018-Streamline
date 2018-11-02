using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transmitter : MonoBehaviour
{
	[HideInInspector]
	public Vector2Int OutputDirection;

	[HideInInspector]
	public Vector2Int Location;

	public int Signal { get; private set; }

	public void Send(int signal)
	{
		Signal = signal;
	}

	public void Step()
	{
		Grid grid = Grid.Instance;
		GameObject outputTile = grid.GetGridComponent(Location + OutputDirection);
		
		if (grid.IsWire(outputTile))
		{
			outputTile.GetComponent<Wire>().SendSignal(Signal);
		}
	}
}