using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : Operator {
	public int number;

	protected override void Start()
	{
		base.Start();
		OpName = "Constant";
	}

	public override void Step() {
		result = number;
		SendToTransmitter();
	}
	public override string GetHint()
    {
		return "Value -> Out.1";
	}
	public override bool[] GetIOMask()
	{
		return new bool[3]{false,false,true};
	}
	public override void LoadConfig(int[] constructor)
	{
		GetComponent<Transmitter>().OutputDirection = SaveData.GetDirection(constructor[3]);
        GetComponent<Constant>().number = constructor[4];
	}
	public override string SaveString()
	{
		Transmitter t = GetComponent<Transmitter>();
		string result = "" + (int)ComponentType.CONSTANT + '\t';
		result += "" + t.Location.x + '\t';
		result += "" + t.Location.y + '\t';
		result += "" + SaveData.GetDirection(t.OutputDirection) + '\t';
		result += number.ToString();
		return result;
	}
}