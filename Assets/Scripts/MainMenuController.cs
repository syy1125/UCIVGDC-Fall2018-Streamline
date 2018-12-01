using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour {

    // Use this for initialization
    public string levelSelectScene;
    public string creditsScene;
    private AudioSource Source;
    public AudioClip SelectSound;
    public AudioClip ClickSound;
    int soundsPlayed = 0;
	void Awake () {
        Source = GetComponent<AudioSource>();
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
    public void PlaySelectSound()
    {
        if(soundsPlayed++ > 0) //Squelch first sound played (when default button is selected by eventsystem)
            Source.PlayOneShot(SelectSound);
    }
    public void PlayPressSound()
    {
        Source.PlayOneShot(ClickSound);
    }
    private IEnumerator TransitionAndLoad(string sceneName)
    {
        ColorLerp c = GameObject.FindGameObjectWithTag("Transition").GetComponent<ColorLerp>();
        c.SetActivated(true);
        yield return new WaitForSeconds(c.ChangeDuration);
        SceneManager.LoadScene(sceneName);
    }
}
