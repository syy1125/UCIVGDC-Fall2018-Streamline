using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Importer : Operator
{
	public ColArray outputColumn;
	public int[] Sequence;

	private int _index;

	protected override void Start()
	{
		base.Start();
		OpName = "Importer";
		_index = 0;
	}

	public override void Step()
	{
		result = Sequence[Mathf.Min(_index++, Sequence.Length - 1)];
		SendToTransmitter();
	}

	public void Reset()
	{
		_index = 0;
	}
}