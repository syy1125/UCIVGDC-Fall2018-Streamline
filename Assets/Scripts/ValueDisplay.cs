using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ValueDisplay : MonoBehaviour {

    // Use this for initialization
    private Text text;
    private enum CopyType
    {
        TRANSMITTER, WIRE, RECEIVER
    }
    private CopyType type;
    private Object copyFrom;
	public void Start () {
        text = GetComponent<Text>();
        if (transform.parent.GetComponent<Transmitter>() != null)
        {
            type = CopyType.TRANSMITTER;
            copyFrom = transform.parent.GetComponent<Transmitter>();
        } 
        else if (transform.parent.GetComponent<Receiver>() != null)
        {
            type = CopyType.RECEIVER;
            copyFrom = transform.parent.GetComponent<Receiver>();
        }
        else if(transform.parent.GetComponent<Wire>() != null)
        {
            type = CopyType.WIRE;
            copyFrom = transform.parent.GetComponent<Wire>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (type)
        {
            case CopyType.TRANSMITTER: // Operators
                text.text = "" + ((Transmitter)copyFrom).Signal;
                break;
            case CopyType.RECEIVER: // Exporters
                text.text = "" + ((Receiver) copyFrom).Num1;
                break;
            case CopyType.WIRE: // Wires
                text.text = "" + ((Wire)copyFrom).SignalStrength;
                break;
        }
	}
}
