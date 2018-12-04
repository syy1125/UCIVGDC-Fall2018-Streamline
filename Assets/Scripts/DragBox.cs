using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBox : MonoBehaviour {

	// Use this for initialization
	[HideInInspector]
	public Vector2 Origin;
	private Camera mainCam;
	private Vector2 Pointer;
	private RectTransform rectTransform;
	private Vector2 LowerLeftBounds = Vector2.zero;
	private Vector2 UpperRightBounds = Vector3.one;
    private Rect bounds;
	void Awake () {
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		Pointer = (Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition);
		rectTransform = GetComponent<RectTransform>();
		SetOrigin(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
		{
			Grid.Instance.SelectAll(Grid.GetWorldRect(transform));
			Destroy(gameObject);
		}
		Pointer = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Pointer = new Vector2(Mathf.Clamp(Pointer.x,bounds.xMin,bounds.xMax),Mathf.Clamp(Pointer.y,bounds.yMax,bounds.yMin));
        Pointer = mainCam.WorldToViewportPoint(Pointer);
		SetRectPos(Origin,Pointer);
		if(Pointer.x > Origin.x)
		{
			if(Pointer.y > Origin.y)
				DrawUpperRight();
			else	
				DrawLowerRight();
		}
		else
		{
			if(Pointer.y > Origin.y)
				DrawUpperLeft();
			else
				DrawLowerLeft();
		}
	}
	private void DrawUpperRight()
	{
		SetRectPos(Origin,Pointer);
	}
	private void DrawLowerRight()
	{
		Vector2 LowerLeft = new Vector2(Origin.x,Pointer.y);
		Vector2 UpperRight = new Vector2(Pointer.x,Origin.y);
		SetRectPos(LowerLeft,UpperRight);
	}
	private void DrawUpperLeft()
	{
		Vector2 LowerLeft = new Vector2(Pointer.x,Origin.y);
		Vector2 UpperRight = new Vector2(Origin.x,Pointer.y);
		SetRectPos(LowerLeft,UpperRight);
	}
	private void DrawLowerLeft()
	{
		SetRectPos(Pointer,Origin);
	}
	private void SetRectPos(Vector2 LowerLeft, Vector2 UpperRight)
	{
		LowerLeft.x = Mathf.Clamp(LowerLeft.x,LowerLeftBounds.x,UpperRightBounds.x);
		LowerLeft.y = Mathf.Clamp(LowerLeft.y,LowerLeftBounds.y,UpperRightBounds.y);
		UpperRight.x = Mathf.Clamp(UpperRight.x,LowerLeftBounds.x,UpperRightBounds.x);
		UpperRight.y = Mathf.Clamp(UpperRight.y,LowerLeftBounds.y,UpperRightBounds.y);
		rectTransform.anchorMin = LowerLeft;
		rectTransform.anchorMax = UpperRight;
	}
	public void SetOrigin(Vector2 WorldCoords)
	{
		Origin = (Vector2)mainCam.WorldToViewportPoint(WorldCoords);
	}
	public void SetBounds(RectTransform newBounds)
	{
        bounds = Grid.GetWorldRect(newBounds.transform);
	}


}
