using UnityEngine;
using System.Collections;
using Devdog.QuestSystemPro.UI;
using UnityEditor;

namespace Devdog.QuestSystemPro.Editors
{
    [CustomEditor(typeof(UIWindow), true)]
    public class UIWindowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (UIWindow)target;
            if (t.gameObject.GetComponent<UIWindowInput>() == null)
            {
                GUI.color = Color.green;
                if (GUILayout.Button("Add input handler"))
                {
                    t.gameObject.AddComponent<UIWindowInput>();
                }
                GUI.color = Color.white;
            }
        }
    }
}