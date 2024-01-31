using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(Vegetable))]
public class Director : MonoBehaviour
{
    Vegetable constructor;

    void Start()
    {

        constructor = GetComponent<Vegetable>();
    }


    void Update()
    {
        
    }
}
