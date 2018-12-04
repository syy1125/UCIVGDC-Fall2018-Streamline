using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameMenuUIGroup : UIGroup {

    public UIGroup ConfirmationCanvas;
    public UIGroup SkipCanvas;
    public Text ClearText;
    public override void EnableUI()
    {
        GameController.gameMenuOpen = true;
        InitializeClearButton();
        base.EnableUI();
    }
    public override void DisableUI()
    {       
        GameController.gameMenuOpen = false;
        base.DisableUI();
    }
    public void OnClearButtonPress()
    {
        if(Tutorial.TutorialActive)
        {
            SkipCanvas.EnableUI();
        } else
        {
            ConfirmationCanvas.EnableUI();
        }
    }
    public void InitializeClearButton()
    {
        if(Tutorial.TutorialActive)
        {
            ClearText.GetComponentInChildren<Text>().text = "Skip Tutorial";
        } else
        {
            ClearText.GetComponentInChildren<Text>().text = "Clear Grid";
        }
    }
}
   
