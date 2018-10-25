using System.Collections;
using System.Collections.Generic;
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
    public Model model;

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
                newButton.GetComponent<GridButton>().model = model;
                newButton.GetComponent<GridButton>().setPosition(new Vector2(row, col));
                buttonList[row][col] = newButton;
            }
        }
    }
    public Transform getGridButton(int x, int y)
    {
        // (x,y) == (col,row)
        return buttonList[x][y];
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
