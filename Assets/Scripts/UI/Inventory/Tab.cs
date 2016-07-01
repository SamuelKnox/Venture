using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Tab : MonoBehaviour {
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// Gets the image associated on this tab
    /// </summary>
    /// <returns>Tab image</returns>
    public Image GetImage()
    {
        return image;
    }
}
