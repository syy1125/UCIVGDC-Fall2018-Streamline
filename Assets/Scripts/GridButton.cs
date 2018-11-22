using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DragType
{
    NONE, DRAW, GROUPMOVE
}
public class GridButton : MonoBehaviour {

    // Use this for initialization
    private Vector2Int position;
    private EventSystem eventSystem;
    public Transform DragBoxPrefab;
    public static DragType DragType = DragType.NONE;
    public static Vector2Int DragBegin = Vector2Int.zero;
    public static Vector2Int DragEnd = Vector2Int.one;

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
    public void onClick()
    {
        Grid.Instance.onGridButtonPress(position);
        Grid.Instance.ClearGroupSelection();
    }
    public void onPress()
    {
        Grid grid = Grid.Instance;
        grid.onGridButtonPress(position);
        DragBegin = position;
        if(ComponentSelection.selected == ComponentType.NONE && Grid.Instance.GroupSelection.PointSet.Contains(position))
        {
            DragType = DragType.GROUPMOVE;
            Grid.Instance.GroupSelection.GraspPoint = position;
        } else {
            Grid.Instance.ClearGroupSelection();
        }
    }
    
    public void OnPointerEnter(PointerEventData data)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }
    
    public void DragDraw()
    {
        //place components if this is a valid mouse drag
        if (GameController.mouseDragging && DragType == DragType.DRAW)
        {
            DragEnd = position;
            Grid.Instance.onGridButtonPress(position);
        }
    }
    public void MoveSelectionToButton()
    {
        if(GameController.mouseDragging && DragType == DragType.GROUPMOVE)
        {
            DragEnd = position;
            Grid.Instance.MoveGroupSelectionTo(DragEnd);
            Grid.Instance.Selected = Vector2Int.one*-1;
        }
        
    }
    public void BeginDragDraw()
    {
        GameController.mouseDragging = true;
        if(ComponentSelection.selected != ComponentType.NONE)
            DragType = DragType.DRAW;
        else if(DragType == DragType.NONE)
        {
            SpawnDragBox();
        }
    }
    public void SpawnDragBox()
    {
        Transform dragBox = Instantiate(DragBoxPrefab,transform.root);
        dragBox.GetComponent<DragBox>().SetOrigin(GameController.MouseWorldPosition);
        dragBox.GetComponent<DragBox>().SetBounds(transform.parent.GetComponent<RectTransform>());
        dragBox.localScale = Vector3.one;
    }
}
