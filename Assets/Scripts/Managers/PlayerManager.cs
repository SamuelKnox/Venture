using UnityEngine;

public static class PlayerManager
{
    private static Player player;
    private static Health playerHealth;
    private static Mana playerMana;

    public static Player Player
    {
        get
        {
            if (!player)
            {
                player = Object.FindObjectOfType<Player>();
            }
            return player;
        }
    }

    public static Health Health
    {
        get
        {
            if (!playerHealth)
            {
                playerHealth = Player.GetComponentInChildren<Health>();
            }
            return playerHealth;
        }
    }

    public static Mana Mana
    {
        get
        {
            if (!playerMana)
            {
                playerMana = Player.GetComponentInChildren<Mana>();
            }
            return playerMana;
        }
    }
}