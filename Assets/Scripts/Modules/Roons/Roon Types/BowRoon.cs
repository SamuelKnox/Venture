public abstract class BowRoon : Roon
{
    private const RoonType TypeOfRoon = RoonType.Bow;

    public override RoonType GetRoonType()
    {
        return TypeOfRoon;
    }
}
