using System;
using UnityEngine;

public class Receiver : MonoBehaviour
{
	public int Num1 { get; private set; }
	public int Num2 { get; private set; }

	public Vector2Int Location;
	public Vector2Int InputDirection1;
	public Vector2Int InputDirection2;

	public void Step()
	{
		// Pull from wire special case
		TryPullFromWire(Location + InputDirection1, num => Num1 = num);
		TryPullFromWire(Location + InputDirection2, num => Num2 = num);
	}

	private static void TryPullFromWire(Vector2Int location, Action<int> setNum)
	{
		if (Grid.Instance.InGrid(location) && Grid.Instance.IsWire(Grid.Instance.GetGridComponent(location)))
		{
			setNum(Grid.Instance.GetGridComponent(location).GetComponent<Wire>().SignalStrength);
		}
	}

	public void FromTransmitter(Vector2Int direction, int signal)
	{
		if (direction == InputDirection1) Num1 = signal;
		if (direction == InputDirection2) Num2 = signal;
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