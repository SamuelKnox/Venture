using LevelGenerator2D;
using UnityEngine;

/// <summary>
/// The controller used to create the Level.  This is for demo purposes and can be extended/replaced.
/// </summary>
[RequireComponent(typeof(Level))]
public class LevelController : MonoBehaviour
{
    [Tooltip("Seed to be used for random number generation.  If set to 0, a random seed will be used.")]
    [SerializeField]
    private int seed = 0;

    [Tooltip("Extent at which to preload the level.  This will cause a loading time, but can prevent runtime loading until the edge is reached.")]
    [SerializeField]
    [Range(0, 25)]
    private int preloadExtent = 5;

    [Tooltip("Maximum number of Rooms before they start closing.  The actual maximum number of Rooms will vary, but there will be at least this many.  If 0, there will be unlimited rooms.  The initial build will not be affected by the max number of rooms.")]
    [SerializeField]
    [Range(0, 1000)]
    private int maxRooms = 0;

    private Level level;
    private string[] roomCategories;

    void Awake()
    {
        Random.seed = seed == 0 ? Random.Range(int.MinValue, int.MaxValue) : seed;
        level = GetComponent<Level>();
    }

    void Start()
    {
        roomCategories = new string[] { RoomCategories.Random };
        PopulateLevelWithPreexistingRooms();
        if (level.GetRooms().Length > 0)
        {
            BuildBaseLevel();
        }
        else
        {
            Debug.LogError("There is no initial Room.  The Level Controller has failed to build the Level.  Add a Room to the Scene, so that the Level Controller can build off of it.", gameObject);
            return;
        }
    }

    void Update()
    {
        ExpandLevel();
    }

    /// <summary>
    /// Moves all existing rooms to the attached level
    /// </summary>
    private void PopulateLevelWithPreexistingRooms()
    {
        var startingRooms = FindObjectsOfType<Room>();
        level.AddRooms(startingRooms);
    }

    /// <summary>
    /// Creates the initial level
    /// </summary>
    private void BuildBaseLevel()
    {
        var cameraPoint = Level.GetCameraPoint();
        int minX = Mathf.RoundToInt(cameraPoint.x - preloadExtent);
        int maxX = Mathf.RoundToInt(cameraPoint.x + preloadExtent);
        int minY = Mathf.RoundToInt(cameraPoint.y - preloadExtent);
        int maxY = Mathf.RoundToInt(cameraPoint.y + preloadExtent);
        level.AddNeighbors(minX, minY, maxX, maxY, roomCategories);
    }

    /// <summary>
    /// Expands the level as needed
    /// </summary>
    private void ExpandLevel()
    {
        if (!IsLevelFull())
        {
            foreach (var room in level.GetRoomsVisibleByCamera())
            {
                if (room.IsOpen())
                {
                    level.AddNeighbors(room, roomCategories);
                }
            }
        }
        else
        {
            var openRooms = level.GetOpenRooms();
            level.CloseRooms(openRooms);
        }
    }

    /// <summary>
    /// Checks if the level has reached its size limit
    /// </summary>
    /// <returns>Whether or not the room is full</returns>
    private bool IsLevelFull()
    {
        return maxRooms > 0 && level.GetRooms().Length >= maxRooms;
    }
}