public abstract class MeleeWeaponRoon : Roon
{
    private const RoonType TypeOfRoon = RoonType.MeleeWeapon;

    public override RoonType GetRoonType()
    {
        return TypeOfRoon;
    }
}
