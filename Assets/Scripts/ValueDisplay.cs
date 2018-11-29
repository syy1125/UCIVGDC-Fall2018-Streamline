using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ValueDisplay : MonoBehaviour {

    // Use this for initialization
    private Text text;
    private enum CopyType
    {
        OMNIRECEIVER, TRANSMITTER, WIRE, RECEIVER
    }
    private CopyType type;
    private Object copyFrom;
	public void Start () {
        text = GetComponent<Text>();
        if(transform.parent.GetComponent<OmniReceiver>() != null)
        {
            type = CopyType.OMNIRECEIVER;
            copyFrom = transform.parent.GetComponent<OmniReceiver>();
        }
        else if (transform.parent.GetComponent<Transmitter>() != null)
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
        Update();
    }
	
	// Update is called once per frame
	void Update () {
        int value = 0;
        switch (type)
        {
            case CopyType.OMNIRECEIVER:
                value = ((OmniReceiver)copyFrom).Num1;
                break;
            case CopyType.TRANSMITTER: // Operators
                value = ((Transmitter)copyFrom).Signal;
                break;
            case CopyType.RECEIVER: // Exporters
                value =((Receiver) copyFrom).Num1;
                break;
            case CopyType.WIRE: // Wires
                value = ((Wire)copyFrom).SignalStrength;
                break;
        }
        if(value == 0)
            text.text = "";
        else
            text.text = value.ToString();
	}
}
