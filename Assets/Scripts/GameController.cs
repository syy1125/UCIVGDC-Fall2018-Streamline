using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SimState
{
    EDITING,RUNNING,PAUSED,CRASHED
}

public class GameController : MonoBehaviour {

    // Use this for initialization
    public static List<Exporter> outputColumns;
    private Grid gameGrid;
    public static bool gameMenuOpen;
    public static bool clearConfirmationOpen;
    public static bool mouseDragging;
    public static bool levelWon;
    public static SimState simState;
    public KeyCode escapeKey;
    public UIGroup gameMenuUIGroup;
    public UIGroup crashUIGroup;
    public UIGroup levelWonGroup;
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
	void Awake(){
        outputColumns = new List<Exporter>();
        gameMenuOpen = false;
        clearConfirmationOpen = false;
        simState = SimState.EDITING;
        levelWon = false;
    }
    void Start () {
        gameGrid = Grid.Instance;
        
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
                }
            }
        }
        for (int h=0; h < gameGrid.Height; ++h) {
            for (int w = 0; w < gameGrid.Width; ++w) {
                GameObject g = gameGrid.GetGridComponent(h, w);
                if (gameGrid.IsOperator(g)) {
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
                if(!levelWon && IsLevelWon())
                {
                    SetSimState((int)SimState.PAUSED);
                    levelWon = true;
                    levelWonGroup.EnableUI();
                }
                if(Input.GetKeyDown(escapeKey))
                {  
                    SetSimState((int)SimState.PAUSED);
                }
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
                if(!levelWon && IsLevelWon())
                {
                    SetSimState((int)SimState.PAUSED);
                    levelWon = true;
                    levelWonGroup.EnableUI();
                }
                if(Input.GetKeyDown(escapeKey))
                {  
                    SetSimState((int)SimState.EDITING);
                }
                break;
            case SimState.CRASHED:
                if(Input.GetKeyDown(escapeKey))
                {  
                    SetSimState((int)SimState.EDITING);
                }
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
                    levelWon = false;
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
        if(IsLevelWon())
        {
            levelWon = true;
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
    public bool IsLevelWon()
    {
        foreach(Exporter ex in outputColumns)
        {
            if(!ex.allCorrect())
                return false;
        }
        return true;
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
        Transmitter tr = null;
        for(int i = 0; i < grid.Height; i++)
        {
            for(int j = 0; j < grid.Width; j++)
            {
                GameObject g = grid.GetGridComponent(i,j);
                if(g == null)
                    continue;
                tr = g.GetComponent<Transmitter>();
                if(tr != null)
                {
                    tr.Reset();
                }
            }
        }

        if(Grid.LevelIOMask[0])
            grid.GetGridComponent(0,grid.Height - 1).GetComponent<Importer>().ResetState();
        if(Grid.LevelIOMask[1])
            grid.GetGridComponent(0,0).GetComponent<Importer>().ResetState();
        if(Grid.LevelIOMask[2])
            grid.GetGridComponent(grid.Width - 1, 0).GetComponent<Exporter>().ResetState();
        if(Grid.LevelIOMask[3])
            grid.GetGridComponent(grid.Width - 1, grid.Height - 1).GetComponent<Exporter>().ResetState();
        
        ValueDisplayManager.DestroyAllValueDisplays();
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
            levelDescription.text = HighlightKeyTerms(gameLevel.Description);
        else
            levelDescription.text = HighlightKeyTerms(gameLevel.Objective);
    }
    public string HighlightKeyTerms(string[] s)
    {
        string[] replaceList = new string[4]{"In.1", "In.2", "Out.1", "Out.2"};
        string highlightColor = "#ff0000";
        string result = string.Join(" " , s);
        for(int i = 0; i < replaceList.Length; i++)
        {
            result = result.Replace(replaceList[i], "<color="+highlightColor+">"+replaceList[i]+"</color>");
            result = result.Replace(replaceList[i].ToLower(), "<color="+highlightColor+">"+replaceList[i].ToLower()+"</color>");
        }
        return result;
    }   
    public void GoToMainMenu(bool autoSave)
    {
        if (autoSave)
            SaveGame();
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToLevelSelect(bool autoSave)
    {
        if(autoSave)
            SaveGame();
        SceneManager.LoadScene("LevelSelect");
    }
}
