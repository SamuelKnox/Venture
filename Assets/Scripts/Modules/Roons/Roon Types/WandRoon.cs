using UnityEngine;

public abstract class WandRoon : Roon
{
    [Tooltip("How much mana it costs to cast this roon's spell")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float manaCost = 10.0f;

    private const RoonType TypeOfRoon = RoonType.Wand;

    /// <summary>
    /// Gets the wand roon type
    /// </summary>
    /// <returns>Roon type of wand</returns>
    public override RoonType GetRoonType()
    {
        return TypeOfRoon;
    }

    /// <summary>
    /// Activates the roon's ability
    /// </summary>
    /// <param name="wand">Wand roon is being activated from</param>
    /// <param name="direction">Direction ability is being activate towards</param>
    public abstract void ActivateAbility(Wand wand, Vector2 direction);

    /// <summary>
    /// Cost in mana to cast this roon's spell
    /// </summary>
    /// <returns>Mana cost for this roon's spell</returns>
    public float GetManaCost()
    {
        return manaCost;
    }
}