using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
public class DictationRecogniser : MonoBehaviour
{
	[SerializeField]
	private Text m_Hyphothesis;

	[SerializeField]
	private Text m_recognitions;

	private DictationRecognizer m_DictationRecognizer;

	public string[] SpellKeywords;
	public string[] TargetKeywords;
	public string[] MenuKeywords;
	public string[] ShopKeywords;

	private string activeKeyword;

	public GameObject cube;

	// Start is called before the first frame update
	void Start()
	{
		m_DictationRecognizer = new DictationRecognizer();

		m_DictationRecognizer.DictationResult += (text, confidence) =>
		{
			Debug.LogFormat("Dictation result: {0}", text);
			m_recognitions.text += text + "/n";
			Keywords(m_recognitions.text);
		};

		m_DictationRecognizer.DictationHypothesis += (text) =>
		{
			Debug.LogFormat("Dictation hypothesis: {0}", text);
			m_recognitions.text += text + "/n";
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
		foreach (string stringToSearch in SpellKeywords)
		{
			int KeywordCheck = sentenceSpoken.IndexOf(stringToSearch);
			print(KeywordCheck);

			if (KeywordCheck != -1) {
				activeKeyword = stringToSearch;
				print(activeKeyword);
			}
		}


		if (activeKeyword == "On")
		{
			cube.SetActive(true);
		}

		if (activeKeyword == "Off") {
			cube.SetActive(false);
		}

		if (activeKeyword == "Back") {
			Debug.Log("Options closed");
		}
	}
}