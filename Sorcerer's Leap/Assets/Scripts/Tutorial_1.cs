using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_1 : MonoBehaviour
{
    public GameObject start;
    public GameObject welcome;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FirstOnClick()
    {
        start.SetActive(false);
        welcome.SetActive(true);
    }
}
