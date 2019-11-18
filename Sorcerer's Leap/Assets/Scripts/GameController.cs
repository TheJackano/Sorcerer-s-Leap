using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Enemy[] Enemies;
    string[] Element = {"Fire","Earth","Water","Metal","Wood"};



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
        if(Input.inputString == "1") strPhase = "Selection";

        switch (strPhase)
        {
            case "Selection":
                SelectedElement();
                SelectedTargets();
                break;
            case "Combat":
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
        //Debug.Log(strSelectedElement);
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
        //Debug.Log(strSelectedTarget);
    }
}
public class Enemy
{
    int Health;
    string WizzardType;
    string SelectedSpell;
}
