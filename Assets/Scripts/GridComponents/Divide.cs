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
            Crash();
        }
        else
        {
            result = num1 / num2;
        }
		SendToTransmitter();
	}
    public override string GetHint()
    {
        return "<Color=#00d2d6>In.1</Color> / <Color=#c9d300>In.2</Color> -> Out.1";
    }
    public override string SaveString()
	{
		Receiver r = GetComponent<Receiver>();
		string result = "" + (int)Selection.DIV + '\t';
		result += "" + r.Location.x + '\t';
		result += "" + r.Location.y + '\t';
		result += "" + SaveData.GetDirection(r.InputDirection1) + '\t';
		result += "" + SaveData.GetDirection(r.InputDirection2) + '\t';
		result += "" + SaveData.GetDirection(GetComponent<Transmitter>().OutputDirection);
		return result;
	}
}
