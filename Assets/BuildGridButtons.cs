using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildGridButtons : MonoBehaviour {

    // Use this for initialization
    public int rows;
    public int columns;
    public Transform gridButtonPrefab;
    [HideInInspector]
    public Transform[][] buttonList;
    public Model model;
	void Start () {
        buildGrid(rows, columns);
	}
	
	// Update is called once per frame
	void Update () {
		

	}
    private void buildGrid(int r, int c)
    {
        //Create grid of buttons with x,y coordinates
        Transform newButton;
        buttonList = new Transform[c][];
        float width = 1f / c;
        float height = 1f / r;
        for (int i = 0; i < c; i++)
        {
            buttonList[i] = new Transform[r];
            for(int j = 0; j < r; j++)
            {
                newButton = Instantiate(gridButtonPrefab);
                newButton.SetParent(transform);
                newButton.GetComponent<RectTransform>().anchorMin = new Vector2(i * width, j * height);
                newButton.GetComponent<RectTransform>().anchorMax = new Vector2((i+1) * width, (j+1) * height);
                newButton.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                newButton.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                newButton.localScale = new Vector3(1, 1, 1);
                newButton.GetComponent<GridButton>().model = model;
                newButton.GetComponent<GridButton>().setPosition(new Vector2(i, j));
                buttonList[i][j] = newButton;
            }
        }
    }
    public Transform getGridButton(int x, int y)
    {
        // (x,y) == (col,row)
        return buttonList[x][y];
    }
}
