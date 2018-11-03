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


    public void Step() {
        for (int h=0; h < gameGrid.Height; ++h) {
            for (int w = 0; h < gameGrid.Width; ++w) {
                GameObject g = gameGrid.GetGridComponent(h, w);
                if (gameGrid.IsOperator(g)) {
                    g.GetComponent<Receiver>().Step();
                    g.GetComponent<Operator>().Step();
                }
            }
        }
                
        Wire.ResetSignals();
                
        for (int h=0; h < gameGrid.Height; ++h) {
            for (int w = 0; h < gameGrid.Width; ++w) {
                GameObject g = gameGrid.GetGridComponent(h, w);
                if (gameGrid.IsOperator(g)) {
                    g.GetComponent<Transmitter>().Step();
                }
            }
        }
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
                Step();

                break;
            case SimState.PAUSED:

                break;
        }
        
	}
    
    
}
