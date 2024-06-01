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
}

[Serializable]
public class Vegetable  
{

    public string name;
    public Rules.states[] myStates;
    public Rules.states myState;
    public int myStateIndex;
    public float width;
    public float height;
    public  Dictionary<Rules.DNAnucleotides, DNAinfo[]> DNA;


    public Vegetable() { }//es encesari per la serialitzacio de l'objecte
    public Vegetable(string _name, Rules.states[] _myStates, int _myStateIndex, Dictionary<Rules.DNAnucleotides, DNAinfo[]> _DNA, float _width, float _height)
    {
        if (_myStates.Length == 0 || _DNA.Count == 0) throw new System.Exception("empty STATES or DNA given");

        if (myStateIndex < _myStates.Length) myStateIndex = _myStateIndex;
        else myStateIndex = 0;

        name = _name;
        myStates =_myStates;
        myState =_myStates[myStateIndex];
        DNA = _DNA;
        width = _width;
        height = _height;
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
}
