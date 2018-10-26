using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Serialization;

public class Grid : MonoBehaviour
{
    public static Grid Instance;
    
    // Use this for initialization
    [FormerlySerializedAs("rows")] public int Height;
    [FormerlySerializedAs("columns")] public int Width;
    public Transform gridButtonPrefab;
    [HideInInspector]
    public Transform[][] buttonList;

    public GameObject wire;
    public GameObject addition;
    public GameObject subtraction;
    public GameObject multiplication;
    public GameObject division;
    public GameObject constant;
    private GameObject[][] _gridComponents;

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
        buildGridButtons(Height, Width);
        
        _gridComponents = new GameObject[Width][];
        for (int x = 0; x < Width; x++)
        {
            _gridComponents[x] = new GameObject[Height];
        }
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
        return _gridComponents[x][y];
    }

    public GameObject GetGridComponent(Vector2Int pos)
    {
        return GetGridComponent(pos.x, pos.y);
    }

    public GameObject SetGridComponent(int x, int y, GameObject prefab)
    {
        _gridComponents[x][y] = Instantiate(prefab, getGridButton(x, y));
        return _gridComponents[x][y];
    }

    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void UpdateAdjacentWires(int x, int y)
    {
        GameObject up = InGrid(x, y + 1) ? GetGridComponent(x, y + 1) : null;
        GameObject down = InGrid(x, y - 1) ? GetGridComponent(x, y - 1) : null;
        GameObject left = InGrid(x - 1, y) ? GetGridComponent(x - 1, y) : null;
        GameObject right = InGrid(x + 1, y) ? GetGridComponent(x + 1, y) : null;

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
    private void buildGridButtons(int h, int w)
    {
        //Create grid of buttons with x,y coordinates
        buttonList = new Transform[w][];
        float width = 1f / w;
        float height = 1f / h;
        for (int row = 0; row < w; row++)
        {
            buttonList[row] = new Transform[h];
            for(int col = 0; col < h; col++)
            {
                Transform newButton = Instantiate(gridButtonPrefab);
                newButton.SetParent(transform);
                RectTransform buttonTransform = newButton.GetComponent<RectTransform>();
                
                buttonTransform.anchorMin = new Vector2(row * width, col * height);
                buttonTransform.anchorMax = new Vector2((row+1) * width, (col+1) * height);
                buttonTransform.offsetMin = Vector2.zero;
                buttonTransform.offsetMax = Vector2.zero;
                
                newButton.localScale = Vector3.one;
                newButton.GetComponent<GridButton>().grid = GetComponent<Grid>();
                newButton.GetComponent<GridButton>().setPosition(new Vector2Int(row, col));
                buttonList[row][col] = newButton;
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
        if (!ValidPlacement(location, ComponentSelection.cursorSelection))
            return;
        switch (ComponentSelection.cursorSelection)
        {
            case Selection.NONE:
                return;
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
        //return g != null && g.GetComponent<Operator>() != null);
        return g != null && g.CompareTag("Operator");   //placeholder until Operator class is built
            
    }
    public void ClearGrid()
    {
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
