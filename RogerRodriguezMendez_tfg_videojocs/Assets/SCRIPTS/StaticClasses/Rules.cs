using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rules
{
    public enum DNAnucleotides
    {
        NONE = 'N',// es l'axioma inicial i la estructura inmutable
        GROW = 'G',//vull que creixi
        GROW_1 = '1',//vull que sigui mida Y = 1
        GROW_2 = '2',//vull que sigui mida Y = 2
        GROW_3 = '3',//vull que sigui mida Y = 3
        GROW_4 = '4',//vull que sigui mida Y = 4
        FLOWER = 'F',//vull una flor 
        FRUIT = 'D',//vull una fruita
        LEAF = 'L',//vull una fulla
        START_BRANCH = '[',
        END_BRANCH = ']',
        POSITIVE_ROTATION = '+',
        NEGATIVE_ROTATION = '-',
        CONTINUO_ROTATION = 'C',//vull que segueixi rotan en la mateixa direccio
        //aquest son diferents auxiliars, per tant no tenen cap funcio especifica, son per la subtitucio de regles, fent que sigui mes felxible la generacio:
        AUX_1 = 'Z',
        AUX_2 = 'X',
        AUX_3 = 'W',
        AUX_4 = 'Y',
        AUX_5 = 'Q',
        AUX_6 = 'K',
        AUX_7 = 'P',
        AUX_8 = 'R',
        AUX_9 = 'T',
        AUX_10 = 'V',

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
