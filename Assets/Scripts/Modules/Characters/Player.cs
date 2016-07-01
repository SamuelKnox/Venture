using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Player : Character
{
    private static readonly ItemType[] WeaponTypes = new ItemType[] { ItemType.MeleeWeapon, ItemType.RangedWeapon };

    [Tooltip("Container for player's weapons")]
    [SerializeField]
    private Transform weaponContainer;

    [Tooltip("Container for player's runes")]
    [SerializeField]
    private Transform runeContainer;

    [Tooltip("Container for player's armor")]
    [SerializeField]
    private Transform armorContainer;

    [Tooltip("Miscellaneous collectables container")]
    [SerializeField]
    private Transform miscellaneousContainer;

    private Inventory inventory;
    private Weapon activeWeapon;

    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<Inventory>();
    }

    void Start()
    {
        SetStartingActiveWeapon();
    }

    /// <summary>
    /// Adds an item to the player's inventory
    /// </summary>
    /// <param name="item">Item to add</param>
    public void Collect(Collectable collectable)
    {
        if (!collectable)
        {
            return;
        }
        var itemContainer = miscellaneousContainer;
        if (collectable.GetComponent<Weapon>())
        {
            itemContainer = weaponContainer;
        }
        else if (collectable.GetComponent<Rune>())
        {
            itemContainer = runeContainer;
        }
        else if (collectable.GetComponent<Armor>())
        {
            throw new NotImplementedException("Armor is not collectable yet.");
        }
        collectable.transform.SetParent(itemContainer);
        collectable.transform.position = itemContainer.position;
        collectable.transform.right *= Mathf.Sign(transform.localScale.x);
        var item = collectable.GetComponent<Item>();
        if (item)
        {
            if (!inventory.Contains(item))
            {
                inventory.Add(item);
            }
            else
            {
                Debug.LogWarning(name + " is attempting to collect " + item.name + ", but it already exists in " + inventory.name + ".", item.gameObject);
            }
            return;
        }
        var resource = collectable.GetComponent<ResourcePool>();
        if (resource)
        {
            var gold = resource.GetComponent<Gold>();
            if (gold)
            {
                AddGold(gold);
            }
        }
    }

    private void AddGold(Gold gold)
    {
        Debug.Log("Collecting gold!");
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
    public void Equip(Equipment equipment)
    {
        if (!inventory.Contains(equipment))
        {
            Debug.LogError("Equipping " + equipment.name + ", but " + inventory + " does not contain it!", inventory.gameObject);
            return;
        }
        var currentlyEquipped = inventory.GetItems(equipment.GetItemType()).Where(e => e.IsEquipped()).FirstOrDefault();
        if (currentlyEquipped)
        {
            currentlyEquipped.SetEquipped(false);
        }
        equipment.SetEquipped(true);
        var weapon = equipment.GetComponent<Weapon>();
        if (weapon)
        {
            SetActiveWeapon(weapon.GetItemType());
        }
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
        var activeWeaponType = activeWeapon.GetItemType();
        var newWeaponIndex = (Array.IndexOf(WeaponTypes, activeWeaponType) + 1) % WeaponTypes.Length;
        if (newWeaponIndex < 0)
        {
            newWeaponIndex = 0;
        }
        var newActiveWeaponType = WeaponTypes[newWeaponIndex];
        SetActiveWeapon(newActiveWeaponType);
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
        var newActiveItem = inventory.GetItems(weaponType).Where(w => w.IsEquipped()).FirstOrDefault();
        if (!newActiveItem)
        {
            Debug.LogError("Attemping to grab the equipped item of type " + weaponType + ", but it does not exist in the inventory!", gameObject);
            return;
        }
        var newActiveWeapon = newActiveItem.GetComponent<Weapon>();
        if (!newActiveWeapon)
        {
            Debug.LogError(newActiveItem + " does not have a Weapon attached!", newActiveItem.gameObject);
            return;
        }
        activeWeapon = newActiveWeapon;
        activeWeapon.gameObject.SetActive(true);
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
    /// Player dies
    /// </summary>
    protected override void Die()
    {
        throw new NotImplementedException("Played death not yet implemented.");
    }

    /// <summary>
    /// Sets the initially active weapon
    /// </summary>
    private void SetStartingActiveWeapon()
    {
        foreach (var weaponType in WeaponTypes)
        {
            if (activeWeapon)
            {
                break;
            }
            SetActiveWeapon(weaponType);
        }
        if (!activeWeapon)
        {
            Debug.LogWarning("There is no active weapon.", gameObject);
        }
    }
}