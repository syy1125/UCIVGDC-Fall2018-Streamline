using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuUIGroup : UIGroup {

   
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
}
   
