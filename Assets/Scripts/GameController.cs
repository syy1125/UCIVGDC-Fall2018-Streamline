using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SimState
{
    EDITING,RUNNING,PAUSED,
}

public class GameController : MonoBehaviour {

    // Use this for initialization
    public static bool gameMenuOpen;
    public static bool clearConfirmationOpen;
    public static SimState simState;
    public KeyCode escapeKey;
    public UIGroup gameMenuUIGroup;
	void Start () {
        gameMenuOpen = false;
        clearConfirmationOpen = false;
        simState = SimState.EDITING;
	}
	
	// Update is called once per frame
	void Update () {
        switch (simState)
        {
            case SimState.EDITING:
                if(!gameMenuOpen && Input.GetKeyDown(escapeKey))
                {
                    
                    gameMenuUIGroup.EnableUI();
                }
                break;
            case SimState.RUNNING:

                break;
            case SimState.PAUSED:

                break;
        }
        
	}
}
