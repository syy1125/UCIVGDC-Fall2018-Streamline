using System;
using UnityEngine;

public class OmniReceiver : Receiver
{
	private static readonly Vector2Int[] Directions =
	{
		Vector2Int.up,
		Vector2Int.down,
		Vector2Int.left,
		Vector2Int.right
	};

	public override void Step()
	{
		Num1 = 0;
		foreach (Vector2Int direction in Directions)
		{
			GetSignal(direction, signal => Num1 += signal);
		}
	}
}