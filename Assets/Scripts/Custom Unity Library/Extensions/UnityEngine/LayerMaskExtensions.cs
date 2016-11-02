using UnityEngine;

public static class LayerMaskExtensions
{
    /// <summary>
    /// Checks to see whether or not the Layer Mask contains the layer
    /// </summary>
    /// <param name="layerMask">Layer mask to check</param>
    /// <param name="layer">Layer to check for</param>
    /// <returns>Whether or not the layer mask contains the layer</returns>
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    /// <summary>
    /// Checks to see whether or not the Layer Mask contains the layer
    /// </summary>
    /// <param name="layerMask">Layer mask to check</param>
    /// <param name="layerName">Name of layer to check for</param>
    /// <returns>Whether or not the player mask contains the layer</returns>
    public static bool Contains(this LayerMask layerMask, string layerName)
    {
        return layerMask.Contains(LayerMask.NameToLayer(layerName));
    }
}