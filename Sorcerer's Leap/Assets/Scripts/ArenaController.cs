﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

public class ArenaController : MonoBehaviour
{
	public Transform shopCam = null;
	public GameObject camera;
	public Animator camAnimation;
	public bool shopOpen;
	public GameObject pauseMenu;
	public bool pauseActive;
	public bool buying;

	private KeywordRecognizer keywordRecognizer;
	private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    // Start is called before the first frame update
    void Start()
    {
		actions.Add("Shop", Shop);
		actions.Add("Checkout", Checkout);
		actions.Add("Pause", Pause);
		actions.Add("Resume", Resume);
		actions.Add("Restart", Restart);
		actions.Add("Main Menu", Main);
		actions.Add("Buy", Buy);

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
		shopOpen = true;
		camAnimation.Play("Shop");
		
}
		
	public void Checkout(){
		if(shopOpen == true){
		camAnimation.Play("BackToFight");
			shopOpen = false;
		}
	}

	public void Pause(){
		pauseMenu.SetActive(true);
		pauseActive = true;
		Time.timeScale = 0;
	}

	public void Resume (){
		if(pauseActive == true){
		pauseMenu.SetActive(false);
		Time.timeScale = 1;
			pauseActive = false;
		}
	}

	public void Restart() {
		if(pauseActive == true){
		pauseMenu.SetActive(false);
		Time.timeScale = 1;
			pauseActive = false;
		SceneManager.LoadScene("MainArena");
		}
	}

	void OnDestroy(){
		if(keywordRecognizer != null){
			keywordRecognizer.Stop();
			keywordRecognizer.Dispose();
		}
	}

	public void Main(){
		if(pauseActive == true){
			pauseMenu.SetActive(false);
			Time.timeScale = 1;
			SceneManager.LoadScene("MainMenu");
		}
	}

	public void Buy(){
		if(shopOpen == true){
			buying = true;
		}
	}

}
