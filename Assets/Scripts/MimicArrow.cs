using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicArrow : MonoBehaviour {

    // Use this for initialization
    private GameObject selectedComponent;
    public ArrowSelection copySelection;
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        selectedComponent = Grid.Instance.GetGridComponent(Grid.Instance.Selected);
        if (selectedComponent != null && Grid.Instance.IsOperator(selectedComponent))
        {
            UpdateArrow();
        }
	}
    void UpdateArrow()
    {
        Vector2Int v = Vector2Int.zero;
        switch (copySelection)
        {
            case ArrowSelection.IN1:
                v = selectedComponent.GetComponent<Receiver>().InputDirection1;
                break;
            case ArrowSelection.IN2:
                v = selectedComponent.GetComponent<Receiver>().InputDirection2;
                break;
            case ArrowSelection.OUT:
                v = selectedComponent.GetComponent<Transmitter>().OutputDirection;
                break;
        }
        if (v == Vector2Int.right)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (v == Vector2Int.up)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else if (v == Vector2Int.left)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
    }
    
}
