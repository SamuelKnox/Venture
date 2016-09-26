public abstract class BlueRoon : Roon
{
    private const RoonType TypeOfRoon = RoonType.Blue;

    public override RoonType GetRoonType()
    {
        return TypeOfRoon;
    }
}
