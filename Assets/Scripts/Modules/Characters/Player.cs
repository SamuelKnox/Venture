using CustomUnityLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(PlayerView))]
public class Player : Character
{
    private const int PrestigePerLevel = 1;
    private const int GoldPerLevel = 100;
    private static readonly ItemType[] WeaponTypes = new ItemType[] { ItemType.MeleeWeapon, ItemType.RangedWeapon };

    [Tooltip("How many prestige points the player has")]
    [SerializeField]
    [Range(0, 1000)]
    private int prestige = 0;

    [Tooltip("How much gold the player has")]
    [SerializeField]
    [Range(0, 100000)]
    private int gold = 0;

    [Tooltip("The player's level, which is based on prestige")]
    [SerializeField]
    [Range(1, 1000)]
    private float level = 1;

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

    [Tooltip("Container for all of the player's quests")]
    [SerializeField]
    private Transform questContainer;

    [Tooltip("Player's currently active quests")]
    [SerializeField]
    private List<Quest> activeQuests = new List<Quest>();

    [Tooltip("Player's completed quests")]
    [SerializeField]
    private List<Quest> completedQuests = new List<Quest>();

    private Inventory inventory;
    private PlayerView playerView;
    private Weapon activeWeapon;

    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<Inventory>();
        playerView = GetComponent<PlayerView>();
    }

    void Start()
    {
        SetStartingActiveWeapon();
    }

    /// <summary>
    /// Adds a quest to the player's active quests
    /// </summary>
    /// <param name="quest">Quest to add</param>
    public bool AddQuest(Quest quest)
    {
        if (activeQuests.Contains(quest))
        {
            return false;
        }
        activeQuests.Add(quest);
        quest.SetActiveQuest(true);
        quest.transform.SetParent(questContainer);
        quest.transform.position = transform.position;
        quest.OnQuestComplete += OnQuestComplete;
        return true;
    }

    /// <summary>
    /// Called when quest is complete
    /// </summary>
    /// <param name="quest">Quest which was completed</param>
    private void OnQuestComplete(Quest quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        prestige += quest.GetPrestige();
        quest.OnQuestComplete -= OnQuestComplete;
    }

    /// <summary>
    /// Gets the amount of prestige the player has
    /// </summary>
    /// <returns>Prestige amount</returns>
    public int GetPrestige()
    {
        return prestige;
    }

    /// <summary>
    /// Adds prestige to player's resources, and increases level
    /// </summary>
    /// <param name="prestige">Prestige to add</param>
    public void AddPrestige(int prestige)
    {
        this.prestige += prestige;
        level += (float)prestige / PrestigePerLevel;
    }

    /// <summary>
    /// Spends prestige if the player has enough
    /// </summary>
    /// <param name="prestige">Whether or not the player had enough prestige</param>
    public void SpendPrestige(int prestige)
    {
        if (this.prestige < prestige)
        {
            Debug.LogError("Attempting to spend more prestige than the player has!", gameObject);
            return;
        }
        this.prestige -= prestige;
    }

    /// <summary>
    /// Gets the amount of gold the player has
    /// </summary>
    /// <returns>Gold amount</returns>
    public int GetGold()
    {
        return gold;
    }

    /// <summary>
    /// Adds gold to player's resources, and increases level
    /// </summary>
    /// <param name="gold">Gold to add</param>
    public void AddGold(int gold)
    {
        this.gold += gold;
        level += (float)gold / GoldPerLevel;
    }

    /// <summary>
    /// Spends gold if the player has enough
    /// </summary>
    /// <param name="gold">Whether or not the player had enough gold</param>
    public void SpendGold(int gold)
    {
        if (this.gold < gold)
        {
            Debug.LogError("Attempting to spend more gold than the player has!", gameObject);
            return;
        }
        this.gold -= gold;
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
            switch (resource.GetResourceType())
            {
                case ResourceType.Gold:
                    AddGold(resource.GetAmount());
                    break;
                case ResourceType.Prestige:
                    AddPrestige(resource.GetAmount());
                    break;
                default:
                    Debug.LogError("An invalid resource type was provided!", gameObject);
                    return;
            }
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
                bool ready = rangedWeapon.IsReady();
                if (!ready)
                {
                    Debug.LogWarning(rangedWeapon + "'s animation is done, and it is trying to attack, but it is still on cooldown.", rangedWeapon.gameObject);
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
        playerView.Die();
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