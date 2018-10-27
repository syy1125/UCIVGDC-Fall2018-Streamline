using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColumnNames : MonoBehaviour {

    // Use this for initialization
    public string[] columnNames;
    private Text[] text;
    void Start () {
        text = new Text[columnNames.Length];
        int i = 0;
        foreach (Transform child in transform)
        {
            text[i] = (child.GetComponent<Text>());
            i++;
        }
        for(i = 0; i < columnNames.Length; i++)
        {
            SetName(i, columnNames[i]);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetName(int i, string s)
    {
        text[i].text = s;
    }
    public string GetName(int i){
        return text[i].text;
    }
}
