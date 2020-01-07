using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public class MainMenu : MonoBehaviour
{

    public GameObject menu;
    public GameObject options;
    public GameObject playMarker;
    public GameObject optionMarker;
    public GameObject quitMarker;

	private KeywordRecognizer keywordRecognizer;
	private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    // Start is called before the first frame update
    void Start()
    {
		actions.Add("Play", Play);
		actions.Add("Options", Options);
		actions.Add("Quit", Quit);
		actions.Add("Back", Back);

		keywordRecognizer = new KeywordRecognizer (actions.Keys.ToArray());
		keywordRecognizer.OnPhraseRecognized += VoiceInput;
		keywordRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void VoiceInput(PhraseRecognizedEventArgs speech){
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

	void OnDestroy(){
		if(keywordRecognizer != null){
			keywordRecognizer.Stop();
			keywordRecognizer.Dispose();
		}
	}
}
