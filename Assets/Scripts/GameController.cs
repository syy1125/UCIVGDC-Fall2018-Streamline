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
    public float stepDelay;
    private float stepTimer = 0;
    public ValueDisplayManager displayManager;
    [HideInInspector]
    public bool isSetUp = false;
    public int levelNum;
	void Start () {
        gameGrid = Grid.Instance;
        gameMenuOpen = false;
        clearConfirmationOpen = false;
        simState = SimState.EDITING;
        SaveData.LoadData(levelNum);
    }


    public void Step() {

        for (int h=0; h < gameGrid.Height; ++h) {
            for (int w = 0; w < gameGrid.Width; ++w) {
                GameObject g = gameGrid.GetGridComponent(h, w);
                if (gameGrid.IsOperator(g)) {
                    g.GetComponent<Receiver>().Step();
                    g.GetComponent<Operator>().Step();
                }
            }
        }
        Wire.ResetSignals();
        for (int h=0; h < gameGrid.Height; ++h) {
            for (int w = 0; w < gameGrid.Width; ++w) {
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
                /*
                if (Input.GetKeyDown(KeyCode.O))
                {
                    SaveData.LoadData(0);
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    SaveData.WriteData(0);
                }
                */
                break;
            case SimState.RUNNING:
                if(stepTimer > stepDelay)
                {
                    stepTimer = 0;
                    Step();
                }
                else
                {
                    stepTimer += Time.deltaTime;
                }

                break;
            case SimState.PAUSED:

                break;
        }
        
	}
    public void SetSimState(int s)
    {
        SimState newSimState = (SimState)s;
        switch (simState)
        {
            case SimState.EDITING:
                if(newSimState == SimState.RUNNING || newSimState == SimState.PAUSED)
                {
                    SetUpSimulation();
                    simState = newSimState;
                } 
                break;
            case SimState.RUNNING:
                if(newSimState == SimState.EDITING)
                {
                    TearDownSimulation();
                    simState = SimState.EDITING;
                } else if(newSimState == SimState.PAUSED)
                {
                    simState = SimState.PAUSED;
                }
                break;
            case SimState.PAUSED:
                if(newSimState == SimState.RUNNING)
                {
                    simState = SimState.RUNNING;
                } else if(newSimState == SimState.EDITING)
                {
                    TearDownSimulation();
                    simState = SimState.EDITING;
                }
                break;
        }


        
    }
    public void SetUpSimulation()
    {
        if (isSetUp)
            return;
        isSetUp = true;
        Wire.GlobalSetUp();
        displayManager.AddAllValueDisplays();
        stepTimer = stepDelay;
    }
    public void TearDownSimulation()
    {
        if (!isSetUp)
            return;
        isSetUp = false;
        Wire.GlobalTearDown();
        displayManager.DestroyAllValueDisplays();
    }
    public void SaveGame()
    {
        Debug.Log("Saving game...");
        SaveData.WriteData(levelNum);
    }
    public IEnumerator LoadSolution()
    {
        Debug.Log("Loading Data...");
        yield return new WaitForEndOfFrame();
        SaveData.LoadData(levelNum);
    }
    
}
