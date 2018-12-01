using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimStateDisplay : MonoBehaviour {

    // Use this for initialization
    public Color EditingColor;
    public Color RunningColor;
    public Color PausedColor;
    public Color CrashedColor;
    public Color WinColor;
    private Text MyText;
	void Awake () {
        MyText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        MyText.text = "Status: ";
        if(GameController.levelWon)
        {
            MyText.text += "<color=#" + ColorToHex(WinColor)+">";
            MyText.text += "LEVEL WON";
        } else
        {
            switch(GameController.simState)
            {
                case SimState.EDITING:
                    MyText.text += "<color=#" + ColorToHex(EditingColor) + ">";
                    break;
                case SimState.RUNNING:
                    MyText.text += "<color=#" + ColorToHex(RunningColor) + ">";
                    break;
                case SimState.PAUSED:
                    MyText.text += "<color=#" + ColorToHex(PausedColor) + ">";
                    break;
                case SimState.CRASHED:
                    MyText.text += "<color=#" + ColorToHex(CrashedColor) + ">";
                    break;

            }
            MyText.text += GameController.simState.ToString();
        }
        MyText.text += "</color>";
    }
    private string ColorToHex(Color color)
    {
        string result = "";
        result += ((int)(color.r * 255)).ToString("X2");
        result += ((int)(color.g * 255)).ToString("X2");
        result += ((int)(color.b * 255)).ToString("X2");
        return result;

    }
}
