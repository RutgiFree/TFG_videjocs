using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rules
{
    public enum DNAnucleotides
    {
        NONE = 'N',
        GROW = 'G',
        FLOWER = 'F',
        FRUIT = 'X',
        LEAF = 'L',
        START_BRANCH = '[',
        END_BRANCH = ']',
        POSITIVE_ROTATION = '+',
        NEGATIVE_ROTATION = '-',
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
