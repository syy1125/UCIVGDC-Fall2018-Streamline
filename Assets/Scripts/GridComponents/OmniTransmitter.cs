using UnityEngine;

public class OmniTransmitter : Transmitter
{
	private static readonly Vector2Int[] Directions =
	{
		Vector2Int.up,
		Vector2Int.down,
		Vector2Int.left,
		Vector2Int.right,
	};

	public override Vector2Int[] TransmissionDirections()
	{
		return Directions;
	}
}