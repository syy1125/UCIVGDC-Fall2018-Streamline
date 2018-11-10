using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Operator : MonoBehaviour {
    public string errorHighlight = "ErrorHighlight";
	protected int num1;
	protected int num2;
	protected int result;

	protected Receiver receiver;
	protected Transmitter transmitter;
    public static int MAX = 999;
    public static int MIN = -999;
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
        result = Mathf.Clamp(result, MIN, MAX);
		transmitter.Send(result);
	}
    protected void Crash()
    {
        GameController.CrashSimulation();
        Instantiate(Resources.Load(errorHighlight),transform);

    }
}
