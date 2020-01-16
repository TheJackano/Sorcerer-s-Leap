using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private Text m_Hyphothesis;

	[SerializeField]
	private Text m_recognitions;

	private DictationRecognizer m_DictationRecognizer;

	public string[] MenuKeywords;

	private string activeKeyword;

    public GameObject menu;
    public GameObject options;
    public GameObject playMarker;
    public GameObject optionMarker;
    public GameObject quitMarker;


    // Start is called before the first frame update
    void Start()
    {
		m_DictationRecognizer = new DictationRecognizer();

		m_DictationRecognizer.DictationResult += (text, confidence) =>
		{
			Debug.LogFormat("Dictation result: {0}", text);
			m_recognitions.text += text + "";
			Keywords(m_recognitions.text);
		};

		m_DictationRecognizer.DictationHypothesis += (text) =>
		{
			Debug.LogFormat("Dictation hypothesis: {0}", text);
			m_recognitions.text += text + "";
		};

		m_DictationRecognizer.DictationComplete += (completionCause) => {
			if (completionCause != DictationCompletionCause.Complete)
				Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}", completionCause);

		};

		m_DictationRecognizer.DictationError += (error, hresult) =>
		{
			Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}", error, hresult);
		};

		m_DictationRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Keywords(string sentenceSpoken) {
		foreach (string stringToSearch in MenuKeywords)
		{
			int KeywordCheck = sentenceSpoken.IndexOf(stringToSearch);
			print(KeywordCheck);

			if (KeywordCheck != -1) {
				activeKeyword = stringToSearch;
				print(activeKeyword);
			}
		}
			
		if (activeKeyword == "play")
		{
			playMarker.SetActive(true);
			SceneManager.LoadScene("TutorialCourtyard");
		}

		if (activeKeyword == "options") {
			optionMarker.SetActive(true);
			menu.SetActive(false);
			options.SetActive(true);
			optionMarker.SetActive(false);
		}

		if (activeKeyword == "back") {
			options.SetActive(false);
			menu.SetActive(true);
		}

		if(activeKeyword == "quit"){
			quitMarker.SetActive(true);
			Application.Quit();
		}
	}

	void OnDestroy(){
		if(m_DictationRecognizer != null){
			m_DictationRecognizer.Stop();
			m_DictationRecognizer.Dispose();
		}
	}
}
