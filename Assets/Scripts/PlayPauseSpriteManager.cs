using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayPauseSpriteManager : MonoBehaviour {

    // Use this for initialization
    public Sprite altSprite;
    private Image image;
	void Awake () {
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if(GameController.simState == SimState.EDITING || GameController.simState == SimState.PAUSED || GameController.simState == SimState.CRASHED)
        {
            image.overrideSprite = null;
        } else
        {
            image.overrideSprite = altSprite;
        }
	}
}
