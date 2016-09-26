public abstract class RedRoon : Roon
{
    private const RoonType TypeOfRoon = RoonType.Red;

    public override RoonType GetRoonType()
    {
        return TypeOfRoon;
    }
}
