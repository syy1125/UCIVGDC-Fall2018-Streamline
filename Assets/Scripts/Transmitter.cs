using UnityEngine;

public class Transmitter : MonoBehaviour
{
	public Vector2Int OutputDirection = Vector2Int.right;

	[HideInInspector]
	public Vector2Int Location;

	public int Signal { get; private set; }

	public void Send(int signal)
	{
		Signal = signal;
	}

	public virtual Vector2Int[] TransmissionDirections()
	{
		return new[] {OutputDirection};
	}

	public void Step()
	{
		Grid grid = Grid.Instance;

		foreach (Vector2Int direction in TransmissionDirections())
		{
			GameObject outputTile = grid.GetGridComponent(Location + direction);
			if (grid.IsWire(outputTile))
			{
				outputTile.GetComponent<Wire>().SendSignal(Signal);
			}
		}
	}
	public void Reset()
	{
		Signal = 0;
	}
}