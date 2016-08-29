using UnityEngine;
using System.Collections;

public abstract class PurpleRune : Rune
{
    private const RuneType TypeOfRune = RuneType.Purple;

    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }
}
