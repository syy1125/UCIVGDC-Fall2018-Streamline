using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Divide : Operator {
	protected override void Start()
	{
		base.Start();
		OpName = "Divide";
	}

	protected override void Step() {
		GetFromReceiver();
		result = num1 / num2;
		SendToTransmitter();
	}
}
