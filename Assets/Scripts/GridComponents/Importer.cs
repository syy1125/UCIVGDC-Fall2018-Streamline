using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Importer : Operator
{
	public static int[][] Sequences = new int[2][];
	
	public ColArray outputColumn;
	public int SequenceIndex;

	private int[] _sequence;
	private int _index;

	protected override void Start()
	{
		base.Start();
		OpName = "Importer";
		_sequence = Sequences[SequenceIndex];
		_index = 0;
	}

	public override void Step()
	{
		result = _sequence[Mathf.Min(_index++, _sequence.Length - 1)];
		outputColumn.AddValue(result);
		SendToTransmitter();
	}

	public void ResetState()
	{
		_index = 0;
		outputColumn.ClearNums();
	}
}