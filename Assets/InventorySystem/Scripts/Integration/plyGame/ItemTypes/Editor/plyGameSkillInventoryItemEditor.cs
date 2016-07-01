#if PLY_GAME

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Editors;
using Devdog.InventorySystem.Integration.plyGame;
using Devdog.InventorySystem.Models;
using plyCommon;
using plyGame;
using plyGameEditor;

namespace Devdog.InventorySystem.Integration.plyGame.Editors
{
    [CustomEditor(typeof(plyGameSkillInventoryItem), true)]
    public class plyGameSkillInventoryItemEditor : InventoryItemBaseEditor
    {
        
        public override void OnEnable()
        {
            base.OnEnable();
            
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            var t = (plyGameSkillInventoryItem)target;

            var l = new List<CustomOverrideProperty>(extraOverride);
            l.Add(new CustomOverrideProperty("skill", () =>
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("plyGame skill", GUILayout.Width(EditorGUIUtility.labelWidth));
                if (GUILayout.Button((t.skill == null) ? "None selected" : t.skill.ToString(), "ObjectField"))
                {
                    var picker = InventoryPlySkillPrefabPickerBase.Get("plyGame Skill picker");
                    picker.Show(true);
                    picker.OnPickObject += (obj) =>
                    {
                        t.skill = obj;
                        EditorUtility.SetDirty(t);
                        Repaint();
                    };
                }

                EditorGUILayout.EndHorizontal();
            }));

            base.OnCustomInspectorGUI(l.ToArray());
        }
    }
}

#endif