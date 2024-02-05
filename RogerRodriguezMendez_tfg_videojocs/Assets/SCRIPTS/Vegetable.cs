using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegetable : MonoBehaviour, IConstructable //es el constructor
{
    Dictionary<string, vegetableObject> myDictionary;
    vegetableObject activeVegetable;


    private void Awake()
    {
        if (myDictionary == null) myDictionary = new Dictionary<string, vegetableObject>();//tenir una classe a aprt on es guiarden la llista completa
        if (activeVegetable == null) activeVegetable = new vegetableObject("Carrot", new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.DYING } , 0 );
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void getFruit()
    {
        throw new System.NotImplementedException();
    }

    public void nextState()
    {
        activeVegetable.myStateIndex++;//canviaro per setes and gets

        if (activeVegetable.myStateIndex >= activeVegetable.myStates.Length)//vol dir que ja hem mort
        {
            Destroy(this);
            return;
        }

        activeVegetable.myState = activeVegetable.myStates[activeVegetable.myStateIndex];//canviaro per setes and gets
    }

    public void pasTime()
    {
        throw new System.NotImplementedException();
    }
}
