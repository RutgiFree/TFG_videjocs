using System;
using System.Linq;
using UnityEngine;

 [RequireComponent(typeof(VegetableContructor))]
public class Director : MonoBehaviour
{
    VegetableContructor constructor;
    [SerializeField] bool load;
    [SerializeField] string vegetablename;
    [SerializeField] string[] vegetablesName = DataManager.vegetablesNames;


    void Start()
    {

        constructor = GetComponent<VegetableContructor>();
    }


    void Update()
    {
        if (load)
        {
            load = !load;
            if (vegetablename.Equals(""))
            {
                vegetablename = "WRITE HERE ¬¬";
                return;
            }
            try
            {
                constructor.setVegetable(DataManager.getVegetable(vegetablename));
            }
            catch (Exception)
            {
                vegetablename = "NOT FOUND :(";
                return;
            }
            vegetablename = "FOUND :)";
        }
    }
}
