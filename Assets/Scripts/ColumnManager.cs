using System;
using UnityEngine;
using UnityEngine.UI;

public class ColumnManager : MonoBehaviour
{
	public GameObject ImporterPrefab;
	public GameObject ExporterPrefab;

	private GameObject _importer1;
	private GameObject _importer2;
	private GameObject _exporter1;
	private GameObject _exporter2;

	public GameObject ColumnsParent;
	public Button Left;
	public Text TestSelection;
	public Button Right;

	public static bool[] LevelIOMask = {true, true, true, true};

	// Use `SetTestIndex` or `OffsetTestIndex` to set it. Those correctly trigger side effects.
	public static int TestIndex { get; private set; }
	
	public void OnGridReady()
	{
		InitIOMask();
		InitIOTiles();
		
		SetTestIndex(0);
	}

	private void InitIOMask()
	{
		GameLevel level = GameController.gameLevel;
		if (level.Tests.Length > 0)
		{
			LevelIOMask = new bool[4];
			LevelIOMask[0] = level.Tests[0].Input1.Length > 0;
			LevelIOMask[1] = level.Tests[0].Input2.Length > 0;
			LevelIOMask[2] = level.Tests[0].Output1.Length > 0;
			LevelIOMask[3] = level.Tests[0].Output2.Length > 0;
		}
		else
		{
			LevelIOMask = new[] {false, false, false, false};
		}
	}

	private void InitIOTiles()
	{
		Grid grid = Grid.Instance;
		int inputCount = 1;
		int outputCount = 1;

		if (LevelIOMask[0])
		{
			_importer1 = grid.SetGridComponent(0, grid.Height - 1, ImporterPrefab);
			_importer1.GetComponent<Text>().text = "I" + inputCount;
			_importer1.GetComponent<Importer>().outputColumn = MakeImporterColumn(inputCount++);
		}

		if (LevelIOMask[1])
		{
			_importer2 = grid.SetGridComponent(0, 0, ImporterPrefab);
			_importer2.GetComponent<Text>().text = "I" + inputCount;
			_importer2.GetComponent<Importer>().outputColumn = MakeImporterColumn(inputCount++);
		}

		ColArray[] exporterColumns = null;
		if (LevelIOMask[2])
		{
			_exporter1 = grid.SetGridComponent(grid.Width - 1, 0, ExporterPrefab);
			_exporter1.GetComponent<Text>().text = "O" + outputCount;
			exporterColumns = MakeExporterColumn(outputCount++);
			_exporter1.GetComponent<Exporter>().expectedOutputColumn = exporterColumns[0];
			_exporter1.GetComponent<Exporter>().outputColumn = exporterColumns[1];
		}

		if (LevelIOMask[3])
		{
			_exporter2 = grid.SetGridComponent(grid.Width - 1, grid.Height - 1, ExporterPrefab);
			_exporter2.GetComponent<Text>().text = "O" + outputCount;
			exporterColumns = MakeExporterColumn(outputCount++);
			_exporter2.GetComponent<Exporter>().expectedOutputColumn = exporterColumns[0];
			_exporter2.GetComponent<Exporter>().outputColumn = exporterColumns[1];
		}
	}

	private ColArray MakeImporterColumn(int columnIndex)
	{
		GameObject col = Instantiate(
			Resources.Load<GameObject>("InputColumn"),
			ColumnsParent.transform
		);
		col.GetComponentsInChildren<Text>()[0].text = "In." + columnIndex;
		return col.GetComponentInChildren<ColArray>();
	}

	private ColArray[] MakeExporterColumn(int columnIndex)
	{
		GameObject col = Instantiate(
			Resources.Load<GameObject>("OutputColumn"),
			ColumnsParent.transform
		);
		col.GetComponentsInChildren<Text>()[0].text = "Out." + columnIndex;
		return col.GetComponentsInChildren<ColArray>();
	}

	public void SetTestIndex(int newIndex)
	{
		if (newIndex < 0)
		{
			throw new IndexOutOfRangeException("Test index cannot be negative.");
		}

		if (newIndex >= GameController.gameLevel.Tests.Length && GameController.gameLevel.Tests.Length > 0)
		{
			throw new IndexOutOfRangeException("Test index cannot be higher than number of test cases.");
		}

		TestIndex = newIndex;

		UpdateTestSequence();
		UpdatePaginationDisplay();
	}

	private void UpdateTestSequence()
	{
		GameLevel level = GameController.gameLevel;
		if (LevelIOMask[0])
			_importer1.GetComponent<Importer>().Sequence = level.Tests[TestIndex].Input1;
		if (LevelIOMask[1])
			_importer2.GetComponent<Importer>().Sequence = level.Tests[TestIndex].Input2;
		if (LevelIOMask[2])
			_exporter1.GetComponent<Exporter>().ExpectedOutput = level.Tests[TestIndex].Output1;
		if (LevelIOMask[3])
			_exporter2.GetComponent<Exporter>().ExpectedOutput = level.Tests[TestIndex].Output2;
	}

	private void UpdatePaginationDisplay()
	{
		TestSelection.text =
			(GameController.gameLevel.Tests.Length > 0 ? TestIndex + 1 : 0)
			+ " / "
			+ GameController.gameLevel.Tests.Length;
		Left.interactable = TestIndex > 0;
		Right.interactable = TestIndex < GameController.gameLevel.Tests.Length - 1;
	}

	public void OffsetTestIndex(int delta)
	{
		SetTestIndex(TestIndex + delta);
	}
}