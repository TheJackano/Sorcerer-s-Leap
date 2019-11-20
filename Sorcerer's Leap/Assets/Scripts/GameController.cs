using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Enemy[] Enemies;
    string[] Element = {"Fire","Earth","Water","Metal","Wood"};
    public GameObject[] objEnemies;



    string strPhase = "";
    string strSelectedElement = "";
    string strSelectedTarget = "";

    void Start()
    {
        /*
        Debug.Log(Element.Length);
        for (int i = 0; i < Element.Length; i++)
        {
            Debug.Log(Element[i]);
        }
        */
    }

    void Update()
    {
        if (Input.inputString == "s") strPhase = "Selection";
        if (Input.inputString == "c") strPhase = "Combat";
        //objEnemies[0].transform.GetChild(3).gameObject.SetActive(false);

        switch (strPhase)
        {
            case "Selection":
                SelectedElement();
                SelectedTargets();
                ToggleSelectedTargetUI();
                break;
            case "Combat":
                //Debug.Log(strSelectedElement);
                //Debug.Log(strSelectedTarget);
                break;
            case "Shopping":
                break;
        }


        /*
         * 1. Select Element
         * 2. Select Target
         * 
         *
         */

    }
    private void SelectedElement()
    {
        switch (Input.inputString)
        {
            case "q":
                strSelectedElement = Element[0];
                break;
            case "w":
                strSelectedElement = Element[1];
                break;
            case "e":
                strSelectedElement = Element[2];
                break;
            case "r":
                strSelectedElement = Element[3];
                break;
            case "t":
                strSelectedElement = Element[4];
                break;
        }
        Debug.Log(strSelectedElement);
    }
    private void SelectedTargets()
    {
        switch (Input.inputString)
        {
            case "4":
                strSelectedTarget = "Left";
                break;
            case "5":
                strSelectedTarget = "Middle";
                break;
            case "6":
                strSelectedTarget = "Right";
                break;
            case "2":
                strSelectedTarget = "All";
                break;
        }
        Debug.Log(strSelectedTarget);
    }
    private void ToggleSelectedTargetUI()
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
    }
}
public class Enemy
{
    int Health;
    string WizzardType;
    string SelectedSpell;
    string position;
}
