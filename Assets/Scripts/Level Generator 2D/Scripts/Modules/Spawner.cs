using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Spawner : MonoBehaviour
{
    private const float DefaultWeight = 1.0f;
    private const float MinSpawnChance = 0.0f;
    private const float MaxSpawnChance = 1.0f;
    private static readonly Color OutlineColor = Color.yellow;

    [Tooltip("Chances that any GameObject will spawn")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float spawnChance = 1.0f;

    [Tooltip("List of GameObjects which can be spawned")]
    [SerializeField]
    private List<GameObject> spawnables = new List<GameObject>();

    [Tooltip("Weights associated with the spawnable Game Objects")]
    [SerializeField]
    private List<float> weights = new List<float>();

    private Vector2 maxSpawnableSize = Vector2.zero;

    void Start()
    {
        SpawnGameObject();
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        OutlineLargestMaximumSize();
    }

    /// <summary>
    /// Gets the chance a GameObject will be spawned
    /// </summary>
    /// <returns>Chance of spawn</returns>
    public float GetSpawnChance()
    {
        return spawnChance;
    }

    /// <summary>
    /// Sets the chance of a GameObject spawning
    /// </summary>
    /// <param name="spawnChance">Spawn chance</param>
    public void SetSpawnChance(float spawnChance)
    {
        this.spawnChance = spawnChance;
    }

    /// <summary>
    /// Gets the spawnable game objects for this spawner
    /// </summary>
    /// <returns>Spawnable game objects</returns>
    public List<GameObject> GetSpawnables()
    {
        return spawnables;
    }

    /// <summary>
    /// Gets the weights for the spawnable game objects
    /// </summary>
    /// <returns>Weights</returns>
    public List<float> GetWeights()
    {
        return weights;
    }

    /// <summary>
    /// Adds a spawnable Game Object
    /// </summary>
    /// <param name="weight">Rate of game object's spawning</param>
    public void AddSpawnable(GameObject spawnable)
    {
        if (spawnables.Contains(spawnable))
        {
            Debug.LogWarning(gameObject + " already contains " + spawnable.gameObject + ", so it has not been added.", spawnable.gameObject);
            return;
        }
        spawnables.Add(spawnable);
        weights.Add(DefaultWeight);
    }

    /// <summary>
    /// Removes a spawnable game object from the options
    /// </summary>
    /// <param name="spawnable">Spawnable to remove</param>
    public void RemoveSpawnable(GameObject spawnable)
    {
        if (!spawnables.Contains(spawnable))
        {
            Debug.LogError("Attempting to remove " + spawnable + ", but " + gameObject + " does not contain it!", gameObject);
        }
        int index = spawnables.IndexOf(spawnable);
        spawnables.RemoveAt(index);
        weights.RemoveAt(index);
    }

    /// <summary>
    /// Sets the weight for a spawnable game object
    /// </summary>
    /// <param name="spawnable">Spawnable to set weight for</param>
    /// <param name="weight">Weight to set</param>
    public void SetWeight(GameObject spawnable, float weight)
    {
        if (!spawnables.Contains(spawnable))
        {
            Debug.LogError("Attemping to set the weight for " + spawnable + ", but it does not exist in " + gameObject + "!", gameObject);
        }
        int index = spawnables.IndexOf(spawnable);
        weights[index] = weight;
    }

    /// <summary>
    /// Sets the size for the spawnable outline
    /// </summary>
    public void SetMaxSpawnableSize()
    {
        var maxSize = Vector2.one;
        foreach (var spawnable in spawnables)
        {
            var spriteRenderer = spawnable.GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                Vector2 spriteSize = spriteRenderer.bounds.size;
                maxSize = Vector2.Max(spriteSize, maxSize);
            }
            var collider = spawnable.GetComponent<Collider2D>();
            if (collider)
            {
                var colliderSize = collider.bounds.size;
                maxSize = Vector2.Max(colliderSize, maxSize);
            }
        }
        maxSpawnableSize = maxSize;
        SceneView.RepaintAll();
    }

    /// <summary>
    /// Spawns a random game object based on their weights
    /// </summary>
    private void SpawnGameObject()
    {
        float spawnChanceValue = UnityEngine.Random.Range(MinSpawnChance, MaxSpawnChance);
        if (spawnChanceValue > spawnChance)
        {
            return;
        }
        float randomValue = UnityEngine.Random.Range(0.0f, weights.Sum());
        float accumulatedValue = 0.0f;
        for (int i = 0; i < weights.Count; i++)
        {
            accumulatedValue += weights[i];
            if (randomValue < accumulatedValue)
            {
                var spawnablePrefab = spawnables[i];
                Instantiate(spawnablePrefab, transform.position, Quaternion.identity);
                return;
            }
        }
        Debug.LogError(gameObject + " failed to spawn random object!", gameObject);
    }

    /// <summary>
    /// Draws an outline for the largest gameobject's x and largest gameobject's y
    /// </summary>
    private void OutlineLargestMaximumSize()
    {
        Gizmos.color = OutlineColor;
        Gizmos.DrawWireCube(transform.position, maxSpawnableSize);
    }
}