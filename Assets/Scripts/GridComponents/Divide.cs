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
        if (num1 == 0 && num2 == 0)
        {
            result = 0;
        }
        else if (num2 == 0)
        {
            GameController.CrashSimulation();
        }
        else
        {
            result = num1 / num2;
        }
		SendToTransmitter();
	}
}
