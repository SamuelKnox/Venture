using TMPro;
using UnityEngine;

public class StatsView : MonoBehaviour
{
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
        playerHealth = PlayerManager.Health; 
        playerMana = PlayerManager.Mana;
    }

    void Update()
    {
        healthText.text = (int)playerHealth.GetCurrentHitPoints() + "/" + (int)playerHealth.GetMaxHitPoints();
        manaText.text = (int)playerMana.GetCurrentManaPoints() + "/" + (int)playerMana.GetMaxManaPoints();
        prestigeText.text = PlayerManager.Player.GetPrestige().ToString();
        goldText.text = PlayerManager.Player.GetGold().ToString();
    }
}