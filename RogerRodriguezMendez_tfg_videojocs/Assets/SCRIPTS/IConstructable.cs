using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConstructable
{
    void pasTime();//pase el temps, per tant, una iteració de més
    void nextState();//pasem a la seguent fase
    void getFruit(); //obtenim el fruit de l'hortaliza
}