using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
public class Thrower : Enemy
{
    [Tooltip("Reward thrown")]
    [SerializeField]
    private Projectile reward;

    [Tooltip("Initial reward frequency")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float rewardFrequency = 0.25f;

    [Tooltip("How often the reward frequency will be cut in half.  If 0, the reward frequency will never be cut in half.")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float rewardFrequencyHalflife = 5.0f;

    [Tooltip("Spread of direction the ranged weapon can fire")]
    [SerializeField]
    private Vector2 spread = new Vector2(10.0f, 25.0f);

    private RangedWeapon rangedWeapon;
    private Projectile defaultProjectile;

    protected override void Awake()
    {
        base.Awake();
        rangedWeapon = GetComponent<RangedWeapon>();
        defaultProjectile = rangedWeapon.GetProjectile();
    }

    void Start()
    {
        if (rewardFrequencyHalflife > 0.0f)
        {
            StartCoroutine(ReduceRewardFrequency());
        }
    }

    void FixedUpdate()
    {
        if (stunned)
        {
            return;
        }
        if (Random.Range(0.0f, 1.0f) < rewardFrequency)
        {
            rangedWeapon.SetProjectile(reward);
        }
        else
        {
            rangedWeapon.SetProjectile(defaultProjectile);
        }
        float xDirection = Random.Range(-spread.x, spread.x);
        float yDirection = Random.Range(0.0f, spread.y);
        var direction = new Vector2(xDirection, yDirection);
        rangedWeapon.Fire(direction);
    }

    private IEnumerator ReduceRewardFrequency()
    {
        while (true)
        {
            yield return new WaitForSeconds(rewardFrequencyHalflife);
            rewardFrequency /= 2.0f;
        }
    }
}