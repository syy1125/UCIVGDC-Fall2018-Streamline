using System;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;

public class Wire : MonoBehaviour
{
	[Serializable]
	public struct Parts
	{
		public GameObject Center;
		public GameObject Up;
		public GameObject Down;
		public GameObject Left;
		public GameObject Right;
	}

	public Parts RedParts;
	public Parts GreenParts;

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
			UpdateTexture(RedParts, HasRedWireAt);
		}
	}

	public bool HasGreen
	{
		get { return _hasGreen; }
		set
		{
			_hasGreen = value;
			UpdateTexture(GreenParts, HasGreenWireAt);
		}
	}

	public Vector2Int Location;

	private void Start()
	{
		_redNetwork = null;
		_greenNetwork = null;
		
		UpdateAllTextures();
	}

	private static bool HasRedWireAt(Vector2Int location)
	{
		GameObject g = Grid.Instance.InGrid(location) ? Grid.Instance.GetGridComponent(location) : null;
		return g != null && g.GetComponent<Wire>() != null && g.GetComponent<Wire>().HasRed;
	}

	private static bool HasGreenWireAt(Vector2Int location)
	{
		GameObject g = Grid.Instance.InGrid(location) ? Grid.Instance.GetGridComponent(location) : null;
		return g != null && g.GetComponent<Wire>() != null && g.GetComponent<Wire>().HasGreen;
	}

	public void UpdateTexture(Parts wireParts, Predicate<Vector2Int> shouldConnect)
	{
		bool selfHasColor = shouldConnect(Location);
		 
		wireParts.Center.SetActive(selfHasColor);
		wireParts.Up.SetActive(selfHasColor && shouldConnect(Location + Vector2Int.up));
		wireParts.Down.SetActive(selfHasColor && shouldConnect(Location + Vector2Int.down));
		wireParts.Left.SetActive(selfHasColor && shouldConnect(Location + Vector2Int.left));
		wireParts.Right.SetActive(selfHasColor && shouldConnect(Location + Vector2Int.right));
	}

	public void UpdateAllTextures()
	{
		UpdateTexture(RedParts, HasRedWireAt);
		UpdateTexture(GreenParts, HasGreenWireAt);
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