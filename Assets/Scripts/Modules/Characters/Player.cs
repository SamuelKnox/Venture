using CreativeSpore.SmartColliders;
using CustomUnityLibrary;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(PlatformCharacterController))]
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

    [Tooltip("How much oxygen percent the player has remaining")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float oxygen;

    [Tooltip("The player's level, which is based on prestige")]
    [SerializeField]
    [Range(1, 1000)]
    private float level = 1;

    [Tooltip("Container for player's weapons")]
    [SerializeField]
    private Transform weaponContainer;

    [Tooltip("Container for player's roons")]
    [SerializeField]
    private Transform roonContainer;

    [Tooltip("Container for all of the player's quests")]
    [SerializeField]
    private Transform questContainer;

    [Tooltip("Handle for melee weapons when sheathed")]
    [SerializeField]
    private SpriteRenderer meleeWeaponHandle;

    [Tooltip("View used to display Oxygen consumption")]
    [SerializeField]
    private OxygenView oxygenView;

    [Tooltip("Rate at which player consumes oxygen (by percentage per second)")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float oxygenConsumptionRate = 0.01f;

    [Tooltip("How long (in seconds) the player can fall before taking damage")]
    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float maxFallTimeWithoutDamage = 3.0f;

    [Tooltip("Damage dealt per second of falling after max fall time has been reached")]
    [SerializeField]
    [Range(1.0f, 100.0f)]
    private float fallDamagePerSecond = 10.0f;

    private PlatformCharacterController platformCharacterController;
    private Inventory inventory;
    private PlayerView playerView;
    private Weapon activeWeapon;
    private QuestsView questsView;
    private float fallCounter = 0.0f;
    private float bowEffectiveness = 1.0f;
    private bool wandCharged = false;

    protected override void Awake()
    {
        base.Awake();
        platformCharacterController = GetComponent<PlatformCharacterController>();
        inventory = GetComponent<Inventory>();
        playerView = GetComponent<PlayerView>();
        AddItemsToInventory();
        questsView = FindObjectOfType<QuestsView>();
        if (!questsView)
        {
            Debug.LogError("Could not find quests view!", gameObject);
            return;
        }
    }

    protected override void Start()
    {
        base.Start();
        DestroyDuplicateUniqueItems();
        SetStartingActiveWeapon();
    }

    protected override void Update()
    {
        base.Update();
        ApplyFallDamage();
    }

    /// <summary>
    /// Updates the player to be overritten with the saved data
    /// </summary>
    public void Load()
    {
        prestige = SaveData.LoadPrestige();
        gold = SaveData.LoadGold();
        level = SaveData.LoadLevel();
        LoadItems();
        LoadQuests();
    }

    /// <summary>
    /// Saves the player
    /// </summary>
    public void Save()
    {
        SaveData.SavePrestige(prestige);
        SaveData.SaveGold(gold);
        SaveData.SaveLevel(level);
        SaveItems();
        SaveData.SaveQuests(GetComponentsInChildren<Quest>());
    }

    /// <summary>
    /// Gets all of the player's weapons
    /// </summary>
    /// <returns>Weapons on the player</returns>
    public Weapon[] GetWeapons()
    {
        return GetComponentsInChildren<Weapon>(true);
    }

    /// <summary>
    /// Gets all of the player's quests
    /// </summary>
    /// <returns>Quests in the player's quest container</returns>
    public Quest[] GetQuests()
    {
        return questContainer.GetComponentsInChildren<Quest>(true);
    }

    /// <summary>
    /// Gets all of the player's roons
    /// </summary>
    /// <returns>Roons in the player's roon container</returns>
    public Roon[] GetRoons()
    {
        return roonContainer.GetComponentsInChildren<Roon>(true);
    }

    /// <summary>
    /// Adds a quest to the player's active quests
    /// </summary>
    /// <param name="quest">Quest to add</param>
    public bool AddQuest(Quest quest)
    {
        if (questContainer.GetComponentsInChildren<Quest>().Contains(quest))
        {
            return false;
        }
        quest.transform.SetParent(questContainer);
        quest.transform.position = transform.position;
        quest.OnQuestComplete += OnQuestComplete;
        questsView.UpdateQuests();
        return true;
    }

    /// <summary>
    /// Gets the player's current level
    /// </summary>
    /// <returns>Player level</returns>
    public float GetLevel()
    {
        return level;
    }

    /// <summary>
    /// Overwrites the player's level
    /// </summary>
    /// <param name="level">Level to use</param>
    public void SetLevel(float level)
    {
        this.level = level;
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
    /// Overwrites the player's prestige.  If adding prestige, player.AddPrestige(int) should be used instead.
    /// </summary>
    /// <param name="prestige">Prestige to set</param>
    public void SetPrestige(int prestige)
    {
        this.prestige = prestige;
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
    /// Overwrites the player's gold.  If adding gold, player.AddGold(int) should be used instead.
    /// </summary>
    /// <param name="gold">Gold to set</param>
    public void SetGold(int gold)
    {
        this.gold = gold;
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
    /// Collect an item and add it to the inventory
    /// </summary>
    /// <param name="collectable">Collectable to add</param>
    public void Collect(Collectable collectable)
    {
        if (!collectable)
        {
            return;
        }
        var weapon = collectable.GetComponent<Weapon>();
        if (weapon && inventory.Find(weapon.name.TrimEnd(GameObjectUtility.CloneSuffix)))
        {
            Destroy(weapon.gameObject);
            return;
        }
        Transform itemContainer = null;
        if (collectable.GetComponent<Weapon>())
        {
            itemContainer = weaponContainer;
        }
        else if (collectable.GetComponent<Roon>())
        {
            itemContainer = roonContainer;
        }
        collectable.transform.SetParent(itemContainer);
        if (itemContainer)
        {
            collectable.transform.position = itemContainer.position;
            collectable.transform.right *= Mathf.Sign(transform.localScale.x);
        }
        var item = collectable.GetComponent<Item>();
        if (item)
        {
            if (!inventory.Contains(item))
            {
                inventory.Add(item);
                item.transform.localPosition = Vector2.zero;
            }
            else
            {
                Debug.LogWarning(name + " is attempting to collect " + item.name + ", but it already exists in " + inventory.name + ".", item.gameObject);
            }
        }
        var consumable = collectable.GetComponent<Consumable>();
        if (consumable)
        {
            consumable.Consume();
        }
        playerView.Collect(collectable);
        if (consumable)
        {
            Destroy(consumable.gameObject);
        }
        DestroyDuplicateUniqueItems();
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
    /// Sets the effectiveness of the player's bow, where 1 is full power, 0.5 is half power, and 0 is no power
    /// </summary>
    /// <param name="effectiveness">Percentage of bow's effectiveness</param>
    public void SetBowEffectiveness(float effectiveness)
    {
        bowEffectiveness = effectiveness;
    }

    /// <summary>
    /// Sets whether or not the player's wand is charged.  If it is charged, it will cast the wand's spell.  If it is not charged, it will cast a projectile.
    /// </summary>
    /// <param name="charged">Whether or not the wand is charged.</param>
    public void SetWandCharged(bool charged)
    {
        wandCharged = charged;
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
        float x = Input.GetAxis(InputNames.Horizontal);
        float y = Input.GetAxis(InputNames.Vertical);
        var direction = new Vector2(x, y);
        if (direction == Vector2.zero)
        {
            direction = new Vector2(GetFacingDirection(), 0);
        }
        var rangedWeapon = bow.GetComponent<RangedWeapon>();
        if (!rangedWeapon)
        {
            Debug.LogError("Bow could not find ranged weapon!", bow.gameObject);
            return;
        }
        float fullPower = rangedWeapon.GetForce();
        rangedWeapon.SetForce(fullPower * bowEffectiveness);
        var projectile = bow.Fire(direction);
        var projectileDamage = projectile.GetDamage();
        projectileDamage.Modify(bowEffectiveness);
        rangedWeapon.SetForce(fullPower);
    }

    /// <summary>
    /// Gets the direction the player is facing, where -1.0 is left and 1.0 is right.
    /// </summary>
    /// <returns>Direction player is facing</returns>
    public float GetFacingDirection()
    {
        return Mathf.Sign(transform.localScale.x);
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
        float x = Input.GetAxis(InputNames.Horizontal);
        float y = Input.GetAxis(InputNames.Vertical);
        var direction = new Vector2(x, y);
        if (direction == Vector2.zero)
        {
            direction = new Vector2(GetFacingDirection(), 0);
        }
        direction.Normalize();
        if (wandCharged)
        {
            wand.CastSpell(direction);
        }
        else
        {
            wand.CastProjectile(direction);
        }
    }

    /// <summary>
    /// Equips the specified item
    /// </summary>
    /// <param name="item">Item to equip</param>
    public void Equip(Weapon weapon)
    {
        if (!inventory.Contains(weapon))
        {
            Debug.LogError("Equipping " + weapon.name + ", but " + inventory + " does not contain it!", inventory.gameObject);
            return;
        }
        var currentlyEquipped = inventory.GetItems(weapon.GetItemType()).Where(e => e.IsEquipped()).FirstOrDefault();
        if (currentlyEquipped)
        {
            currentlyEquipped.SetEquipped(false);
        }
        weapon.SetEquipped(true);
        SetActiveWeapon(weapon.GetItemType());
        var meleeWeapon = weapon.GetComponent<MeleeWeapon>();
        if (meleeWeapon)
        {
            meleeWeaponHandle.sprite = meleeWeapon.GetHandle();
        }
        playerView.EquipWeapon(weapon);
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
        playerView.EquipWeapon(activeWeapon);
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
        bool meleeWeapon = activeWeapon.GetComponent<MeleeWeapon>();
        meleeWeaponHandle.gameObject.SetActive(!meleeWeapon);
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

    public void RefillOxygen()
    {
        oxygen = 1.0f;
        oxygenView.gameObject.SetActive(false);
    }

    /// <summary>
    /// Reduces the remainin amount of oxygen the player has
    /// </summary>
    public void ConsumeOxygen()
    {
        oxygenView.gameObject.SetActive(true);
        if (oxygen <= 0.0f)
        {
            health.SetCurrentHitPoints(health.GetCurrentHitPoints() - health.GetMaxHitPoints() * oxygenConsumptionRate * Time.deltaTime);
        }
        else
        {
            oxygen -= oxygenConsumptionRate * Time.deltaTime;
            oxygen = Mathf.Max(0.0f, oxygen);
            oxygenView.SetOxygen(oxygen);
        }
    }

    /// <summary>
    /// Player dies
    /// </summary>
    public override void Die()
    {
        playerView.Die();
    }

    /// <summary>
    /// Deals damage to the player if they fall for long enough
    /// </summary>
    private void ApplyFallDamage()
    {
        if (platformCharacterController.InstantVelocity.y < 0)
        {
            fallCounter += Time.deltaTime;
        }
        else
        {
            if (fallCounter >= maxFallTimeWithoutDamage)
            {
                float damage = (fallCounter - maxFallTimeWithoutDamage) * fallDamagePerSecond;
                health.SetCurrentHitPoints(health.GetCurrentHitPoints() - damage);
            }
            fallCounter = 0.0f;
        }
    }

    /// <summary>
    /// Destroys duplicate items if they are unique and the player has it in their inventory
    /// </summary>
    private void DestroyDuplicateUniqueItems()
    {
        foreach (var item in FindObjectsOfType<Item>())
        {
            if (!item.IsUnique())
            {
                continue;
            }
            if (!inventory.Contains(item) && inventory.Find(item.name))
            {
                Debug.LogWarning("Destroying " + item.name + ", because the player already has it equipped.", gameObject);
                Destroy(item.gameObject);
            }
        }
    }

    /// <summary>
    /// Called when quest is complete
    /// </summary>
    /// <param name="quest">Quest which was completed</param>
    private void OnQuestComplete(Quest quest)
    {
        prestige += quest.GetPrestige();
        quest.OnQuestComplete -= OnQuestComplete;
        questsView.UpdateQuests();
    }

    /// <summary>
    /// Sets the initially active weapon
    /// </summary>
    private void SetStartingActiveWeapon()
    {
        activeWeapon = null;
        foreach (var weapon in weaponContainer.GetComponentsInChildren<Weapon>())
        {
            weapon.gameObject.SetActive(false);
        }
        foreach (var weaponType in WeaponTypes)
        {
            if (!activeWeapon)
            {
                SetActiveWeapon(weaponType);
            }
        }
        if (!activeWeapon)
        {
            Debug.LogWarning("There is no active weapon.", gameObject);
        }
        for (int i = 0; i < WeaponTypes.Length; i++)
        {
            ToggleWeapon();
        }
        var meleeWeapon = activeWeapon.GetComponent<MeleeWeapon>();
        if (meleeWeapon)
        {
            meleeWeaponHandle.sprite = meleeWeapon.GetHandle();
        }
    }

    /// <summary>
    /// Add pre-existing items to player inventory
    /// </summary>
    private void AddItemsToInventory()
    {
        foreach (var item in weaponContainer.GetComponentsInChildren<Item>(true))
        {
            inventory.Add(item);
        }
        foreach (var item in roonContainer.GetComponentsInChildren<Item>(true))
        {
            inventory.Add(item);
        }
        foreach (var item in questContainer.GetComponentsInChildren<Item>(true))
        {
            inventory.Add(item);
        }
    }

    /// <summary>
    /// Loads and equips the player's items
    /// </summary>
    private void LoadItems()
    {
        var items = SaveData.LoadItems();
        foreach (var item in items)
        {
            var weapon = item.GetComponent<Weapon>();
            if (weapon)
            {
                var existingWeapon = inventory.Find(weapon.name);
                if (existingWeapon)
                {
                    inventory.Remove(existingWeapon, true);
                }
            }
            Collect(item.GetComponent<Collectable>());
            if (weapon && weapon.IsEquipped())
            {
                Equip(weapon);
            }
            Transform container = null;
            switch (item.GetItemType())
            {
                case ItemType.MeleeWeapon:
                    container = weaponContainer;
                    break;
                case ItemType.RangedWeapon:
                    container = weaponContainer;
                    break;
                case ItemType.Roon:
                    container = roonContainer;
                    break;
                default:
                    Debug.LogError("Invalid item type provided!", gameObject);
                    return;
            }
            item.transform.SetParent(container);
        }
        SetStartingActiveWeapon();
    }

    /// <summary>
    /// Saves the player's items
    /// </summary>
    private void SaveItems()
    {
        var items = inventory.GetItems();
        var weapons = items.Where(i => i.GetComponent<Weapon>()).Select(e => e.GetComponent<Weapon>());
        foreach (var weapon in weapons)
        {
            weapon.DeactivateRoons();
        }
        SaveData.SaveItems(inventory.GetItems());
    }

    /// <summary>
    /// Loads the player's quests
    /// </summary>
    private void LoadQuests()
    {
        var quests = SaveData.LoadQuests();
        foreach (var quest in quests)
        {

            if (!quest.IsComplete())
            {
                quest.OnQuestComplete += OnQuestComplete;
            }
            quest.transform.SetParent(questContainer);
        }
        questsView.UpdateQuests();
    }
}