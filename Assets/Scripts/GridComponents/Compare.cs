using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compare : Operator {

	// Use this for initialization
	public static int LESSVALUE = 1;
	public static int EQUALVALUE = 2;	//zero as equal value is indistinguishable from lacking inputs
	public static int GREATERVALUE = 3;
	protected override void Start()
	{
		base.Start();
		OpName = "Compare";
	}

	public override void Step() {
		GetFromReceiver();
		if(num1 < num2)
			result = LESSVALUE;
		if(num1 == num2)
			result = EQUALVALUE;
		if(num1 > num2)
			result = GREATERVALUE;
		SendToTransmitter();
	}
}
