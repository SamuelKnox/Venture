﻿using CustomUnityLibrary;
using UnityEngine;

/// <summary>
/// Ranged Weapons can be used to spawn and fire Projectiles
/// </summary>
[ExecuteInEditMode]
public class RangedWeapon : MonoBehaviour
{
    private const string SpawnPositionGizmoName = "gizmo_ranged_weapon_spawn_position.png";
    private static readonly Color RangeColor = Color.red;

    [Tooltip("Projectile shot by this Ranged Weapon")]
    [SerializeField]
    private Projectile projectile;

    [Tooltip("Speed at which the Projectile fires.  This will be adjusted based on the arch used.")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float force = 10.0f;

    [Tooltip("Rate at which the RangedWeapon can fire")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float fireCooldown = 1.0f;

    [Tooltip("Whether or not this RangedWeapon will fire even if the target is out of its range")]
    [SerializeField]
    private bool unlimitedRange = false;

    [Tooltip("How far this Ranged Weapon can fire, assuming it does not have unlimited range.  It will not fire if its target is beyond this distance.")]
    [SerializeField]
    [Range(1.0f, 100.0f)]
    private float range = 25.0f;

    [Tooltip("Arch to be used when firing.  An arch of 0 or 1 will fire at the full force.")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float arch = 0.5f;

    [Tooltip("Position where the projectiles will spawn")]
    [SerializeField]
    private Vector3 projectileSpawnOffset;

    private float totalCooldown;
    private Vector3 oldRightVector;

    void Awake()
    {
        totalCooldown = fireCooldown;
        oldRightVector = transform.right;
    }

    void Update()
    {
        UpdateProjectileSpawnPosition();
        UpdateFireCooldown();
    }

    void OnValidate()
    {
        if (unlimitedRange)
        {
            range = Mathf.Infinity;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon(transform.position + projectileSpawnOffset, SpawnPositionGizmoName);
        Gizmos.color = RangeColor;
        Gizmos.DrawWireSphere(transform.position + projectileSpawnOffset, range);
    }

    /// <summary>
    /// Fires the equipped Projectile at the target.
    /// </summary>
    /// <param name="target">Target to fire the projectile at</param>
    /// <param name="ignoreGravity">Whether or not to ignore gravity while firing the projectile</param>
    /// <returns>The projectile that was fired, or null if the fire failed</returns>
    public Projectile Fire(Transform target, bool ignoreGravity = false)
    {
        if (ignoreGravity)
        {
            var direction = target.position - transform.position + projectileSpawnOffset;
            return Fire(direction, ignoreGravity);
        }
        if (!IsValidShot(target.position - transform.position))
        {
            return null;
        }
        var projectileInstance = Instantiate(projectile, transform.position + projectileSpawnOffset, Quaternion.identity) as Projectile;
        GameObjectUtility.ChildCloneToContainer(projectileInstance.gameObject);
        projectileInstance.tag = gameObject.tag;
        var projectileBody = projectileInstance.GetRigidBody2D();
        if (!unlimitedRange && !projectileBody.IsWithinRange(target.position, force, arch))
        {
            Destroy(projectileInstance.gameObject);
            return null;
        }
        projectileBody.SetTrajectory(target.position, force, arch);
        DisableProjectileColliders(projectileInstance);
        fireCooldown = totalCooldown;
        return projectileInstance;
    }

    /// <summary>
    /// Check whether the target is within range and the Ranged Weapon is ready to fire
    /// </summary>
    /// <param name="target">Target to aim at</param>
    /// <returns>Whether or not the target is a valid shot</returns>
    public bool IsValidShot(Transform target)
    {
        if (!IsValidShot(target.position - transform.position))
        {
            return false;
        }
        var projectileBody = new GameObject().AddComponent<Rigidbody2D>();
        bool inRange = projectileBody.IsWithinRange(target.position, force, arch);
        Destroy(projectileBody.gameObject);
        return inRange;
    }

    /// <summary>
    /// Fires the equipped Projectile in the direction
    /// </summary>
    /// <param name="direction">The direction to fire the Projectile in</param>
    /// <param name="ignoreGravity">Whether or not to ignore gravity when firing the Projectile</param>
    /// <returns>The projectile that was fired, or null if the fire failed</returns>
    public Projectile Fire(Vector2 direction, bool ignoreGravity = false)
    {
        if (!IsValidShot(direction))
        {
            return null;
        }
        direction.Normalize();
        var projectileInstance = Instantiate(projectile, transform.position + projectileSpawnOffset, Quaternion.identity) as Projectile;
        GameObjectUtility.ChildCloneToContainer(projectileInstance.gameObject);
        projectileInstance.tag = gameObject.tag;
        var projectileBody = projectileInstance.GetRigidBody2D();
        if (ignoreGravity)
        {
            projectileBody.gravityScale = 0;
        }
        if (projectileBody.gravityScale != 0)
        {
            projectileBody.AddForce(direction * force, ForceMode2D.Impulse);
        }
        else
        {
            projectileBody.velocity = direction * force;
        }
        DisableProjectileColliders(projectileInstance);
        fireCooldown = totalCooldown;
        return projectileInstance;
    }

    /// <summary>
    /// Gets the ranged weapon's force
    /// </summary>
    /// <returns>Ranged weapon's force</returns>
    public float GetForce()
    {
        return force;
    }

    /// <summary>
    /// Sets the force used when firing the ranged weapon
    /// </summary>
    /// <param name="force">Force to apply when firing</param>
    public void SetForce(float force)
    {
        this.force = force;
    }

    /// <summary>
    /// Checks if the ranged weapon is ready to be fired, opposed to still being on cooldown
    /// </summary>
    /// <returns>Whether or not the weapon is ready to fire</returns>
    public bool IsReady()
    {
        return fireCooldown <= 0;
    }

    /// <summary>
    /// Gets the time it takes for the ranged weapon to be able to fire again
    /// </summary>
    /// <returns>Cooldown rate</returns>
    public float GetFireCooldown()
    {
        return fireCooldown;
    }

    /// <summary>
    /// Gets the projectile prefab used for the RangedWeapon
    /// </summary>
    /// <returns>Projectile Prefab</returns>
    public Projectile GetProjectile()
    {
        return projectile;
    }

    /// <summary>
    /// Sets the projectile prefab used with this RangedWeapon
    /// </summary>
    /// <param name="projectile">Projectile Prefab</param>
    public void SetProjectile(Projectile projectile)
    {
        if (!projectile)
        {
            Debug.LogError("Cannot set Projectile to be null!", gameObject);
            return;
        }
        this.projectile = projectile;
    }

    /// <summary>
    /// Makes the Projectiles not check for collision with the Ranged Weapon
    /// </summary>
    /// <param name="projectileInstance">Projectile whose collision will be ignored</param>
    private void DisableProjectileColliders(Projectile projectileInstance)
    {
        var rangedWeaponColliders = GetComponents<Collider2D>();
        var projectileColliders = projectileInstance.GetComponents<Collider2D>();
        if (rangedWeaponColliders.Length > 0 && projectileColliders.Length > 0)
        {
            foreach (var rangedWeaponCollider in rangedWeaponColliders)
            {
                foreach (var projectileCollider in projectileColliders)
                {
                    Physics2D.IgnoreCollision(rangedWeaponCollider, projectileCollider);
                }
            }
        }
    }

    /// <summary>
    /// Moves the Projectile to the barrel of the Ranged Weapon
    /// </summary>
    private void UpdateProjectileSpawnPosition()
    {
        if (transform.right == oldRightVector)
        {
            return;
        }
        bool clockwiseRotation = Vector3.Cross(transform.right, oldRightVector).z > 0;
        var angleToRotate = Vector2.Angle(oldRightVector, transform.right);
        if (clockwiseRotation)
        {
            angleToRotate *= -1;
        }
        var updatedDirection = (Quaternion.AngleAxis(angleToRotate, Vector3.forward) * projectileSpawnOffset).normalized;
        projectileSpawnOffset = updatedDirection * projectileSpawnOffset.magnitude;
        oldRightVector = transform.right;
    }

    /// <summary>
    /// Checks to make sure the shot power required is within the capabilities of this Ranged Weapon
    /// </summary>
    /// <param name="direction">The direction whose magnitude will be used</param>
    /// <returns>Whether or not the shot is within range</returns>
    private bool IsValidShot(Vector2 direction)
    {
        if (!projectile)
        {
            Debug.LogWarning(name + " is missing a Projectile.");
            return false;
        }
        if (!unlimitedRange && direction.magnitude > range || fireCooldown > 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Updates the cooldown rate of the RangedWeapon
    /// </summary>
    private void UpdateFireCooldown()
    {
        fireCooldown -= Time.deltaTime;
        fireCooldown = Mathf.Max(0, fireCooldown);
    }
}