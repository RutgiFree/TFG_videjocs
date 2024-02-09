using System;

[Serializable]
public class VegetableSerializatorBase
{
    public string name;
    public Rules.states[] myStates;
    public Rules.states myState;
    public int myStateIndex;
}