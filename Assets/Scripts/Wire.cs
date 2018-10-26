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

	private bool _hasRed;
	private bool _hasGreen;

	public bool HasRed
	{
		get { return _hasRed; }
		set
		{
			_hasRed = value;
			UpdateTexture();
		}
	}

	public bool HasGreen
	{
		get { return _hasGreen; }
		set
		{
			_hasGreen = value;
			UpdateTexture();
		}
	}

	public Vector2Int Location;

	private void Start()
	{
		_redNetwork = null;
		_greenNetwork = null;
	}

	public void UpdateTexture()
	{
	}

	public void SetUp()
	{
		if (HasRed && _redNetwork == null)
		{
			var network = new WireNetwork();
			
			FloodFill(
				Location,
				wire => wire._hasRed,
				wire => wire._redNetwork = network
			);
		}

		if (HasGreen && _greenNetwork == null)
		{
			var network = new WireNetwork();
			
			FloodFill(
				Location,
				wire => wire._hasGreen,
				wire => wire._greenNetwork = network
			);
		}
	}

	private void FloodFill(
		Vector2Int start,
		Predicate<Wire> include,
		Action<Wire> update
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
				if (w == null || !include(w)) continue;

				// Update Wire
				update(w);

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