using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Divide : Operator {
	protected override void Start()
	{
		base.Start();
		OpName = "Divide";
	}

	public override void Step() {
		GetFromReceiver();
		result = (num2 != 0) ? num1 / num2 : 0;
		SendToTransmitter();
	}
}
