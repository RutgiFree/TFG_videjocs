using System;
using System.Linq;
using UnityEngine;

 [RequireComponent(typeof(VegetableProxy))]
public class Director : MonoBehaviour
{//vegetable sistem? com a nom
    VegetableProxy vProxy;
    [SerializeField] string[] vegetablesName = DataManager.vegetablesNames;
    [SerializeField] bool load;
    [SerializeField] string vName;
    [SerializeField] bool nextState;
    [SerializeField] Rules.states vState;


    void Start()
    {
        vProxy = GetComponent<VegetableProxy>();
    }


    void Update()
    {
        if (load)
        {
            load = !load;
            if (vName.Equals(""))
            {
                vName = "WRITE HERE ¬¬";
                return;
            }
            try
            {
                vProxy.setVegetable(DataManager.getVegetable(vName));
            }
            catch (Exception)
            {
                vName = "NOT FOUND :(";
                return;
            }
            vName = "FOUND :)";
        }

        if (nextState)
        {
            nextState = !nextState;
            try
            {
                vState = vProxy.nextState();
                if (vState == Rules.states.DEATH) Destroy(this);
            }
            catch (Exception e)
            {
                Debug.LogError("Something goes wrong: "+e.Message);
            }
        }
    }
}
