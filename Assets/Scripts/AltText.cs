using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AltText : MonoBehaviour {

    // Use this for initialization
    private float timer = 0;
    public string altText;
    private string defaultText;
    private Text text;
	void Start () {
        text = GetComponent<Text>();
        defaultText = text.text;
	}
	
	// Update is called once per frame
	void Update () {
		if(timer > 0)
        {
            timer -= Time.deltaTime;
        } else
        {
            text.text = defaultText;
        }
	}
    public void ChangeTextForDuration(float x)
    {
        timer = x;
        text.text = altText;
    }
}
