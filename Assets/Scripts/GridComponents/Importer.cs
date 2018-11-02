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

	protected override void Step()
	{
		if (_index >= Sequence.Length)
		{
			result = 0;
		}
		else
		{
			result = Sequence[_index];
			_index++;
		}

		SendToTransmitter();
	}

	public void Reset()
	{
		_index = 0;
	}
}