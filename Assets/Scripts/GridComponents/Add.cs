using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Add : Operator {
	protected override void Start()
	{
		base.Start();
		OpName = "Add";
	}

	public override void Step() {
		GetFromReceiver();
		result = num1 + num2;
		SendToTransmitter();
	}
}
