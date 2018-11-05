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
    
    
    public static ArrowSelection selection = ArrowSelection.NONE;
    private GameObject currentlySelectedComponent;
    public KeyCode input1Key;
    public KeyCode input2Key;
    public KeyCode outputKey;
    public GameObject input1Button;
    public GameObject input2Button;
    public GameObject outputButton;
    public RectTransform mimicArrows;
    public Color input1Color;
    public Color input2Color;
    public Color outputColor;
    public DirectionButton[] directionButtons;
    private void Start()
    {
        
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
                for (int i = 0; i < directionButtons.Length; i++)
                {
                    directionButtons[i].GetComponent<Image>().enabled = true;
                }
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
    public void ClearSelection()
    {
        ComponentName.text = "Empty";
        foreach (Transform t in ComponentPreview.transform)
        {
            Destroy(t.gameObject);
        }
        SetArrowSelection(ArrowSelection.NONE);

    }
    private void Update()
    {
        if(Grid.Instance.GetGridComponent(Grid.Instance.Selected) == null)
        {
            //Remove component editor content and clear green selection square
            ClearSelection();
            Grid.Instance.Selected = new Vector2Int(-1,-1);
            for(int i = 0; i < directionButtons.Length; i++)
            {
                directionButtons[i].GetComponent<Image>().enabled = false;
            }
        }
        if (Grid.Instance.GetGridComponent(Grid.Instance.Selected) != null 
            && Grid.Instance.IsOperator(Grid.Instance.GetGridComponent(Grid.Instance.Selected)))
        {
            //Run if selected is not null and is Operator
            //Watch for selecting arrow inputs.
            //Set position for the mimic arrows on the grid
            //Make selecting arrow buttons visible
            currentlySelectedComponent = Grid.Instance.GetGridComponent(Grid.Instance.Selected);
            if (Input.GetKeyDown(input1Key))
            {
                SetArrowSelection(ArrowSelection.IN1);
            } else if (Input.GetKeyDown(input2Key))
            {
                SetArrowSelection(ArrowSelection.IN2);
            } else if (Input.GetKeyDown(outputKey))
            {
                SetArrowSelection(ArrowSelection.OUT);
            }
            input1Button.SetActive(true);
            input2Button.SetActive(true);
            outputButton.SetActive(true);
            SetMimicArrows(Grid.Instance.GetGridComponent(Grid.Instance.Selected).transform.parent, Vector2.zero, Vector2.one);
        }
        else
        {
            //If selected is null or is not Operator
            //Make things invisible
            input1Button.SetActive(false);
            input2Button.SetActive(false);
            outputButton.SetActive(false);
            SetMimicArrows(null, Vector2.zero, Vector2.zero);
            currentlySelectedComponent = null;
        }

    }
    public void SetMimicArrows(Transform newParent, Vector2 minAnchor, Vector2 maxAnchor)
    {
        //mimic arrows show the overall inputs and outputs on the game grid
        mimicArrows.SetParent(newParent);
        mimicArrows.anchorMin = minAnchor;
        mimicArrows.anchorMax = maxAnchor;
        mimicArrows.offsetMax = Vector2.zero;
        mimicArrows.offsetMin = Vector2.zero;
    }
    public void SetArrowSelection(ArrowSelection a)
    {
        
        SetArrowSelection((int)a);
    }
    public void SetArrowSelection(int i)
    {
        //Buttons use this function
        selection = (ArrowSelection)i;
        InitializeArrows((ArrowSelection)i);
    }
    public void SetArrowDirection(Vector2Int v)
    {
        switch (selection)
        {
            case ArrowSelection.IN1:
                Grid.Instance.GetGridComponent(Grid.Instance.Selected).GetComponent<Receiver>().InputDirection1 = v;
                break;
            case ArrowSelection.IN2:
                Grid.Instance.GetGridComponent(Grid.Instance.Selected).GetComponent<Receiver>().InputDirection2 = v;
                break;
            case ArrowSelection.OUT:
                Grid.Instance.GetGridComponent(Grid.Instance.Selected).GetComponent<Transmitter>().OutputDirection = v;
                break;
        }
        InitializeArrows(selection);
    }
    public void SetArrowDirection(int d)
    {
        switch (d)
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
                Debug.Log("Angle not 0,90,180,270: " + d);
                break;
        }
    }
    /*
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
    */
    public void InitializeArrows(ArrowSelection s)
    {
        Color newColor = Color.white;
        float newAngle = 0;
        Vector2Int inputVector = Vector2Int.one * -1;
        switch (s)
        {
            case ArrowSelection.NONE:
                inputVector = Vector2Int.one * -1;
                newColor = new Color(0, 0, 0, 0);
                newAngle = 0;
                break;
            case ArrowSelection.IN1:
                inputVector = currentlySelectedComponent.GetComponent<Receiver>().InputDirection1;
                newColor = input1Color;
                newAngle = 90;
                break;
            case ArrowSelection.IN2:
                inputVector = currentlySelectedComponent.GetComponent<Receiver>().InputDirection2;
                newColor = input2Color;
                newAngle = 90;
                break;
            case ArrowSelection.OUT:
                inputVector = currentlySelectedComponent.GetComponent<Transmitter>().OutputDirection;
                newColor = outputColor;
                newAngle = -90;
                break;
        }
        if(inputVector == Vector2Int.right)
        {
            SetActiveSprite(0);
        } else if(inputVector == Vector2Int.up)
        {
            SetActiveSprite(1);
        } else if(inputVector == Vector2Int.left)
        {
            SetActiveSprite(2);
        } else
        {
            SetActiveSprite(3);
        }
        for(int i = 0; i < directionButtons.Length; i++)
        {
            directionButtons[i].GetComponent<Image>().color = newColor;
            directionButtons[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, newAngle));
        }

    }
    public void SetActiveSprite(int x)
    {
        //x is index of directionButton to activate
        for(int i = 0; i < directionButtons.Length; i++)
        {
            directionButtons[i].SetActivated(false);
        }
        directionButtons[x].SetActivated(true);
    }
}