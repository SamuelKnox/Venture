using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRoonDescriptionView : MonoBehaviour
{
    [Tooltip("Container for when there is a roon to describe")]
    [SerializeField]
    private Transform roonDescriptionContainer;

    [Tooltip("Container for when there is no roon to describe")]
    [SerializeField]
    private Transform noRoonDescriptionContainer;

    [Tooltip("Icon for roon")]
    [SerializeField]
    private Image roonIcon;

    [Tooltip("Name of roon")]
    [SerializeField]
    private TextMeshProUGUI roonName;

    [Tooltip("Level of roon")]
    [SerializeField]
    private Image level;

    [Tooltip("General roon description")]
    [SerializeField]
    private TextMeshProUGUI generalDescription;

    [Tooltip("Specific roon description")]
    [SerializeField]
    private TextMeshProUGUI specificDescription;

    [Tooltip("Icon for weapon which roon is attached to")]
    [SerializeField]
    private Image weaponIcon;

    [Tooltip("Name of weapon which roon is attached to")]
    [SerializeField]
    private TextMeshProUGUI weaponName;

    [Tooltip("Selector for icon display for roons by level")]
    [SerializeField]
    public RoonLevelIconSelector roonLevelIconSelector;

    /// <summary>
    /// Sets the full description for the specified roon
    /// </summary>
    /// <param name="roon">Roon to describe</param>
    /// <param name="weapon">Weapon this roon is attached to, or null if there is none</param>
    public void SetDescription(Roon roon, Weapon weapon)
    {
        if (roon)
        {
            roonDescriptionContainer.gameObject.SetActive(true);
            noRoonDescriptionContainer.gameObject.SetActive(false);
            roonIcon.sprite = roon.GetIcon();
            roonName.text = roon.name;
            level.sprite = roonLevelIconSelector.GetIcon(roon.GetLevel());
            generalDescription.text = roon.GetDescription();
            specificDescription.text = roon.GetLevelDescription(roon.GetLevel());
            weaponIcon.enabled = weapon;
            weaponName.enabled = weapon;
            if (weapon)
            {
                weaponIcon.sprite = weapon.GetIcon();
                weaponName.text = weapon.name;
            }
        }
        else
        {
            roonDescriptionContainer.gameObject.SetActive(false);
            noRoonDescriptionContainer.gameObject.SetActive(true);
        }
    }
}