using UnityEngine;

public class ManaPotion : Consumable
{
    [Tooltip("Amount of mana to restore")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float manaRestore = 10;

    /// <summary>
    /// Uses the potion
    /// </summary>
    public override void Consume()
    {
        RestorePlayerMana();
    }

    /// <summary>
    /// Gets the potions mana restore
    /// </summary>
    /// <returns>Mana restore</returns>
    public float GetManaRestore()
    {
        return manaRestore;
    }

    /// <summary>
    /// Sets the potion's mana restore
    /// </summary>
    /// <param name="manaRestore">New mana restore</param>
    public void SetManaRestore(float manaRestore)
    {
        this.manaRestore = manaRestore;
    }

    /// <summary>
    /// Restores the player's mana
    /// </summary>
    private void RestorePlayerMana()
    {
        var playerMana = PlayerManager.Player.GetComponentInChildren<Mana>();
        if (!playerMana)
        {
            Debug.LogError(gameObject + " could not find player mana!", gameObject);
            return;
        }
        playerMana.SetCurrentManaPoints(playerMana.GetCurrentManaPoints() + manaRestore);
    }
}