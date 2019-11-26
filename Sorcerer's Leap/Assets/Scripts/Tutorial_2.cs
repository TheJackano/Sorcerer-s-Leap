using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_2 : MonoBehaviour
{
    public GameObject elements;
    public GameObject welcome;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SecondOnClick()
    {
        welcome.SetActive(false);
        elements.SetActive(true);
    }
}
