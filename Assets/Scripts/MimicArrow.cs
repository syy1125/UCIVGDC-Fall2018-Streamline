using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicArrow : MonoBehaviour {

    // Use this for initialization
    public Transform copyTransform;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = copyTransform.rotation;
	}
    
}
