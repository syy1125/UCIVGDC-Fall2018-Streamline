using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ConfirmationUIGroup : UIGroup {

    // Use this for initialization
    private Grid grid;
    public UIGroup gameMenuUIGroup;
    public KeyCode backKey = KeyCode.Escape;
	protected override void Start () {
        base.Start();
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
	}
    private void Update()
    {
        if (focused && Input.GetKeyDown(backKey))
        {
            StartCoroutine(EscapePress());
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
        gameMenuUIGroup.DisableUI();
    }
    public void NoPress()
    {
        if (!focused)
            return;
        this.DisableUI();
        gameMenuUIGroup.EnableUI();
    }
    public IEnumerator EscapePress()
    {
        yield return new WaitForEndOfFrame();
        this.DisableUI();
        gameMenuUIGroup.EnableUI();
    }

}
