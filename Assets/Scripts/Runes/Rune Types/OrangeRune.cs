using UnityEngine;
using System.Collections;

public abstract class OrangeRune : Rune
{
    private const RuneType TypeOfRune = RuneType.Orange;

    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }
}