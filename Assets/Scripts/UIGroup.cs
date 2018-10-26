using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIGroup : MonoBehaviour {

    // Use this for initialization
    private Canvas myCanvas;
    private EventSystem eventSystem;
    public GameObject defaultButton;
    public UIGroup parentGroup;
    protected virtual void Start () {
        myCanvas = GetComponent<Canvas>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

	}
	
	
    public virtual void EnableUI()
    {
        myCanvas.enabled = true;
        eventSystem.SetSelectedGameObject(defaultButton);
    }
    public virtual void DisableUI()
    {
        myCanvas.enabled = false;
        if(parentGroup != null)
        {
            eventSystem.SetSelectedGameObject(parentGroup.defaultButton);
        } else
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }
}
