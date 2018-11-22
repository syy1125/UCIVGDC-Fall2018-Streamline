using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorLerp : MonoBehaviour {

    // Use this for initialization
    public enum InitialColor
    {
        START, END
    }
    public Color StartColor;
    public Color EndColor;
    public InitialColor InitializeColor;
    public bool Activated = false;
    private Graphic TargetGraphic;
    public float ChangeDuration;
    private float Timer;
    public bool EnableGraphicOnStart;
    private void Awake()
    {
        TargetGraphic = GetComponent<Graphic>();
        if (EnableGraphicOnStart)
            TargetGraphic.enabled = true;
        if(ChangeDuration == 0)
        {
            ChangeDuration = 0.001f;
        }
        if(InitializeColor == InitialColor.START)
        {
            TargetGraphic.color = StartColor;
            Timer = 0;
        } else
        {
            TargetGraphic.color = EndColor;
            Timer = ChangeDuration;
        }
    }
    void Start () {
		
    }

    // Update is called once per frame
    void Update () {
        switch (Activated)
        {
            case true:
                Timer += Time.deltaTime;
                Timer = (Timer > ChangeDuration) ? ChangeDuration : Timer;
                break;
            case false:
                Timer -= Time.deltaTime;
                Timer = (Timer < 0) ? 0 : Timer;
                break;
        }
        TargetGraphic.color = Color.Lerp(StartColor, EndColor, Timer / ChangeDuration);
    }
    public void SetActivated(bool x)
    {
        Activated = x;
    }
    public void SwapColorImmediately(bool x)
    {
        Activated = x;
        if (Activated)
        {
            TargetGraphic.color = EndColor;
            Timer = ChangeDuration;
        }
        else
        {
            TargetGraphic.color = StartColor;
            Timer = 0;
        }
    }
}
