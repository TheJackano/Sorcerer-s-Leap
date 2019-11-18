using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbAniamtionSpeed : MonoBehaviour
{
    public float HighVal;
    public float LowVal;

    private Animator orbAnimator;
    private float animSpeed;
    // Start is called before the first frame update
    void Start()
    {
        orbAnimator = GetComponent<Animator>();
        animSpeed = Random.Range(HighVal, LowVal);
        orbAnimator.speed = animSpeed;
    }

}
