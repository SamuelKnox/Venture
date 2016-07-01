using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Health health;

    protected virtual void Awake()
    {
        health = GetComponentInChildren<Health>();
        if (!health)
        {
            Debug.LogError(gameObject + " or one of its children must have a Health component!", gameObject);
            return;
        }
    }

    protected virtual void OnEnable()
    {
        health.OnDamageDealt += OnDamageDealt;
    }

    protected virtual void OnDisable()
    {
        health.OnDamageDealt -= OnDamageDealt;
    }

    protected virtual void OnDamageDealt(Damage damage)
    {
        if (health.IsDead())
        {
            Die();
        }
    }

    protected abstract void Die();
}