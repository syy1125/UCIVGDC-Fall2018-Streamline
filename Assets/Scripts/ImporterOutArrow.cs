using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImporterOutArrow : MonoBehaviour {

    // Use this for initialization
    public enum Direction
    {
        UP, RIGHT, DOWN, LEFT
    }
    public Vector2Int Location;
    private Image[] Arrows;
    private Grid GameGrid;
	void Start () {
        Arrows = GetComponentsInChildren<Image>();
        GameGrid = Grid.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < Arrows.Length; i++)
        {
            Arrows[i].enabled = false;
        }
        AnalyzeTile(Direction.UP);
        AnalyzeTile(Direction.RIGHT);
        AnalyzeTile(Direction.DOWN);
        AnalyzeTile(Direction.LEFT);

    }
    private void AnalyzeTile(Direction d)
    {
        GameObject g = GameGrid.GetGridComponent(Location+ConvertToVector(d));
        if (g == null)
            return;
        if (GameGrid.IsWire(g))
        {
            Arrows[(int)d].enabled = true;
        } else if(GameGrid.IsOperator(g))
        {
            Receiver r = g.GetComponent<Receiver>();
            if (r == null)
                return;
            if (r.Location + r.InputDirection1 == Location
                || r.Location + r.InputDirection2 == Location)
                Arrows[(int)d].enabled = true;
        }
    }
    private Vector2Int ConvertToVector(Direction d)
    {
        switch(d)
        {
            case Direction.UP:
                return Vector2Int.up;
            case Direction.RIGHT:
                return Vector2Int.right;
            case Direction.DOWN:
                return Vector2Int.down;
            default:
                return Vector2Int.left;
        }
    }
}
