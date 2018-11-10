using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SimState
{
    EDITING,RUNNING,PAUSED,CRASHED
}

public class GameController : MonoBehaviour {

    // Use this for initialization
    private Grid gameGrid;
    public static bool gameMenuOpen;
    public static bool clearConfirmationOpen;
    public static bool mouseDragging;
    public string errorPrefab = "ErrorHighlight";
    public static SimState simState;
    public KeyCode escapeKey;
    public UIGroup gameMenuUIGroup;
    public UIGroup crashUIGroup;
    public float stepDelay;
    private float stepTimer = 0;
    public ValueDisplayManager displayManager;
    [HideInInspector]
    public bool isSetUp = false;
    public static GameLevel gameLevel = new GameLevel();
    public static int solutionNum = 0;
    public Transform wireEffectPrefab;
    public Text levelNameText;
    public Text levelDescription;
    public string levelOverride = "";
	void Start () {
        gameGrid = Grid.Instance;
        gameMenuOpen = false;
        clearConfirmationOpen = false;
        simState = SimState.EDITING;
        LoadSolution();


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
        if (mouseDragging && !Input.GetMouseButton(0)) //Left mouse button
        {
            mouseDragging = false;
        }
        switch (simState)
        {
            case SimState.EDITING:
                if(!gameMenuOpen && Input.GetKeyDown(escapeKey))
                {  
                    gameMenuUIGroup.EnableUI();
                }
                
              
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
            case SimState.CRASHED:

                break;
        }
        
	}
    public void OnStepButtonPress()
    {
        switch (simState)
        {
            case SimState.PAUSED:
                Step();
                break;
            case SimState.RUNNING:
                SetSimState((int)SimState.PAUSED);
                break;
            case SimState.EDITING:
                SetSimState((int)SimState.PAUSED);
                Step();
                break;
            case SimState.CRASHED:
                simState = SimState.PAUSED;
                CrashSimulation();
                break;
        }
    }
    public void PlayPauseToggle()
    {
        switch (simState)
        {
            case SimState.EDITING:
                SetSimState((int)SimState.RUNNING);
                break;
            case SimState.RUNNING:
                SetSimState((int)SimState.PAUSED);
                break;
            case SimState.PAUSED:
                SetSimState((int)SimState.RUNNING);
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
                } else if(newSimState == SimState.CRASHED)
                {
                    crashUIGroup.EnableUI();
                    simState = SimState.CRASHED;
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
                } else if (newSimState == SimState.CRASHED)
                {
                    crashUIGroup.EnableUI();
                    simState = SimState.CRASHED;
                }
                break;
            case SimState.CRASHED:
                if(newSimState == SimState.EDITING)
                {
                    TearDownSimulation();
                    simState = SimState.EDITING;
                } else if(newSimState == SimState.RUNNING)
                {
                    TearDownSimulation();
                    SetUpSimulation();
                    simState = SimState.RUNNING;
                }
                break;
        }


        
    }
    public static void CrashSimulation()
    {
        if (simState != SimState.CRASHED)
        {
            GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            gc.SetSimState((int)SimState.CRASHED);
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
        string saveFile = "";
        if (!levelOverride.Equals(""))
            saveFile = levelOverride;
        else
            saveFile = gameLevel.Name;
        SaveData.WriteData(saveFile, solutionNum);
    }
    public void LoadSolution()
    {
        string saveFile = "";
        if (!levelOverride.Equals(""))
            saveFile = levelOverride;
        else
            saveFile = gameLevel.Name;

        SaveData.LoadData(saveFile, solutionNum);
        levelNameText.text = gameLevel.Name;
        if (gameLevel.Objective == null)
            levelDescription.text = string.Join(" ", gameLevel.Description);
        else
            levelDescription.text = string.Join(" ", gameLevel.Objective);
    }
    
}
