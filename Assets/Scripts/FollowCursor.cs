using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowCursor : MonoBehaviour {

    // Use this for initialization
    private Camera mainCamera;
    public Vector2 offset;
    private Image image;
	void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) + offset;
        updateColors();
	}
    private void updateColors()
    {
        switch (ComponentSelection.cursorSelection)
        {
            case Selection.NONE:
                image.color = Color.white;
                break;
            case Selection.ERASER:
                image.color = Color.magenta;
                break;
            case Selection.REDWIRE:
                image.color = Color.red;
                break;
            case Selection.GREENWIRE:
                image.color = Color.green;
                break;
            default:
                image.color = Color.blue;
                break;
        }
    }
}
