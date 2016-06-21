using UnityEngine;

[RequireComponent(typeof(Pacer))]
public class Executioner : Enemy
{
    private Pacer pacer;

    protected override void Awake()
    {
        base.Awake();
        pacer = GetComponent<Pacer>();
    }

    void OnVisionEnter(Collider2D other)
    {
        if (other.GetComponent<Player>())
        {
            animator.SetTrigger(AnimationNames.Enemy.Triggers.Attack);
            pacer.SetPacingActive(false);
        }
    }

    /// <summary>
    /// Returns to pacing after an interuption
    /// </summary>
    public void ResumePacing()
    {
        pacer.SetPacingActive(true);
    }

    /// <summary>
    /// React to damage being dealt
    /// </summary>
    /// <param name="damage">Damage that was dealt</param>
    protected override void OnDamageDealt(Damage damage)
    {
        base.OnDamageDealt(damage);
        pacer.SetPacingActive(false);
    }

    /// <summary>
    /// Dies
    /// </summary>
    protected override void Die()
    {
        //throw new NotImplementedException();
    }
}