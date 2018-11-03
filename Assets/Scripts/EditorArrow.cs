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
            transform.Rotate(new Vector3(0,0,90));
        }
	}
    public void InitializePosition(Vector2Int v)
    {
        if(v == Vector2Int.right)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }
        if(v == Vector2Int.up)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            return;
        }
        if (v == Vector2Int.left)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            return;
        }
        if (v == Vector2Int.down)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
            return;
        }
    }
    public void BeginDrag()
    {
        isHeld = true;
    }
    public void EndDrag()
    {
        isHeld = false;
        int angle = (int)(Mathf.Round((transform.rotation.eulerAngles.z % 360) / 90) * 90);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }
}
