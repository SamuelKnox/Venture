public class Bow : RangedWeapon
{
    /// <summary>
    /// Shoots the bow
    /// </summary>
    public void Fire()
    {
        var projectile = base.Fire();
        projectile.tag = transform.root.tag;
    }
}