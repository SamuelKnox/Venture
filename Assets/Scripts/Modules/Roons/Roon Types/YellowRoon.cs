public abstract class YellowRoon : Roon
{
    private const RoonType TypeOfRoon = RoonType.Yellow;

    public override RoonType GetRoonType()
    {
        return TypeOfRoon;
    }
}
