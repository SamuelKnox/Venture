using UnityEngine;

public class ScaleRandomizer : MonoBehaviour
{
    [Tooltip("Percent scale which transform can be shrunk to at the minimum.  0 means the size will be completely random, while 1 means the size will not change at all")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minScale = 0.5f;

    void Awake()
    {
        ResizeScale();
    }

    /// <summary>
    /// Rescales the GameObject
    /// </summary>
    private void ResizeScale()
    {
        float scale = Random.Range(minScale, 1.0f);
        while (scale == 0)
        {
            scale = Random.Range(minScale, 1.0f);
        }
        transform.localScale = new Vector3(transform.localScale.x * scale, transform.localScale.y * scale, transform.localScale.z);
    }
}