using System.Collections;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Health health;
    protected bool stunned = false;

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

    /// <summary>
    /// Called when character dies
    /// </summary>
    protected abstract void Die();

    /// <summary>
    /// Calls when character is stunned
    /// </summary>
    protected abstract void EnableStun();

    /// <summary>
    /// Called when character is no longer stunned
    /// </summary>
    protected abstract void DisableStun();

    /// <summary>
    /// Called when damage is dealt to the character
    /// </summary>
    /// <param name="damage">The damage which is dealt</param>
    protected virtual void OnDamageDealt(Damage damage)
    {
        if (Player.GetStunTime() > 0 && GetComponent<Player>())
        {
            Stun(Player.GetStunTime());
        }
        else if (Enemy.GetStunTime() > 0 && GetComponent<Enemy>())
        {
            Stun(Enemy.GetStunTime());
        }
        if (health.IsDead())
        {
            Die();
        }
    }

    /// <summary>
    /// Stuns a character for the specified number of seconds
    /// </summary>
    /// <param name="duration">Stun duration in seconds</param>
    private void Stun(float duration)
    {
        StartCoroutine(PerformStun(duration));
    }

    /// <summary>
    /// Enables and disables the characters stun
    /// </summary>
    /// <param name="duration">Duration to enable the stun for</param>
    /// <returns>Required IEnumerator</returns>
    private IEnumerator PerformStun(float duration)
    {
        EnableStun();
        stunned = true;
        yield return new WaitForSeconds(duration);
        stunned = false;
        DisableStun();
    }
}