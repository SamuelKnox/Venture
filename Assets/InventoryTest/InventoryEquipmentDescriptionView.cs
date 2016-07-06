using System;
using TMPro;
using UnityEngine;

public class InventoryEquipmentDescriptionView : MonoBehaviour
{
    [Tooltip("Text used to display the detailed equipment description")]
    [SerializeField]
    private TextMeshProUGUI description;

    [Tooltip("Description when there is no item to describe")]
    [SerializeField]
    private string noEquipmentDescription;

    void Awake()
    {
        if (string.IsNullOrEmpty(noEquipmentDescription))
        {
            Debug.LogWarning("There is no equipment description to choose from if no equipment is selected.", gameObject);
            return;
        }
    }

    /// <summary>
    /// Updates the long description for the piece of equipment
    /// </summary>
    /// <param name="equipment">Equipment to get description for</param>
    public void UpdateDescription(Equipment equipment)
    {
        var equipmentDescription = equipment ? equipment.GetDescription() : noEquipmentDescription;
        description.text = equipmentDescription;
    }
}