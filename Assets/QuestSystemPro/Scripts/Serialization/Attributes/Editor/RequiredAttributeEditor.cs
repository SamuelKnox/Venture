using UnityEngine;
using UnityEditor;

namespace Devdog.QuestSystemPro.Editors
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredAttributeEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var prevColor = GUI.color;
            if (property.objectReferenceValue == null)
                GUI.color = Color.red;

            EditorGUI.PropertyField(position, property, label);
            GUI.color = prevColor;

            EditorGUI.EndProperty();
        }
    }
}