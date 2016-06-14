using UnityEngine;

public class Wand : RangedWeapon
{
    /// <summary>
    /// Casts the wand's spell
    /// </summary>
    public void CastSpell()
    {
        var root = transform.root;
        var direction = Mathf.Sign(root.localScale.x) * root.right;
        var projectile = Fire(direction, true);
        if (projectile)
        {
            projectile.tag = transform.root.tag;
        }
    }
}