using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Operator : MonoBehaviour {
	protected int num1;
	protected int num2;
	protected int result;

	protected Receiver receiver;
	protected Transmitter transmitter;
	
	public string OpName { get; protected set; }

	public abstract void Step();

	protected virtual void Start() {
		receiver = GetComponent<Receiver>();
		transmitter = GetComponent<Transmitter>();
		OpName = "Operator";
	}

	protected void GetFromReceiver() {
		num1 = receiver.GetNum1();
		num2 = receiver.GetNum2();
	}

	protected void SendToTransmitter() {
		transmitter.Send(result);
	}
}
