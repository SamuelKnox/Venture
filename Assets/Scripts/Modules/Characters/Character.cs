using CustomUnityLibrary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Tooltip("Speed modifier for the Character, where 0.0 is immobile, 0.5 is half speed, 1.0f is normal speed, and 2.0 is double speed.")]
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float speed = 1.0f;

    [Tooltip("Tint applied to the Character's sprite(s)")]
    [SerializeField]
    private Color tint = Color.white;

    private Dictionary<float, float> speedModifiers = new Dictionary<float, float>();
    private Dictionary<Color, float> tintModifiers = new Dictionary<Color, float>();

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

    protected virtual void Start()
    {
        tint = ColorUtility.GetMinColor(tint);
        ApplyTintToSprites();
    }

    protected virtual void Update()
    {
        UpdateSpeed();
        UpdateTint();
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
    public abstract void Die();

    public float GetSpeed()
    {
        return speed;
    }

    /// <summary>
    /// Modifies the character's speed where 0 is stunned, 0.5 is half speed, 1.0 is normal speed, and 2.0 is double speed
    /// </summary>
    /// <param name="intensity">Speed modifier</param>
    /// <param name="duration">Duration in seconds of speed modification</param>
    public void AddSpeedModifer(float intensity, float duration)
    {
        if (intensity == 1.0f || duration <= 0.0f)
        {
            return;
        }
        if (speedModifiers.ContainsKey(intensity))
        {
            speedModifiers[intensity] += duration;
        }
        else
        {
            speedModifiers.Add(intensity, duration);
            speed *= intensity;
        }
    }

    /// <summary>
    /// Adds a tint to the character
    /// </summary>
    /// <param name="tint">Tint to add</param>
    /// <param name="duration">Duration to add tint for</param>
    public void AddTintModifier(Color tint, float duration)
    {
        if (tint == Color.white || duration <= 0.0f)
        {
            return;
        }
        tint = ColorUtility.GetMinColor(tint);
        if (tintModifiers.ContainsKey(tint))
        {
            tintModifiers[tint] += duration;
        }
        else
        {
            tintModifiers.Add(tint, duration);
            this.tint *= tint;
        }
        ApplyTintToSprites();
    }

    /// <summary>
    /// Called when damage is dealt to the character
    /// </summary>
    /// <param name="damage">The damage which is dealt</param>
    protected virtual void OnDamageDealt(Damage damage)
    {
        if (health.IsDead())
        {
            Die();
        }
    }

    /// <summary>
    /// Updates the Character speed to include the speed modifiers
    /// </summary>
    private void UpdateSpeed()
    {
        speedModifiers = speedModifiers.ToDictionary(s => s.Key, s => s.Value - Time.deltaTime);
        foreach (var speedModifier in speedModifiers)
        {
            if (speedModifier.Value <= 0.0f)
            {
                speed /= speedModifier.Key;
            }
        }
        speedModifiers = speedModifiers.Where(s => s.Value > 0.0f).ToDictionary(s => s.Key, s => s.Value);
    }

    /// <summary>
    /// Update the Character's tint to include the tint modifiers
    /// </summary>
    private void UpdateTint()
    {
        bool dirty = false;
        tintModifiers = tintModifiers.ToDictionary(t => t.Key, t => t.Value - Time.deltaTime);
        foreach (var tintModifier in tintModifiers)
        {
            if (tintModifier.Value <= 0.0f)
            {
                dirty = true;
                tint = tint.DivideBy(tintModifier.Key);
            }
        }
        tintModifiers = tintModifiers.Where(t => t.Value > 0.0f).ToDictionary(t => t.Key, t => t.Value);
        if (dirty)
        {
            ApplyTintToSprites();
        }
    }

    /// <summary>
    /// Applies the tint to the Character
    /// </summary>
    private void ApplyTintToSprites()
    {
        var spriteRenderers = transform.root.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = tint;
        }
    }
}