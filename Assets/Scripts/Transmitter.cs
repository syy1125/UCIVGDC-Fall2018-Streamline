using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transmitter : MonoBehaviour
{
	[HideInInspector]
	public Vector2Int OutputDirection;

	[HideInInspector]
	public Vector2Int Location;

	private int _signal;

	public void Send(int signal)
	{
		_signal = signal;
	}

	public void Step()
	{
		Grid grid = Grid.Instance;
		GameObject outputWire = grid.GetGridComponent(Location + OutputDirection);
		if (grid.IsWire(outputWire))
		{
			outputWire.GetComponent<Wire>().SendSignal(_signal);
		}
	}
}