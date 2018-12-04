using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
	public GameObject WirePrefab;

	private IEnumerator Start()
	{
		// Note that I'm just hacking together some code that works. This is fairly inefficient.
		yield return new WaitForSeconds(1);
		Grid.Instance.SetGridComponent(2, 1, WirePrefab).GetComponent<Wire>().HasRed = true;
		Grid.Instance.GetGridComponent(2, 1).GetComponent<Wire>().Location = new Vector2Int(2, 1);

		Grid.Instance.SetGridComponent(2, 2, WirePrefab).GetComponent<Wire>().HasRed = true;
		Grid.Instance.GetGridComponent(2, 2).GetComponent<Wire>().Location = new Vector2Int(2, 2);

		Grid.Instance.SetGridComponent(2, 3, WirePrefab).GetComponent<Wire>().HasRed = true;
		Grid.Instance.GetGridComponent(2, 3).GetComponent<Wire>().Location = new Vector2Int(2, 3);

		Grid.Instance.SetGridComponent(3, 1, WirePrefab).GetComponent<Wire>().HasRed = true;
		Grid.Instance.GetGridComponent(3, 1).GetComponent<Wire>().Location = new Vector2Int(3, 1);

		Grid.Instance.SetGridComponent(3, 3, WirePrefab).GetComponent<Wire>().HasRed = true;
		Grid.Instance.GetGridComponent(3, 3).GetComponent<Wire>().Location = new Vector2Int(3, 3);

		Grid.Instance.SetGridComponent(4, 1, WirePrefab).GetComponent<Wire>().HasRed = true;
		Grid.Instance.GetGridComponent(4, 1).GetComponent<Wire>().Location = new Vector2Int(4, 1);

		Grid.Instance.SetGridComponent(4, 2, WirePrefab).GetComponent<Wire>().HasRed = true;
		Grid.Instance.GetGridComponent(4, 2).GetComponent<Wire>().Location = new Vector2Int(4, 2);

		Grid.Instance.SetGridComponent(4, 3, WirePrefab).GetComponent<Wire>().HasRed = true;
		Grid.Instance.GetGridComponent(4, 3).GetComponent<Wire>().Location = new Vector2Int(4, 3);

		Grid.Instance.GetGridComponent(2, 2).GetComponent<Wire>().HasGreen = true;
		Grid.Instance.SetGridComponent(3, 2, WirePrefab).GetComponent<Wire>().HasGreen = true;

		Grid.Instance.GetGridComponent(2, 2).GetComponent<Wire>().SetUp();

		Grid.Instance.GetGridComponent(2, 1).GetComponent<Wire>().SendSignal(10);

		Debug.Log("Expected red channel signal: 10. Actual: " +
		          Grid.Instance.GetGridComponent(2, 3).GetComponent<Wire>().SignalStrength);
		Debug.Log("Expected green channel signal: 0. Actual: " +
		          Grid.Instance.GetGridComponent(3, 2).GetComponent<Wire>().SignalStrength);

		Grid.Instance.GetGridComponent(2, 2).GetComponent<Wire>().SendSignal(12);

		Debug.Log("Expected red channel signal: 22. Actual: " +
		          Grid.Instance.GetGridComponent(2, 3).GetComponent<Wire>().SignalStrength);
		Debug.Log("Expected green channel signal: 12. Actual: " +
		          Grid.Instance.GetGridComponent(3, 2).GetComponent<Wire>().SignalStrength);

		Wire.ResetSignals();

		Grid.Instance.GetGridComponent(2, 2).GetComponent<Wire>().SendSignal(12);

		Debug.Log("Expected red channel signal: 12. Actual: " +
		          Grid.Instance.GetGridComponent(2, 3).GetComponent<Wire>().SignalStrength);
		Debug.Log("Expected green channel signal: 12. Actual: " +
		          Grid.Instance.GetGridComponent(3, 2).GetComponent<Wire>().SignalStrength);
	}
}