using System.Diagnostics.CodeAnalysis;
using UnityEngine;
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

    public ColArray[] Columns;

    public GameObject wire;
    public GameObject addition;
    public GameObject subtraction;
    public GameObject multiplication;
    public GameObject division;
    public GameObject constant;
    public GameObject importer;
    public GameObject exporter;
    private GameObject[][] _gridComponents;

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

    private void Start () {
        
        _gridComponents = new GameObject[Width][];
        for (int x = 0; x < Width; x++)
        {
            _gridComponents[x] = new GameObject[Height];
        }
        
        buildGridButtons();
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
        Destroy(GetGridComponent(x,y));
        _gridComponents[x][y] = null;
    }
    public void DestroyGridComponent(Vector2Int pos)
    {
        DestroyGridComponent(pos.x, pos.y);
    }
    private void buildGridButtons()
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
        
        // Set up imports and exports
        GameObject importer1 = SetGridComponent(0, Height - 1, importer);
        importer1.GetComponent<Importer>().SequenceIndex = 0;
        importer1.GetComponent<Importer>().outputColumn = Columns[0];
        
        GameObject importer2 = SetGridComponent(0, 0, importer);
        importer2.GetComponent<Importer>().SequenceIndex = 1;
        importer2.GetComponent<Importer>().outputColumn = Columns[1];
        
        GameObject exporter1 = SetGridComponent(Width - 1, 0, exporter);
        exporter1.GetComponent<Exporter>().outputColumn = Columns[2];
        
        GameObject exporter2 = SetGridComponent(Width - 1, Height - 1, exporter);
        exporter2.GetComponent<Exporter>().outputColumn = Columns[3];
    }
    public Transform getGridButton(int x, int y)
    {
        // (x,y) == (col,row)
        return buttonList[x][y];
    }
    public void onGridButtonPress(Vector2Int location)
    {
        if (ComponentSelection.cursorSelection == Selection.NONE)
        {
            Selected = location;
            return;
        }
        
        if (!ValidPlacement(location, ComponentSelection.cursorSelection))
            return;
        switch (ComponentSelection.cursorSelection)
        {
            case Selection.ERASER:
                DestroyGridComponent(location);
                break;
            case Selection.REDWIRE:
                if (IsWire(GetGridComponent(location)))
                {
                    GetGridComponent(location).GetComponent<Wire>().HasRed = true;
                    break;
                }
                SetGridComponent(location, wire).GetComponent<Wire>().Location = location;
                GetGridComponent(location).GetComponent<Wire>().HasRed = true;
                break;
            case Selection.GREENWIRE:
                if (IsWire(GetGridComponent(location)))
                {
                    GetGridComponent(location).GetComponent<Wire>().HasGreen = true;
                    break;
                }
                SetGridComponent(location, wire).GetComponent<Wire>().Location = location;
                GetGridComponent(location).GetComponent<Wire>().HasGreen = true;
                break;
            case Selection.ADD:
                SetGridComponent(location, addition);
                break;
            case Selection.SUB:
                SetGridComponent(location, subtraction);
                break;
            case Selection.MULT:
                SetGridComponent(location, multiplication);
                break;
            case Selection.DIV:
                SetGridComponent(location, division);
                break;
            case Selection.CONSTANT:
                SetGridComponent(location, constant);
                break;
        }
        
        UpdateAdjacentWires(location);
    }
    public bool ValidPlacement(Vector2Int position, Selection selection)
    {
        bool result = false;
        if (!InGrid(position))
            return false;
        GameObject g = GetGridComponent(position);
        if (g == null)
            return true;
        switch (selection)
        {
            case Selection.NONE:
                return true;
            case Selection.ERASER:
                return true;
            case Selection.REDWIRE:
                if (IsOperator(g))
                    return false;
                if (IsWire(g) && !g.GetComponent<Wire>().HasRed)
                    return true;
                break;
            case Selection.GREENWIRE:
                if (IsOperator(g))
                    return false;
                if (IsWire(g) && !g.GetComponent<Wire>().HasGreen)
                    return true;
                break;
            case Selection.ADD:
                return !(IsOperator(g) || IsWire(g));
            case Selection.SUB:
                return !(IsOperator(g) || IsWire(g));
            case Selection.MULT:
                return !(IsOperator(g) || IsWire(g));
            case Selection.DIV:
                return !(IsOperator(g) || IsWire(g));
            case Selection.CONSTANT:
                return !(IsOperator(g) || IsWire(g));
        }
        return result;
        
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
    public void ClearGrid()
    {
        GameObject g = null;
        for(int i = 0; i < Height; i++)
        {
            for(int j = 0; j < Width; j++)
            {
                g = GetGridComponent(i, j);
                //Do not delete importers/exporters
                if (g != null && !(IsOperator(g) && (g.GetComponent<Operator>().OpName.Equals("Importer")
                    || g.GetComponent<Operator>().OpName.Equals("Output"))))
                {
                    DestroyGridComponent(i, j);
                }
            }
        }
    }
    private void OnDestroy()
    {
        Instance = null;
    }
}
