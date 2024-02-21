using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DNAinfo
{
    public string info;
    public float probability;
    public DNAinfo() { }//es encesari per la serialitzacio de l'objecte
    public DNAinfo(string _info, float _probability)
    {
        info = _info;
        probability = _probability;
    }

    public string debug()//es per comprar que funcione be el que vull intentar
    {
        return "{" + info + ", " + probability + "}";
    }
}

[Serializable]
public class Vegetable  //es el tipus especific de contructor
{//vegetavle -> nom //eredar i vegetabler?

    public string name;
    public Rules.states[] myStates;
    public Rules.states myState;
    public int myStateIndex;
    public  Dictionary<Rules.DNAnucleotides, DNAinfo[]> DNA;


    public Vegetable() { }//es encesari per la serialitzacio de l'objecte
    public Vegetable(string _name, Rules.states[] _myStates, int _myStateIndex, Dictionary<Rules.DNAnucleotides, DNAinfo[]> _DNA)
    {
        if (_myStates.Length == 0 || _DNA.Count == 0) throw new System.Exception("empty STATES or DNA given");

        if (myStateIndex < _myStates.Length) myStateIndex = _myStateIndex;
        else myStateIndex = 0;

        name = _name;
        myStates =_myStates;
        myState =_myStates[myStateIndex];
        DNA = _DNA;
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

    public string pasTime(string currentDNA)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in currentDNA)
        {
            sb.Append(DNA.ContainsKey((Rules.DNAnucleotides) c ) ? getOneDNAinfo(DNA[(Rules.DNAnucleotides)c]) : c.ToString());
        }

        currentDNA = sb.ToString();

        return currentDNA;
    }
    private string getOneDNAinfo(DNAinfo[] DNAinfos)
    {
        float minProb = Random.Range(1, 10);
        foreach (DNAinfo DNAinfo in DNAinfos)
        {
            if (DNAinfo.probability >= minProb)
            {
                return DNAinfo.info;
            }

        }
        return DNAinfos[0].info;
    }



    public string debug()//es per comprar que funcione be el que vull intentar
    {
        return "{\n\t" + name+", "+ debugStates() + ", " + myState + ", " + myStateIndex+ ", \n\t" + debugDNA() + "\n}";
    }
    private string debugStates()//es per comprar que funcione be el que vull intentar
    {
        string retorn = "";
        for (int i = 0; i < myStates.Length; i++)
        {
            if(retorn.Equals("")) retorn = myStates[i].ToString();
            else retorn = retorn + ", " + myStates[i];
        }
        return "["+retorn+"]";
    }
    private string debugDNA()//es per comprar que funcione be el que vull intentar
    {
        string retorn = "";

        foreach(Rules.DNAnucleotides key in DNA.Keys)
        {
            foreach(DNAinfo dna in DNA[key])
            {
                if (retorn.Equals("")) retorn = "{ "+key +", "+dna.debug()+ "}";
                else retorn = retorn + ",\n\t" + "{ " + key + ", " + dna.debug() + "}";
            }
        }
        return "{\n\t" + retorn + "\n\t}";
    }
}
