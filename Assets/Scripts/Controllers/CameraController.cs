using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Player, which will be followed by the camera")]
    [SerializeField]
    private Player player;

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    /// <summary>
    /// Updates the camera's x and y to follow the player
    /// </summary>
    private void UpdateCameraPosition()
    {
        float x = player.transform.position.x;
        float y = player.transform.position.y;
        float z = transform.position.z;
        transform.position = new Vector3(x, y, z);
    }
}