﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject PlayerHealthBar;

    string strPhase = "";
    string strSelectedElement = "";
    string strSelectedTarget = "";

    int PlayerHealth = 10;

    bool AIAssignedElement = false;
    bool CombatComplete = false;

    //Leap Motion Variables
    bool isPointingOnlyWithIndexFinger;
    bool isPointingIndexFingerAtLeftEnemy;
    bool isPointingIndexFingerAtMiddleEnemy;
    bool isPointingIndexFingerAtRightEnemy;
    bool isRightHandOpen;


    void Start()
    {
        StartNewRound();
    }

    void Update()
    {
        switch (strPhase)
        {
            case "Selection":
                SelectedElement();
                LeapMotionSelectedTarget();
                GUISelectedTargets();
                if (Input.GetKey(KeyCode.Space) && strSelectedElement != "" && strSelectedTarget !="") strPhase = "Combat";
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
    private void SelectedElement()
    {
        switch (Input.inputString)
        {
            case "q":
                strSelectedElement = Element[0];
                Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[0];
                break;
            case "w":
                strSelectedElement = Element[1];
                Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[1];
                break;
            case "e":
                strSelectedElement = Element[2];
                Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[2];
                break;
            case "r":
                strSelectedElement = Element[3];
                Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[3];
                break;
            case "t":
                strSelectedElement = Element[4];
                Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[4];
                break;
        }
    } //Allows the User to Select an Element
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
                objEnemies[0].transform.Find("Selection").gameObject.SetActive(true);
                objEnemies[1].transform.Find("Selection").gameObject.SetActive(false);
                objEnemies[2].transform.Find("Selection").gameObject.SetActive(false);
                break;
            case "Middle":
                objEnemies[0].transform.Find("Selection").gameObject.SetActive(false);
                objEnemies[1].transform.Find("Selection").gameObject.SetActive(true);
                objEnemies[2].transform.Find("Selection").gameObject.SetActive(false);
                break;
            case "Right":
                objEnemies[0].transform.Find("Selection").gameObject.SetActive(false);
                objEnemies[1].transform.Find("Selection").gameObject.SetActive(false);
                objEnemies[2].transform.Find("Selection").gameObject.SetActive(true);
                break;
            case "All":
                objEnemies[0].transform.Find("Selection").gameObject.SetActive(true);
                objEnemies[1].transform.Find("Selection").gameObject.SetActive(true);
                objEnemies[2].transform.Find("Selection").gameObject.SetActive(true);
                break;
        }
    } //Allows the User to Select a Target
    private void CalculateCombat()
    {
        if (strSelectedTarget == "All")
        {
            for (int i = 0; i < objEnemies.Length; i++)
            {
                string result = CompareTwoElements(strSelectedElement, Enemies[i].SelectedSpell);
                Debug.Log("You have picked " + strSelectedElement + " and " + result + " against the " + Enemies[i].SelectedSpell + " Wizzard that picked " + Enemies[i].SelectedSpell);
                if (result == "Win") Enemies[i].Health--;
                else if (result == "Lose") PlayerHealth--;
                Debug.Log("Your Health: " + PlayerHealth + " Enemy Aboves Health: " + Enemies[i].Health);
            }
        }
        else if(strSelectedTarget == "Left")
        {
            string result = CompareTwoElements(strSelectedElement, Enemies[0].SelectedSpell);
            if (result == "Win") Enemies[0].Health -= 3;
            else if (result == "Lose") PlayerHealth -= 3;
            Debug.Log("You "+ result + " this Fight. Your Health: " + PlayerHealth + ". Left Enemy's Health: " + Enemies[0].Health);
        }
        else if (strSelectedTarget == "Middle")
        {
            string result = CompareTwoElements(strSelectedElement, Enemies[1].SelectedSpell);
            if (result == "Win") Enemies[1].Health -= 3;
            else if (result == "Lose") PlayerHealth -= 3;
            Debug.Log("You " + result + " this Fight. Your Health: " + PlayerHealth + ". Middle Enemy's Health: " + Enemies[1].Health);
        }
        else if (strSelectedTarget == "Right")
        {
            string result = CompareTwoElements(strSelectedElement, Enemies[2].SelectedSpell);
            if (result == "Win") Enemies[2].Health -= 3;
            else if (result == "Lose") PlayerHealth -= 3;
            Debug.Log("You " + result + " this Fight. Your Health: " + PlayerHealth + ". Right Enemy's Health: " + Enemies[2].Health);
        }
        PlayerHealthBar.GetComponent<Image>().fillAmount = (float)PlayerHealth/10;
        CombatComplete = true;
    }
    private void StartNewRound()
    {
        {
            objEnemies[0].transform.Find("Selection").gameObject.SetActive(false);
            objEnemies[1].transform.Find("Selection").gameObject.SetActive(false);
            objEnemies[2].transform.Find("Selection").gameObject.SetActive(false);
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
    private void RandomAIElement()
    {
        for(int i = 0; i < objEnemies.Length; i++)
        {
            int RandomNumber = Random.Range(0,99);
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
        Debug.Log("Pointing Index Finger");
    }
    public void PointingOnlyWithIndexFingerIsDeactive()
    {
        isPointingOnlyWithIndexFinger = false;
        Debug.Log("No Longer Pointing Index Finger");
    }
    public void PointingIndexFingerTowardsLeftEnemyIsActive()
    {
        isPointingIndexFingerAtLeftEnemy = true;
        Debug.Log("Index is pointing at the Left Enemy");
    }
    public void PointingIndexFingerTowardsLeftEnemyIsDeactive()
    {
        isPointingIndexFingerAtLeftEnemy = false;
        Debug.Log("Index is no longer pointing at the Left Enemy");
    }
    public void PointingIndexFingerTowardsMiddleEnemyIsActive()
    {
        isPointingIndexFingerAtMiddleEnemy = true;
        Debug.Log("Index is pointing at the Middle Enemy");
    }
    public void PointingIndexFingerTowardsMiddleEnemyIsDeactive()
    {
        isPointingIndexFingerAtMiddleEnemy = false;
        Debug.Log("Index is no longer pointing at the Middle Enemy");
    }
    public void PointingIndexFingerTowardsRightEnemyIsActive()
    {
        isPointingIndexFingerAtRightEnemy = true;
        Debug.Log("Index is pointing at the Right Enemy");
    }
    public void PointingIndexFingerTowardsRightEnemyIsDeactive()
    {
        isPointingIndexFingerAtRightEnemy = false;
        Debug.Log("Index is no longer pointing at the Right Enemy");
    }
    public void RightHandOpenIsActive()
    {
        isRightHandOpen = true;
        Debug.Log("Right Hand Is Targeting All Enemies");
    }
    public void RightHandOpenIsDeactive()
    {
        isRightHandOpen = false;
        Debug.Log("Right Hand Is No Longer Targeting All Enemies");
    }
}
public class Enemy
{
    public int Health = 10;
    public string WizzardType;
    public string SelectedSpell;

    public Enemy(string _WizzardType)
    {
        WizzardType = _WizzardType;
    }
}

