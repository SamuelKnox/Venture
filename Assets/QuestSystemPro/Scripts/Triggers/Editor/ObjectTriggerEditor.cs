using UnityEngine;
using UnityEditor;
using System;

namespace Devdog.QuestSystemPro.Editors
{
    [CustomEditor(typeof(ObjectTrigger), true)]
    [CanEditMultipleObjects]
    public class ObjectTriggererEditor : Editor
    {
        private SerializedProperty window;

        private static Color outOfRangeColor;
        private static Color inRangeColor;

        public virtual void OnEnable()
        {
            window = serializedObject.FindProperty("_window");

            outOfRangeColor = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.2f);
            inRangeColor = new Color(Color.green.r, Color.green.g, Color.green.b, 0.3f);
        }


        public void OnSceneGUI()
        {
            var trigger = (ObjectTrigger)target;

            var discColor = outOfRangeColor;
            if (Application.isPlaying && trigger.inRange)
            {
                discColor = inRangeColor;
            }

            Handles.color = discColor;
            var euler = trigger.transform.rotation.eulerAngles;
            euler.x += 90.0f;
            Handles.DrawSolidDisc(trigger.transform.position, Vector3.up, trigger.useDistance);

            discColor.a += 0.2f;
            Handles.color = discColor;
            Handles.CircleCap(0, trigger.transform.position, Quaternion.Euler(euler), trigger.useDistance);
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draws remaining items
            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "_window"
            });

            var t = (ObjectTrigger)target;

            var windowHandler = t.gameObject.GetComponent<IObjectTriggerWindowHandler>();
            if (windowHandler != null)
            {
                GUI.enabled = false;
            }

            EditorGUILayout.PropertyField(window);
            GUI.enabled = true;

            if (windowHandler != null)
            {
                EditorGUILayout.HelpBox("Window is managed by " + windowHandler.GetType().Name, MessageType.Info);
            }

            var inputHandler = t.gameObject.GetComponent<IObjectTriggerInputHandler>();
            if (inputHandler == null)
            {
                EditorGUILayout.HelpBox("No input handler found", MessageType.Warning);
                foreach (var type in ReflectionUtility.GetAllTypesThatImplement(typeof(IObjectTriggerInputHandler)))
                {
                    var tempType = type;
                    if (GUILayout.Button("Add: " + tempType.Name))
                    {
                        t.gameObject.AddComponent(tempType);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Input is handled by by " + inputHandler.GetType().Name, MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}