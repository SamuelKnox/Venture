using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Editors
{

    [CustomPropertyDrawer(typeof(UIWindowField), true)]
    public class UIWindowFieldEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var window = property.FindPropertyRelative("_window");
            var name = property.FindPropertyRelative("name");
            var useDynamicSearch = property.FindPropertyRelative("useDynamicSearch");

            position.width -= 30;
            if (useDynamicSearch.boolValue)
            {
                EditorGUI.PropertyField(position, name, label);
            }
            else
            {
                EditorGUI.PropertyField(position, window, label);
            }

            position.x += position.width + 10;
            position.width = 30;
            EditorGUI.PropertyField(position, useDynamicSearch, GUIContent.none);
            
            EditorGUI.EndProperty();
        }
    }
}