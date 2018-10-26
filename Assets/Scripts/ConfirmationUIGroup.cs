using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationUIGroup : UIGroup {

    // Use this for initialization
    private Grid grid;
	protected override void Start () {
        base.Start();
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
	}
	
	public void YesPress()
    {
        grid.ClearGrid();
        this.DisableUI();
    }
    public void NoPress()
    {
        this.DisableUI();
    }
}
