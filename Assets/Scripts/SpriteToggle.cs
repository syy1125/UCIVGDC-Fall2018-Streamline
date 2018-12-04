using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpriteToggle : MonoBehaviour {

    // Use this for initialization
    private Sprite defaultSprite;
    public Sprite altSprite;
    public enum GraphicType
    {
        SPRITERENDERER, IMAGE
    }
    private GraphicType graphicType;
    private Object graphicRenderer;
    public bool drawAltSprite = false;
	void Awake () {
		if(GetComponent<SpriteRenderer>() != null)
        {
            graphicRenderer = GetComponent<SpriteRenderer>();
            defaultSprite = ((SpriteRenderer)graphicRenderer).sprite;
            graphicType = GraphicType.SPRITERENDERER;
        } else if(GetComponent<Image>() != null)
        {
            graphicRenderer = GetComponent<Image>();
            defaultSprite = ((Image)graphicRenderer).sprite;
            graphicType = GraphicType.IMAGE;
        }
    }
    public void ToggleSprite()
    {
        drawAltSprite = !drawAltSprite;
        switch (graphicType)
        {
            case GraphicType.SPRITERENDERER:
                ((SpriteRenderer)graphicRenderer).sprite = (drawAltSprite) ? altSprite : defaultSprite;
                break;
            case GraphicType.IMAGE:
                ((Image)graphicRenderer).sprite = (drawAltSprite) ? altSprite : defaultSprite;
                break;
        }
    }
}
