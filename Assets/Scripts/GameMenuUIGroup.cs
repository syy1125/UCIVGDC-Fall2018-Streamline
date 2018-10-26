using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuUIGroup : UIGroup {

    public KeyCode backKey = KeyCode.Escape;
    private void Update()
    {
        if (focused && Input.GetKeyDown(backKey))
        {
            StartCoroutine(DelayedDisable());
        }
        
    }
    public override void EnableUI()
    {
        GameController.gameMenuOpen = true;
        base.EnableUI();
    }
    public override void DisableUI()
    {       
        GameController.gameMenuOpen = false;
        base.DisableUI();
    }
    private IEnumerator DelayedDisable()
    {
        yield return new WaitForEndOfFrame();
        DisableUI();
    }
}
