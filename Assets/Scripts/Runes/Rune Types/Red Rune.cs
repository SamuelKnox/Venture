public abstract class RedRune : Rune
{
    private const RuneType TypeOfRune = RuneType.Red;

    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }
}
