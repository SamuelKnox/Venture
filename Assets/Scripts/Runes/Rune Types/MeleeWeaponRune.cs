public abstract class MeleeWeaponRune : Rune
{
    private const RuneType TypeOfRune = RuneType.MeleeWeapon;

    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }
}
