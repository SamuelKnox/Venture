using CustomUnityLibrary;
using LevelGenerator2D;
using UnityEngine;

[RequireComponent(typeof(Spawner))]
public abstract class Quest : MonoBehaviour
{
    private const int RewardCollectablePriority = 5;
    private static readonly Vector2 RewardOffset = new Vector2(0.0f, 5.0f);
    private static readonly Vector2 RewardForce = new Vector2(250.0f, 500.0f);

    /// <summary>
    /// Delegate for completing quest
    /// </summary>
    public delegate void QuestComplete(Quest quest);

    /// <summary>
    /// Event called when quest is complete
    /// </summary>
    public event QuestComplete OnQuestComplete;

    [Tooltip("How many prestige this quest is worth")]
    [SerializeField]
    [Range(0, 10)]
    private int prestige = 1;

    [Tooltip("Difficulty of this quest")]
    [SerializeField]
    [Range(1, 100)]
    private int difficulty = 1;

    [Tooltip("What the quest giver will say when presenting the quest")]
    [SerializeField]
    private string description;

    [Tooltip("Whether or not the quest has been completed")]
    [SerializeField]
    private bool completedQuest = false;

    [Tooltip("Whether or not this quest is completable over multiple lives")]
    [SerializeField]
    private bool longTermQuest = false;

    protected Player player;

    private Spawner[] spawners;

    protected virtual void Awake()
    {
        SetUpSpawners();
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError("Could not find player!", gameObject);
            return;
        }
    }

    void Start()
    {
        if (string.IsNullOrEmpty(description))
        {
            Debug.LogError(this + " does not have a description!", gameObject);
            return;
        }
    }

    /// <summary>
    /// Whether or not the player is qualified to take on this quest
    /// </summary>
    /// <returns>Is qualified</returns>
    public abstract bool IsQualified();

    /// <summary>
    /// Checks whether or the quest is long term and should be saved across multiple playthroughs
    /// </summary>
    /// <returns>Whether or not the quest is long term</returns>
    public bool IsLongTermQuest()
    {
        return longTermQuest;
    }

    /// <summary>
    /// Sets whether or not this is a long term quest, and should be saved across multiple playthroughs
    /// </summary>
    /// <param name="longTermQuest">Whether or not a long term quest</param>
    public void SetLongTermQuest(bool longTermQuest)
    {
        this.longTermQuest = longTermQuest;
    }

    /// <summary>
    /// Gets how many prestige it is worth to complete this quest
    /// </summary>
    /// <returns>How many prestige it's worth</returns>
    public int GetPrestige()
    {
        return prestige;
    }

    /// <summary>
    /// Gets the quest difficulty
    /// </summary>
    /// <returns>Difficulty of quest</returns>
    public int GetDifficulty()
    {
        return difficulty;
    }

    /// <summary>
    /// Checks whether or not the quest has been completed
    /// </summary>
    /// <returns>If quest is complete</returns>
    public bool IsComplete()
    {
        return completedQuest;
    }

    /// <summary>
    /// Sets whether or not the quest is complete.  If this is the first time it is complete, and a reward is expected, use quest.Complete() instead.
    /// </summary>
    /// <param name="complete">Whether to set the quest as complete</param>
    public void SetComplete(bool complete)
    {
        completedQuest = complete;
    }

    /// <summary>
    /// Gets the quest description
    /// </summary>
    /// <returns>Quest description</returns>
    public string GetDescription()
    {
        return description;
    }

    /// <summary>
    /// Completes quest
    /// </summary>
    public void Complete()
    {
        if (completedQuest)
        {
            Debug.LogError("Attempting to complete a quest that has already been completed!");
            return;
        }
        completedQuest = true;
        SpawnRewards();
        if (OnQuestComplete != null)
        {
            OnQuestComplete(this);
        }
    }

    /// <summary>
    /// Initializes and turns off spawners
    /// </summary>
    private void SetUpSpawners()
    {
        spawners = GetComponents<Spawner>();
        foreach (var spawner in spawners)
        {
            spawner.SetSpawnOnStart(false);
        }
    }

    /// <summary>
    /// Spawns the rewards
    /// </summary>
    private void SpawnRewards()
    {
        transform.position += (Vector3)RewardOffset;
        foreach (var spawner in spawners)
        {
            var rewards = spawner.Spawn();
            foreach (var reward in rewards)
            {
                var body2D = reward.GetComponent<Rigidbody2D>();
                if (body2D)
                {
                    body2D.AddForce(new Vector2(Random.Range(-RewardForce.x, RewardForce.x), Random.Range(0.0f, RewardForce.y)));
                    GameObjectUtility.ChildCloneToContainer(reward);
                }
                else
                {
                    var collectable = reward.GetOrAddComponent<Collectable>();
                    collectable.SetHighPriority(true);
                    player.Collect(collectable);
                }
            }
        }
    }
}