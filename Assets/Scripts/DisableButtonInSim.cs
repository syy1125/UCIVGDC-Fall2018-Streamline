using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableButtonInSim : MonoBehaviour {

	// Use this for initialization
	private Button button;
	void Awake () {
		button = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameController.simState != SimState.EDITING){
			button.interactable = false;
		} else {
			button.interactable = true;
		}
	}
}
