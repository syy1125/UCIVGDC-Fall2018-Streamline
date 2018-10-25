using System;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
	public bool HasRed;
	public bool HasGreen;

	private class WireNetwork
	{
		public int Value;
	}

	private WireNetwork _redNetwork;
	private WireNetwork _greenNetwork;

	public int SignalStrength
	{
		get
		{
			int value = 0;
			if (_redNetwork != null) value += _redNetwork.Value;
			if (_greenNetwork != null) value += _greenNetwork.Value;
			return value;
		}
	}

	public Vector2Int Location;

	private void Start()
	{
		_redNetwork = null;
		_greenNetwork = null;
	}

	public void SetUp()
	{
		if (HasRed && _redNetwork == null)
		{
			BuildNetwork(
				new WireNetwork(),
				Location,
				wire => wire.HasRed,
				(wire, network) => wire._redNetwork = network
			);
		}

		if (HasGreen && _greenNetwork == null)
		{
			BuildNetwork(
				new WireNetwork(),
				Location,
				wire => wire.HasGreen,
				(wire, network) => wire._greenNetwork = network
			);
		}
	}

	private void BuildNetwork(
		WireNetwork network,
		Vector2Int start,
		Predicate<Wire> hasColor,
		Action<Wire, WireNetwork> setNetwork
	)
	{
		// Flood fill algorithm - avoids loops in the wire system and so on.
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		Queue<Vector2Int> boundary = new Queue<Vector2Int>();
		boundary.Enqueue(start);

		while (boundary.Count > 0)
		{
			Vector2Int next = boundary.Dequeue();

			if (visited.Add(next))
			{
				// Checks
				GameObject g = Grid.Instance.GetGridComponent(next);
				if (g == null) continue;
				Wire w = g.GetComponent<Wire>();
				if (w == null || !hasColor(w)) continue;

				// Update Wire
				setNetwork(w, network);

				// Propagate search
				Vector2Int up = next + Vector2Int.up;
				if (!visited.Contains(up) && Grid.Instance.InGrid(up))
				{
					boundary.Enqueue(up);
				}

				Vector2Int down = next + Vector2Int.down;
				if (!visited.Contains(down) && Grid.Instance.InGrid(down))
				{
					boundary.Enqueue(down);
				}

				Vector2Int left = next + Vector2Int.left;
				if (!visited.Contains(left) && Grid.Instance.InGrid(left))
				{
					boundary.Enqueue(left);
				}

				Vector2Int right = next + Vector2Int.right;
				if (!visited.Contains(right) && Grid.Instance.InGrid(right))
				{
					boundary.Enqueue(right);
				}
			}
		}
	}

	public void SendSignal(int signal)
	{
		if (_redNetwork != null) _redNetwork.Value += signal;
		if (_greenNetwork != null) _greenNetwork.Value += signal;
	}

	public void ResetSignal()
	{
		if (_redNetwork != null) _redNetwork.Value = 0;
		if (_greenNetwork != null) _greenNetwork.Value = 0;
	}

	public void TearDown()
	{
		_redNetwork = null;
		_greenNetwork = null;
	}
}