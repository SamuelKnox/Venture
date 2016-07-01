using System;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    [System.Serializable]
    public struct CursorIcon
    {
        public Texture2D texture;
        public Vector2 hotspot;
        public CursorMode cursorMode;

        public CursorIcon(Texture2D texture, Vector2 hotspot, CursorMode cursorMode = CursorMode.Auto)
        {
            this.texture = texture;
            this.hotspot = hotspot;
            this.cursorMode = cursorMode;
        }

        public void Enable()
        {
            Cursor.SetCursor(texture, hotspot, cursorMode);    
        }
    }
}
