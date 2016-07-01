using System;
using System.Collections.Generic;
using System.Reflection;
using Devdog.QuestSystemPro.Editors;
using Devdog.QuestSystemPro.Editors.ReflectionDrawers;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Devdog.QuestSystemPro.Dialogue.Editors
{
    [CustomNodeEditor(typeof(PlayerInputNode))]
    public class PlayerInputNodeEditor : DefaultNodeEditor
    {
        protected override void DoDrawEdge(Edge edge, uint index, Vector2 from, Vector2 to, Color defaultColor)
        {
            var color = Color.grey;
            if (index == 0)
            {
                color = Color.green;
            }
            else if (index == 1)
            {
                color = Color.red;
            }

            if (editor.selectedEdges.Contains(edge))
            {
                DialogueEditorUtility.DrawCurves(from, to, color, 8f);
            }
            else
            {
                DialogueEditorUtility.DrawCurves(from, to, color);
            }
        }
    }
}
