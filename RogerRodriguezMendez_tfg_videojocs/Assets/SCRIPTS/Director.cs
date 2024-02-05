using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(Vegetable))]
public class Director : MonoBehaviour
{
    Vegetable constructor;
    [SerializeField] bool load;

    void Start()
    {

        constructor = GetComponent<Vegetable>();
    }


    void Update()
    {
        if (load)
        {
            DataManager.LoadVegetableObject();
            load = !load;
        }
    }
}
