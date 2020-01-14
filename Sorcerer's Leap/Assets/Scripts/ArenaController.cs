using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public bool removing;

    Enemy[] Enemies = new Enemy[]
       {
            new Enemy("Metal"),
            new Enemy("Water"),
            new Enemy("Fire"),
       };
    string[] Element = { "Fire", "Earth", "Water", "Metal", "Wood" };
    public GameObject[] objEnemies;
    public GameObject Player;
    public Sprite[] imgElements;
    public Image PlayerHealthBar;

    public Image ElementTargetConfirmation;

    string strPhase = "";
    string strSelectedElement = "";
    string strSelectedTarget = "";

    string strOldElement = "";
    string strOldTarget = "";
    float selectionConfirmationTimerLength = 3f;
    float selectionConfirmationTimer = 3f;
    float shopConfirmationTimerLength = 3f;
    float shopConfirmationTimer = 3f;

    float PlayerHealth = 100f;

    bool AIAssignedElement = false;
    bool CombatComplete = false;

    float AttackDamage = 10f;
    float CritChance = 0f;
    float CritDamage = 1.5f;

    //Leap Motion Variables
    //Right Hand
    bool isPointingOnlyWithIndexFinger;
    bool isPointingIndexFingerAtLeftEnemy;
    bool isPointingIndexFingerAtMiddleEnemy;
    bool isPointingIndexFingerAtRightEnemy;
    bool isRightHandOpen;

    bool isPointingAtNextRoundButton, isPointingAtShopButton, isPointingAtHealthUpgrade, isPointingAtCritChanceUpgrade, isPointingAtPendantGoldUpgrade, isPointingAtDefenceUpgrade, isPointingAtAttackDamageUpgrade, isPointingAtCheckout;

    //Left Hand
    bool isLeftHandClosed;
    bool isLeftHandOpen;
    bool isLeftHandPalmPointingDown;
    bool isLeftHandPalmPointingRight;
    bool isLeftHandPalmPointingUp;

    public GameObject healthSelect;
    public GameObject attackSelect;
    public GameObject criticalSelect;
    public GameObject defenceSelect;
    public GameObject pendantSelect;
    public GameObject checkoutSelect;
    public GameObject newroundSelect;
    public GameObject shopSelect;

    public float DefenceStat = 0f;
    public int moneyMultiplyer = 1;

    public int gold = 0;
    public Text goldCount;

    public GameObject between;
    public bool roundEnd;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    // Start is called before the first frame update
    void Start()
    {
        StartNewRound();

        actions.Add("Shop", Shop);
		actions.Add("Go to Shop", Shop);
        actions.Add("Checkout", Checkout);
		actions.Add("Leave the Shop", Checkout);
        actions.Add("Pause", Pause);
		actions.Add("Pause the Game", Pause);
		actions.Add("Stop the game", Pause);
		actions.Add("I'm not having fun", Pause);
		actions.Add("I'm tired", Pause);
		actions.Add("Hold on", Pause);
        actions.Add("Resume", Resume);
		actions.Add("Resume the game", Resume);
		actions.Add("Back to game", Resume);
        actions.Add("Restart", Restart);
		actions.Add("Restart the Level", Restart);
		actions.Add("Start over", Restart);
        actions.Add("Main Menu", Main);
        actions.Add("Buy", Buy);
		actions.Add("I want to buy", Buy);
		actions.Add("I want", Buy);
        actions.Add("Health", Health);
		actions.Add("The red potion", Health);
        actions.Add("Attack", Attack);
		actions.Add("The sword", Attack);
        actions.Add("Defence", Defence);
		actions.Add("The shield", Defence);
        actions.Add("Crit Chance", Crit);
		actions.Add("The blue potion", Crit);
        actions.Add("Pendant", Pendant);
		actions.Add("The pendant", Pendant);
        actions.Add("Remove", Remove);
		actions.Add("Take off", Remove);
		actions.Add("I don't want", Remove);
		actions.Add("fire", Fire);
		actions.Add("I choose fire", Fire);
		actions.Add("Cast fire", Fire);
		actions.Add("water", Water);
		actions.Add("Cast water", Water);
		actions.Add("I choose water", Water);
		actions.Add("earth", Earth);
		actions.Add("Cast earth", Earth);
		actions.Add("I choose earth", Earth);
		actions.Add("wood", Wood);
		actions.Add("Cast wood", Wood);
		actions.Add("I choose wood", Wood);
		actions.Add("metal", Metal);
		actions.Add("Cast metal", Metal);
		actions.Add("I choose metal", Metal);
		actions.Add("left", Left);
		actions.Add("On the left guy", Left);
		actions.Add("Attack the guy on the left", Left);
		actions.Add("right", Right);
		actions.Add("On the right guy", Right);
		actions.Add("Attack the guy on the right", Right);
		actions.Add("middle", Middle);
		actions.Add("On the middle guy", Middle);
		actions.Add("Attack the guy in middle", Middle);
		actions.Add("Attack everyone", All);
		actions.Add("On all of them", All);
		actions.Add("All", All);
		actions.Add("Do an area attack", All);
		actions.Add("Area attack", All);
		actions.Add("Next", StartNewRound);
		actions.Add("Start the next round", StartNewRound);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += VoiceInput;
        keywordRecognizer.Start();

        camAnimation = camera.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        goldCount.text = "" + gold;

        switch (strPhase)
        {
            case "Selection":
                KeyboardSelect();
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
                if (Input.GetKey(KeyCode.N)) StartNewRound();
                LeapMotionPointingAtObject();
                break;
            case "Shopping":
                
                break;
        }
    }
    private void KeyboardSelect()
    {
        if (Input.GetKeyDown("q")) strSelectedElement = "Fire";
        if(Input.GetKeyDown("w")) strSelectedElement = "Water";
        if(Input.GetKeyDown("e")) strSelectedElement = "Earth";
        if(Input.GetKeyDown("r")) strSelectedElement = "Wood";
        if(Input.GetKeyDown("t")) strSelectedElement = "Metal";

        if(Input.GetKeyDown("a")) strSelectedTarget = "Left";
        if(Input.GetKeyDown("s")) strSelectedTarget = "Middle";
        if(Input.GetKeyDown("d")) strSelectedTarget = "Right";
        if(Input.GetKeyDown("f")) strSelectedTarget = "All";

    }
    private void VoiceInput(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    public void Shop()
    {
        if (roundEnd == true)
        {
            shopOpen = true;
            camAnimation.Play("Shop");
        }
    }

    public void Checkout()
    {
        if (shopOpen == true)
        {
            if (buying == true)
            {
                if (health == true)
                {
                    PlayerHealth = 100f;
					gold = gold - 100;
                }
                if (defence == true)
                {
                    DefenceStat = DefenceStat + 1;
					gold = gold - 200;
                }
                if (criticalChance == true)
                {
                    CritChance = CritChance + 1;
					gold = gold - 200;
                }
                if (attack == true)
                {
                    AttackDamage = AttackDamage + 1;
					gold = gold - 200;
                }

                if (pendant == true)
                {
                    moneyMultiplyer = moneyMultiplyer + 1;
					gold = gold - 100;
                }

                shopOpen = false;
                buying = false;
                health = false;
                defence = false;
                criticalChance = false;
                pendant = false;
                attack = false;
                removing = false;
                healthSelect.SetActive(false);
                attackSelect.SetActive(false);
                criticalSelect.SetActive(false);
                defenceSelect.SetActive(false);
                pendantSelect.SetActive(false);

            }
            camAnimation.Play("BackToFight");
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        pauseActive = true;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        if (pauseActive == true)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            pauseActive = false;
        }
    }

    public void Restart()
    {
        if (pauseActive == true)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            pauseActive = false;
            SceneManager.LoadScene("MainArena");
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

    public void Main()
    {
        if (pauseActive == true)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Buy()
    {
        if (shopOpen == true)
        {
            buying = true;
            removing = false;
        }
    }

    public void Remove()
    {
        if (shopOpen == true)
        {
            removing = true;
            buying = false;
        }
    }

    public void Health()
    {
        if (buying == true)
        {
			if(gold >= 100){
            health = true;
            healthSelect.SetActive(true);
			}
        }
        if (removing == true)
        {
            health = false;
            healthSelect.SetActive(false);
        }
    }

    public void Defence()
    {
        if (buying == true)
        {
			if(gold >= 200){
            defence = true;
            defenceSelect.SetActive(true);
			}
        }
        if (removing == true)
        {
            defence = false;
            defenceSelect.SetActive(false);
        }
    }

    public void Attack()
    {
        if (buying == true)
        {
			if(gold >= 200){
            attack = true;
            attackSelect.SetActive(true);
			}
        }
        if (removing == true)
        {
            attack = false;
            attackSelect.SetActive(false);
        }
    }

    public void Crit()
    {
        if (buying == true)
        {
			if(gold >= 200){
            criticalChance = true;
            criticalSelect.SetActive(true);
			}
        }
        if (removing == true)
        {
            criticalChance = false;
            criticalSelect.SetActive(false);
        }
    }

    public void Pendant()
    {
        if (buying == true)
        {
			if(gold >= 100){
            pendant = true;
            pendantSelect.SetActive(true);
			}
        }
        if (removing == true)
        {
            pendant = false;
            pendantSelect.SetActive(false);
        }
    }

	public void All(){
		strSelectedTarget = "All";
	}
    private void LeapMotionPointingAtObject()
    {
        if (isPointingOnlyWithIndexFinger && roundEnd == true)
        {
            if (isPointingAtNextRoundButton | isPointingAtShopButton | isPointingAtHealthUpgrade | isPointingAtCritChanceUpgrade | isPointingAtPendantGoldUpgrade | isPointingAtDefenceUpgrade | isPointingAtAttackDamageUpgrade | isPointingAtCheckout)
            {
                shopConfirmationTimer -= Time.deltaTime;
                if (isPointingAtNextRoundButton)
                {
                    newroundSelect.SetActive(true);
                    if (shopConfirmationTimer <= 0)
                    {
                        shopConfirmationTimer = shopConfirmationTimerLength;
                        StartNewRound();
                    }
                }
                if (isPointingAtShopButton)
                {
                    shopSelect.SetActive(true);
                    if (shopConfirmationTimer <= 0)
                    {
                        shopConfirmationTimer = shopConfirmationTimerLength;
                        Shop();
                    }
                }
                if (isPointingAtHealthUpgrade && gold >= 100)
                {
                    healthSelect.SetActive(true);
                    if (shopConfirmationTimer <= 0)
                    {
                        shopConfirmationTimer = shopConfirmationTimerLength;
                        PlayerHealth = 100f;
                        gold = gold - 100;
                    }
                }
                if (isPointingAtCritChanceUpgrade)
                {
                    criticalSelect.SetActive(true);
                    if (shopConfirmationTimer <= 0 && gold >= 200)
                    {
                        shopConfirmationTimer = shopConfirmationTimerLength;
                        CritChance = CritChance + 1;
                        gold = gold - 200;
                    }
                }
                if (isPointingAtPendantGoldUpgrade)
                {
                    pendantSelect.SetActive(true);
                    if (shopConfirmationTimer <= 0 && gold >= 100)
                    {
                        shopConfirmationTimer = shopConfirmationTimerLength;
                        moneyMultiplyer = moneyMultiplyer + 1;
                        gold = gold - 100;
                    }
                }
                if (isPointingAtDefenceUpgrade)
                {
                    defenceSelect.SetActive(true); 
                    if (shopConfirmationTimer <= 0 && gold >= 200)
                    {
                        shopConfirmationTimer = shopConfirmationTimerLength;
                        DefenceStat = DefenceStat + 1;
                        gold = gold - 200;
                    }
                }
                if (isPointingAtAttackDamageUpgrade)
                {
                    attackSelect.SetActive(true); 
                    if (shopConfirmationTimer <= 0 && gold >= 200)
                    {
                        shopConfirmationTimer = shopConfirmationTimerLength;
                        AttackDamage = AttackDamage + 1;
                        gold = gold - 200;
                    }
                }
                if (isPointingAtCheckout)
                {
                    checkoutSelect.SetActive(true);
                    if (shopConfirmationTimer <= 0)
                    {
                        shopConfirmationTimer = shopConfirmationTimerLength;
                        camAnimation.Play("BackToFight");
                        shopOpen = false;
                    }
                }
            }
            else
            {
                shopConfirmationTimer = shopConfirmationTimerLength;
                healthSelect.SetActive(false);
                criticalSelect.SetActive(false);
                pendantSelect.SetActive(false);
                defenceSelect.SetActive(false);
                attackSelect.SetActive(false);
                checkoutSelect.SetActive(false);
                newroundSelect.SetActive(false);
                shopSelect.SetActive(false);
            }
        }
        else
        {
            shopConfirmationTimer = shopConfirmationTimerLength;
        }
        ElementTargetConfirmation.fillAmount = 1 - (shopConfirmationTimer / shopConfirmationTimerLength);
    }

    private void SelectionConfirmationTimer()
    {
        if (strSelectedElement == strOldElement && strSelectedTarget == strOldTarget && strSelectedElement != "" && strSelectedTarget != "") selectionConfirmationTimer -= Time.deltaTime;
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
                    Enemies[i].Health -= CalculatedDamage / 3;
                }

                else if (result == "Lose")
                {
                    CalculatedDamage = Enemies[i].AttackDamage - DefenceStat;
                    if (CalculatedDamage < 0) CalculatedDamage = 0;
                    PlayerHealth -= CalculatedDamage / 3;
                }
            }
        }
        else if (strSelectedTarget == "Left")
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
                CalculatedDamage = Enemies[0].AttackDamage - DefenceStat;
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
                CalculatedDamage = Enemies[1].AttackDamage - DefenceStat;
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
                CalculatedDamage = Enemies[2].AttackDamage - DefenceStat;
                if (CalculatedDamage < 0) CalculatedDamage = 0;
                PlayerHealth -= CalculatedDamage;
            }
        }
        PlayerHealthBar.GetComponent<Image>().fillAmount = PlayerHealth / 100;
        for (int i = 0; i < objEnemies.Length; i++)
        {
            objEnemies[i].transform.Find("Canvas").Find("HealthFill").gameObject.GetComponent<Image>().fillAmount = Enemies[i].Health / 100;
            if (Enemies[i].Health <= 0) Enemies[i].Alive = false;
        }

        BetweenRounds();

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
		roundEnd = false;
		between.SetActive(false);
		gold = gold * moneyMultiplyer + 50;
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
            if (EnemyElement == "Wood") result = "Draw";
        }
        return result;
    }
    private bool HasCritBool()
    {
        int RandomNumber = UnityEngine.Random.Range(0, 99);
        if (RandomNumber <= CritChance - 1) return true;
        else return false;
    }
    private void RandomAIElement()
    {
        for (int i = 0; i < objEnemies.Length; i++)
        {
            int RandomNumber = UnityEngine.Random.Range(0, 99);
            switch (RandomNumber)//"Fire","Earth","Water","Metal","Wood"
            {
                case int n when n >= 0 && n <= 9://Fire
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
            for (int k = 0; k < Element.Length; k++)
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
    public void RightIndexFingerIsPointingAtShopButtonIsActive()
    {
        isPointingAtShopButton = true;
        //Debug.Log("Pointing At Shop Button");
    }
    public void RightIndexFingerIsPointingAtShopButtonIsDeactive()
    {
        isPointingAtShopButton = false;
        //Debug.Log("NOT Pointing At Shop Button");
    }
    public void RightIndexFingerIsPointingAtNextRoundButtonIsActive()
    {
        isPointingAtNextRoundButton = true;
        //Debug.Log("Pointing At Next Round Button");
    }
    public void RightIndexFingerIsPointingAtNextRoundButtonIsDeactive()
    {
        isPointingAtNextRoundButton = false;
        //Debug.Log("NOT Pointing Next Round Button");
    }
    public void RightIndexFingerIsPointingAtHealthUpgradeButtonIsActive()
    {
        isPointingAtHealthUpgrade = true;
        //Debug.Log("Pointing At Health Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtHealthUpgradeButtonIsDeactive()
    {
        isPointingAtHealthUpgrade = false;
        //Debug.Log("NOT Pointing Health Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtCritChanceUpgradeButtonIsActive()
    {
        isPointingAtCritChanceUpgrade = true;
        //Debug.Log("Pointing At Crit Chance Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtCritChanceUpgradeButtonIsDeactive()
    {
        isPointingAtCritChanceUpgrade = false;
        //Debug.Log("NOT Pointing Crit Chance Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtAttackDamageUpgradeButtonIsActive()
    {
        isPointingAtAttackDamageUpgrade = true;
        //Debug.Log("Pointing At Attack Damage Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtAttackDamageUpgradeButtonIsDeactive()
    {
        isPointingAtAttackDamageUpgrade = false;
        //Debug.Log("NOT Pointing Attack Damage Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtDefenceUpgradeButtonIsActive()
    {
        isPointingAtDefenceUpgrade = true;
        //Debug.Log("Pointing At Defence Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtDefenceUpgradeButtonIsDeactive()
    {
        isPointingAtDefenceUpgrade = false;
        //Debug.Log("NOT Pointing Defence Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtPendantUpgradeButtonIsActive()
    {
        isPointingAtPendantGoldUpgrade = true;
        //Debug.Log("Pointing At Pendant Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtPendantUpgradeButtonIsDeactive()
    {
        isPointingAtPendantGoldUpgrade = false;
        //Debug.Log("NOT Pointing Pendant Upgrade Button");
    }
    public void RightIndexFingerIsPointingAtCheckoutButtonIsActive()
    {
        isPointingAtCheckout = true;
        //Debug.Log("Pointing At Checkout Button");
    }
    public void RightIndexFingerIsPointingAtCheckoutButtonIsDeactive()
    {
        isPointingAtCheckout = false;
        //Debug.Log("NOT Pointing Checkout Button");
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

    private void BetweenRounds() {
        between.SetActive(true);
        roundEnd = true;
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
}
