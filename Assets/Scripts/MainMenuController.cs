﻿using System.Collections;
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
        StartCoroutine(TransitionAndLoad(levelSelectScene));
    }
    public void CreditsPress()
    {
        StartCoroutine(TransitionAndLoad(creditsScene));
    }
    public void QuitPress()
    {
        Application.Quit();
    }
    private IEnumerator TransitionAndLoad(string sceneName)
    {
        ColorLerp c = GameObject.FindGameObjectWithTag("Transition").GetComponent<ColorLerp>();
        c.SetActivated(true);
        yield return new WaitForSeconds(c.ChangeDuration);
        SceneManager.LoadScene(levelSelectScene);
    }
}
