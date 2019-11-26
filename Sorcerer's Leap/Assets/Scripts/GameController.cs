using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Enemy[] Enemies;
    string[] Element = {"Fire","Earth","Water","Metal","Wood"};
    public GameObject[] objEnemies;
    public GameObject Player;
    public Sprite[] imgElements;



    string strPhase = "";
    string strSelectedElement = "";
    string strSelectedTarget = "";

    void Start()
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
        strPhase = "Selection";

        /*
        Player.transform.Find("Element").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[3];
        objEnemies[0].transform.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[1];
        objEnemies[1].transform.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().overrideSprite = imgElements[3];
        */
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

        switch (strPhase)
        {
            case "Selection":
                SelectedElement();
                SelectedTargets();
                break;
            case "Combat":
                //Debug.Log(strSelectedElement);
                //Debug.Log(strSelectedTarget);
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
        Debug.Log(strSelectedElement);
    } //Allows the User to Select an Element
    private void SelectedTargets()
    {
        switch (Input.inputString)
        {
            case "4":
                strSelectedTarget = "Left";
                {
                    objEnemies[0].transform.Find("Selection").gameObject.SetActive(true);
                    objEnemies[1].transform.Find("Selection").gameObject.SetActive(false);
                    objEnemies[2].transform.Find("Selection").gameObject.SetActive(false);
                }//Displays Left Target Selection

                break;
            case "5":
                strSelectedTarget = "Middle";
                {
                    objEnemies[0].transform.Find("Selection").gameObject.SetActive(false);
                    objEnemies[1].transform.Find("Selection").gameObject.SetActive(true);
                    objEnemies[2].transform.Find("Selection").gameObject.SetActive(false);
                }//Displays Middle Target Selection
                break;
            case "6":
                strSelectedTarget = "Right";
                {
                    objEnemies[0].transform.Find("Selection").gameObject.SetActive(false);
                    objEnemies[1].transform.Find("Selection").gameObject.SetActive(false);
                    objEnemies[2].transform.Find("Selection").gameObject.SetActive(true);
                }//Displays Right Target Selection
                break;
            case "2":
                strSelectedTarget = "All";
                //Displays All Target Selection
                break;
        }
        Debug.Log(strSelectedTarget);
    } //Allows the User to Select a Target
}
public class Enemy
{
    int Health;
    string WizzardType;
    string SelectedSpell;
    string position;
}
