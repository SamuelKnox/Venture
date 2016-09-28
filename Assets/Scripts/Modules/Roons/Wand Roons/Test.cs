using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public LayerMask obstructionLayers;

    void Update()
    {
        var origin = (Vector2)transform.position;
        RaycastHit2D[] downRaycastHit2D = RaycastAll(origin, Vector2.down, Mathf.Infinity, obstructionLayers);
        Debug.DrawRay(origin, Vector2.down);
        Debug.Log("Count " + downRaycastHit2D.Length);
        foreach (RaycastHit2D hit in downRaycastHit2D)
        {
            DebugDrawRect(hit.point, new Rect(-0.1f, -0.1f, 0.2f, 0.2f), Color.red);
        }
    }

    public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
    {
        Vector2 vInc = direction * Vector2.kEpsilon;
        List<RaycastHit2D> output = new List<RaycastHit2D>();
        RaycastHit2D hit2D;
        do
        {
            hit2D = Physics2D.Raycast(origin, Vector2.down, Mathf.Infinity, layerMask);
            if (hit2D.collider)
            {
                if (hit2D.fraction != 0f)
                    output.Add(hit2D);
                origin = hit2D.point + vInc;
            }
        }
        while (hit2D.collider);
        return output.ToArray();
    }

    public static void DebugDrawRect(Vector3 pos, Rect rect, Color color)
    {
        rect.position += new Vector2(pos.x, pos.y);
        Debug.DrawLine(new Vector3(rect.x, rect.y, pos.z), new Vector3(rect.x + rect.width, rect.y, pos.z), color);
        Debug.DrawLine(new Vector3(rect.x, rect.y, pos.z), new Vector3(rect.x, rect.y + rect.height, pos.z), color);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y, pos.z), new Vector3(rect.x + rect.width, rect.y + rect.height, pos.z), color);
        Debug.DrawLine(new Vector3(rect.x, rect.y + rect.height, pos.z), new Vector3(rect.x + rect.width, rect.y + rect.height, pos.z), color);
    }
}
