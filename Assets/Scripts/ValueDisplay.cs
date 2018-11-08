using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ValueDisplay : MonoBehaviour {

    // Use this for initialization
    private Text text;
    private enum CopyType
    {
        TRANSMITTER, WIRE,
    }
    private CopyType type;
    private Object copyFrom;
	public void Start () {
        text = GetComponent<Text>();
        if (transform.parent.GetComponent<Transmitter>() != null)
        {
            type = CopyType.TRANSMITTER;
            copyFrom = transform.parent.GetComponent<Transmitter>();
        } else if(transform.parent.GetComponent<Wire>() != null)
        {
            type = CopyType.WIRE;
            copyFrom = transform.parent.GetComponent<Wire>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (type)
        {
            case CopyType.TRANSMITTER:
                text.text = "" + ((Transmitter)copyFrom).Signal;
                break;
            case CopyType.WIRE:
                text.text = "" + ((Wire)copyFrom).SignalStrength;
                break;
        }
	}
}
