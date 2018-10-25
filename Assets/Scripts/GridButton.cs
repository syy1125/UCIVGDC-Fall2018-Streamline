using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GridButton : MonoBehaviour {

    // Use this for initialization
    public Grid grid;
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
        // Update is called once per frame
        void Update () {
		
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
        grid.onGridButtonPress(position);
    }
    public void OnPointerEnter(PointerEventData data)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
