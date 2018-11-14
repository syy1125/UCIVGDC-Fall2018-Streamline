using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessThan : Operator {

	public static int TRUEVALUE = 1;
	public static int FALSEVALUE = -1;
	protected override void Start()
	{
		base.Start();
		OpName = "LessThan";
	}

	public override void Step() {
		GetFromReceiver();
		result = (num1 < num2) ? TRUEVALUE : FALSEVALUE;
		SendToTransmitter();
	}
}
