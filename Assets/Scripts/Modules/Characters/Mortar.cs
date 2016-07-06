using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
public class Mortar : Enemy
{
    [Tooltip("Maximum time in seconds that the mortar will wait before attempting to fire a shot")]
    [SerializeField]
    [Range(0.0f, 30.0f)]
    private float maximumWaitTime = 10.0f;

    private RangedWeapon rangedWeapon;
    private Player player;
    private bool firing = false;

    protected override void Awake()
    {
        base.Awake();
        rangedWeapon = GetComponent<RangedWeapon>();
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
    }

    /// <summary>
    /// Lets mortar know that the firing animation is complete
    /// </summary>
    public void FinishedFiring()
    {
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
                enemyView.Attack();
            }
        }
    }
}