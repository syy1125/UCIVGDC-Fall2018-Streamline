using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : Operator {
	public int number;

	protected override void Step() {
		result = number;
		SendToTransmitter();
	}
}