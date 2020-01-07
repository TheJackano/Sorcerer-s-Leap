using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class ArenaController : MonoBehaviour
{
	public Transform shopCam = null;
	public GameObject camera;
	public Animator camAnimation;

	private KeywordRecognizer keywordRecognizer;
	private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    // Start is called before the first frame update
    void Start()
    {
		actions.Add("Shop", Shop);
		actions.Add("Checkout", Checkout);

		keywordRecognizer = new KeywordRecognizer (actions.Keys.ToArray());
		keywordRecognizer.OnPhraseRecognized += VoiceInput;
		keywordRecognizer.Start();

		camAnimation = camera.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void VoiceInput(PhraseRecognizedEventArgs speech){
		Debug.Log(speech.text);
		actions[speech.text].Invoke();
	}

	public void Shop() {
		camAnimation.Play("Shop");
		
}
		
	public void Checkout(){
		camAnimation.Play("BackToFight");
	}

}
