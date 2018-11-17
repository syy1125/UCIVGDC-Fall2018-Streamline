using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GridButton : MonoBehaviour {

    // Use this for initialization
    private Vector2Int position;
    private EventSystem eventSystem;
    public Transform DragBoxPrefab;
    public static Vector2Int DragBegin;
    public static Vector2Int DragEnd;

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
        DragBegin = position;
        SpawnDragBox();
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
        Grid.Instance.onGridButtonPress(position);
    }
    public void BeginDragDraw()
    {
        GameController.mouseDragging = true;
    }
    public void SpawnDragBox()
    {
        Transform dragBox = Instantiate(DragBoxPrefab,transform.root);
        dragBox.GetComponent<DragBox>().SetOrigin(GameController.MouseWorldPosition);
        dragBox.GetComponent<DragBox>().SetBounds(transform.parent.GetComponent<RectTransform>());
        dragBox.localScale = Vector3.one;
    }
}
