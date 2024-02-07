using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class VegetableObjectWorker //es el tipus especific de contructor
{
    public string name;
    public Rules.states[] myStates;
    public Rules.states myState;
    public int myStateIndex;

    public VegetableObjectWorker(string name, Rules.states[] myStates, int myStateIndex)
    {
        if (myStates.Length == 0) throw new System.Exception("EMPTY STATES ARRARY");

        if (myStateIndex < myStates.Length) this.myStateIndex = myStateIndex;
        else this.myStateIndex = 0;

        this.name = name;
        this.myStates = myStates;
        this.myState = myStates[this.myStateIndex];
    }

    public string debug()//es per comprar que funcione be el que vull intentar
    {
        return "{"+name+","+ abc() + "," + myState + "," + myStateIndex +"}";
    }

    private string abc()//es per comprar que funcione be el que vull intentar
    {
        string retorn = "";
        for (int i = 0; i < myStates.Length; i++)
        {
            if(retorn.Equals("")) retorn = myStates[i].ToString();
            else retorn = retorn + ", " + myStates[i];
        }
        return "["+retorn+"]";
    }
}
