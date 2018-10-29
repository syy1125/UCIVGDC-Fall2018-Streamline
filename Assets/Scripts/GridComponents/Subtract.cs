using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subtract : Operator {
	protected override void Start()
	{
		base.Start();
		OpName = "Subtract";
	}

	protected override void Step() {
		GetFromReceiver();
		result = num1 - num2;
		SendToTransmitter();
	}
}
