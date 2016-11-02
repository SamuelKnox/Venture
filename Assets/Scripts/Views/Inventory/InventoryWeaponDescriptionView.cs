using CustomUnityLibrary;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWeaponDescriptionView : MonoBehaviour
{
    [Tooltip("Name of weapon")]
    [SerializeField]
    private TextMeshProUGUI weaponName;

    [Tooltip("Weapon Icon")]
    [SerializeField]
    private Image icon;

    [Tooltip("Description of weapon")]
    [SerializeField]
    private TextMeshProUGUI generalDescription;

    [Tooltip("Description of weapon's roons")]
    [SerializeField]
    private TextMeshProUGUI specificDescription;

    [Tooltip("Roon sockets used to display weapon roons")]
    [SerializeField]
    private RoonSocket[] roonSockets;

    /// <summary>
    /// Updates the long description for the piece of weapon
    /// </summary>
    /// <param name="weapon">Weapon to get description for</param>
    public void UpdateDescription(Weapon weapon)
    {
        weaponName.text = weapon.name;
        icon.sprite = weapon.GetIcon();
        generalDescription.text = weapon.GetDescription();
        specificDescription.text = "";
        foreach (var roon in weapon.GetRoons())
        {
            specificDescription.text += roon.GetLevelDescription(roon.GetLevel()) + StringUtility.NewLine();
        }
        foreach (var roonSocket in roonSockets)
        {
            bool containsSocket = Array.IndexOf(weapon.GetRoonSocketTypes(), roonSocket.GetRoonType()) >= 0;
            if (containsSocket)
            {
                var roonIcon = roonSocket.GetIcon();
                var roon = weapon.GetRoon(roonSocket.GetRoonType());
                roonIcon.sprite = roon ? roon.GetIcon() : null;
                roonIcon.enabled = roonIcon.sprite;
            }
            roonSocket.gameObject.SetActive(containsSocket);
        }
    }
}