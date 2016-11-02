using UnityEngine;

public class TeleportMage : Enemy
{
    [Tooltip("Destinations where the player can be teleported to")]
    [SerializeField]
    private Vector2[] teleportationDestinations;

    [Tooltip("How often the mage teleports the player")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float teleportationTime = 5.0f;

    private float totalTeleportationTime;
    private bool playerNearby = false;

    protected override void Awake()
    {
        base.Awake();
        if (teleportationDestinations.Length == 0)
        {
            Debug.LogError("Teleport Mage must have at least one teleportation destination!", gameObject);
            return;
        }
        totalTeleportationTime = teleportationTime;
    }

    protected override void Update()
    {
        base.Update();
        if (playerNearby)
        {
            teleportationTime -= Time.deltaTime;
        }
        if (teleportationTime <= 0.0f)
        {
            teleportationTime = totalTeleportationTime;
            TeleportPlayer();
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (var teleportationDestination in teleportationDestinations)
        {
            Gizmos.DrawIcon(teleportationDestination, GizmoNames.Teleport);
        }
    }

    void OnVisionEnter(Collider2D other)
    {
        if (other.GetComponent<Player>())
        {
            playerNearby = true;
        }
    }

    void OnVisionExit(Collider2D other)
    {
        if (other.GetComponent<Player>())
        {
            playerNearby = false;
        }
    }

    /// <summary>
    /// Teleports the player to one of the predetermined positions
    /// </summary>
    private void TeleportPlayer()
    {
        var destination = teleportationDestinations[Random.Range(0, teleportationDestinations.Length)];
        PlayerManager.Player.transform.position = destination;
    }
}