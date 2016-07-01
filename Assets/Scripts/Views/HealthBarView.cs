using UnityEngine;

[RequireComponent(typeof(EnergyBar))]
public class HealthBarView : MonoBehaviour
{
    [Tooltip("Health to be used")]
    [SerializeField]
    private Health health;

    private EnergyBar energyBar;

    void Awake()
    {
        energyBar = GetComponent<EnergyBar>();
    }

    void Update()
    {
        UpdatePlayerHealthDisplay();
    }

    private void UpdatePlayerHealthDisplay()
    {
        float percent = health.GetCurrentHitPoints() / health.GetMaxHitPoints();
        energyBar.SetValueCurrent((int)(percent * (energyBar.valueMax - energyBar.valueMin)));
    }
}