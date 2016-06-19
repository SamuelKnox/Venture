using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
[RequireComponent(typeof(Animator))]
public class Mortar : Enemy
{
    [Tooltip("Maximum time in seconds that the mortar will wait before attempting to fire a shot")]
    [SerializeField]
    [Range(0.0f, 30.0f)]
    private float maximumWaitTime = 10.0f;

    private RangedWeapon rangedWeapon;
    private Animator animator;
    private Player player;
    private bool firing = false;

    protected override void Awake()
    {
        base.Awake();
        rangedWeapon = GetComponent<RangedWeapon>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        StartCoroutine(HoldFire());
    }

    /// <summary>
    /// Fires the ranged weapon
    /// </summary>
    public void FireAtPlayer()
    {
        rangedWeapon.Fire(player.transform);
        firing = false;
    }

    /// <summary>
    /// Triggers animation to fire after a delay
    /// </summary>
    /// <returns>Waiting for next fire</returns>
    private IEnumerator HoldFire()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, maximumWaitTime));
            if (!firing && rangedWeapon.IsValidShot(player.transform))
            {
                firing = true;
                animator.SetTrigger(AnimationNames.Mortar.Triggers.Fire);
            }
        }
    }

    /// <summary>
    /// Causes mortar to die
    /// </summary>
    public override void Die()
    {
        animator.SetTrigger(AnimationNames.Mortar.Triggers.Die);
        var damage = GetComponent<Damage>();
        if (damage)
        {
            Destroy(damage);
        }
    }
}