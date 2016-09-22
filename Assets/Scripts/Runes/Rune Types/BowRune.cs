public abstract class BowRune : Rune
{
    private const RuneType TypeOfRune = RuneType.Bow;

    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }
}
