using System;
using UnityEngine;
using UnityEngine.UI;
public enum ArrowSelection
{
    NONE, IN1, IN2, OUT,
}

public class ComponentEditor : MonoBehaviour
{
	public Text ComponentName;
	public GameObject ComponentPreview;
    /*
	[Serializable]
	public class DirectionGroup
	{
		public GameObject Input1;
		public GameObject Input2;
		public GameObject Output;
	}

	public DirectionGroup Up;
	public DirectionGroup Down;
	public DirectionGroup Left;
	public DirectionGroup Right;
    */
    public GameObject inputArrow1;
    public GameObject inputArrow2;
    public GameObject outputArrow;
    private Image inputImage1;
    private Image inputImage2;
    private Image outputImage;
    private ArrowSelection selection = ArrowSelection.NONE;
    private GameObject currentlySelectedComponent;
    private void Start()
    {
        inputImage1 = inputArrow1.GetComponentInChildren<Image>();
        inputImage2 = inputArrow2.GetComponentInChildren<Image>();
        outputImage = outputArrow.GetComponentInChildren<Image>();
        inputImage1.enabled = false;
        inputImage2.enabled = false;
        outputImage.enabled = false;
    }
    public void UpdateUI()
	{
		foreach (Transform t in ComponentPreview.transform)
		{
			Destroy(t.gameObject);
		}
		
		if (Grid.Instance.InGrid(Grid.Instance.Selected))
		{
			if (Grid.Instance.IsOperator(Grid.Instance.GetGridComponent(Grid.Instance.Selected)))
			{
				GameObject gridComponent = Grid.Instance.GetGridComponent(Grid.Instance.Selected);
				Instantiate(gridComponent, ComponentPreview.transform);
				// LOL super hacky. Is there a better way?
				ComponentName.text = gridComponent.GetComponent<Operator>().OpName;
                currentlySelectedComponent = Grid.Instance.GetGridComponent(Grid.Instance.Selected);
                SetArrowSelection(ArrowSelection.IN1);
				return;
			} else
            {
                SetArrowSelection(ArrowSelection.NONE);
                return;
            }
		}
        SetArrowSelection(ArrowSelection.NONE);
        ComponentName.text = "Empty";
	}
    private void Update()
    {
        if(Grid.Instance.Selected != Vector2Int.one * -1)
            currentlySelectedComponent = Grid.Instance.GetGridComponent(Grid.Instance.Selected);
    }
    
    public void SetArrowSelection(ArrowSelection a)
    {
        SetArrowSelection((int)a);
    }
    public void SetArrowSelection(int i)
    {
        //Buttons use this function
        selection = (ArrowSelection)i;
        inputImage1.enabled = false;
        inputImage2.enabled = false;
        outputImage.enabled = false;
        switch (selection)
        {
            case ArrowSelection.IN1:
                inputImage1.enabled = true;
                inputArrow1.GetComponent<EditorArrow>().InitializePosition(currentlySelectedComponent.GetComponent<Receiver>().InputDirection1);
                break;
            case ArrowSelection.IN2:
                inputImage2.enabled = true;
                inputArrow2.GetComponent<EditorArrow>().InitializePosition(currentlySelectedComponent.GetComponent<Receiver>().InputDirection2);
                break;
            case ArrowSelection.OUT:
                outputImage.enabled = true;
                //outputArrow.GetComponent<EditorArrow>().InitializePosition(currentlySelectedComponent.GetComponent<Receiver>().OutputDirection);
                break;
            case ArrowSelection.NONE:
                break;
        }
    }
    public void SetArrowDirection(Vector2Int v)
    {
        switch (selection)
        {
            case ArrowSelection.IN1:
                Debug.Log(v);
                Grid.Instance.GetGridComponent(Grid.Instance.Selected).GetComponent<Receiver>().InputDirection1 = v;
                break;
            case ArrowSelection.IN2:
                Grid.Instance.GetGridComponent(Grid.Instance.Selected).GetComponent<Receiver>().InputDirection2 = v;
                break;
            case ArrowSelection.OUT:
                //Grid.Instance.GetGridComponent(Grid.Instance.Selected).GetComponent<Transmitter>(). = v;
                break;
        }
    }
    public void OnEndDrag()
    {
        int angle = 0;
        switch (selection)
        {
            case ArrowSelection.IN1:
                angle = (int)(Mathf.Round((inputArrow1.transform.rotation.eulerAngles.z % 360) / 90) * 90);
                break;
            case ArrowSelection.IN2:
                angle = (int)(Mathf.Round((inputArrow2.transform.rotation.eulerAngles.z % 360) / 90) * 90);
                break;
            case ArrowSelection.OUT:
                angle = (int)(Mathf.Round((outputArrow.transform.rotation.eulerAngles.z % 360) / 90) * 90);
                break;
        }
        switch (angle)
        {
            case 0:
                SetArrowDirection(Vector2Int.right);
                break;
            case 90:
                SetArrowDirection(Vector2Int.up);
                break;
            case 180:
                SetArrowDirection(Vector2Int.left);
                break;
            case 270:
                SetArrowDirection(Vector2Int.down);
                break;
            default:
                Debug.Log("Angle not 0,90,180,270: " + angle);
                break;
        }
    }
}