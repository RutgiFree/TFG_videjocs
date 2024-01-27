using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carrot : MonoBehaviour, IConstructable
{
    [SerializeField] Rules.states[] myStates = { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.DYING };
    [SerializeField] Rules.states myState;

    int myStateIndex = 0;

    void Awake()
    {
        myState = myStates[myStateIndex];
    }

    void Update()
    {
        
    }

    void IConstructable.nextState()
    {
        myStateIndex++;

        if (myStateIndex >= myStates.Length)//vol dir que ja hem mort
        {
            Destroy(this);
            return;
        }

        myState = myStates[myStateIndex];
    }

    void IConstructable.pasTime()
    {
        throw new System.NotImplementedException();


    }
    public void getFruit()
    {
        throw new System.NotImplementedException();
    }


}
