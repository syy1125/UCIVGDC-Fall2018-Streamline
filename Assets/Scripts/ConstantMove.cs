using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMove : MonoBehaviour {

    // Use this for initialization
    public Vector2 velocity;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += (Vector3)velocity*Time.deltaTime;
	}
}
