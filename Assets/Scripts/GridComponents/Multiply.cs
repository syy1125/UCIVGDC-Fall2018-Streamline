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
	public override string GetHint()
    {
		return "<Color=#00d2d6>In.1</Color> x <Color=#c9d300>In.2</Color> -> Out.1";

	}
	public override string SaveString()
	{
		Receiver r = GetComponent<Receiver>();
		string result = "" + (int)ComponentType.MULT + '\t';
		result += "" + r.Location.x + '\t';
		result += "" + r.Location.y + '\t';
		result += "" + SaveData.GetDirection(r.InputDirection1) + '\t';
		result += "" + SaveData.GetDirection(r.InputDirection2) + '\t';
		result += "" + SaveData.GetDirection(GetComponent<Transmitter>().OutputDirection);
		return result;
	}
}
