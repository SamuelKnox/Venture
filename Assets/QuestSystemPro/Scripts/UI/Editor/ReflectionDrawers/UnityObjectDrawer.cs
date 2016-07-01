using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public class UnityObjectDrawer : SimpleValueDrawer
    {
        protected List<DrawerBase> children = new List<DrawerBase>();
        protected bool allowSceneObjects;

        public readonly int UnityObjectPickerControlID;


        public UnityObjectDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

            UnityObjectPickerControlID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            if (fieldInfo != null)
            {
                allowSceneObjects = fieldInfo.GetCustomAttributes(typeof(AllowSceneObjectsAttribute), true).Length > 0;
            }
        }

        protected override object DrawInternal(Rect rect)
        {
            rect = GetSingleLineHeightRect(rect);

            if (string.IsNullOrEmpty(fieldName.text) == false)
            {
                var labelRect = rect;
                labelRect.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(labelRect, fieldName);

                rect.width -= EditorGUIUtility.labelWidth;
                rect.x += EditorGUIUtility.labelWidth;
            }

            //            if (value == null)
            //            {
            //                GUI.contentColor = Color.yellow;
            //            }

            string objectName = "(empty)";
            var unityEngineObject = (UnityEngine.Object)value; // Cast first, otherwise unity thinks it's not null (wrapped C# / C++ object fails check for some reason)
            if (unityEngineObject != null)
            {
                objectName = unityEngineObject.name;
            }
            else
            {
                if (required)
                {
                    GUI.color = Color.red;
                }
            }

            if (GUI.Button(rect, objectName, UnityEditor.EditorStyles.objectField))
            {
                var objectField = rect;
                objectField.width -= 20;

                var rightSide = rect;
                rightSide.width = 20;
                rightSide.x += objectField.width;

                if (objectField.Contains(Event.current.mousePosition) && Event.current.button == 0)
                {
                    OnClickedFieldLeftSide();
                }

                if (rightSide.Contains(Event.current.mousePosition) && Event.current.button == 0)
                {
                    OnClickedFieldRightSide();
                }
            }

            GUI.color = Color.white;

            // Set the value from unity's object picker.
            if (EditorGUIUtility.GetObjectPickerControlID() == UnityObjectPickerControlID)
            {
                var obj = EditorGUIUtility.GetObjectPickerObject();
                if (obj != null)
                {
                    if (obj.GetType().IsAssignableFrom(GetFieldType(true)) == false)
                    {
                        obj = ((GameObject) obj).GetComponent(GetFieldType(true));
                    }

                    if (obj == null || obj.GetType().IsAssignableFrom(GetFieldType(true)) == false)
                    {
                        QuestLogger.LogWarning("Selected object can't be assigned. It's not assignable to " + GetFieldTypeName(true));
                        return value;
                    }

                    value = obj;
                    NotifyValueChanged(value);
                }
                else
                {
                    value = null;
                    NotifyValueChanged(value);
                }
            }

            return value;
        }

        protected virtual void OnClickedFieldRightSide()
        {
            if (allowSceneObjects || ReflectionUtility.IsBuiltInUnityObjectType(GetFieldType(true)))
            {
                ShowObjectPicker((UnityEngine.Object) value);
            }
            else
            {
                // Use custom picker only for prefabs. Use Unity's picker for scene objects and built-in types.
                ObjectPickerUtility.GetObjectPickerForType(GetFieldType(true), (asset) =>
                {
                    value = asset;
                    NotifyValueChanged(value);
                });
            }
        }

        protected virtual void OnClickedFieldLeftSide()
        {
            EditorGUIUtility.PingObject((UnityEngine.Object) value);
        }

        protected virtual void ShowObjectPicker(UnityEngine.Object obj)
        {
            // No non-generic call to SHowObjectPicker and filter doesn't seem to work...

            // TODO: Make sure the basic UnityEngine.Objects are added here (sealed classes)
            var t = GetFieldType(true);
            if (t == typeof(Sprite))
                ShowSinglePicker<Sprite>(value);
            else if(t == typeof(Texture))
                ShowSinglePicker<Texture>(value);
            else if (t == typeof(Texture2D))
                ShowSinglePicker<Texture2D>(value);
            else if (t == typeof(Texture3D))
                ShowSinglePicker<Texture3D>(value);
            else if (t == typeof(AnimationClip))
                ShowSinglePicker<AnimationClip>(value);
            else if (t == typeof(AudioClip))
                ShowSinglePicker<AudioClip>(value);
            else if (t == typeof(Motion))
                ShowSinglePicker<Motion>(value);

        }

        protected virtual void ShowSinglePicker<T>(object obj) where T : UnityEngine.Object
        {
            EditorGUIUtility.ShowObjectPicker<T>((T)value, true, "", UnityObjectPickerControlID);
        }
    }
}
