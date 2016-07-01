using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Target to follow")]
    [SerializeField]
    private Transform target;

    [Tooltip("Time to damp following movement")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float positionDampTime = 0.15f;

    [Tooltip("Follow movement on X axis")]
    [SerializeField]
    private bool followXPosition = true;

    [Tooltip("Follow movement on Y axis")]
    [SerializeField]
    private bool followYPosition = true;

    [Tooltip("Follow movement on Z axis")]
    [SerializeField]
    private bool followZPosition = false;

    [Tooltip("Time to damp following rotation")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float rotationDampTime = 0.25f;

    [Tooltip("Follow movement on X rotation")]
    [SerializeField]
    private bool followXRotation = false;

    [Tooltip("Follow movement on Y rotation")]
    [SerializeField]
    private bool followYRotation = false;

    [Tooltip("Follow movement on Z rotation")]
    [SerializeField]
    private bool followZRotation = false;

    private Vector3 velocity;
    private Vector3 positionOffset;
    private Vector3 rotationOffset;

    void Start()
    {
        positionOffset = target.position - transform.position;
        rotationOffset = target.eulerAngles - transform.eulerAngles;
    }

    void LateUpdate()
    {
        FollowPosition();
        FollowRotation();
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public float GetPositionDampTime()
    {
        return positionDampTime;
    }

    public void SetPositionDampTime(float dampTime)
    {
        positionDampTime = dampTime;
    }

    public bool GetFollowXPosition()
    {
        return followXPosition;
    }

    public void SetFollowXPosition(bool followX)
    {
        followXPosition = followX;
    }

    public bool GetFollowYPosition()
    {
        return followYPosition;
    }

    public void SetFollowYPosition(bool followY)
    {
        followYPosition = followY;
    }

    private void FollowPosition()
    {
        float x = followXPosition ? target.position.x - positionOffset.x : transform.position.x;
        float y = followYPosition ? target.position.y - positionOffset.y : transform.position.y;
        float z = followZPosition ? target.position.z - positionOffset.z : transform.position.z;
        var position = new Vector3(x, y, z);
        transform.position = Vector3.SmoothDamp(transform.position, position, ref velocity, positionDampTime);
    }

    private void FollowRotation()
    {
        float x = followXRotation ? target.eulerAngles.x - rotationOffset.x : transform.eulerAngles.x;
        float y = followYRotation ? target.eulerAngles.y - rotationOffset.y : transform.eulerAngles.y;
        float z = followZRotation ? target.eulerAngles.z - rotationOffset.z : transform.eulerAngles.z;
        var rotation = Quaternion.Euler(x, y, z);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationDampTime);
    }
}