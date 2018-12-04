using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour {

    // Use this for initialization
    public float angularSpeed;
    private Vector3 rotateBy;
	void Start () {
        rotateBy = new Vector3(0, 0, angularSpeed);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotateBy * Time.deltaTime);
	}
}
