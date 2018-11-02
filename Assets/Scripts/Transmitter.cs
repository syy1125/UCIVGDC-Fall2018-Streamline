using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transmitter : MonoBehaviour
{
	private int _signal;
	
	public void Send(int signal)
	{
		_signal = signal;
	}

	public void Step()
	{
		Grid grid = Grid.Instance;
		
		
	}
}