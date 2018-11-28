using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public static Grid Instance;
    
    // Use this for initialization
    [FormerlySerializedAs("rows")] public int Height;
    [FormerlySerializedAs("columns")] public int Width;
    public Transform gridButtonPrefab;
    [HideInInspector]
    public Transform[][] buttonList;
    public GameObject columnsParent;

    public GameObject wire;
    public GameObject addition;
    public GameObject subtraction;
    public GameObject multiplication;
    public GameObject division;
    public GameObject constant;
    public GameObject equality;
    public GameObject lessThan;
    public GameObject importer;
    public GameObject exporter;

    private GameObject[][] _gridComponents;

    public UnityEvent OnGridReady;

    public Color SelectedColor;
   
    private ColorBlock SelectedButtonColors
    {
        get
        {
            return new ColorBlock()
            {
                normalColor = SelectedColor,
                highlightedColor = SelectedColor,
                pressedColor = SelectedColor,
                disabledColor = SelectedColor,
                colorMultiplier = 1
            };
        }
    }
    
    private Vector2Int _selected = Vector2Int.one * -1;
    
    public Vector2Int Selected
    {
        get { return _selected; }
        set
        {
            if (InGrid(_selected))
            {
                buttonList[_selected.x][_selected.y].GetComponent<Button>().colors = gridButtonPrefab.GetComponent<Button>().colors;
            }

            _selected = value;
            if (InGrid(_selected))
            {
                buttonList[_selected.x][_selected.y].GetComponent<Button>().colors = SelectedButtonColors;
                EditorInstance.UpdateUI();
            }
        }
    }
    public class SelectionGroup{
        public HashSet<Vector2Int> PointSet = new HashSet<Vector2Int>();
        public Vector2Int GraspPoint = Vector2Int.zero;
    }
    public SelectionGroup GroupSelection = new SelectionGroup();
    public ComponentEditor EditorInstance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Another `Grid` is being instantiated. Destroying the new `Grid`.");
            Destroy(gameObject);
        }
    }

    private void Start ()
    {
        // Build grid
        _gridComponents = new GameObject[Width][];
        for (int x = 0; x < Width; x++)
        {
            _gridComponents[x] = new GameObject[Height];
        }
        BuildGridButtons();
        
        OnGridReady.Invoke();
	}

    public bool InGrid(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool InGrid(Vector2Int pos)
    {
        return InGrid(pos.x, pos.y);
    }

    public GameObject GetGridComponent(int x, int y)
    {
        return InGrid(x, y) ? _gridComponents[x][y] : null;
    }

    public GameObject GetGridComponent(Vector2Int pos)
    {
        return GetGridComponent(pos.x, pos.y);
    }
    public HashSet<GameObject> GetGridComponents(Vector2Int pointA, Vector2Int pointB)
    {
        //A and B are any 2 arbitrary grid positions
        //Does not include empty spaces or Importer/Exporters in result set
        HashSet<GameObject> resultSet = new HashSet<GameObject>();
        Vector2Int LowerLeft = new Vector2Int(Mathf.Min(pointA.x,pointB.x),Mathf.Min(pointA.y,pointB.y));
        Vector2Int UpperRight = new Vector2Int(Mathf.Max(pointA.x,pointB.x),Mathf.Max(pointA.y,pointB.y));
        for(int i = LowerLeft.x; i <= UpperRight.x; i++)
        {
            for(int j = LowerLeft.y; j <= UpperRight.y; j++)
            {
                GameObject g = GetGridComponent(i,j);
                if(g != null && !IsImporterOrExporter(g))   
                {
                    resultSet.Add(g);
                }
            }
        }
        return resultSet;
    }
    public GameObject SetGridComponent(int x, int y, GameObject prefab)
    {
        _gridComponents[x][y] = Instantiate(prefab, getGridButton(x, y));
        if (_gridComponents[x][y].GetComponent<Receiver>() != null)
        {
            _gridComponents[x][y].GetComponent<Receiver>().Location = new Vector2Int(x, y);
        }
        if (_gridComponents[x][y].GetComponent<Transmitter>() != null)
        {
            _gridComponents[x][y].GetComponent<Transmitter>().Location = new Vector2Int(x, y);
        }
        return _gridComponents[x][y];
    }

    public void UpdateAdjacentWires(int x, int y)
    {
        GameObject up = GetGridComponent(x, y + 1);
        GameObject down = GetGridComponent(x, y - 1);
        GameObject left = GetGridComponent(x - 1, y);
        GameObject right = GetGridComponent(x + 1, y);

        if (IsWire(up)) up.GetComponent<Wire>().UpdateAllTextures();
        if (IsWire(down)) down.GetComponent<Wire>().UpdateAllTextures();
        if (IsWire(left)) left.GetComponent<Wire>().UpdateAllTextures();
        if (IsWire(right)) right.GetComponent<Wire>().UpdateAllTextures();
    }

    public void UpdateAdjacentWires(Vector2Int location)
    {
        UpdateAdjacentWires(location.x, location.y);
    }

    public GameObject SetGridComponent(Vector2Int pos, GameObject prefab)
    {
        return SetGridComponent(pos.x, pos.y, prefab);
    }
    public void DestroyGridComponent(int x, int y)
    {
        GameObject g = GetGridComponent(x, y);
        if(g != null && IsImporterOrExporter(g))
        {
            //Do not delete importers/exporters
            return;
        } 

        Destroy(GetGridComponent(x,y));
        _gridComponents[x][y] = null;
        UpdateAdjacentWires(x,y);
    }
    public void DestroyGridComponent(Vector2Int pos)
    {
        DestroyGridComponent(pos.x, pos.y);
    }
    private void BuildGridButtons()
    {
        //Create grid of buttons with x,y coordinates
        buttonList = new Transform[Width][];
        float buttonWidth = 1f / Width;
        float buttonHeight = 1f / Height;
        for (int x = 0; x < Width; x++)
        {
            buttonList[x] = new Transform[Height];
            for(int y = 0; y < Height; y++)
            {
                Transform newButton = Instantiate(gridButtonPrefab);
                newButton.SetParent(transform);
                RectTransform buttonTransform = newButton.GetComponent<RectTransform>();
                
                buttonTransform.anchorMin = new Vector2(x * buttonWidth, y * buttonHeight);
                buttonTransform.anchorMax = new Vector2((x+1) * buttonWidth, (y+1) * buttonHeight);
                buttonTransform.offsetMin = Vector2.zero;
                buttonTransform.offsetMax = Vector2.zero;
                
                newButton.localScale = Vector3.one;
                newButton.GetComponent<GridButton>().setPosition(new Vector2Int(x, y));
                buttonList[x][y] = newButton;
            }
        }
    }
    
    public Transform getGridButton(int x, int y)
    {
        // (x,y) == (col,row)
        return buttonList[x][y];
    }
    public void onGridButtonPress(Vector2Int location)
    {
        if (ComponentSelection.selected == ComponentType.NONE)
        {
            
            Selected = location;
            return;
        }
        if (GameController.simState != SimState.EDITING)
            return;
        if (!ValidPlacement(location, ComponentSelection.selected))
            return;
        switch (ComponentSelection.selected)
        {
            case ComponentType.ERASER:
                DestroyGridComponent(location);
                break;
            case ComponentType.REDWIRE:
                if (IsWire(GetGridComponent(location)))
                {
                    GetGridComponent(location).GetComponent<Wire>().HasRed = true;
                    break;
                }
                SetGridComponent(location, wire).GetComponent<Wire>().Location = location;
                GetGridComponent(location).GetComponent<Wire>().HasRed = true;
                break;
            case ComponentType.GREENWIRE:
                if (IsWire(GetGridComponent(location)))
                {
                    GetGridComponent(location).GetComponent<Wire>().HasGreen = true;
                    break;
                }
                SetGridComponent(location, wire).GetComponent<Wire>().Location = location;
                GetGridComponent(location).GetComponent<Wire>().HasGreen = true;
                break;
            case ComponentType.ADD:
                SetGridComponent(location, addition);
                break;
            case ComponentType.SUB:
                SetGridComponent(location, subtraction);
                break;
            case ComponentType.MULT:
                SetGridComponent(location, multiplication);
                break;
            case ComponentType.DIV:
                SetGridComponent(location, division);
                break;
            case ComponentType.CONSTANT:
                SetGridComponent(location, constant);
                break;
            case ComponentType.EQUALITY:
                SetGridComponent(location, equality);
                break;
            case ComponentType.LESSTHAN:
                SetGridComponent(location, lessThan);
                break;
        }
        
        UpdateAdjacentWires(location);
    }
    
    public bool ValidPlacement(Vector2Int position, ComponentType type)
    {
        bool result = false;
        if (!InGrid(position))
            return false;
        GameObject g = GetGridComponent(position);
        if (g == null)
            return true;
        switch (type)
        {
            case ComponentType.NONE:
                return true;
            case ComponentType.ERASER:
                return true;
            case ComponentType.REDWIRE:
                if (IsOperator(g))
                    return false;
                if (IsWire(g) && !g.GetComponent<Wire>().HasRed)
                    return true;
                break;
            case ComponentType.GREENWIRE:
                if (IsOperator(g))
                    return false;
                if (IsWire(g) && !g.GetComponent<Wire>().HasGreen)
                    return true;
                break;
            default:
                return !(IsOperator(g) || IsWire(g));
        }
        return result;
        
    }
    
    public void SelectAll(Rect dragBox)
    {
        
        GroupSelection.PointSet = new HashSet<Vector2Int>();
        GameObject g;
        for (int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                if(dragBox.Overlaps(GetWorldRect(buttonList[x][y]),true))
                {
                    g = GetGridComponent(x,y);
                    if(g != null && !IsImporterOrExporter(g)){
                        GroupSelection.PointSet.Add(new Vector2Int(x,y));
                    }
                    
                }
            }
        }
        foreach(Vector2Int point in GroupSelection.PointSet)
        {
            g = GetGridComponent(point);
            if (IsOperator(g))
                GetGridComponent(point).GetComponent<Operator>().IsSelected = true;
            else if (IsWire(g))
                GetGridComponent(point).GetComponent<Wire>().IsSelected = true;
        }
    }
    public void ClearGroupSelection()
    {
        foreach(Vector2Int point in GroupSelection.PointSet)
        {
            if(IsOperator(GetGridComponent(point)))
                GetGridComponent(point).GetComponent<Operator>().IsSelected = false;
            else if(IsWire(GetGridComponent(point)))
                GetGridComponent(point).GetComponent<Wire>().IsSelected = false;
        }
        GroupSelection.PointSet.Clear();
    }
    public void MoveGroupSelectionTo(Vector2Int targetPos)
    {
        Vector2Int moveBy = targetPos - GroupSelection.GraspPoint;
        if(ValidGroupMove(moveBy)){
            ShiftGroupSelection(moveBy);
        } else {
            ShiftGroupSelection(new Vector2Int(moveBy.x,0));
            ShiftGroupSelection(new Vector2Int(0,moveBy.y));
        }
        
    }
    public void ShiftGroupSelection(Vector2Int moveBy)
    {
        //Attempts to move group selection to target position.
        GameObject g;
        if(ValidGroupMove(moveBy))
        {
            List<string> saveStrings = new List<string>();
            foreach(Vector2Int point in GroupSelection.PointSet)    //Save position and config of components in GroupSelection
            {
                g = GetGridComponent(point);
                if(IsWire(g))
                {
                    saveStrings.Add(g.GetComponent<Wire>().SaveString());
                } else if(IsOperator(g) && !IsImporterOrExporter(g)){
                    saveStrings.Add(g.GetComponent<Operator>().SaveString());
                }
            }
            foreach(Vector2Int point in GroupSelection.PointSet)    //Destroy all components in GroupSelection, because we will be spawning new components
            {
                DestroyGridComponent(point);
            }
            GroupSelection.PointSet.Clear();
            foreach(string saveString in saveStrings)               //Spawn in new components with saved configuration at correct position
            {
                Vector2Int pos = LoadComponentWithOffset(saveString,moveBy);
                GroupSelection.PointSet.Add(pos);
                if(IsOperator(GetGridComponent(pos)))
                {
                    GetGridComponent(pos).GetComponent<Operator>().IsSelected = true;
                } else if (IsWire(GetGridComponent(pos)))
                {
                    GetGridComponent(pos).GetComponent<Wire>().IsSelected = true;
                }

            }
            GroupSelection.GraspPoint += moveBy;    //Update grasp point
        }
    }
    private Vector2Int LoadComponentWithOffset(string saveString, Vector2Int offset)
    {
        string[] data = saveString.Split('\t');
        int[] values = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            values[i] = int.Parse(data[i]);
        }
        values[1] += offset.x;  //apply offset
        values[2] += offset.y;
        SaveData.LoadComponent(values);
        return new Vector2Int(values[1],values[2]); //return new position
    }
    public bool ValidGroupMove(Vector2Int offset)
    {
        //All components in GroupSelection must have a valid empty space or space occupied by a Selected component at specified offset
        foreach(Vector2Int point in GroupSelection.PointSet)
        {
            if(!InGrid(point+offset))
                return false;   //none of the components in group selection can be off the grid
            if(!(GetGridComponent(point+offset) == null || GroupSelection.PointSet.Contains(point+offset)))
                return false;   //space must be empty or already part of the group selection
        }
        return true;
    }
    
    public static Rect GetWorldRect(Transform trans)
    {
        Vector3[] corners = new Vector3[4];
        trans.GetComponent<RectTransform>().GetWorldCorners(corners);
        return new Rect(corners[1].x,corners[1].y,corners[3].x-corners[0].x,corners[0].y-corners[1].y);
    }
    public bool IsWire(GameObject g)
    {
        return g != null && g.GetComponent<Wire>() != null;
    }
    public bool IsOperator(GameObject g)
    {
        return g != null && g.GetComponent<Operator>() != null;
//        return g != null && g.CompareTag("Operator");   //placeholder until Operator class is built          
    }
    public bool IsImporterOrExporter(GameObject g)
    {
        if (g == null || !IsOperator(g))
            return false;
        return (g.GetComponent<Importer>() != null) || (g.GetComponent<Exporter>() != null);
    }
    
    public void ClearGrid()
    {
        GameObject g = null;
        ValueDisplayManager.DestroyAllValueDisplays();
        for(int i = 0; i < Height; i++)
        {
            for(int j = 0; j < Width; j++)
            {
                
                DestroyGridComponent(i, j);
                
            }
        }
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    
   
}
