using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExportErrors : MonoBehaviour {

	// Use this for initialization
	public ColArray leftColumn;
	public ColArray rightColumn;
	private List<Image> highlights;
	void Awake () {
		highlights = new List<Image>();
		foreach(Transform t in transform)
		{
			highlights.Add(t.GetComponent<Image>());
		}
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < highlights.Count; i++){
			CheckAndHandleMistakes(i);
		}
	}
	private void CheckAndHandleMistakes(int index)
	{
		if(index >= highlights.Count)
		{
			return;
		}
		if(index >= rightColumn.Length())
		{
			//Index is between right column length and highlights length
			//This is when right column values are null
			highlights[index].enabled = false;
			return;
		} else {
			highlights[index].enabled = (leftColumn.GetValue(index) != rightColumn.GetValue(index));
		}

	}

}
