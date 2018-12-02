using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButton : MonoBehaviour {

    // Use this for initialization
    Tutorial[] Tutorials;
    int ActiveTutorial;
	void Start () {
        Tutorials = GameObject.FindGameObjectWithTag("Tutorials").GetComponentsInChildren<Tutorial>();
        for(int i = 0; i < Tutorials.Length; i++)
        {
            if (Tutorials[i].gameObject.activeInHierarchy)
            {
                ActiveTutorial = i;
                return;

            }
        }
        Destroy(gameObject);
	}
	
	public void SkipTutorial()
    {
        Tutorials[ActiveTutorial].MarkAsComplete();
        Tutorials[ActiveTutorial].transform.parent.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
