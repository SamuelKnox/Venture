using UnityEngine;
using UnityEngine.UI;

public class RoonSocket : MonoBehaviour
{
    [Tooltip("Type of roon this tab represents")]
    [SerializeField]
    private RoonType roonType;

    [Tooltip("Icon of roon in socket")]
    [SerializeField]
    private Image icon;

    /// <summary>
    /// Gets the type of roon represented by this tab
    /// </summary>
    /// <returns>Type of roon</returns>
    public RoonType GetRoonType()
    {
        return roonType;
    }

    /// <summary>
    /// Gets the image holding this roon socket's roon's icon
    /// </summary>
    /// <returns>Roon icon holder</returns>
    public Image GetIcon()
    {
        return icon;
    }
}