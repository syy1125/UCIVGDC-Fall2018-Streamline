using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GridButton : MonoBehaviour {

    // Use this for initialization
    private Vector2Int position;
    private EventSystem eventSystem;
    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(entry);

    }
    
    public void setPosition(Vector2Int p)
    {
        position = p;
    }
    public Vector2Int getPosition()
    {
        return position;
    }
    public void onPress()
    {
        Grid.Instance.onGridButtonPress(position);
    }
    public void OnPointerEnter(PointerEventData data)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }
    public void DragDraw()
    {
        //place components if this is a valid mouse drag
        if (!GameController.mouseDragging)
            return;
        onPress();
    }
    public void BeginDragDraw()
    {
        GameController.mouseDragging = true;
    }
}
