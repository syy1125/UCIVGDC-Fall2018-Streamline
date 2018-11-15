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
	public virtual string GetHint()
	{
		return "";
	}
	public virtual bool[] GetIOMask()
	{
		return new bool[3]{true,true,true};
	}
	public virtual void LoadConfig(int[] constructor)
	{
		GetComponent<Receiver>().InputDirection1 = SaveData.GetDirection(constructor[3]);
        GetComponent<Receiver>().InputDirection2 = SaveData.GetDirection(constructor[4]);
        GetComponent<Transmitter>().OutputDirection = SaveData.GetDirection(constructor[5]);
	}
	public abstract string SaveString();	
}
