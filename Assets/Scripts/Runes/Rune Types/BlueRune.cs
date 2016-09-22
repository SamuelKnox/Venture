public abstract class BlueRune : Rune
{
    private const RuneType TypeOfRune = RuneType.Blue;

    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }
}
