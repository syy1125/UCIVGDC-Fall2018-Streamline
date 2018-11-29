using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessThan : Operator {

	public static int TRUEVALUE = 1;
	public static int FALSEVALUE = 0;
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
	public override string GetHint()
    {
		return "<Color=#00d2d6>In.1</Color> < <Color=#c9d300>In.2</Color> -> "+TRUEVALUE + 
				"\n<Color=#00d2d6>In.1</Color> >= <Color=#c9d300>In.2</Color> -> "+FALSEVALUE;
	}
	public override string SaveString()
	{
		Receiver r = GetComponent<Receiver>();
		string result = "" + (int)ComponentType.LESSTHAN + '\t';
		result += "" + r.Location.x + '\t';
		result += "" + r.Location.y + '\t';
		result += "" + SaveData.GetDirection(r.InputDirection1) + '\t';
		result += "" + SaveData.GetDirection(r.InputDirection2) + '\t';
		result += "" + SaveData.GetDirection(GetComponent<Transmitter>().OutputDirection);
		return result;
	}
}
