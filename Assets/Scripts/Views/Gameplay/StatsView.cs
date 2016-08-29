using TMPro;
using UnityEngine;

public class StatsView : MonoBehaviour
{
    [Tooltip("Player to get stats from")]
    [SerializeField]
    private Player player;

    [Tooltip("Text display for player health")]
    [SerializeField]
    private TextMeshProUGUI healthText;

    [Tooltip("Text display for player mana")]
    [SerializeField]
    private TextMeshProUGUI manaText;

    [Tooltip("Text display for player prestige")]
    [SerializeField]
    private TextMeshProUGUI prestigeText;

    [Tooltip("Text display for player gold")]
    [SerializeField]
    private TextMeshProUGUI goldText;

    private Health playerHealth;
    private Mana playerMana;

    void Awake()
    {
        playerHealth = player.GetComponentInChildren<Health>();
        if (!playerHealth)
        {
            Debug.LogError(gameObject + " could not find player health!", gameObject);
            return;
        }
        playerMana = player.GetComponentInChildren<Mana>();
        if (!playerMana)
        {
            Debug.LogError(gameObject + " could not find player mana!", gameObject);
            return;
        }
    }

    void Update()
    {
        healthText.text = (int)playerHealth.GetCurrentHitPoints() + "/" + (int)playerHealth.GetMaxHitPoints();
        manaText.text = (int)playerMana.GetCurrentManaPoints() + "/" + (int)playerMana.GetMaxManaPoints();
        prestigeText.text = player.GetPrestige().ToString();
        goldText.text = player.GetGold().ToString();
    }
}