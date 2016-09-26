
using UnityEngine;

public class Test : MonoBehaviour
{
    public LayerMask obstructionLayers;

    void Update()
    {
        var origin = (Vector2)transform.position;
        var downRaycastHit2D = Physics2D.Raycast(origin, Vector2.down, Mathf.Infinity, obstructionLayers);
        Debug.DrawRay(origin, Vector2.down);
        if (downRaycastHit2D.collider.bounds.Contains(origin))
        {
            Debug.Log(origin);
        }
    }
}