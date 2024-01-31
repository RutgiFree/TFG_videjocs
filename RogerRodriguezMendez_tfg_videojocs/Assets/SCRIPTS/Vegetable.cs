using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegetable : MonoBehaviour, IConstructable //podria ser aquest el constructor?
{
    Dictionary<string, vegetableObject> myDictionary;
    vegetableObject activeVegetable;

    private class vegetableObject //i aquest podria ser els diferents tipus de contructors?
    {
        public string name;
        public Rules.states[] myStates;
        public Rules.states myState;
        public int myStateIndex;

        public vegetableObject(string name, Rules.states[] myStates, int myStateIndex)
        {
            if (myStates.Length == 0) throw new System.Exception("EMPTY STATES ARRARY");

            this.name = name;
            this.myStates = myStates;
            this.myStateIndex = myStateIndex;
            if(myStateIndex < myStates.Length) myState = myStates[myStateIndex];
            else  myState = myStates[myStateIndex];
        }
    }

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
