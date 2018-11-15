using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ComponentType
{
    NONE, ERASER, REDWIRE, GREENWIRE,
    ADD, SUB, MULT, DIV, CONSTANT, EQUALITY, LESSTHAN, COMPARE, IMPORTER, EXPORTER
}
public class ComponentSelection : MonoBehaviour 
{
    public static ComponentType selected;
    private Image image;
	void Start () {
        selected = ComponentType.NONE;
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        image.enabled = !GameController.gameMenuOpen;
	}
    public ComponentType getSelection()
    {
        return selected;
    }
    public void setSelection(ComponentType s)
    {
        selected = s;
    }
}
