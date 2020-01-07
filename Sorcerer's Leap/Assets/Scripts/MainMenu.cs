using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject menu;
    public GameObject options;
    public GameObject playMarker;
    public GameObject optionMarker;
    public GameObject quitMarker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
