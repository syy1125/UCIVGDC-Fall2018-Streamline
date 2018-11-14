using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ConfirmationUIGroup : UIGroup {

    // Use this for initialization
    private Grid grid;
	protected override void Start () {
        base.Start();
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
	}
    protected override void Update()
    {
        if (focused && Input.GetKeyDown(backKey))
        {
            StartCoroutine(BackKeyPress());
        }
    }
    public override void EnableUI()
    {
        GameController.clearConfirmationOpen = true;
        base.EnableUI();
    }
    public override void DisableUI()
    {
        GameController.clearConfirmationOpen = false;
        base.DisableUI();
    }
    public void YesPress()
    {
        if (!focused)
            return;
        grid.ClearGrid();
        this.DisableUI();
        parentGroup.DisableUI();
    }
    public void NoPress()
    {
        if (!focused)
            return;
        this.DisableUI();
        parentGroup.EnableUI();
    }
    

}
