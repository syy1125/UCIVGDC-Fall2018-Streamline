using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour {

    // Use this for initialization
    public string levelSelectScene;
    public string creditsScene;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PlayPress()
    {
        SceneManager.LoadScene(levelSelectScene);
    }
    public void CreditsPress()
    {
        SceneManager.LoadScene(creditsScene);
    }
    public void QuitPress()
    {
        Application.Quit();
    }
}
