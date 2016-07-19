using UnityEngine;
using System.Collections;

public abstract class GreenRune : Rune
{
    private const RuneType TypeOfRune = RuneType.Green;

    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }
}
