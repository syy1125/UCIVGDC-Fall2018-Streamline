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

	public int Num { get; private set; }

	public override void Step()
	{
		Num = 0;
		foreach (Vector2Int direction in Directions)
		{
			GetSignal(direction, signal => Num += signal);
		}
	}
}