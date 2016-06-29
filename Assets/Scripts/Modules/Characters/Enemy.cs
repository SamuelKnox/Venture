using System;
using UnityEngine;

[RequireComponent(typeof(EnemyView))]
[RequireComponent(typeof(Spawner))]
public abstract class Enemy : Character
{
    private static readonly Vector2 RewardForce = new Vector2(250.0f, 500.0f);

    protected EnemyView enemyView;

    private Spawner spawner;

    protected override void Awake()
    {
        base.Awake();
        enemyView = GetComponent<EnemyView>();
        spawner = GetComponent<Spawner>();
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
        if (spawner)
        {
            var stats = spawner.Spawn();
            foreach (var stat in stats)
            {
                var body2D = stat.GetComponent<Rigidbody2D>();
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