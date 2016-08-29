using UnityEngine;
using System.Collections;
using CreativeSpore.SmartColliders;

public class FollowController : MonoBehaviour 
{

	public Transform Target;
	public float DampTime = 0.15f;
    public bool ApplyTargetRotation = false;
    public float RotationDampTime = 0.25f;


	private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (Target)
        {
            transform.position = new Vector3(Target.position.x, Target.position.y, transform.position.z);
        }
    }
		
	// Update is called once per frame
	void FixedUpdate() 
	{
		if (Target)
		{
            Vector3 destination = Target.position; destination.z = transform.position.z;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, DampTime);
            if( ApplyTargetRotation )
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Target.localRotation, RotationDampTime);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, RotationDampTime);
            }
		}		
	}
}
