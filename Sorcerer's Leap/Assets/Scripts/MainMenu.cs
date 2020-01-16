using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject menu;
    public GameObject options;
    public GameObject playMarker;
    public GameObject optionMarker;
    public GameObject quitMarker;

    float selectionConfirmationTimerLength = 3f;
    float selectionConfirmationTimer = 3f;
    public Image ConfirmationUI;

    public GameObject PlayButtonHighlight;
    public GameObject OptionsButtonHighlight;
    public GameObject QuitButtonHighlight;

    bool isPointingAtPlayButton;
    bool isPointingAtOptionsButton;
    bool isPointingAtQuitButton;


    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    // Start is called before the first frame update
    void Start()
    {
        actions.Add("Play", Play);
        actions.Add("Options", Options);
        actions.Add("Quit", Quit);
        actions.Add("Back", Back);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += VoiceInput;
        keywordRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        LeapMotionPointingAtObject();
    }
    private void LeapMotionPointingAtObject()
    {
        selectionConfirmationTimer -= Time.deltaTime;
        ConfirmationUI.fillAmount = 1 - (selectionConfirmationTimer / selectionConfirmationTimerLength);
        if (isPointingAtPlayButton)
        {
            PlayButtonHighlight.SetActive(true);
            OptionsButtonHighlight.SetActive(false);
            QuitButtonHighlight.SetActive(false);
            if (selectionConfirmationTimer <= 0)
            {
                selectionConfirmationTimer = selectionConfirmationTimerLength;
                SceneManager.LoadScene("TutorialCourtyard");
            }
        }
        else if (isPointingAtOptionsButton)
        {
            PlayButtonHighlight.SetActive(false);
            OptionsButtonHighlight.SetActive(true);
            QuitButtonHighlight.SetActive(false);
        }
        else if (isPointingAtQuitButton)
        {
            PlayButtonHighlight.SetActive(false);
            OptionsButtonHighlight.SetActive(false);
            QuitButtonHighlight.SetActive(true);
        }
        else if (!isPointingAtPlayButton & !isPointingAtOptionsButton & !isPointingAtQuitButton)
        {
            PlayButtonHighlight.SetActive(false);
            OptionsButtonHighlight.SetActive(false);
            QuitButtonHighlight.SetActive(false);
            selectionConfirmationTimer = selectionConfirmationTimerLength;
        }
        else
        {
            selectionConfirmationTimer = selectionConfirmationTimerLength;
        }
    }
    private void VoiceInput(PhraseRecognizedEventArgs speech) {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    public void Play() {
        playMarker.SetActive(true);
        SceneManager.LoadScene("TutorialCourtyard");
    }

    public void Quit() {
        quitMarker.SetActive(true);
        Application.Quit();
    }

    public void Options() {
        optionMarker.SetActive(true);
        menu.SetActive(false);
        options.SetActive(true);
        optionMarker.SetActive(false);
    }

    public void Back() {
        options.SetActive(false);
        menu.SetActive(true);
    }
    public void PointingAtPlayButtonIsActive()
    {
        isPointingAtPlayButton = true;
    }
    public void PointingAtPlayButtonIsDeactive()
    {
        isPointingAtPlayButton = false;
    }
    public void PointingAtOptionButtonIsActive()
    {
        isPointingAtOptionsButton = true;
    }
    public void PointingAtOptionButtonIsDeactive()
    {
        isPointingAtOptionsButton = false;
    }
    public void PointingAtQuitButtonIsActive()
    {
        isPointingAtQuitButton = true;
    }
    public void PointingAtQuitButtonIsDeactive()
    {
        isPointingAtQuitButton = false;
    }

    void OnDestroy(){
		if(keywordRecognizer != null){
			keywordRecognizer.Stop();
			keywordRecognizer.Dispose();
		}
	}
}
