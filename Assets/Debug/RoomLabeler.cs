using LevelGenerator2D;
using TMPro;
using UnityEngine;

public class RoomLabeler : MonoBehaviour
{
    [Tooltip("Text field used to display room name")]
    [SerializeField]
    private TextMeshProUGUI roomNameText;

    void Start()
    {
        var level = FindObjectOfType<Level>();
        if (!level)
        {
            Debug.LogError("Could not find level!", gameObject);
            return;
        }
        level.AddListener(this);
    }

    public void OnRoomEnter(Room room)
    {
        roomNameText.text = room.name;
    }
}