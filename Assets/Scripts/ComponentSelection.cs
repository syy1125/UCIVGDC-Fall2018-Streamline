using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Selection
{
    NONE, ERASER, REDWIRE, GREENWIRE,
    ADD, SUB, MULT, DIV, CONSTANT,
}
public class ComponentSelection : MonoBehaviour {

    // Use this for initialization
    //hard coded, to be removed later
    public KeyCode none;
    public KeyCode eraser;
    public KeyCode redWire;
    public KeyCode greenWire;
    public KeyCode addition;
    public KeyCode subtraction;
    public KeyCode multiplication;
    public KeyCode division;
    public KeyCode constant;
    public static Selection cursorSelection;
    private Image image;
	void Start () {
        cursorSelection = Selection.NONE;
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        image.enabled = !GameController.gameMenuOpen;
        if (Input.GetKeyDown(none))
        {
            setSelection(Selection.NONE);
        } else if (Input.GetKeyDown(eraser))
        {
            setSelection(Selection.ERASER);
        }
        else if (Input.GetKeyDown(redWire))
        {
            setSelection(Selection.REDWIRE);
        } else if (Input.GetKeyDown(greenWire))
        {
            setSelection(Selection.GREENWIRE);
        }
        else if (Input.GetKeyDown(addition))
        {
            setSelection(Selection.ADD);
        } else if (Input.GetKeyDown(subtraction))
        {
            setSelection(Selection.SUB);
        } else if (Input.GetKeyDown(multiplication))
        {
            setSelection(Selection.MULT);
        } else if (Input.GetKeyDown(division))
        {
            setSelection(Selection.DIV);
        } else if (Input.GetKeyDown(constant))
        {
            setSelection(Selection.CONSTANT);
        }

	}
    public Selection getSelection()
    {
        return cursorSelection;
    }
    public void setSelection(Selection s)
    {
        cursorSelection = s;
    }
    public void setSelection(int x)
    {
        //This is the only way buttons can call setSelection, because of the enum parameter I think
        cursorSelection = (Selection)x;
    }
}
