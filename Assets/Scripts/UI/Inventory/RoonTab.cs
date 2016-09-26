using UnityEngine;

public class RoonTab : MonoBehaviour
{
    [Tooltip("Type of roon this tab represents")]
    [SerializeField]
    private RoonType roonType;

    /// <summary>
    /// Gets the type of roon represented by this tab
    /// </summary>
    /// <returns>Type of roon</returns>
    public RoonType GetRoonType()
    {
        return roonType;
    }
}