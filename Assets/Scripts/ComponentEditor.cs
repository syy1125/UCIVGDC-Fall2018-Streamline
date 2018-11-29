using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum ArrowSelection
{
    NONE, IN1, IN2, OUT,
}

public class ComponentEditor : MonoBehaviour
{
	public Text ComponentName;
    public Text helpText;
	public GameObject ComponentPreview;
   
    
    public static ArrowSelection selection = ArrowSelection.NONE;
    private GameObject currentlySelectedComponent;
    public KeyCode input1Key;
    public KeyCode input2Key;
    public KeyCode outputKey;
    public GameObject input1Button;
    public GameObject input2Button;
    public GameObject outputButton;
    public GameObject constEditor;
    private Image[] ioUnderlines;
    public RectTransform mimicArrows;
    public Color input1Color;
    public Color input2Color;
    public Color outputColor;
    public DirectionButton[] directionButtons;
    private EventSystem eventSystem;
    private bool[] IOMask;
    public AudioClip ArrowPressSound;
    public AudioClip InputTypeSwapSound;
    private AudioSource Source;

    private void Awake()
    {
        ioUnderlines = new Image[3];
        ioUnderlines[0] = input1Button.GetComponentsInChildren<Image>()[1];
        ioUnderlines[1] = input2Button.GetComponentsInChildren<Image>()[1];
        ioUnderlines[2] = outputButton.GetComponentsInChildren<Image>()[1];
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        Source = GetComponent<AudioSource>();
        IOMask = new bool[3];
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
				GameObject previewedObject = Instantiate(gridComponent, ComponentPreview.transform);
                foreach (Transform t in previewedObject.transform)
                {
                    Destroy(t.gameObject);
                }
                ComponentName.text = gridComponent.GetComponent<Operator>().OpName;
                currentlySelectedComponent = Grid.Instance.GetGridComponent(Grid.Instance.Selected);
                DisableStart(previewedObject);
                for (int i = 0; i < directionButtons.Length; i++)
                {
                    directionButtons[i].GetComponent<Image>().enabled = true;
                }
                if (currentlySelectedComponent.GetComponent<Operator>().OpName.Equals("Constant"))
                {
                    UpdateConst(constEditor.GetComponentInChildren<InputField>());
                    constEditor.SetActive(true);
                } else
                {
                    constEditor.SetActive(false);
                }
                IOMask = currentlySelectedComponent.GetComponent<Operator>().GetIOMask();
                if (IOMask[0])
                    SetArrowSelection(ArrowSelection.IN1);
                else if (IOMask[1])
                    SetArrowSelection(ArrowSelection.IN2);
                else if (IOMask[2])
                    SetArrowSelection(ArrowSelection.OUT);
                else
                    SetArrowSelection(ArrowSelection.NONE);
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
        SetButtonsActive(false);
        SetArrowSelection(ArrowSelection.NONE);
        helpText.text = "";
        Grid.Instance.Selected = new Vector2Int(-1, -1);
        for (int i = 0; i < directionButtons.Length; i++)
        {
            directionButtons[i].GetComponent<Image>().enabled = false;
        }
    }
    private void Update()
    {
        DrawButtonUnderlines();
        if(Grid.Instance.GetGridComponent(Grid.Instance.Selected) == null)
        {
            //Remove component editor content and clear green selection square
            ClearSelection();
            
        }
        if (Grid.Instance.GetGridComponent(Grid.Instance.Selected) != null 
            && Grid.Instance.IsOperator(Grid.Instance.GetGridComponent(Grid.Instance.Selected)))
        {
            //Run if selected is not null and is Operator
            //Watch for selecting arrow inputs.
            //Set position for the mimic arrows on the grid
            //Make selecting arrow buttons visible
            currentlySelectedComponent = Grid.Instance.GetGridComponent(Grid.Instance.Selected);
            CheckForInput();
            helpText.text = currentlySelectedComponent.GetComponent<Operator>().GetHint();
            SetButtonsActive(true);
            SetMimicArrows(Grid.Instance.GetGridComponent(Grid.Instance.Selected).transform.parent, Vector2.zero, Vector2.one);
        }
        else
        {
            //If selected is null or is not Operator
            //Make things invisible
            SetButtonsActive(false);
            SetMimicArrows(null, Vector2.zero, Vector2.zero);
            currentlySelectedComponent = null;
            helpText.text = "";
        }

    }
    private void CheckForInput()
    {
        //1,2,3 button switching
        if (Input.GetKeyDown(input1Key) && IOMask[0])
        {
            SetArrowSelection(ArrowSelection.IN1);
        }
        else if (Input.GetKeyDown(input2Key) && IOMask[1])
        {
            SetArrowSelection(ArrowSelection.IN2);
        }
        else if (Input.GetKeyDown(outputKey) && IOMask[2])
        {
            SetArrowSelection(ArrowSelection.OUT);
        }
        if(!(constEditor.activeSelf && eventSystem.currentSelectedGameObject == constEditor.GetComponentInChildren<InputField>().gameObject))
        {
            //Arrow key control
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                SetArrowDirection(Vector2Int.right);
                Source.PlayOneShot(ArrowPressSound);
            } else if (Input.GetKeyDown(KeyCode.UpArrow)|| Input.GetKeyDown(KeyCode.W))
            {
                SetArrowDirection(Vector2Int.up);
                Source.PlayOneShot(ArrowPressSound);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.A))
            {
                SetArrowDirection(Vector2Int.left);
                Source.PlayOneShot(ArrowPressSound);
            } else if (Input.GetKeyDown(KeyCode.DownArrow)|| Input.GetKeyDown(KeyCode.S))
            {
                SetArrowDirection(Vector2Int.down);
                Source.PlayOneShot(ArrowPressSound);
            }
        }
        //Tab and Shift+Tab
        int x = ((int)selection)-1; //0->IN1,1->IN2,2->OUT
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
        {
            do
            {
                x -= 1;
                if(x < 0)
                    x = 2;
            }
            while(!IOMask[x]);
            x += 1;
            SetArrowSelection((ArrowSelection)x);
            Source.PlayOneShot(InputTypeSwapSound);
        }
        else if(Input.GetKeyDown(KeyCode.Tab))
        {
            do
            {
                x += 1;
                if(x >= 3)
                    x = 0;
            }
            while(!IOMask[x]);
            x += 1;
            SetArrowSelection((ArrowSelection)x);
            Source.PlayOneShot(InputTypeSwapSound);
        }


    }
    private void DrawButtonUnderlines()
    {
        ioUnderlines[0].enabled = selection == ArrowSelection.IN1;
        ioUnderlines[1].enabled = selection == ArrowSelection.IN2;
        ioUnderlines[2].enabled = selection == ArrowSelection.OUT;

    }
    private void DisableStart(GameObject g)
    {
        if (g.GetComponent<Operator>() != null)
        {
            g.GetComponent<Text>().color = Color.red;
            g.GetComponent<Operator>().enabled = false;
        }
    }
    private void SetButtonsActive(bool b)
    {
        if (b)
        {
            input1Button.SetActive(IOMask[0]);
            input2Button.SetActive(IOMask[1]);
            outputButton.SetActive(IOMask[2]);
            
        } else
        {
            input1Button.SetActive(false);
            input2Button.SetActive(false);
            outputButton.SetActive(false);
            constEditor.SetActive(false);
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
        if (GameController.simState != SimState.EDITING)
            return;
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

        if (currentlySelectedComponent != null)
        {
            Grid.Instance.UpdateAdjacentWires(currentlySelectedComponent.GetComponent<Transmitter>().Location);
        }
    }
    public void SetArrowDirection(int d)
    {
        if (GameController.simState != SimState.EDITING)
            return;
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
    /*
    public void UpdateConst(Text t)
    {
        if (currentlySelectedComponent.GetComponent<Constant>() != null)
        {
            Constant constant = currentlySelectedComponent.GetComponent<Constant>();
            constant.number = Mathf.Clamp(constant.number, Operator.MIN, Operator.MAX);
            t.text = "Value=" + constant.number;
        }
    }
    
    
    public void IncrementConst(Text t)
    {
        if (GameController.simState != SimState.EDITING)
            return;
        if (currentlySelectedComponent.GetComponent<Constant>() != null)
        {
            Constant constant = currentlySelectedComponent.GetComponent<Constant>();
            constant.number += 1;
            if(constant.number > Operator.MAX)
                constant.number = Operator.MAX;
            t.text = "Value=" + constant.number;
        }
        
    }
    public void DecrementConst(Text t)
    {
        if (GameController.simState != SimState.EDITING)
            return;
        if (currentlySelectedComponent.GetComponent<Constant>() != null)
        {
            Constant constant = currentlySelectedComponent.GetComponent<Constant>();
            constant.number -= 1;
            if (constant.number < Operator.MIN)
                constant.number = Operator.MIN;
            t.text = "Value=" + constant.number;
        }
    }
    */
    public void SetConstValue(InputField field)
    {
        if (currentlySelectedComponent.GetComponent<Constant>() != null)
        {
            if(!field.text.Equals(""))
            {
                field.text = Mathf.Clamp(Int32.Parse(field.text),Operator.MIN,Operator.MAX).ToString();
                currentlySelectedComponent.GetComponent<Constant>().number = Int32.Parse(field.text);
            } else {
                field.text = "";
                currentlySelectedComponent.GetComponent<Constant>().number = 0;
            }
            
        }
    }
    public void UpdateConst(InputField field)
    {
        if (currentlySelectedComponent.GetComponent<Constant>() != null)
        {
            Constant constant = currentlySelectedComponent.GetComponent<Constant>();
            field.text = constant.number.ToString();

        }
    }
    
}