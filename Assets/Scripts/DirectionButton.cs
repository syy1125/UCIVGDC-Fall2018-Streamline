using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionButton : MonoBehaviour {

    // Use this for initialization
    private Image image;
    public Sprite disabledSprite;
    public Sprite enabledSprite;
    public bool activated = false;
	void Awake () {
        image = GetComponent<Image>();
        if (activated)
        {
            image.sprite = enabledSprite;
        }
        else
        {
            image.sprite = disabledSprite;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (activated)
        {
            image.sprite = enabledSprite;
        } else
        {
            image.sprite = disabledSprite;
        }
	}
    public void SetRotation(float degrees)
    {
        transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, degrees));
    }
    public void SetActivated(bool b)
    {
        activated = b;
    }
}
