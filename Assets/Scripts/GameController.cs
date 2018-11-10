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
    public static string levelName = "TestLevel";
    public static int solutionNum = 0;
    public Transform wireEffectPrefab;
    
	void Start () {
        gameGrid = Grid.Instance;
        gameMenuOpen = false;
        clearConfirmationOpen = false;
        simState = SimState.EDITING;
        SaveData.LoadData(levelName, solutionNum);
    }


    public void Step() {
        HashSet<Vector2Int> redSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> greenSet = new HashSet<Vector2Int>();
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
                    SpawnEffect(ref redSet,ref greenSet, g.GetComponent<Transmitter>());
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
    private void SpawnEffect(ref HashSet<Vector2Int> redSet, ref HashSet<Vector2Int> greenSet, Transmitter tr)
    {
        if (Grid.Instance.GetGridComponent(tr.Location + tr.OutputDirection) == null)
            return;
        Wire wire = Grid.Instance.GetGridComponent(tr.Location + tr.OutputDirection).GetComponent<Wire>();
        Receiver op = Grid.Instance.GetGridComponent(tr.Location + tr.OutputDirection).GetComponent<Receiver>();
        Transform effect = null;
        if (wire != null)
        {
            if (wire.HasRed)
            {
                effect = Instantiate(wireEffectPrefab, tr.transform.position
                    + (wire.RedParts.Center.transform.position-wire.transform.position), Quaternion.identity);
                effect.GetComponent<WireEffect>().SetHashSets(ref redSet, ref greenSet);
                effect.GetComponent<WireEffect>().destTile = wire.Location;
                effect.GetComponent<WireEffect>().destination = wire.RedParts.Center.transform.position;
                effect.GetComponent<WireEffect>().SetColor(effect.GetComponent<WireEffect>().redColor);
                effect.GetComponent<WireEffect>().followColor = WireEffect.FollowColor.RED;

            }
            if (wire.HasGreen)
            {
                effect = Instantiate(wireEffectPrefab, tr.transform.position 
                    + (wire.GreenParts.Center.transform.position - wire.transform.position), Quaternion.identity);
                effect.GetComponent<WireEffect>().SetHashSets(ref redSet, ref greenSet);
                effect.GetComponent<WireEffect>().destTile = wire.Location;
                effect.GetComponent<WireEffect>().destination = wire.GreenParts.Center.transform.position;
                effect.GetComponent<WireEffect>().SetColor(effect.GetComponent<WireEffect>().greenColor);
                effect.GetComponent<WireEffect>().followColor = WireEffect.FollowColor.GREEN;

            }
        }
        if(op != null)
        {
            if(op.Location+op.InputDirection1 == tr.Location || op.Location+op.InputDirection2 == tr.Location)
            {
                effect = Instantiate(wireEffectPrefab, tr.transform.position, Quaternion.identity);
                effect.GetComponent<WireEffect>().SetHashSets(ref redSet, ref greenSet);
                effect.GetComponent<WireEffect>().destTile = op.Location;
                effect.GetComponent<WireEffect>().destination = op.transform.position;
                effect.GetComponent<WireEffect>().SetColor(effect.GetComponent<WireEffect>().redColor);
                effect.GetComponent<WireEffect>().followColor = WireEffect.FollowColor.RED;
                effect.GetComponent<WireEffect>().noSpread = true;
            }
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

        Grid grid = Grid.Instance;
        grid.GetGridComponent(0,0).GetComponent<Importer>().ResetState();
        grid.GetGridComponent(0,grid.Height - 1).GetComponent<Importer>().ResetState();
        grid.GetGridComponent(grid.Width - 1, 0).GetComponent<Exporter>().ResetState();
        grid.GetGridComponent(grid.Width - 1, grid.Height - 1).GetComponent<Exporter>().ResetState();
        
        displayManager.DestroyAllValueDisplays();
    }
    public void SaveGame()
    {
        Debug.Log("Saving game...");
        SaveData.WriteData(levelName,solutionNum);
    }
    public void LoadSolution()
    {
        Debug.Log("Loading Data...");
        SaveData.LoadData(levelName,solutionNum);
    }
    
}
