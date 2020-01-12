using System;
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
	public bool health;
	public bool attack;
	public bool criticalChance;
	public bool defence;
	public bool pendant;

	public float PlayerHealth = 100f;
	public float AttackDamage = 10f;
	public float CritChance = 0f;
	public float CritDamage = 1.5f;
	public float DefenceStat = 0f;
	public float moneyMultiplyer = 1f;

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
		actions.Add("Health", Health);
		actions.Add("Attack", Attack);
		actions.Add("Defence", Defence);
		actions.Add("Crit Chance", Crit);
		actions.Add("Pendant", Pendant);

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
			if(buying == true){
				if(health == true){
					PlayerHealth = 100f;
				}
				if(defence == true){
					DefenceStat = DefenceStat + 1;
				}
				if(criticalChance == true){
					CritChance = CritChance + 1;
				}
				if(attack == true){
					AttackDamage = AttackDamage + 1;
				}

				if(pendant == true){
					moneyMultiplyer = moneyMultiplyer + 1;
				}
					
			shopOpen = false;
			buying = false;
			health = false;
			defence = false;
			criticalChance = false;
			pendant = false;
			attack = false;
			}
			camAnimation.Play("BackToFight");
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

	public void Health(){
		if(buying == true){
			health = true;
		}
	}

	public void Defence(){
		if(buying == true){
			defence = true;
		}
	}

	public void Attack(){
		if(buying == true){
			attack = true;
		}
	}

	public void Crit(){
		if(buying == true){
			criticalChance = true;
		}
	}

	public void Pendant(){
		if(buying == true){
			pendant = true;
		}
	}

}
