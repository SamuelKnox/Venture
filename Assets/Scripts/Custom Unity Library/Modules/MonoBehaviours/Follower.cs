namespace CustomUnityLibrary
{
    using UnityEngine;

    /// <summary>
    /// Makes the GameObject follow the target's position and rotation
    /// </summary>
    public class Follower : MonoBehaviour
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
        private bool followZPosition = true;

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
        private bool gameObjectIsCamera = false;

        void Awake()
        {
            gameObjectIsCamera = GetComponentInChildren<Camera>();
        }

        void Start()
        {
            positionOffset = target.position - transform.position;
            rotationOffset = target.eulerAngles - transform.eulerAngles;
        }

        void Update()
        {
            if (!gameObjectIsCamera)
            {
                FollowPosition();
                FollowRotation();
            }
        }

        void LateUpdate()
        {
            if (gameObjectIsCamera)
            {
                FollowPosition();
                FollowRotation();
            }
        }

        /// <summary>
        /// Gets the target the GameObject is following
        /// </summary>
        /// <returns>The target being followed</returns>
        public Transform GetTarget()
        {
            return target;
        }

        /// <summary>
        /// Sets the target to be followed by the GameObject
        /// </summary>
        /// <param name="target">The target for the GameObject to follow</param>
        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        /// <summary>
        /// Gets the damping of the GameObject's position follow
        /// </summary>
        /// <returns>The damping time to follow the GameObject</returns>
        public float GetPositionDampTime()
        {
            return positionDampTime;
        }

        /// <summary>
        /// Sets the damping of the GameObject's position follow
        /// </summary>
        /// <param name="dampTime">The damping time to follow the GameObject</param>
        public void SetPositionDampTime(float dampTime)
        {
            positionDampTime = dampTime;
        }

        /// <summary>
        /// Gets the damping time of the rotation follow
        /// </summary>
        /// <returns>The damping time to rotate</returns>
        public float GetRotationDampTime()
        {
            return rotationDampTime;
        }

        /// <summary>
        /// Sets the damping time of the rotation follow
        /// </summary>
        /// <param name="dampTime">The damping time to rotate</param>
        public void SetRotationDampTime(float dampTime)
        {
            rotationDampTime = dampTime;
        }

        /// <summary>
        /// Checks whether or not the GameObject should follow the target position on the x axis
        /// </summary>
        /// <returns>Whether or not the GameObject should follow the target position on its x axis</returns>
        public bool GetFollowXPosition()
        {
            return followXPosition;
        }

        /// <summary>
        /// Sets whether or not the GameObject should follow the target's position on the x axis
        /// </summary>
        /// <param name="followX">Whether or not the GameObject should follow the target's position on the x axis</param>
        public void SetFollowXPosition(bool followX)
        {
            followXPosition = followX;
        }

        /// <summary>
        /// Checks whether or not the GameObject should follow the target position on the y axis
        /// </summary>
        /// <returns>Whether or not the GameObject should follow the target position on its y axis</returns>
        public bool GetFollowYPosition()
        {
            return followYPosition;
        }

        /// <summary>
        /// Sets whether or not the GameObject should follow the target's position on the y axis
        /// </summary>
        /// <param name="followX">Whether or not the GameObject should follow the target's position on the y axis</param>
        public void SetFollowYPosition(bool followY)
        {
            followYPosition = followY;
        }

        /// <summary>
        /// Checks whether or not the GameObject should follow the target position on the z axis
        /// </summary>
        /// <returns>Whether or not the GameObject should follow the target position on its z axis</returns>
        public bool GetFollowZPosition()
        {
            return followZPosition;
        }

        /// <summary>
        /// Sets whether or not the GameObject should follow the target's position on the z axis
        /// </summary>
        /// <param name="followZ">Whether or not the GameObject should follow the target's position on the z axis</param>
        public void SetFollowZPosition(bool followZ)
        {
            followZPosition = followZ;
        }

        /// <summary>
        /// Checks whether or not the GameObject should follow the target's rotation on the y axis
        /// </summary>
        /// <returns>Whether or not the GameObject should follow the target's rotation on the y axis</returns>
        public bool GetFollowYRotation()
        {
            return followYRotation;
        }

        /// <summary>
        /// Sets whether or not the GameObject should follow the target's rotation on the y axis
        /// </summary>
        /// <param name="followY">Whether or not the GameObject will follow the target's rotation on the y axis</param>
        public void SetFollowYRotation(bool followY)
        {
            followYRotation = followY;
        }

        /// <summary>
        /// Checks whether or not the GameObject should follow the target's rotation on the z axis
        /// </summary>
        /// <returns>Whether or not the GameObject should follow the target's rotation on the z axis</returns>
        public bool GetFollowZRotation()
        {
            return followZRotation;
        }

        /// <summary>
        /// Sets whether or not the GameObject should follow the target's rotation on the z axis
        /// </summary>
        /// <param name="followZ">Whether or not the GameObject will follow the target's rotation on the z axis</param>
        public void SetFollowZRotation(bool followZ)
        {
            followZRotation = followZ;
        }

        /// <summary>
        /// Makes GameObject follow the targets position
        /// </summary>
        private void FollowPosition()
        {
            if (!target)
            {
                return;
            }
            float x = followXPosition ? target.position.x - positionOffset.x : transform.position.x;
            float y = followYPosition ? target.position.y - positionOffset.y : transform.position.y;
            float z = followZPosition ? target.position.z - positionOffset.z : transform.position.z;
            var position = new Vector3(x, y, z);
            transform.position = Vector3.SmoothDamp(transform.position, position, ref velocity, positionDampTime);
        }

        /// <summary>
        /// Makes GameObject follow the target's rotation
        /// </summary>
        private void FollowRotation()
        {
            if (!target)
            {
                return;
            }
            float x = followXRotation ? target.eulerAngles.x - rotationOffset.x : transform.eulerAngles.x;
            float y = followYRotation ? target.eulerAngles.y - rotationOffset.y : transform.eulerAngles.y;
            float z = followZRotation ? target.eulerAngles.z - rotationOffset.z : transform.eulerAngles.z;
            var rotation = Quaternion.Euler(x, y, z);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationDampTime);
        }
    }
}