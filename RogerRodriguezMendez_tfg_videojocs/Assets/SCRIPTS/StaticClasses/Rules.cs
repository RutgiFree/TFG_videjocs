using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rules
{
    public static readonly char NONE = 'N';
    public static readonly char GROW = 'G';
    public static readonly char FLOWER = 'F';
    public static readonly char FRUIT = 'X';
    public static readonly char LEAF = 'L';
    public static readonly char START_BRANCH = '[';
    public static readonly char END_BRANCH = ']';
    public static readonly char POSITIVE_ROTATION = '+';
    public static readonly char NEGATIVE_ROTATION = '-';
    public enum states
    {
        GERMINATION,
        GROWING,
        FLOWERING,
        RIPENING,//maduració
        DYING,//La planta està assecant-se, per tant, morin
    }

}
