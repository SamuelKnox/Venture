using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


namespace Devdog.QuestSystemPro.Editors
{
    public class IntegrationHelperEditor : EditorWindow
    {

        private BuildTargetGroup[] allTargets;
        private Vector2 scrollPos = new Vector2();
        private Color grayishColor;

        [MenuItem(QuestSystemPro.ToolsMenuPath + "Integrations", false, 0)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<IntegrationHelperEditor>(true, "Integrations", true);
        }

        public void OnEnable()
        {
            allTargets = new BuildTargetGroup[]
            {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.WebPlayer,
                BuildTargetGroup.iOS,
                BuildTargetGroup.PS3,
                BuildTargetGroup.XBOX360,
                BuildTargetGroup.Android,
                BuildTargetGroup.WebGL,
                BuildTargetGroup.WSA,
                BuildTargetGroup.WP8,
                BuildTargetGroup.BlackBerry,
                BuildTargetGroup.Tizen,
                BuildTargetGroup.PSP2,
                BuildTargetGroup.PS4,
                BuildTargetGroup.PSM,
                BuildTargetGroup.XboxOne,
                BuildTargetGroup.SamsungTV
            };

            grayishColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);

        }


        public void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            ShowIntegration("Inventory Pro", "", "", "INVENTORY_PRO");
            ShowIntegration("PlayMaker", "Quickly make gameplay prototypes, A.I. behaviors, animation graphs, interactive objects, cut-scenes, walkthroughs", "https://www.assetstore.unity3d.com/en/#!/content/368", "PLAYMAKER");
            ShowIntegration("Rewired", "Rewired is an advanced input system that completely redefines how you work with input, giving you an unprecedented level of control over one of the most important components of your game.", "https://www.assetstore.unity3d.com/en/#!/content/21676", "REWIRED");
            ShowIntegration("SALSA", "Simple Automated Lip Sync Approximation provides high quality, language-agnostic, lip sync approximation for your 2D and 3D characters using Sprite texture types or BlendShapes.", "https://www.assetstore.unity3d.com/en/#!/content/16944", "SALSA");

            // ShowIntegration("Easy Save 2", "Easy save is a fast and easy tool to load and save almost any data type.", "https://www.assetstore.unity3d.com/en/#!/content/768", "EASY_SAVE_2");
            // ShowIntegration("Behavior Designer", "Behavior trees are used by AAA studios to create a lifelike AI. With Behavior Designer, you can bring the power of behaviour trees to Unity!", "https://www.assetstore.unity3d.com/en/#!/content/15277", "BEHAVIOR_DESIGNER");
            // ShowIntegration("Dialogue System", "Dialogue System for Unity makes it easy to add interactive dialogue to your game. It's a complete, robust solution including a visual node-based editor, dialogue UIs, cutscenes, quests, save/load, and more.", "https://www.assetstore.unity3d.com/en/#!/content/11672", "DIALOGUE_SYSTEM");

            GUILayout.EndScrollView();

        }

        private void ShowIntegration(string name, string description, string link, string defineName, bool showBox = true)
        {
            if(showBox)
                EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(IsEnabled(defineName), name))
            {
                EnableIntegration(defineName);
            }
            else
            {
                DisableIntegration(defineName);
            }
            if (GUILayout.Button("View in Asset store", UnityEditor.EditorStyles.toolbarButton))
                Application.OpenURL(link);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUI.color = grayishColor;
            EditorGUILayout.LabelField(description, EditorStyles.labelStyle);
            GUI.color = Color.white;

            if(showBox)
                EditorGUILayout.EndVertical();

            GUILayout.Space(10);
        }

        private bool IsEnabled(string name)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Contains(name);
        }

        private void DisableIntegration(string name)
        {
            if (IsEnabled(name) == false) // Already disabled
                return;

            foreach (var target in allTargets)
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                string[] items = symbols.Split(';');
                var l = new List<string>(items);
                l.Remove(name);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", l.ToArray()));
            }
        }

        private void EnableIntegration(string name)
        {
            if (IsEnabled(name)) // Already enabled
                return;

            foreach (var target in allTargets)
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                string[] items = symbols.Split(';');
                var l = new List<string>(items);
                l.Add(name);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", l.ToArray()));
            }
        }
    }
}