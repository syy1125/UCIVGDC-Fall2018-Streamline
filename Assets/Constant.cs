using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : Operator {

	protected override void Step() {
		GetFromReceiver();
		SendToTransmitter();
	}
}
