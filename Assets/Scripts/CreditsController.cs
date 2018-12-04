using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CreditsController : MonoBehaviour {

    // Use this for initialization
    public UIRevealer[] Names;
    public UIRevealer[] Roles;
    public float InitialDelay;
    public float RepeatDelay;
    public string MainMenuSceneName;
	void Awake () {
        StartCoroutine(RunCredits());
	}
	
	
    private IEnumerator RunCredits()
    {
        yield return new WaitForSeconds(InitialDelay);
        for(int i = 0; i < Names.Length; i++)
        {
            Names[i].revealUI();
            Roles[i].revealUI();
            yield return new WaitForSeconds(RepeatDelay);
        }
    }
    public void GoToMainMenu()
    {
        StartCoroutine(GameController.TransitionAndLoad(MainMenuSceneName));
    }
    
}
