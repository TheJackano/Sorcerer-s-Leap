using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_3 : MonoBehaviour
{
    public GameObject elements;
    public GameObject toStart;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ThirdOnClick()
    {
        elements.SetActive(false);
        toStart.SetActive(true);
    }
}
