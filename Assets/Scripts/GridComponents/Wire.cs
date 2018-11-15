using System;
using System.Collections.Generic;
using System.Linq;
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
	private static readonly HashSet<WireNetwork> _networks = new HashSet<WireNetwork>();

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
			UpdateTexture(RedParts, RedWireConnects);
		}
	}

	public bool HasGreen
	{
		get { return _hasGreen; }
		set
		{
			_hasGreen = value;
			UpdateTexture(GreenParts, GreenWireConnects);
		}
	}

	[HideInInspector]
	public Vector2Int Location;

	private void Start()
	{
		_redNetwork = null;
		_greenNetwork = null;

		UpdateAllTextures();
	}

	private bool RedWireConnects(Vector2Int direction)
	{
		return HasRedWireAt(direction)
		       || TileHasIO(direction);
	}

	private bool GreenWireConnects(Vector2Int direction)
	{
		return HasGreenWireAt(direction)
		       || TileHasIO(direction);
	}

	private bool HasRedWireAt(Vector2Int direction)
	{
		GameObject g = Grid.Instance.GetGridComponent(Location + direction);
		return g != null && g.GetComponent<Wire>() != null && g.GetComponent<Wire>().HasRed;
	}

	private bool HasGreenWireAt(Vector2Int direction)
	{
		GameObject wireTile = Grid.Instance.GetGridComponent(Location + direction);
		return wireTile != null && wireTile.GetComponent<Wire>() != null && wireTile.GetComponent<Wire>().HasGreen;
	}

	private bool TileHasIO(Vector2Int direction)
	{
		GameObject tile = Grid.Instance.GetGridComponent(Location + direction);

		if (!Grid.Instance.IsOperator(tile)) return false;

		return tile.GetComponent<Transmitter>() != null
		       && tile.GetComponent<Transmitter>().TransmissionDirections().Contains(direction * -1)
		       || tile.GetComponent<Receiver>() != null
		       && tile.GetComponent<Receiver>().ReceptionDirections().Contains(direction * -1);
	}

	public void UpdateTexture(Parts wireParts, Predicate<Vector2Int> shouldConnect)
	{
		bool selfHasColor = shouldConnect(Vector2Int.zero);

		wireParts.Center.SetActive(selfHasColor);
		wireParts.Up.SetActive(selfHasColor && shouldConnect(Vector2Int.up));
		wireParts.Down.SetActive(selfHasColor && shouldConnect(Vector2Int.down));
		wireParts.Left.SetActive(selfHasColor && shouldConnect(Vector2Int.left));
		wireParts.Right.SetActive(selfHasColor && shouldConnect(Vector2Int.right));
	}

	public void UpdateAllTextures()
	{
		UpdateTexture(RedParts, RedWireConnects);
		UpdateTexture(GreenParts, GreenWireConnects);
	}

	private static void ForAllWires(Action<Wire> action)
	{
		Grid grid = Grid.Instance;

		for (int x = 0; x < grid.Width; x++)
		{
			for (int y = 0; y < grid.Height; y++)
			{
				if (grid.IsWire(grid.GetGridComponent(x, y)))
				{
					action(grid.GetGridComponent(x, y).GetComponent<Wire>());
				}
			}
		}
	}

	public static void GlobalSetUp()
	{
		ForAllWires(wire => wire.SetUp());
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

			_networks.Add(network);
		}

		if (HasGreen && _greenNetwork == null)
		{
			var network = new WireNetwork();

			FloodFill(
				Location,
				wire => wire._hasGreen,
				wire => wire._greenNetwork = network
			);

			_networks.Add(network);
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

	public static void ResetSignals()
	{
		foreach (WireNetwork network in _networks)
		{
			network.Value = 0;
		}
	}

	public static void GlobalTearDown()
	{
		ForAllWires(wire => wire.TearDown());

		_networks.Clear();
	}

	public void TearDown()
	{
		_redNetwork = null;
		_greenNetwork = null;
	}

	public string SaveString()
	{
		string result = "";
		result += "" + (int) ComponentType.REDWIRE + "\t"; //can be either REDWIRE or GREENWIRE
		//redness or greenness is encoded later
		result += "" + Location.x + "\t";
		result += "" + Location.y + "\t";
		if (HasGreen && HasRed)
			result += "" + (int) SaveData.WireType.BOTH;
		else if (HasGreen)
			result += "" + (int) SaveData.WireType.GREEN;
		else
			result += "" + (int) SaveData.WireType.RED;
		return result;
	}
}