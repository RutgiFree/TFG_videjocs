using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class vegetableObject //es el tipus especific de contructor
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
        if (myStateIndex < myStates.Length) myState = myStates[myStateIndex];
        else myState = myStates[myStateIndex];
    }

    public string debug()//es per comprar que funcione be el que vull intentar
    {
        return "{"+name+","+ abc() + "," + myState + "," + myStateIndex +"}";
    }

    private string abc()
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
