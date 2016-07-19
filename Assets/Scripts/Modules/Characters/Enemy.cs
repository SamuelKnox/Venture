using UnityEngine;

[RequireComponent(typeof(EnemyView))]
[RequireComponent(typeof(Spawner))]
public abstract class Enemy : Character
{
    /// <summary>
    /// Delegate for enemy dying
    /// </summary>
    public delegate void Death(Enemy enemy);

    /// <summary>
    /// Event called when damage is dealt
    /// </summary>
    public static event Death OnDeath;

    private static readonly Vector2 RewardForce = new Vector2(250.0f, 500.0f);

    protected EnemyView enemyView;

    protected override void Awake()
    {
        base.Awake();
        enemyView = GetComponent<EnemyView>();
    }

    /// <summary>
    /// Receives damage
    /// </summary>
    /// <param name="damage">Damage dealt</param>
    protected override void OnDamageDealt(Damage damage)
    {
        base.OnDamageDealt(damage);
        if (health.IsDead())
        {
            enemyView.Die();
        }
        else
        {
            enemyView.Hurt();
        }
    }

    /// <summary>
    /// Death for enemy
    /// </summary>
    protected override void Die()
    {
        if (OnDeath != null)
        {
            OnDeath(this);
        }
        var spawners = GetComponentsInChildren<Spawner>();
        foreach (var spawner in spawners)
        {
            var rewards = spawner.Spawn();
            foreach (var reward in rewards)
            {
                var body2D = reward.GetComponent<Rigidbody2D>();
                if (body2D)
                {
                    body2D.AddForce(new Vector2(UnityEngine.Random.Range(-RewardForce.x, RewardForce.x), UnityEngine.Random.Range(0.0f, RewardForce.y)));
                }
            }
        }
        foreach (var monoBehaviour in GetComponentsInChildren<MonoBehaviour>())
        {
            monoBehaviour.enabled = false;
        }
    }
}