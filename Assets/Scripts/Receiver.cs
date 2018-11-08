using System;
using System.Linq;
using UnityEngine;

public class Receiver : MonoBehaviour
{
	public int Num1 { get; private set; }
	public int Num2 { get; private set; }

	public Vector2Int Location;
	public Vector2Int InputDirection1;
	public Vector2Int InputDirection2;

	public virtual void Step()
	{
		// Pull from wire special case
		GetSignal(InputDirection1, num => Num1 = num);
		GetSignal(InputDirection2, num => Num2 = num);
	}

	protected void GetSignal(Vector2Int direction, Action<int> setNum)
	{
		Grid grid = Grid.Instance;
		GameObject tile = grid.GetGridComponent(Location + direction);

		if (grid.IsWire(tile))
		{
			setNum(tile.GetComponent<Wire>().SignalStrength);
		}
		else if (
			grid.IsOperator(tile)
			&& tile.GetComponent<Transmitter>().TransmissionDirections().Contains(direction * -1)
		)
		{
			setNum(tile.GetComponent<Transmitter>().Signal);
		}
	}

	public int GetNum1()
	{
		return Num1;
	}

	public int GetNum2()
	{
		return Num2;
	}
}