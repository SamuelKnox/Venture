using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
public class Wand : Weapon
{
    [Tooltip("Cost in mana to cast a spell")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float manaCost = 1.0f;

    private RangedWeapon rangedWeapon;
    private Mana mana;

    void Awake()
    {
        rangedWeapon = GetComponent<RangedWeapon>();
        mana = transform.root.GetComponentInChildren<Mana>();
        if (!mana)
        {
            Debug.LogError(gameObject + " could not find Mana!", gameObject);
            return;
        }
    }

    /// <summary>
    /// Casts the wand's spell
    /// </summary>
    public void CastSpell()
    {
        if (mana.GetCurrentManaPoints() < manaCost || !rangedWeapon.IsReady())
        {
            return;
        }
        mana.SetCurrentManaPoints(mana.GetCurrentManaPoints() - manaCost);
        var root = transform.root;
        var direction = Mathf.Sign(root.localScale.x) * root.right;
        var projectile = rangedWeapon.Fire(direction, true);
        if (projectile)
        {
            projectile.tag = transform.root.tag;
        }
    }
}