public abstract class WandRune : Rune
{
    private const RuneType TypeOfRune = RuneType.Wand;

    /// <summary>
    /// Gets the wand rune type
    /// </summary>
    /// <returns>Rune type of wand</returns>
    public override RuneType GetRuneType()
    {
        return TypeOfRune;
    }

    /// <summary>
    /// Activates the rune's ability
    /// </summary>
    /// <param name="wand">Wand rune is being activated from</param>
    public abstract void ActivateAbility(Wand wand);
}