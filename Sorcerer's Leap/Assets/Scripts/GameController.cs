using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    Enemy[] Enemies = new Enemy[]
        {
            new Enemy("Water"),
            new Enemy("Earth"),
            new Enemy("Fire"),
        };
    string[] Element = {"Fire","Earth","Water","Metal","Wood"};
    public GameObject[] objEnemies;
    public GameObject Player;
    public Sprite[] imgElements;
    public Image PlayerHealthBar;
    public bool pauseActive;

	private KeywordRecognizer keywordRecognizer;
	private Dictionary<string, Action> actions = new Dictionary<string, Action>();
	public GameObject pause;

    public Image ElementTargetConfirmation;

    string strPhase = "";
    string strSelectedElement = "";
    string strSelectedTarget = "";

    string strOldElement = "";
    string strOldTarget = "";
    float selectionConfirmationTimerLength = 3f;
    float selectionConfirmationTimer = 3f;

    float PlayerHealth = 100f;

    bool AIAssignedElement = false;
    bool CombatComplete = false;

    float AttackDamage = 10f;
    float CritChance = 0f;
    float CritDamage = 1.5f;
    float Defence = 0f;

	public int tutorial = 0;
	public bool tutorialComplete = false;

	public GameObject tutorial1;
	public GameObject tutorial2;
	public GameObject tutorial3;
	public GameObject tutorial4;

    //Leap Motion Variables
    //Right Hand
    bool isPointingOnlyWithIndexFinger;
    bool isPointingIndexFingerAtLeftEnemy;
    bool isPointingIndexFingerAtMiddleEnemy;
    bool isPointingIndexFingerAtRightEnemy;
    bool isRightHandOpen;

    //Left Hand
    bool isLeftHandClosed;
    bool isLeftHandOpen;
    bool isLeftHandPalmPointingDown;
    bool isLeftHandPalmPointingRight;
    bool isLeftHandPalmPointingUp;
    public GameObject End;

    void Start()
    {
        StartNewRound();

		actions.Add("fire", Fire);
		actions.Add("water", Water);
		actions.Add("earth", Earth);
		actions.Add("wood", Wood);
		actions.Add("metal", Metal);
		actions.Add("left", Left);
		actions.Add("right", Right);
		actions.Add("middle", Middle);
		actions.Add("pause", Pause);
		actions.Add("resume", Resume);
		actions.Add("restart", Restart);
		actions.Add("main menu", Main);
		actions.Add("Pause the Game", Pause);
		actions.Add("Stop the game", Pause);
		actions.Add("I'm not having fun", Pause);
		actions.Add("Stop", Pause);
		actions.Add("I'm tired", Pause);
		actions.Add("Hold on", Pause);
		actions.Add("Resume", Resume);
		actions.Add("Resume the game", Resume);
		actions.Add("Back to game", Resume);
		actions.Add("Restart the Level", Restart);
		actions.Add("Start over", Restart);
		actions.Add("Next", Next);
		actions.Add("Okay", Next);
		actions.Add("Next one", Next);
		actions.Add("Help", Help);
		actions.Add("I don't know what to do", Help);
		actions.Add("I choose fire", Fire);
		actions.Add("Cast fire", Fire);
		actions.Add("Cast water", Water);
		actions.Add("I choose water", Water);
		actions.Add("Cast earth", Earth);
		actions.Add("I choose earth", Earth);
		actions.Add("Cast wood", Wood);
		actions.Add("I choose wood", Wood);
		actions.Add("Cast metal", Metal);
		actions.Add("I choose metal", Metal);
		actions.Add("On the left guy", Left);
		actions.Add("Attack the guy on the left", Left);
		actions.Add("On the right guy", Right);
		actions.Add("Attack the guy on the right", Right);
		actions.Add("On the middle guy", Middle);
		actions.Add("Attack the guy in middle", Middle);
		actions.Add("Attack everyone", All);
		actions.Add("On all of them", All);
		actions.Add("All", All);
		actions.Add("Do an area attack", All);
		actions.Add("Area attack", All);
		actions.Add("Everyone", All);
		actions.Add("Target everyone", All);
		actions.Add("Next round", StartNewRound);
		actions.Add("Next", StartNewRound);
		actions.Add("What now", StartNewRound);
		actions.Add("Retry", Restart);
		actions.Add("Try again", Restart);
		actions.Add("Play", Arena);
		actions.Add("Play game", Arena);
		actions.Add("Go to Arena", Arena);

		keywordRecognizer = new KeywordRecognizer (actions.Keys.ToArray());
		keywordRecognizer.OnPhraseRecognized += VoiceInput;
		keywordRecognizer.Start();
    }
    void Update()
    {
        switch (strPhase)
        {
            case "Selection":
                LeapMotionSelectedElement();
                GUISelectedElement();
                LeapMotionSelectedTarget();
                GUISelectedTargets();
                SelectionConfirmationTimer();
                //if (Input.GetKey(KeyCode.Space) && strSelectedElement != "" && strSelectedTarget !="") strPhase = "Combat";
                break;
            case "Combat":
                if (!AIAssignedElement) RandomAIElement();
                if (!CombatComplete) CalculateCombat();
                if (Input.GetKey(KeyCode.R)) StartNewRound();
                break;
            case "Shopping":
                break;
        }
    }
    private void SelectionConfirmationTimer()
    {   
        if(strSelectedElement == strOldElement && strSelectedTarget == strOldTarget && strSelectedElement != "" && strSelectedTarget != "") selectionConfirmationTimer -= Time.deltaTime;
        else selectionConfirmationTimer = selectionConfirmationTimerLength;
        strOldElement = strSelectedElement;
        strOldTarget = strSelectedTarget;
        ElementTargetConfirmation.fillAmount = 1 - (selectionConfirmationTimer / selectionConfirmationTimerLength);
        if (selectionConfirmationTimer <= 0) strPhase = "Combat";
    }
    private void LeapMotionSelectedElement()
    {
        if (isLeftHandOpen)
        {
            if (isLeftHandPalmPointingDown && !isLeftHandPalmPointingRight && !isLeftHandPalmPointingUp) strSelectedElement = "Water";
            if (!isLeftHandPalmPointingDown && isLeftHandPalmPointingRight && !isLeftHandPalmPointingUp) strSelectedElement = "Wood";
            if (!isLeftHandPalmPointingDown && !isLeftHandPalmPointingRight && isLeftHandPalmPointingUp) strSelectedElement = "Fire";
        }
        if (isLeftHandClosed)
        {
            if (isLeftHandPalmPointingDown && !isLeftHandPalmPointingRight && !isLeftHandPalmPointingUp) strSelectedElement = "Earth";
            if (!isLeftHandPalmPointingDown && !isLeftHandPalmPointingRight && isLeftHandPalmPointingUp) strSelectedElement = "Metal";
        }
    }
    private void GUISelectedElement()
    {
        if (strSelectedElement == "Fire") Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[0];
        if (strSelectedElement == "Earth") Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[1];
        if (strSelectedElement == "Water") Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[2];
        if (strSelectedElement == "Metal") Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[3];
        if (strSelectedElement == "Wood") Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[4];
    } //Updates the UI element infront of the enemy
    private void LeapMotionSelectedTarget()
    {
        if (isPointingOnlyWithIndexFinger)
        {
            if (isPointingIndexFingerAtLeftEnemy) strSelectedTarget = "Left";
            if (isPointingIndexFingerAtMiddleEnemy) strSelectedTarget = "Middle";
            if (isPointingIndexFingerAtRightEnemy) strSelectedTarget = "Right";
        }
        if (isRightHandOpen) strSelectedTarget = "All";
    }
    private void GUISelectedTargets()
    {
        switch (strSelectedTarget)
        {
            case "Left":
                objEnemies[0].transform.Find("Canvas").Find("Target").gameObject.SetActive(true);
                objEnemies[1].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
                objEnemies[2].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
                break;
            case "Middle":
                objEnemies[0].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
                objEnemies[1].transform.Find("Canvas").Find("Target").gameObject.SetActive(true);
                objEnemies[2].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
                break;
            case "Right":
                objEnemies[0].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
                objEnemies[1].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
                objEnemies[2].transform.Find("Canvas").Find("Target").gameObject.SetActive(true);
                break;
            case "All":
                objEnemies[0].transform.Find("Canvas").Find("Target").gameObject.SetActive(true);
                objEnemies[1].transform.Find("Canvas").Find("Target").gameObject.SetActive(true);
                objEnemies[2].transform.Find("Canvas").Find("Target").gameObject.SetActive(true);
                break;
        }
    } //Allows the User to Select a Target
    private void CalculateCombat()
    {
        float CalculatedDamage;
        if (strSelectedTarget == "All")
        {
            for (int i = 0; i < objEnemies.Length; i++)
            {
                bool CritBool = HasCritBool();
                string result = CompareTwoElements(strSelectedElement, Enemies[i].SelectedSpell);
                if (result == "Win")
                {
                    if (CritBool) CalculatedDamage = (AttackDamage * CritDamage) - Enemies[i].Defence;
                    else CalculatedDamage = AttackDamage - Enemies[i].Defence;
                    if (CalculatedDamage < 0) CalculatedDamage = 0;
                    Enemies[i].Health -= CalculatedDamage/3;
                }

                else if (result == "Lose")
                {
                    CalculatedDamage = Enemies[i].AttackDamage - Defence;
                    if (CalculatedDamage < 0) CalculatedDamage = 0;
                    PlayerHealth -= CalculatedDamage/3;
                }
            }
        }
        else if(strSelectedTarget == "Left")
        {
            bool CritBool = HasCritBool();
            string result = CompareTwoElements(strSelectedElement, Enemies[0].SelectedSpell);
            if (result == "Win")
            {
                if (CritBool) CalculatedDamage = (AttackDamage * CritDamage) - Enemies[0].Defence;
                else CalculatedDamage = AttackDamage - Enemies[0].Defence;
                if (CalculatedDamage < 0) CalculatedDamage = 0;
                Enemies[0].Health -= CalculatedDamage;
            }
            else if (result == "Lose")
            {
                CalculatedDamage = Enemies[0].AttackDamage - Defence;
                if (CalculatedDamage < 0) CalculatedDamage = 0;
                PlayerHealth -= CalculatedDamage;
            }
        }
        else if (strSelectedTarget == "Middle")
        {
            bool CritBool = HasCritBool();
            string result = CompareTwoElements(strSelectedElement, Enemies[1].SelectedSpell);
            if (result == "Win")
            {
                if (CritBool) CalculatedDamage = (AttackDamage * CritDamage) - Enemies[1].Defence;
                else CalculatedDamage = AttackDamage - Enemies[1].Defence;
                if (CalculatedDamage < 0) CalculatedDamage = 0;
                Enemies[1].Health -= CalculatedDamage;
            }

            else if (result == "Lose")
            {
                CalculatedDamage = Enemies[1].AttackDamage - Defence;
                if (CalculatedDamage < 0) CalculatedDamage = 0;
                PlayerHealth -= CalculatedDamage;
            }
        }
        else if (strSelectedTarget == "Right")
        {
            bool CritBool = HasCritBool();
            string result = CompareTwoElements(strSelectedElement, Enemies[2].SelectedSpell);
            if (result == "Win")
            {
                if (CritBool) CalculatedDamage = (AttackDamage * CritDamage) - Enemies[2].Defence;
                else CalculatedDamage = AttackDamage - Enemies[2].Defence;
                if (CalculatedDamage < 0) CalculatedDamage = 0;
                Enemies[2].Health -= CalculatedDamage;
            }

            else if (result == "Lose")
            {
                CalculatedDamage = Enemies[2].AttackDamage - Defence;
                if (CalculatedDamage < 0) CalculatedDamage = 0;
                PlayerHealth -= CalculatedDamage;
            }
        }
        PlayerHealthBar.GetComponent<Image>().fillAmount = PlayerHealth/100;
        for (int i = 0; i < objEnemies.Length; i++)
        {
            objEnemies[i].transform.Find("Canvas").Find("HealthFill").gameObject.GetComponent<Image>().fillAmount = Enemies[i].Health / 100;
            if (Enemies[i].Health <= 0) Enemies[i].Alive = false;
        }
        if (Enemies[0].Health <= 0 && Enemies[1].Health <= 0 && Enemies[2].Health <= 0) End.SetActive(true);
        if (PlayerHealth <= 0) End.SetActive(true);
        CombatComplete = true;
    }
    private void StartNewRound()
    {
        {
            objEnemies[0].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
            objEnemies[1].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
            objEnemies[2].transform.Find("Canvas").Find("Target").gameObject.SetActive(false);
        }//Deselects Enemy Targets Visually
        {
            Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[5];
            objEnemies[0].transform.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[5];
            objEnemies[1].transform.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[5];
            objEnemies[2].transform.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[5];
        }//Sets all the UI Elements Selections to ? image
        strSelectedElement = "";
        strSelectedTarget = "";
        for (int i = 0; i < objEnemies.Length; i++) Enemies[i].SelectedSpell = "";
        AIAssignedElement = false;
        CombatComplete = false;
        strPhase = "Selection";
    }
    private string CompareTwoElements(string PlayerElement, string EnemyElement)
    {
        string result = "";
        if (PlayerElement == "Fire")
        {
            if (EnemyElement == "Fire") result = "Draw";
            if (EnemyElement == "Earth") result = "Draw";
            if (EnemyElement == "Water") result = "Lose";
            if (EnemyElement == "Metal") result = "Win";
            if (EnemyElement == "Wood") result = "Draw";
        }
        else if (PlayerElement == "Earth")
        {
            if (EnemyElement == "Fire") result = "Draw";
            if (EnemyElement == "Earth") result = "Draw";
            if (EnemyElement == "Water") result = "Win";
            if (EnemyElement == "Metal") result = "Draw";
            if (EnemyElement == "Wood") result = "Lose";
        }
        else if (PlayerElement == "Water")
        {
            if (EnemyElement == "Fire") result = "Win";
            if (EnemyElement == "Earth") result = "Lose";
            if (EnemyElement == "Water") result = "Draw";
            if (EnemyElement == "Metal") result = "Draw";
            if (EnemyElement == "Wood") result = "Draw";
        }
        else if (PlayerElement == "Metal")
        {
            if (EnemyElement == "Fire") result = "Lose";
            if (EnemyElement == "Earth") result = "Draw";
            if (EnemyElement == "Water") result = "Draw";
            if (EnemyElement == "Metal") result = "Draw";
            if (EnemyElement == "Wood") result = "Win";
        }
        else if (PlayerElement == "Wood")
        {
            if (EnemyElement == "Fire") result = "Draw";
            if (EnemyElement == "Earth") result = "Win";
            if (EnemyElement == "Water") result = "Draw";
            if (EnemyElement == "Metal") result = "Lose";
            if (EnemyElement == "Wood") result= "Draw";
        }
        return result;
    }
    private bool HasCritBool()
    {
        int RandomNumber = UnityEngine.Random.Range(0, 99);
        if (RandomNumber <= CritChance-1) return true;
        else return false;
    }
    private void RandomAIElement()
    {
        for(int i = 0; i < objEnemies.Length; i++)
        {
            int RandomNumber = UnityEngine.Random.Range(0,99);
            switch (RandomNumber)//"Fire","Earth","Water","Metal","Wood"
            {
                case int n when n >=0 && n<=9://Fire
                    Enemies[i].SelectedSpell = "Fire";
                    break;
                case int n when n >= 10 && n <= 19://Earth
                    Enemies[i].SelectedSpell = "Earth";
                    break;
                case int n when n >= 20 && n <= 29://Water
                    Enemies[i].SelectedSpell = "Water";
                    break;
                case int n when n >= 30 && n <= 39://Metal
                    Enemies[i].SelectedSpell = "Metal";
                    break;
                case int n when n >= 40 && n <= 49://Wood
                    Enemies[i].SelectedSpell = "Wood";
                    break;
                case int n when n >= 50 && n <= 99://Affinity
                    Enemies[i].SelectedSpell = Enemies[i].WizzardType;
                    //Debug.Log(Enemies[i].WizzardType + "Special");
                    break;
            }
            for(int k = 0; k < Element.Length; k++)
            {
                if (Enemies[i].SelectedSpell == Element[k]) objEnemies[i].transform.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[k];
            }
            //Debug.Log(Enemies[i].WizzardType + Enemies[i].SelectedSpell);
        }
        AIAssignedElement = true; 
    } //Selectes a random element to each AI
    public void PointingOnlyWithIndexFingerIsActive()
    { 
        isPointingOnlyWithIndexFinger = true;
        //Debug.Log("Pointing Index Finger");
    }
    public void PointingOnlyWithIndexFingerIsDeactive()
    {
        isPointingOnlyWithIndexFinger = false;
        //Debug.Log("No Longer Pointing Index Finger");
    }
    public void PointingIndexFingerTowardsLeftEnemyIsActive()
    {
        isPointingIndexFingerAtLeftEnemy = true;
        //Debug.Log("Index is pointing at the Left Enemy");
    }
    public void PointingIndexFingerTowardsLeftEnemyIsDeactive()
    {
        isPointingIndexFingerAtLeftEnemy = false;
        //Debug.Log("Index is no longer pointing at the Left Enemy");
    }
    public void PointingIndexFingerTowardsMiddleEnemyIsActive()
    {
        isPointingIndexFingerAtMiddleEnemy = true;
        //Debug.Log("Index is pointing at the Middle Enemy");
    }
    public void PointingIndexFingerTowardsMiddleEnemyIsDeactive()
    {
        isPointingIndexFingerAtMiddleEnemy = false;
        //Debug.Log("Index is no longer pointing at the Middle Enemy");
    }
    public void PointingIndexFingerTowardsRightEnemyIsActive()
    {
        isPointingIndexFingerAtRightEnemy = true;
        //Debug.Log("Index is pointing at the Right Enemy");
    }
    public void PointingIndexFingerTowardsRightEnemyIsDeactive()
    {
        isPointingIndexFingerAtRightEnemy = false;
        //Debug.Log("Index is no longer pointing at the Right Enemy");
    }
    public void RightHandOpenIsActive()
    {
        isRightHandOpen = true;
        //Debug.Log("Right Hand Is Targeting All Enemies");
    }
    public void RightHandOpenIsDeactive()
    {
        isRightHandOpen = false;
        //Debug.Log("Right Hand Is No Longer Targeting All Enemies");
    }
    public void LeftHandClosedIsActive()
    {
        isLeftHandClosed = true;
        //Debug.Log("Left Hand Is Closed");
    }
    public void LeftHandClosedIsDeactive()
    {
        isLeftHandClosed = false;
        //Debug.Log("Left Hand Is No Longer Closed");
    }
    public void LeftHandOpenIsActive()
    {
        isLeftHandOpen = true;
       // Debug.Log("Left Hand Is Open");
    }
    public void LeftHandOpenIsDeactive()
    {
        isLeftHandOpen = false;
        //Debug.Log("Left Hand Is No Longer Open");
    }
    public void LeftHandPalmPointingDownIsActive()
    {
        isLeftHandPalmPointingDown = true;
        //Debug.Log("Palm is Pointing Down");
    }
    public void LeftHandPalmPointingDownIsDeactive()
    {
        isLeftHandPalmPointingDown = false;
        //Debug.Log("Palm is Not Pointing Down");
    }
    public void LeftHandPalmPointingRightIsActive()
    {
        isLeftHandPalmPointingRight = true;
        //Debug.Log("Palm is Pointing Right");
    }
    public void LeftHandPalmPointingRightIsDeactive()
    {
        isLeftHandPalmPointingRight = false;
        //Debug.Log("Palm is Not Pointing Right");
    }
    public void LeftHandPalmPointingUpIsActive()
    {
        isLeftHandPalmPointingUp = true;
        //Debug.Log("Palm is Pointing Up");
    }
    public void LeftHandPalmPointingUptIsDeactive()
    {
        isLeftHandPalmPointingUp = false;
        //Debug.Log("Palm is Not Pointing Up");
    }
    private void VoiceInput(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }
    private void Fire()
    {
        strSelectedElement = "Fire";
    }
    private void Water()
    {
        strSelectedElement = "Water";
    }
    private void Earth()
    {
        strSelectedElement = "Earth";
    }
    private void Wood()
    {
        strSelectedElement = "Wood";
    }
    private void Metal()
    {
        strSelectedElement = "Metal";
    }
    private void Left()
    {
        strSelectedTarget = "Left";
    }
    private void Right()
    {
        strSelectedTarget = "Right";
    }
    private void Middle()
    {
        strSelectedTarget = "Middle";
    }
    private void Pause()
    {
        pause.SetActive(true);
        pauseActive = true;
        Time.timeScale = 0;
    }
    private void Resume()
    {
        if (pauseActive == true)
        {
            pause.SetActive(false);
            pauseActive = false;
            Time.timeScale = 1;
        }
    }
    private void Restart()
    {
        if (pauseActive == true)
        {
            SceneManager.LoadScene("TutorialCourtyard");
            pauseActive = false;
            Time.timeScale = 1;
        }
    }
    private void Main()
    {
        if (pauseActive == true)
        {
            SceneManager.LoadScene("MainMenu");
            pauseActive = false;
            Time.timeScale = 1;
        }
    }
    void OnDestroy()
    {
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }

	public void Next(){
		tutorial++;
		if(tutorial == 0){
			tutorial1.SetActive(true);
		}
		if(tutorial == 1){
			tutorial2.SetActive(true);
			tutorial1.SetActive(false);
		}
		if(tutorial == 2){
			tutorial3.SetActive(true);
			tutorial2.SetActive(false);
		}
		if(tutorial == 3){
			tutorial4.SetActive(true);
			tutorial3.SetActive(false);
		}
		if(tutorial >= 4){
			tutorial4.SetActive(false);
		}
	}

	private void Help(){
		tutorial4.SetActive(true);

	}

	public void All(){
		strSelectedTarget = "All";
	}

	public void Arena(){
		if(tutorialComplete == true){
		SceneManager.LoadScene("MainArena");
		}
	}
}
    public class Enemy
{
    public float Health = 100f;
    public bool Alive = true;
    public string WizzardType;
    public string SelectedSpell;
    public float AttackDamage = 10f;
    public float Defence = 0f;

    public Enemy(string _WizzardType)
    {
        WizzardType = _WizzardType;
    }
}

