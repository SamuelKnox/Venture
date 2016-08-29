using UnityEngine;
using System.Collections;

public class RotateTowards : MonoBehaviour 
{

    public Transform Target;

    void Reset()
    {
        Target = transform.parent;
    }

	void Update () 
    {
        if (Target != null)
        {
            Vector2 vDiff = Target.position - transform.position;
            float rot_z = Mathf.Atan2(vDiff.y, vDiff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90); ;
        }
	}

    void OnDrawGizmos()
    {
        Update();
    }
}
