namespace LevelGenerator2D
{
    using CustomUnityLibrary;
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The editor used when displaying rooms in the inspector
    /// </summary>
    [CustomEditor(typeof(Room))]
    public class RoomEditor : Editor
    {
        private const string GlobalPointLabel = "Global Point";
        private const string GlobalPointTooltip = "This is Room's world point in grid space.";
        private const string SizeLabel = "Size";
        private const string SizeTooltip = "These are the dimensions of the Room in grid space.";
        private const string DoorCountLabel = "Door Count";
        private const string DoorCountTooltip = "This is how many doors are on this room.";
        private const string DoorLabelPrefix = "Door #";
        private const string DoorTooltipPrefix = "This is the Door at index #";
        private const string DoorTooltipSuffix = ".  The number to the right is its position in the room.";
        private const string WeightLabel = "Weight";
        private const string WeightTooltip = "This determines how often the room will spawn.";
        private const float MinWeight = 0.0f;
        private const float MaxWeight = 1.0f;

        public override void OnInspectorGUI()
        {
            var room = (Room)target;
            MaintainGlobalPoint(room);
            MaintainSize(room);
            MaintainWeight(room);
            MaintainDoors(room);
            UpdateGUI(room);
        }

        /// <summary>
        /// Draws the UI in the inspector for the Room's global point
        /// </summary>
        /// <param name="room">The room to draw the UI for</param>
        private void MaintainGlobalPoint(Room room)
        {
            var guiContent = new GUIContent(GlobalPointLabel, GlobalPointTooltip);
            var globalPoint = EditorGUILayout.Vector2Field(guiContent, room.GetGlobalPoint());
            room.SetGlobalPoint(globalPoint.ToPoint());
        }

        /// <summary>
        /// Draws the UI in the inspector for the Room's width and height
        /// </summary>
        /// <param name="room">The room to draw the UI for</param>
        private void MaintainSize(Room room)
        {
            var guiContent = new GUIContent(SizeLabel, SizeTooltip);
            var size = new Point2(room.GetWidth(), room.GetHeight());
            var vectorSize = EditorGUILayout.Vector2Field(guiContent, size);
            var width = Mathf.Clamp(Mathf.RoundToInt(vectorSize.x), 1, Room.MaxWidth);
            room.SetWidth(width);
            var height = Mathf.Clamp(Mathf.RoundToInt(vectorSize.y), 1, Room.MaxHeight);
            room.SetHeight(height);
        }

        /// <summary>
        /// Draws the UI for the room's doors
        /// </summary>
        /// <param name="room">The room to draw the UI for</param>
        private void MaintainDoors(Room room)
        {
            var doorCountGUIContent = new GUIContent(DoorCountLabel, DoorCountTooltip);
            int maxDoors = room.GetWidth() * 2 + room.GetHeight() * 2;
            var doors = room.GetDoors();
            var count = EditorGUILayout.IntSlider(doorCountGUIContent, doors.Length, 1, maxDoors);
            Array.Resize(ref doors, count);
            room.SetDoors(doors);
            for (int i = 0; i < doors.Length; i++)
            {
                var doorGUIContent = new GUIContent(DoorLabelPrefix + i, DoorTooltipPrefix + i + DoorTooltipSuffix);
                var index = EditorGUILayout.IntSlider(doorGUIContent, doors[i].GetIndex(), 0, maxDoors - 1);
                doors[i].SetIndex(index);
            }
        }

        private void MaintainWeight(Room room)
        {
            var guiContent = new GUIContent(WeightLabel, WeightTooltip);
            float weight = EditorGUILayout.Slider(guiContent, room.GetWeight(), MinWeight, MaxWeight);
            room.SetWeight(weight);
        }

        /// <summary>
        /// Updates and redraws the room after changes have been made
        /// </summary>
        /// <param name="room">The room to update and redraw</param>
        private void UpdateGUI(Room room)
        {
            room.UpdatePosition();
            room.InitializeDoors();
            SceneView.RepaintAll();
        }
    }
}