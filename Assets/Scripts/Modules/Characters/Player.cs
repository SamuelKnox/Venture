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

    void Awake()
    {
        inventory = GetComponent<Inventory>();
        SetStartingActiveWeapon();
    }

    void Start()
    {
        TurnOffMeleeWeaponColliders();
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
            Transform itemContainer;
            if (item.GetComponent<Weapon>())
            {
                itemContainer = weaponContainer;
            }
            else if (item.GetComponent<Rune>())
            {
                itemContainer = runeContainer;
            }
            item.transform.SetParent(weaponContainer);
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
            Debug.Log("Attempting to begin melee attack, but a Melee Weapon is not the active weapon!", gameObject);
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
        }
        inventory.SetActiveItem(item);
        var weapon = item.GetComponent<Weapon>();
        if (weapon)
        {
            activeWeapon.gameObject.SetActive(false);
            activeWeapon = weapon;
            activeWeapon.gameObject.SetActive(true);
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
    /// Sets the player's currently active weapon
    /// </summary>
    /// <param name="weaponType">Type of weapon to activate</param>
    public void SetActiveWeapon(ItemType weaponType)
    {
        activeWeapon = null;
        SetActiveWeapon(ItemType.MeleeWeapon, weaponType == ItemType.MeleeWeapon);
        SetActiveWeapon(ItemType.Bow, weaponType == ItemType.Bow);
        SetActiveWeapon(ItemType.Wand, weaponType == ItemType.Wand);
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
            case ItemType.Bow:
                var bow = activeWeapon.GetComponent<Bow>();
                if (!bow)
                {
                    Debug.LogError("The item of type " + ItemType.Bow.ToString() + " does not have a Bow component!", activeWeapon.gameObject);
                }
                return bow.IsReady();
            case ItemType.Wand:
                var wand = activeWeapon.GetComponent<Wand>();
                if (!wand)
                {
                    Debug.LogError("The item of type " + ItemType.Wand.ToString() + " does not have a Wand component!", activeWeapon.gameObject);
                }
                return wand.IsReady();
            default:
                Debug.LogError("An invalid WeaponType is active!", activeWeapon.gameObject);
                return false;
        }
    }

    /// <summary>
    /// Disables the colliders for all melee weapons in the player's inventory
    /// </summary>
    private void TurnOffMeleeWeaponColliders()
    {
        var meleeWeapons = inventory.GetItems(ItemType.MeleeWeapon);
        foreach (var meleeWeapon in meleeWeapons)
        {
            var collider = meleeWeapon.GetComponent<BoxCollider2D>();
            collider.enabled = false;
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
            SetActiveWeapon(ItemType.Bow);
        }
        if (!activeWeapon)
        {
            SetActiveWeapon(ItemType.Wand);
        }
        if (!activeWeapon)
        {
            Debug.LogWarning("There is no active weapon.", gameObject);
        }
    }

    /// <summary>
    /// Sets a weapon type to be the player's currently equipped weapon, or disables the weapon type
    /// </summary>
    /// <param name="weaponType">Type of weapon to change</param>
    /// <param name="active">Whether to set the weapon to be the currently equipped weapon or not</param>
    private void SetActiveWeapon(ItemType weaponType, bool active)
    {
        var activeItem = inventory.GetActiveItem(weaponType);
        if (activeItem)
        {
            activeItem.gameObject.SetActive(active);
            if (active)
            {
                activeWeapon = activeItem.GetComponent<Weapon>();
            }
        }
    }
}