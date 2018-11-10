using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorHighlight : MonoBehaviour {

    // Use this for initialization
    public SimState liveDuring = SimState.CRASHED;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GameController.simState != liveDuring)
            Destroy(gameObject);
	}
}
