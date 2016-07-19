using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEquipmentDescriptionView : MonoBehaviour
{
    [Tooltip("Name of equipment")]
    [SerializeField]
    private TextMeshProUGUI equipmentName;

    [Tooltip("Equipment Icon")]
    [SerializeField]
    private Image icon;

    [Tooltip("Description of equipment")]
    [SerializeField]
    private TextMeshProUGUI generalDescription;

    [Tooltip("Description of equipment's runes")]
    [SerializeField]
    private TextMeshProUGUI specificDescription;

    [Tooltip("Rune sockets used to display equipment runes")]
    [SerializeField]
    private RuneSocket[] runeSockets;

    /// <summary>
    /// Updates the long description for the piece of equipment
    /// </summary>
    /// <param name="equipment">Equipment to get description for</param>
    public void UpdateDescription(Equipment equipment)
    {
        equipmentName.text = equipment.name;
        icon.sprite = equipment.GetIcon();
        generalDescription.text = equipment.GetDescription();
        specificDescription.text = "";
        foreach (var rune in equipment.GetRunes())
        {
            specificDescription.text += rune.GetLevelDescription(rune.GetLevel()) + StringUtility.NewLine();
        }
        foreach (var runeSocket in runeSockets)
        {
            bool containsSocket = Array.IndexOf(equipment.GetRuneSocketTypes(), runeSocket.GetRuneType()) >= 0;
            if (containsSocket)
            {
                var runeIcon = runeSocket.GetIcon();
                var rune = equipment.GetRune(runeSocket.GetRuneType());
                runeIcon.sprite = rune ? rune.GetIcon() : null;
                runeIcon.enabled = runeIcon.sprite;
            }
            runeSocket.gameObject.SetActive(containsSocket);
        }
    }
}