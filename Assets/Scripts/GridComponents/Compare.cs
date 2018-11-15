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
	public override string GetHint()
    {
		string result = "<Color=#00d2d6>In.1</Color> < <Color=#c9d300>In.2</Color> -> "+ Compare.LESSVALUE;
		result += "\n<Color=#00d2d6>In.1</Color> == <Color=#c9d300>In.2</Color> -> "+ Compare.EQUALVALUE;
		result += "\n<Color=#00d2d6>In.1</Color> > <Color=#c9d300>In.2</Color> -> "+ Compare.GREATERVALUE;
		return result;    
	}
	public override string SaveString()
	{
		Receiver r = GetComponent<Receiver>();
		string result = "" + (int)ComponentType.COMPARE + '\t';
		result += "" + r.Location.x + '\t';
		result += "" + r.Location.y + '\t';
		result += "" + SaveData.GetDirection(r.InputDirection1) + '\t';
		result += "" + SaveData.GetDirection(r.InputDirection2) + '\t';
		result += "" + SaveData.GetDirection(GetComponent<Transmitter>().OutputDirection) + '\t';
		return result;
	}
    
}
