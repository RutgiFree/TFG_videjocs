using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rules
{
    public enum DNAnucleotides
    {
        NONE = 'N',//no fa res, es l'axioma inicial i la estructura inmutable
        GROW = 'G',//vull que creixi
        FLOWER = 'F',//vull una flor 
        FRUIT = 'X',//vull una fruita
        LEAF = 'L',//vull una fulla
        START_BRANCH = '[',
        END_BRANCH = ']',
        POSITIVE_ROTATION = '+',
        NEGATIVE_ROTATION = '-',
        INCREMENT_ROTATION = 'R',//vull que s'afegeixe mes rotacio a futur
        MORE_GROW= 'M',//vull que s'iniciin nopus creiements
    }

    public enum states
    {
        NONE,//No s'ha definit cap estat
        GERMINATION,
        GROWING,
        FLOWERING,
        RIPENING,//maduració
        DYING,//La planta està assecant-se, per tant, morin
        DEATH,//La planta està morta
    }

}
