using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestCompletionDisplay : MonoBehaviour {

    // Use this for initialization
    private Text MyText;
    private int TestsComplete;
	void Awake () {
        MyText = GetComponent<Text>();
        
	}
	
	// Update is called once per frame
	void Update () {
        TestsComplete = 0;
        for(int i = 0; i < GameController.TestCaseCompletion.Length; i++)
        {
            if (GameController.TestCaseCompletion[i])
                TestsComplete++;
        }
        MyText.text =
            TestsComplete.ToString()
            + "/"
            + GameController.gameLevel.Tests.Length 
            + " Tests Complete";
    }
}
