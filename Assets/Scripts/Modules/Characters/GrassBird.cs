using CustomUnityLibrary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrassBird : Enemy
{
    [Tooltip("Destination landing points for grass bird")]
    [SerializeField]
    private List<Vector2> landingPositions = new List<Vector2>();

    [Tooltip("How long the grass bird stays at a landing position")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float landingDuration = 5.0f;

    [Tooltip("Speed at which grass bird flies")]
    [SerializeField]
    [Range(0.0f, 25.0f)]
    private float speed = 10.0f;

    private float totalLandingDuration;
    private bool playerNearby = false;
    private bool flying = false;
    private Vector2 origin;
    private Vector2 playerPosition;
    private Vector2 destination;
    private float flightTime = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        totalLandingDuration = landingDuration;
        origin = transform.position;
        playerPosition = PlayerManager.Player.transform.position;
        destination = transform.position;
        if (landingPositions.Count == 0)
        {
            Debug.LogError("The Grass Bird must have at least one additional landing position!", gameObject);
            return;
        }
        landingPositions.Add(transform.position);
    }

    protected override void Update()
    {
        base.Update();
        if (flying)
        {
            UpdateFlight();
            return;
        }
        if (playerNearby)
        {
            UpdateLandingTime();
        }
        if (landingDuration <= 0.0f)
        {
            flying = true;
            BeginFlight();
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (var landingPosition in landingPositions)
        {
            Gizmos.DrawIcon(landingPosition, GizmoNames.GrassBirdLandingPosition);
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
    /// Updates the amount of time the grass bird is landed
    /// </summary>
    private void UpdateLandingTime()
    {
        landingDuration -= Time.deltaTime;
    }

    /// <summary>
    /// Initiates the grass bird's flight
    /// </summary>
    private void BeginFlight()
    {
        origin = destination;
        playerPosition = PlayerManager.Player.transform.position;
        var potentialDestinations = landingPositions.Where(p => p != origin).ToArray();
        destination = potentialDestinations[Random.Range(0, potentialDestinations.Length)];
        health.SetInvincible(true);
    }

    /// <summary>
    /// Updates the grass bird's flight
    /// </summary>
    private void UpdateFlight()
    {
        float distanceTraveledSqrMagnitude = (destination - playerPosition).sqrMagnitude + (playerPosition - origin).sqrMagnitude;
        float time = flightTime / distanceTraveledSqrMagnitude * speed;
        transform.position = Vector2Utility.Bezier(origin, playerPosition, destination, time);
        flightTime += Time.deltaTime;
        if (Vector2.Distance(transform.position, destination) <= MathUtility.MaxDistanceInaccuracy)
        {
            flying = false;
            FinishFlight();
        }
    }

    /// <summary>
    /// Finishes the grass bird's flight
    /// </summary>
    private void FinishFlight()
    {
        health.SetInvincible(false);
        flightTime = 0.0f;
        landingDuration = totalLandingDuration;
    }
}