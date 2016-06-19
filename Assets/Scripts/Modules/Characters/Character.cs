using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class Character : MonoBehaviour
{
    private Health health;

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
    }

    protected virtual void OnEnable()
    {
        health.OnDamageDealt += OnDamageDealt;
    }

    protected virtual void OnDisable()
    {
        health.OnDamageDealt -= OnDamageDealt;
    }

    private void OnDamageDealt(Damage damage)
    {
        if (health.IsDead())
        {
            Die();
        }
    }

    public abstract void Die();
}