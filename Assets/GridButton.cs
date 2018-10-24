using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridButton : MonoBehaviour {

    // Use this for initialization
    public Model model;
    private Vector2 position;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void setPosition(Vector2 p)
    {
        position = p;
    }
    public Vector2 getPosition()
    {
        return position;
    }
    public void onPress()
    {
        model.onGridButtonPress(position);
    }
      
}
