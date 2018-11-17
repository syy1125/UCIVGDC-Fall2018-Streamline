using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Importer : Operator
{
	public ColArray outputColumn;
	private int[] _sequence = { 0 };

	public int[] Sequence
	{
		get { return _sequence; }
		set
		{
			_sequence = value;
			UpdateColumn();
		}
	}

	private int _index;

	protected override void Start()
	{
		base.Start();
		OpName = "Importer";
		_index = 0;

		UpdateColumn();
	}

	private void UpdateColumn()
	{
		outputColumn.ClearNums();
		for (int i = 0; i < Sequence.Length; i++)
		{
			outputColumn.AddValue(Sequence[i]);
		}
	}

	public override void Step()
	{
		result = Sequence[Mathf.Min(_index++, Sequence.Length - 1)];
		//outputColumn.AddValue(result);
		SendToTransmitter();
	}

	public void ResetState()
	{
		_index = 0;
	}

	public override string GetHint()
	{
		return "[NEXT] -> [ANY]";
	}

	public override bool[] GetIOMask()
	{
		return new bool[3] {false, false, false};
	}

	public override string SaveString()
	{
		return "";
	}
}