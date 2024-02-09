using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableProxy : MonoBehaviour, IServiceable //es el constructor
{
    //patro representant? -> patro proxy o patro decorator?
    Vegetable myVegetable;
    [SerializeField] string vName;
    [SerializeField] public Rules.states vState { get; private set; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void setVegetable(Vegetable vegetable)//li diem quina hortalissa es
    {
        if (vegetable == null) throw new System.NotImplementedException();
        myVegetable = vegetable;
        vName = myVegetable.name;
        vState = myVegetable.myState;
    }

    public void getFruit()
    {
        if (myVegetable == null) throw new System.NotImplementedException();
        myVegetable.getFruit();
    }
    public void pasTime()
    {
        if (myVegetable == null) throw new System.NotImplementedException();
        myVegetable.pasTime();
    }

    public Rules.states nextState()//pasem a la seguent fase
    {
        if (myVegetable == null) throw new System.NotImplementedException();
        vState = myVegetable.nextState();
        return vState;
    }



}
