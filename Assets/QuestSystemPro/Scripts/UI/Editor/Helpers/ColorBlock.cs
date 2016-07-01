using System;
using UnityEngine;
using UnityEditor;

namespace Devdog.QuestSystemPro.Editors
{
    public class ColorBlock : IDisposable
    {
        private Color _before;

        public ColorBlock(Color color)
        {
            _before = GUI.color;
            GUI.color = color;
        }

        public void Dispose()
        {
            GUI.color = _before;
        }
    }
}