using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableContructor : MonoBehaviour, IConstructable //es el constructor
{
    VegetableObjectContructor activeVegetable;
    [SerializeField] string vName;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void getFruit()
    {
        throw new System.NotImplementedException();
    }
    public void pasTime()
    {
        throw new System.NotImplementedException();
    }

    public void nextState()
    {
        activeVegetable.myStateIndex++;//canviaro per sets and gets

        if (activeVegetable.myStateIndex >= activeVegetable.myStates.Length)//vol dir que ja hem mort
        {
            Destroy(this);
            return;
        }

        activeVegetable.myState = activeVegetable.myStates[activeVegetable.myStateIndex];//canviaro per setes and gets
    }


    public void setVegetable(VegetableObjectContructor vegetable)
    {
        this.activeVegetable = vegetable;
        vName = activeVegetable.name;
    }
}
