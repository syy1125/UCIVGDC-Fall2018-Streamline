using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIGroup : MonoBehaviour {

    // Use this for initialization
    protected Canvas myCanvas;
    protected EventSystem eventSystem;
    public GameObject defaultButton;
    public UIGroup parentGroup; //can be null
    public bool focused = false;
    protected virtual void Start () {
        myCanvas = GetComponent<Canvas>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

	}
	
	
    public virtual void EnableUI()
    {
        focused = true;
        myCanvas.enabled = true;
        eventSystem.SetSelectedGameObject(defaultButton);
    }
    public virtual void DisableUI()
    {
        focused = false;
        myCanvas.enabled = false;
        if(parentGroup != null)
        {
            eventSystem.SetSelectedGameObject(parentGroup.defaultButton);
        } else
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }
    public void SetSelectedGameObject(GameObject g)
    {
        eventSystem.SetSelectedGameObject(g);
    }
    
}
