using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServiceable 
{
    void pasTime();//pase el temps, per tant, una iteració de més
    Rules.states nextState();//pasem a la seguent fase
    void getFruit(); //obtenim el fruit de l'hortaliza
}