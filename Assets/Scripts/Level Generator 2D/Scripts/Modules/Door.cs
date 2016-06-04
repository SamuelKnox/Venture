namespace LevelGenerator2D
{
    using CustomUnityLibrary;
    using UnityEngine;

    /// <summary>
    /// An opening where two Rooms can be attached
    /// </summary>
    [System.Serializable]
    public class Door
    {
#pragma warning disable 0414
        [Tooltip("Name assigned to this door")]
        [SerializeField]
        private string name;
#pragma warning restore 0414

        [Tooltip("Indexed position of this door")]
        [SerializeField]
        private int number;

        private bool open = true;
        private Point2 localPoint;
        private Point2 globalPoint;
        private Direction side;

        /// <summary>
        /// Creates a door with the given name and index
        /// </summary>
        public Door(string name, int number)
        {
            SetName(name);
            SetIndex(number);
        }

        /// <summary>
        /// Checks if the door is a valid match for another Door
        /// </summary>
        public bool PairsWith(Door door)
        {
            if (door == null)
            {
                return false;
            }
            return GetNeighborPoint() == door.GetGlobalPoint() && door.GetNeighborPoint() == GetGlobalPoint();
        }

        /// <summary>
        /// Gets the global point to which this door connects
        /// </summary>
        /// <returns></returns>
        public Point2 GetNeighborPoint()
        {
            var neighborPoint = GetGlobalPoint();
            switch (GetSide())
            {
                case Direction.West:
                    neighborPoint.x -= 1;
                    break;
                case Direction.North:
                    neighborPoint.y += 1;
                    break;
                case Direction.East:
                    neighborPoint.x += 1;
                    break;
                case Direction.South:
                    neighborPoint.y -= 1;
                    break;
                default:
                    Debug.LogError("Invalid Door Direction (" + GetSide() + " received!");
                    break;
            }
            return neighborPoint;
        }

        /// <summary>
        /// Gets the indexed number within the Room of this Door
        /// </summary>
        public int GetIndex()
        {
            return number;
        }

        /// <summary>
        /// Sets the indexed number within the Room of this Door
        /// </summary>
        public void SetIndex(int index)
        {
            number = index;
        }

        /// <summary>
        /// Sets the name associated with this Door
        /// </summary>
        public void SetName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets whether or not this door is open and available
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return open;
        }

        /// <summary>
        /// Sets whether or not this Door is open and available
        /// </summary>
        public void SetOpen(bool open)
        {
            this.open = open;
        }

        /// <summary>
        /// Gets which side of its Room this door is on
        /// </summary>
        public Direction GetSide()
        {
            return side;
        }

        /// <summary>
        /// Sets which side of its Room this Door is on
        /// </summary>
        public void SetSide(Direction side)
        {
            this.side = side;
        }

        /// <summary>
        /// Gets the Point local to the Room where this Door is located
        /// </summary>
        public Point2 GetLocalPoint()
        {
            return localPoint;
        }

        /// <summary>
        /// Sets the Point local to the Room where this Door is located
        /// </summary>
        public void SetLocalPoint(Point2 localPoint)
        {
            this.localPoint = localPoint;
        }

        /// <summary>
        /// Gets the Point where this Door is located globally
        /// </summary>
        public Point2 GetGlobalPoint()
        {
            return globalPoint;
        }

        /// <summary>
        /// Sets the Point where this Door is located globally
        /// </summary>
        public void SetGlobalPoint(Point2 globalPoint)
        {
            this.globalPoint = globalPoint;
        }
    }
}