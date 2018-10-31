using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorArrow : MonoBehaviour {

    // Use this for initialization
    private Camera mainCamera;
    private bool isHeld;
	void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        if (isHeld)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, mainCamera.ScreenToWorldPoint(Input.mousePosition)-transform.position);
            
        }
	}
    public void BeginDrag()
    {
        isHeld = true;
    }
    public void EndDrag()
    {
        isHeld = false;
        float angle = transform.rotation.eulerAngles.z;
        angle = Mathf.Round(angle / 90) * 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
