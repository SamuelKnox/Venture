using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpikeTrapView))]
[RequireComponent(typeof(Damage))]
public class SpikeTrap : MonoBehaviour
{
    [Tooltip("How long in seconds after player contact until the trap is deployed")]
    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float trapTime = 0.5f;

    [Tooltip("How long after being deployed before trap retracts")]
    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float retractTime = 1.0f;

    private SpikeTrapView spikeTrapView;
    private Damage damage;

    void Awake()
    {
        spikeTrapView = GetComponent<SpikeTrapView>();
        damage = GetComponent<Damage>();
    }

    void Start()
    {
        damage.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        var player = collider2D.GetComponent<Player>();
        if (!player)
        {
            return;
        }
        StartCoroutine(Deploy());
    }

    /// <summary>
    /// Deploys the trap
    /// </summary>
    /// <returns>Unity required IEnumerator</returns>
    private IEnumerator Deploy()
    {
        yield return new WaitForSeconds(trapTime);
        spikeTrapView.Deploy();
        damage.SetActive(true);
        yield return new WaitForSeconds(retractTime);
        damage.SetActive(false);
        spikeTrapView.Retract();
    }
}