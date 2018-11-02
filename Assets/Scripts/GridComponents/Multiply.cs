using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiply : Operator {
	protected override void Start()
	{
		base.Start();
		OpName = "Multiply";
	}

	public override void Step() {
		GetFromReceiver();
		result = num1 * num2;
		SendToTransmitter();
	}
}
