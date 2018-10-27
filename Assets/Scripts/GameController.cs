using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SimState
{
    EDITING,RUNNING,PAUSED,
}

public class GameController : MonoBehaviour {

    // Use this for initialization
    private Grid gameGrid;
    public static bool gameMenuOpen;
    public static bool clearConfirmationOpen;
    public static bool mouseDragging;
    public static SimState simState;
    public KeyCode escapeKey;
    public UIGroup gameMenuUIGroup;
	void Start () {
        gameGrid = Grid.Instance;
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
                if (mouseDragging && !Input.GetMouseButton(0)) //Left mouse button
                {
                    mouseDragging = false;
                }
                
                break;
            case SimState.RUNNING:

                break;
            case SimState.PAUSED:

                break;
        }
        
	}
    
    
}
