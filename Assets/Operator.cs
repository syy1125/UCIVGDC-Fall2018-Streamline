using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Operator : MonoBehaviour {
	protected float num1;
	protected float num2;
	protected float result;

	protected Receiver receiver;
	protected Transmitter transmitter;
	
	protected abstract void Step();

	private void Start() {
		receiver = GetComponent<Receiver>();
		transmitter = GetComponent<Transmitter>();
	}

	protected void GetFromReceiver() {
		num1 = receiver.GetNum1();
		num2 = receiver.GetNum2();
	}

	protected void SendToTransmitter() {
		transmitter.Send(result);
	}
}
