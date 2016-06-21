using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Player : Character
{
    [Tooltip("Container for player's weapons")]
    [SerializeField]
    private Transform weaponContainer;

    [Tooltip("Container for player's runes")]
    [SerializeField]
    private Transform runeContainer;

    private Inventory inventory;
    private Weapon activeWeapon;

    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<Inventory>();
        SetStartingActiveWeapon();
    }

    /// <summary>
    /// Adds an item to the player's inventory
    /// </summary>
    /// <param name="item">Item to add</param>
    public void CollectItem(Item item)
    {
        if (!item)
        {
            return;
        }
        if (!inventory.Contains(item))
        {
            Transform itemContainer = null;
            if (item.GetComponent<Weapon>())
            {
                itemContainer = weaponContainer;
            }
            else if (item.GetComponent<Rune>())
            {
                itemContainer = runeContainer;
            }
            item.transform.SetParent(itemContainer);
            item.transform.position = weaponContainer.transform.position;
            item.transform.right *= Mathf.Sign(transform.localScale.x);
            inventory.Add(item);
        }
        else
        {
            Debug.LogWarning(name + " is attempting to collect " + item.name + ", but it already exists in " + inventory.name + ".", item.gameObject);
        }
    }

    /// <summary>
    /// Swings the melee weapon
    /// </summary>
    public void BeginMeleeAttack()
    {
        var meleeWeapon = activeWeapon.GetComponent<MeleeWeapon>();
        if (!meleeWeapon)
        {
            Debug.Log("Attempting to begin melee attack, but a Melee Weapon is not the active weapon!", gameObject);
            return;
        }
        meleeWeapon.BeginSwing();
    }

    /// <summary>
    /// Finishes the melee weaponn swing
    /// </summary>
    public void FinishMeleeAttack()
    {
        var meleeWeapon = activeWeapon.GetComponent<MeleeWeapon>();
        if (!meleeWeapon)
        {
            Debug.Log("Attempting to finish melee attack, but a Melee Weapon is not the active weapon!", gameObject);
            return;
        }
        meleeWeapon.FinishSwing();
    }

    /// <summary>
    /// Fires the player's bow
    /// </summary>
    public void FireBow()
    {
        var bow = activeWeapon.GetComponent<Bow>();
        if (!bow)
        {
            Debug.LogError("Attempting to fire a Bow, but it is not the active weapon!", gameObject);
            return;
        }
        bow.Fire(transform.root.right);
    }

    /// <summary>
    /// Casts a spell based on the player's currently equipped wand
    /// </summary>
    public void CastSpell()
    {
        var wand = activeWeapon.GetComponent<Wand>();
        if (!wand)
        {
            Debug.LogError("Attempting to use a Wand, but it is not the active weapon!", gameObject);
            return;
        }
        wand.CastSpell();
    }

    /// <summary>
    /// Equips the specified item
    /// </summary>
    /// <param name="item">Item to equip</param>
    public void Equip(Item item)
    {
        if (!inventory.Contains(item))
        {
            Debug.LogError("Equipping " + item.name + ", but " + inventory + " does not contain it!", inventory.gameObject);
            return;
        }
        inventory.SetActiveItem(item);
        SetActiveWeapon(item.GetItemType());
    }

    /// <summary>
    /// Gets the player's inventory
    /// </summary>
    /// <returns>Player's inventory</returns>
    public Inventory GetInventory()
    {
        return inventory;
    }

    /// <summary>
    /// Gets the player's active weapon
    /// </summary>
    /// <returns>Currently Active Weapon</returns>
    public Weapon GetActiveWeapon()
    {
        return activeWeapon;
    }

    /// <summary>
    /// Toggles between melee and ranged weapon
    /// </summary>
    public void ToggleWeapon()
    {
        if (activeWeapon.GetComponent<MeleeWeapon>())
        {
            SetActiveWeapon(ItemType.RangedWeapon);
        }
        else if (activeWeapon.GetComponent<RangedWeapon>())
        {
            SetActiveWeapon(ItemType.MeleeWeapon);
        }
        else
        {
            Debug.LogError("Neither " + ItemType.MeleeWeapon.ToString() + " nor " + ItemType.RangedWeapon.ToString() + " are equipped!", gameObject);
            return;
        }
    }

    /// <summary>
    /// Sets a weapon type to be the player's currently equipped weapon, or disables the weapon type
    /// </summary>
    /// <param name="weaponType">Type of weapon to change</param>
    public void SetActiveWeapon(ItemType weaponType)
    {
        if (activeWeapon)
        {
            activeWeapon.gameObject.SetActive(false);
        }
        if (!inventory.GetActiveItem(weaponType))
        {
            return;
        }
        var meleeWeapon = inventory.GetActiveItem(ItemType.MeleeWeapon);
        if (meleeWeapon)
        {
            if (weaponType == ItemType.MeleeWeapon)
            {
                meleeWeapon.gameObject.SetActive(true);
                activeWeapon = meleeWeapon.GetComponent<Weapon>();
            }
        }
        var rangedWeapon = inventory.GetActiveItem(ItemType.RangedWeapon);
        if (rangedWeapon)
        {
            if (weaponType == ItemType.RangedWeapon)
            {
                rangedWeapon.gameObject.SetActive(true);
                activeWeapon = rangedWeapon.GetComponent<Weapon>();
            }
        }
    }

    /// <summary>
    /// Checks whether or not the player is able to perform an attack
    /// </summary>
    /// <returns>If the attack is valid</returns>
    public bool IsAttackValid()
    {
        if (!activeWeapon)
        {
            return false;
        }
        var activeWeaponType = activeWeapon.GetItemType();
        switch (activeWeaponType)
        {
            case ItemType.MeleeWeapon:
                return true;
            case ItemType.RangedWeapon:
                var rangedWeapon = activeWeapon.GetComponent<RangedWeapon>();
                if (!rangedWeapon)
                {
                    Debug.LogError("The item of type " + ItemType.RangedWeapon.ToString() + " does not have a Ranged Weapon Component!", activeWeapon.gameObject);
                    return false;
                }
                return rangedWeapon.IsReady();
            default:
                Debug.LogError("An invalid WeaponType is active!", activeWeapon.gameObject);
                return false;
        }
    }

    /// <summary>
    /// Sets the initially active weapon
    /// </summary>
    private void SetStartingActiveWeapon()
    {
        if (!activeWeapon)
        {
            SetActiveWeapon(ItemType.MeleeWeapon);
        }
        if (!activeWeapon)
        {
            SetActiveWeapon(ItemType.RangedWeapon);
        }
        if (!activeWeapon)
        {
            Debug.LogWarning("There is no active weapon.", gameObject);
        }
    }

    /// <summary>
    /// Player dies
    /// </summary>
    protected override void Die()
    {
        Debug.Log("Player died.");
        throw new NotImplementedException();
    }
}