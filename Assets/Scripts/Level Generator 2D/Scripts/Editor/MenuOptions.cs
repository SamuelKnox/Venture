namespace LevelGenerator2D
{
    using UnityEngine;
    using UnityEditor;

    public class MenuOptions
    {
        private const string LevelName = "Level";
        private const string RoomName = "Room";

        /// <summary>
        /// Creates a new Room
        /// </summary>
        [MenuItem("GameObject/2D Object/Room %#r")]
        private static void InstantiateRoom()
        {
            var level = Object.FindObjectOfType<Level>();
            if (!level)
            {
                level = new GameObject(LevelName).AddComponent<Level>();
            }
            var room = new GameObject(RoomName);
            room.AddComponent<Room>();
            room.transform.SetParent(level.transform);
        }
    }
}