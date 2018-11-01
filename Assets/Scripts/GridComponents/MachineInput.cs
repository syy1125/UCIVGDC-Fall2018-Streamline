using UnityEngine;

public class MachineInput : Operator
{
	public int[] Inputs;
	private int _index;

	protected override void Step()
	{
		result = Inputs[Mathf.Min(_index++, Inputs.Length - 1)];
		SendToTransmitter();
	}
}