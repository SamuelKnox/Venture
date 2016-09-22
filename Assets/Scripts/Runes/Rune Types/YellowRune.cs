public abstract class YellowRune : Rune
{
    private const RuneType TypeOfRune = RuneType.Yellow;

    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }
}
