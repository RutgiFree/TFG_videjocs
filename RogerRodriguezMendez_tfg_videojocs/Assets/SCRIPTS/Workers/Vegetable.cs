using System;

[Serializable]
public class Vegetable : IServiceable //es el tipus especific de contructor
{//vegetavle -> nom //eredar i vegetabler?
    public string name;
    public Rules.states[] myStates;
    public Rules.states myState;
    public int myStateIndex;
    public Vegetable(string name, Rules.states[] myStates, int myStateIndex)
    {
        if (myStates.Length == 0) throw new System.Exception("EMPTY STATES ARRARY");

        if (myStateIndex < myStates.Length) this.myStateIndex = myStateIndex;
        else this.myStateIndex = 0;

        this.name = name;
        this.myStates = myStates;
        this.myState = myStates[this.myStateIndex];
    }


    public void getFruit()
    {
        throw new NotImplementedException();
    }

    public Rules.states nextState()
    {
        myStateIndex++;//canviaro per sets and gets

        if (myStateIndex >= myStates.Length)  myState = Rules.states.DEATH; //per seguretat, si ens pasem del index segurisim que esta morta
        else myState = myStates[myStateIndex];

        return myState;
    }

    public void pasTime()
    {
        throw new NotImplementedException();
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
