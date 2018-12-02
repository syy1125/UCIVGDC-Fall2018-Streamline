using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    public UIGroup SkipUIGroup;
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
    [HideInInspector]
    public static bool[] TestCaseCompletion;
    public ColumnManager ColManager;
    public static Vector2 MouseWorldPosition;
    private Camera mainCamera;
    private AudioSource Source;
    public AudioClip StepSound;
    public AudioClip WinSound;
    public AudioClip CrashSound;
    public KeyCode[] DeleteKeys;

	void Awake(){
        outputColumns = new List<Exporter>();
        gameMenuOpen = false;
        clearConfirmationOpen = false;
        simState = SimState.EDITING;
        levelWon = false;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Source = GetComponent<AudioSource>();
    }
    void Start () {
        gameGrid = Grid.Instance;
        
        LoadSolution();
        TestCaseCompletion = new bool[gameLevel.Tests.Length];
    }


    public void Step() {
        Source.PlayOneShot(StepSound);
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
        if(TestCaseComplete())
        {
            TestCaseCompletion[ColumnManager.TestIndex] = true;
            if(!IsLevelWon())
            {
                ColManager.OffsetTestIndex(1);
                SoftReset();
            }
        }
        if(!levelWon && IsLevelWon())
        {
            SetSimState((int)SimState.PAUSED);
            levelWon = true;
            levelWonGroup.EnableUI();
            Source.PlayOneShot(WinSound);
        }
    }
	// Update is called once per frame
	void Update () {
        MouseWorldPosition = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (mouseDragging && !Input.GetMouseButton(0)) //Left mouse button
        {
            mouseDragging = false;
            GridButton.DragType = DragType.NONE;
        }
        switch (simState)
        {
            case SimState.EDITING:
                bool deletedComponents = false;
                if(!gameMenuOpen)
                {
                    for(int i = 0; i < DeleteKeys.Length; i++){
                        if(Input.GetKeyDown(DeleteKeys[i])){
                            foreach(Vector2Int point in Grid.Instance.GroupSelection.PointSet)
                            {
                                Grid.Instance.DestroyGridComponent(point);
                                deletedComponents = true;
                            }
                            Grid.Instance.ClearGroupSelection();
                        }
                    }
                    
                }
                if(!deletedComponents && !gameMenuOpen && Input.GetKeyDown(escapeKey))
                {  
                    
                    if(Grid.Instance.Selected != Vector2Int.one*-1){
                        Grid.Instance.Selected = new Vector2Int(-1,-1);
                        Grid.Instance.ClearGroupSelection();
                    } else if(Grid.Instance.GroupSelection.PointSet.Count > 0)
                    {
                        Grid.Instance.ClearGroupSelection();
                    }
                    else
                        StartCoroutine(DelayedOpenUI(gameMenuUIGroup));
                }
                
              
                break;
            case SimState.RUNNING:
                
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
        

        
    }
    public static void CrashSimulation()
    {
        if (simState != SimState.CRASHED)
        {
            
            GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            gc.Source.PlayOneShot(gc.CrashSound);
            gc.SetSimState((int)SimState.CRASHED);
        }
    }
    public bool IsLevelWon()
    {
        for(int i = 0 ; i < TestCaseCompletion.Length; i++)
        {
            if(!TestCaseCompletion[i])
                return false;
        }
        return true;
    }
    public bool TestCaseComplete()
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
    public void SoftReset()
    {
        //Reset simulation, does not reset test completion
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

        if(ColumnManager.LevelIOMask[0])
            grid.GetGridComponent(0,grid.Height - 1).GetComponent<Importer>().ResetState();
        if(ColumnManager.LevelIOMask[1])
            grid.GetGridComponent(0,0).GetComponent<Importer>().ResetState();
        if(ColumnManager.LevelIOMask[2])
            grid.GetGridComponent(grid.Width - 1, 0).GetComponent<Exporter>().ResetState();
        if(ColumnManager.LevelIOMask[3])
            grid.GetGridComponent(grid.Width - 1, grid.Height - 1).GetComponent<Exporter>().ResetState();
        ValueDisplayManager.DestroyAllValueDisplays();        
        isSetUp = true;
        Grid.Instance.ClearGroupSelection();
        Wire.GlobalSetUp();
        displayManager.AddAllValueDisplays();
    }
    public void SetUpSimulation()
    {
        if (isSetUp)
            return;
        SaveGame();
        isSetUp = true;
        Grid.Instance.ClearGroupSelection();
        Wire.GlobalSetUp();
        displayManager.AddAllValueDisplays();
        stepTimer = stepDelay;
        for(int i = 0; i < TestCaseCompletion.Length; i++)
        {
            TestCaseCompletion[i] = false;
        }
        levelWon = false;
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

        if(ColumnManager.LevelIOMask[0])
            grid.GetGridComponent(0,grid.Height - 1).GetComponent<Importer>().ResetState();
        if(ColumnManager.LevelIOMask[1])
            grid.GetGridComponent(0,0).GetComponent<Importer>().ResetState();
        if(ColumnManager.LevelIOMask[2])
            grid.GetGridComponent(grid.Width - 1, 0).GetComponent<Exporter>().ResetState();
        if(ColumnManager.LevelIOMask[3])
            grid.GetGridComponent(grid.Width - 1, grid.Height - 1).GetComponent<Exporter>().ResetState();
        ColManager.SetTestIndex(0);
        ValueDisplayManager.DestroyAllValueDisplays();
        for (int i = 0; i < TestCaseCompletion.Length; i++)
        {
            TestCaseCompletion[i] = false;
        }
        levelWon = false;
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
    private IEnumerator DelayedOpenUI(UIGroup group)
    {
        yield return new WaitForEndOfFrame();
        group.EnableUI();
    }
    public void GoToMainMenu(bool autoSave)
    {
        if (autoSave)
            SaveGame();
        StartCoroutine(TransitionAndLoad("MainMenu"));
    }
    public void GoToLevelSelect(bool autoSave)
    {
        if(autoSave)
            SaveGame();
        StartCoroutine(TransitionAndLoad("LevelSelect"));
    }
    public void FadeMusic()
    {
        if (MusicController.Instance != null)
        {
            MusicController.Instance.VolumeFade(0,
                GameObject.FindGameObjectWithTag("Transition").GetComponent<ColorLerp>().ChangeDuration);
        }
    }
    public static IEnumerator TransitionAndLoad(string sceneName)
    {
        ColorLerp c = GameObject.FindGameObjectWithTag("Transition").GetComponent<ColorLerp>();
        c.SetActivated(true);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().enabled = false;
        yield return new WaitForSeconds(c.ChangeDuration);
        SceneManager.LoadScene(sceneName);
    }
    public void OpenGameMenu()
    {
        if(simState == SimState.EDITING && !gameMenuOpen)
        {  
            StartCoroutine(DelayedOpenUI(gameMenuUIGroup));
        }
    }
    public void ForceOpenGameMenu()
    {
        //Typical esc key press does not always open game menu
        //due to deselecting things and stuff that overrides the key.
        //That is normal behavior, so this is used to override all of that stuff.
        if (simState != SimState.EDITING && !levelWon)
        {
            SetSimState((int)SimState.EDITING);
        }
        StartCoroutine(DelayedOpenUI(gameMenuUIGroup));
    }
    
}
